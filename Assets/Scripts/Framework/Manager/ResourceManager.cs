using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        /// <summary>
        /// Bundle信息
        /// </summary>
        internal class BundleInfo
        {
            public string AssetsName;
            public string BundleName;
            public List<string> Dependences;
        }

        private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
        private void ParseVersionFile()
        {
            // 版本文件路径
            string url = Path.Combine(PathDefine.BundleResourcePath, ConstDefine.FileListName);
            string[] data = File.ReadAllLines(url);

            // 获取bundle信息
            for (int i = 0; i < data.Length; i++)
            {
                BundleInfo bundleInfo = new BundleInfo();
                string[] info = data[i].Split("|");

                bundleInfo.AssetsName = info[0];
                bundleInfo.BundleName = info[1];
                bundleInfo.Dependences = new List<string>(info.Length - 2);
                for (int j = 2; j < info.Length; j++)
                {
                    bundleInfo.Dependences.Add(info[j]);
                }
                m_BundleInfos.Add(bundleInfo.AssetsName, bundleInfo);
            }
        }

        /// <summary>
        /// 异步加载bundle资源
        /// </summary>
        /// <param name="assetName">资源路径名</param>
        /// <param name="callback">加载完成回调</param>
        /// <returns></returns>
        IEnumerator LoadBundleAsync(string assetName, Action<UnityEngine.Object> callback = null)
        {
            BundleInfo bundleInfo = m_BundleInfos[assetName];
            string bundleName = bundleInfo.BundleName;
            string bundlePath = Path.Combine(PathDefine.BundleResourcePath, bundleName);
            List<string> dependences = bundleInfo.Dependences;

            // 优先加载依赖bundle资源
            for (int i = 0; i < dependences.Count; i++)
            {
                yield return LoadBundleAsync(dependences[i]);
            }

            // 从磁盘上的文件异步加载AssetBundle
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return createRequest;

            // 从bundle中异步加载指定类型的资产
            AssetBundleRequest bundleRequest = createRequest.assetBundle.LoadAssetAsync(assetName);
            yield return bundleRequest;

            Debug.Log("LoadBundleAsync -> Load success! -> assetName = " + assetName);
            callback?.Invoke(bundleRequest?.asset);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器环境加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        private void EditorLoadAsset(string assetName, Action<UnityEngine.Object> callback = null)
        {
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
            if (obj == null)
            {
                Debug.LogError("EditorLoadAsset -> The asset is not found! -> assetName = " + assetName);
                return;
            }
            Debug.Log("EditorLoadAsset -> Load success! -> assetName = " + assetName);
            callback?.Invoke(obj);
        }
#endif

        private void LoadAsset(string assetName, Action<UnityEngine.Object> callback = null)
        {
            switch (ConstDefine.GameMode)
            {
                case GameMode.EditorMode:
#if UNITY_EDITOR
                    EditorLoadAsset(assetName, callback);
#endif
                    break;
                case GameMode.AssetBundleMode:
                    StartCoroutine(LoadBundleAsync(assetName, callback));
                    break;
                case GameMode.UpdateMode:
                    StartCoroutine(LoadBundleAsync(assetName, callback));
                    break;
                default:
                    Debug.LogError("LoadAsset -> ConstDefine.GameMode is not");
                    break;
            }
        }

        public void LoadMusic(string assetName, Action<UnityEngine.Object> callback = null)
        {
            LoadAsset(PathDefine.GetMusicPath(assetName), callback);
        }

        public void LoadSound(string assetName, Action<UnityEngine.Object> callback = null)
        {
            LoadAsset(PathDefine.GetSoundPath(assetName), callback);
        }

        public void LoadEffect(string assetName, Action<UnityEngine.Object> callback = null)
        {
            LoadAsset(PathDefine.GetEffectPath(assetName), callback);
        }

        public void LoadLuaScript(string assetName, Action<UnityEngine.Object> callback = null)
        {
            LoadAsset(PathDefine.GetLuaScriptPath(assetName), callback);
        }

        public void LoadModel(string assetName, Action<UnityEngine.Object> callback = null)
        {
            LoadAsset(PathDefine.GetModelPath(assetName), callback);
        }

        public void LoadScene(string assetName, Action<UnityEngine.Object> callback = null)
        {
            LoadAsset(PathDefine.GetScenePath(assetName), callback);
        }

        public void LoadUI(string assetName, Action<UnityEngine.Object> callback = null)
        {
            LoadAsset(PathDefine.GetUIPath(assetName), callback);
        }

        // todo 卸载Bundle

        void Start()
        {
            // 测试
            ParseVersionFile();
            LoadUI("UIMain", (obj) =>
            {
                GameObject go = Instantiate(obj) as GameObject;
                go.transform.SetParent(this.transform);
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