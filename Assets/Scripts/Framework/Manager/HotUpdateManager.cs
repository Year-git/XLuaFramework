using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    /// <summary>
    /// 热更管理器
    /// </summary>
    public class HotUpdateManager : MonoBehaviour
    {
        // 只读目录的版本文件数据
        private byte[] m_ReadPathFileListData;

        // 服务器的版本文件数据
        private byte[] m_ServerFileListData;

        /// <summary>
        /// 数据文件信息
        /// </summary>
        internal class DownFileInfo
        {
            public string url;
            public string fileName;
            public DownloadHandler fileData;
        }

        /// <summary>
        /// 下载单个文件
        /// </summary>
        /// <param name="downFileInfo"></param>
        /// <param name="complete"></param>
        /// <returns></returns>
        IEnumerator DownLoadFile(DownFileInfo downFileInfo, Action<DownFileInfo> complete)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(downFileInfo.url))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("DownLoadFile -> error result : " + webRequest.result.ToString());
                    Debug.Log("DownLoadFile -> error url : " + downFileInfo.url);
                    yield break;
                }

                downFileInfo.fileData = webRequest.downloadHandler;
                complete?.Invoke(downFileInfo);
            }
        }

        /// <summary>
        /// 下载多个文件
        /// </summary>
        /// <param name="downFileInfos"></param>
        /// <param name="complete"></param>
        /// <param name="downLoadAllCallback"></param>
        /// <returns></returns>
        IEnumerator DownLoadFile(List<DownFileInfo> downFileInfos, Action<DownFileInfo> complete, Action downLoadAllCallback)
        {
            foreach (var downFileInfo in downFileInfos)
            {
                yield return DownLoadFile(downFileInfo, complete);
            }
            downLoadAllCallback?.Invoke();
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<DownFileInfo> GetFileList(string fileData, string path)
        {
            string content = fileData.Trim().Replace("\r", "");
            string[] files = content.Split("\n");

            List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                string[] info = files[i].Split("|");
                DownFileInfo downFileInfo = new DownFileInfo();
                downFileInfo.fileName = info[1];
                downFileInfo.url = Path.Combine(path, info[1]);
                downFileInfos.Add(downFileInfo);
            }
            return downFileInfos;
        }

        void Start()
        {
            if (IsFristInstall())
            {
                ReleaseResurces();
            }
            else
            {
                CheckUpdate();
            }
        }

        /// <summary>
        /// 是否首次安装 
        /// </summary>
        private bool IsFristInstall()
        {
            // 判断只读目录释放存在版本文件
            bool isExistsReadPath = FileUtil.IsExists(Path.Combine(PathDefine.ReadPath, ConstDefine.FileListName));
            // 判断可读写目录是否存在版本文件
            bool isExistsReadWritePath = FileUtil.IsExists(Path.Combine(PathDefine.ReadWritePath, ConstDefine.FileListName));
            return isExistsReadPath && !isExistsReadWritePath;
        }

        /// <summary>
        /// 释放资源
        /// 将首包内的资源从<只读路径>释放到<可读写路径>
        /// 方便加载资源使用统一路径
        /// </summary>
        private void ReleaseResurces()
        {
            string url = Path.Combine(PathDefine.ReadPath, ConstDefine.FileListName);
            DownFileInfo downFileInfo = new DownFileInfo();
            downFileInfo.url = url;
            // 下载版本文件
            StartCoroutine(DownLoadFile(downFileInfo, OnDownLoadReadPathFileListComplete));
        }

        /// <summary>
        /// 只读目录的版本文件 下载完成回调
        /// </summary>
        private void OnDownLoadReadPathFileListComplete(DownFileInfo downFileInfo)
        {
            m_ReadPathFileListData = downFileInfo.fileData.data;
            List<DownFileInfo> downFileInfos = GetFileList(downFileInfo.fileData.text, PathDefine.ReadPath);
            StartCoroutine(DownLoadFile(downFileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));
        }

        /// <summary>
        /// 需要释放的资源 下载完成回调
        /// </summary>
        private void OnReleaseFileComplete(DownFileInfo downFileInfo)
        {
            Debug.Log("OnReleaseFileComplete -> url：" + downFileInfo.url);
            // 将资源写入可读写目录
            string writeFile = Path.Combine(PathDefine.ReadWritePath, downFileInfo.fileName);
            FileUtil.WriteFile(writeFile, downFileInfo.fileData.data);
        }

        /// <summary>
        /// 所有需要释放的资源 下载完成回调
        /// </summary>
        private void OnReleaseAllFileComplete()
        {
            Debug.Log("OnReleaseAllFileComplete");
            // 最后将版本文件写入可读写目录, 可用于操作中断后处理
            string writeFile = Path.Combine(PathDefine.ReadWritePath, ConstDefine.FileListName);
            FileUtil.WriteFile(writeFile, m_ReadPathFileListData);

            CheckUpdate();
        }

        /// <summary>
        /// 检测更新
        /// </summary>
        private void CheckUpdate()
        {
            string url = Path.Combine(ConstDefine.ResourceUrl, ConstDefine.FileListName);
            DownFileInfo downFileInfo = new DownFileInfo() { url = url };
            StartCoroutine(DownLoadFile(downFileInfo, OnDownLoadServerFileListComplete));
        }

        /// <summary>
        /// 服务器版本文件 下载完成回调
        /// </summary>
        private void OnDownLoadServerFileListComplete(DownFileInfo downFileInfo)
        {
            m_ServerFileListData = downFileInfo.fileData.data;
            List<DownFileInfo> downFileInfos = GetFileList(downFileInfo.fileData.text, ConstDefine.ResourceUrl);
            // 需要下载的文件列表
            List<DownFileInfo> needDownListFiles = new List<DownFileInfo>();

            for (int i = 0; i < downFileInfos.Count; i++)
            {
                string localFile = Path.Combine(PathDefine.ReadWritePath, downFileInfos[i].fileName);

                if (!FileUtil.IsExists(localFile))
                {
                    downFileInfos[i].url = Path.Combine(ConstDefine.ResourceUrl, downFileInfos[i].fileName);
                    needDownListFiles.Add(downFileInfos[i]);
                }
            }

            if (needDownListFiles.Count > 0)
                StartCoroutine(DownLoadFile(needDownListFiles, OnUpdateFileComplete, OnUpdateAllFileComplete));
            else
                EnterGame();
        }

        /// <summary>
        /// 服务器资源文件 下载完成回调
        /// </summary>
        /// <param name="downFileInfo"></param>
        private void OnUpdateFileComplete(DownFileInfo downFileInfo)
        {
            Debug.Log("OnUpdateFileComplete -> url：" + downFileInfo.url);
            string writeFile = Path.Combine(PathDefine.ReadWritePath, downFileInfo.fileName);
            FileUtil.WriteFile(writeFile, downFileInfo.fileData.data);
        }

        /// <summary>
        /// 所有服务器资源文件 下载完成回调
        /// </summary>
        private void OnUpdateAllFileComplete()
        {
            Debug.Log("OnUpdateAllFileComplete");
            string writeFile = Path.Combine(PathDefine.ReadWritePath, ConstDefine.FileListName);
            FileUtil.WriteFile(writeFile, m_ServerFileListData);
            EnterGame();
        }

        /// <summary>
        /// 进入游戏
        /// </summary>
        private void EnterGame()
        {
            Debug.Log("EnterGame");
            
            Manager.ResourceManager.ParseVersionFile();
            Manager.ResourceManager.LoadUI("UIMain", (obj) =>
            {
                GameObject go = Instantiate(obj) as GameObject;
                go.transform.SetParent(GameObject.Find("Canvas").transform);
                go.SetActive(true);
                go.name = obj.name;
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = Vector3.one;
                RectTransform rectTransform = go.transform.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.offsetMin = Vector2.zero;
                }
            });
        }
    }
}