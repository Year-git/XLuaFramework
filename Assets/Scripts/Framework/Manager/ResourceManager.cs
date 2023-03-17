using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        string url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
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
        string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);
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

    public void LoadAsset(string assetName, Action<UnityEngine.Object> callback = null)
    {
        StartCoroutine(LoadBundleAsync(assetName, callback));
    }

    void Start()
    {
        // 测试
        ParseVersionFile();
        LoadAsset("Assets/AssetsPackage/UI/Prefab/UIMain.prefab", (obj) =>
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
