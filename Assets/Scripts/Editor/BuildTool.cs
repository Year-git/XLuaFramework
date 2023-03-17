using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

// 关于AssetBundle中BuildAssetBundleOptions各个选项的说明：
// BuildAssetBundleOptions.None：使用LZMA算法压缩，压缩的包更小，但是加载时间更长。使用 之前需要整体解压。一旦被解压，这个包会使用LZ4重新压缩。使用资源的时候不需要整体解压。 在下载的时候可以使用LZMA算法，一旦它被下载了之后，它会使用LZ4算法保存到本地上； 
// BuildAssetBundleOptions.UncompressedAssetBundle：不压缩，包大，加载快； 
// BuildAssetBundleOptions.ChunkBasedCompression：使用LZ4压缩，压缩率没有LZMA高，但是 我们可以加载指定资源而不用解压全部注意使用LZ4压缩，可以获得可以跟不压缩想媲美的加载速度，而且比不压缩文件要小。 BuildTarget选择build出来的AB包要使用的平台

/// <summary>
/// 构建AssetBundle包
/// </summary>
public class BuildTool : Editor
{
    [MenuItem("Tools/Build Bundle/StandaloneWindows")]
    static void BuildBundleWindows()
    {
        Build(BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build Bundle/Android")]
    static void BuildBundleAndroid()
    {
        Build(BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    // [MenuItem("Tools/Build Bundle/iOS")]
    // static void BuildBundleIOS()
    // {
    //     Build(BuildAssetBundleOptions.None, BuildTarget.iOS);
    // }

    static void Build(BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        List<string> bundleInfos = new List<string>();
        string[] files = Directory.GetFiles(PathUtil.AssetsPackagePath, "*", SearchOption.AllDirectories);
        // 获取AssetsPackage目录下的资源
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta")) continue;

            string fileName = PathUtil.GetStandardPath(files[i]);
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();

            // 资源文件名 == Assets下的资源路径
            string assetName = PathUtil.GetUnityPath(fileName);
            assetBundleBuild.assetNames = new string[] { assetName };

            // 资源bundle名 == 输出文件夹下的相对路径
            string bundleName = fileName.Replace(PathUtil.AssetsPackagePath, "").ToLower();
            assetBundleBuild.assetBundleName = bundleName + AppConst.BundleExtension;

            assetBundleBuilds.Add(assetBundleBuild);

            // 添加文件依赖信息
            List<string> dependenceInfo = GetDependence(assetName);
            string bundleInfo = assetName + "|" + assetBundleBuild.assetBundleName;
            if (dependenceInfo.Count > 0)
                bundleInfo = bundleInfo + "|" + string.Join("|", dependenceInfo);

            bundleInfos.Add(bundleInfo);

            Debug.Log("-><color=#9AEBA3>" + "assetName -> " + assetName + "</color>");
        }

        // 构建bundle输出目录
        if (Directory.Exists(PathUtil.BundleOutPath))
            Directory.Delete(PathUtil.BundleOutPath, true);

        Directory.CreateDirectory(PathUtil.BundleOutPath);

        // 构建AssetBundles
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), assetBundleOptions, targetPlatform);
        Debug.Log("-><color=#9AEBA3>" + "BundleOutPath -> " + PathUtil.BundleOutPath + "</color>");
        Debug.Log("-><color=#9AEBA3>" + "Building Success!" + "</color>");

        // 构建依赖文件
        File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName, bundleInfos);

        // 刷新目录
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取依赖文件列表
    /// </summary>
    /// <param name="curFile"></param>
    /// <returns></returns>
    static List<string> GetDependence(string curFile)
    {
        List<string> dependence = new List<string>();
        string[] files = AssetDatabase.GetDependencies(curFile);
        dependence = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToList();
        return dependence;
    }
}
