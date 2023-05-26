using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 路径定义
    /// Application.会产生GC,缓存下来减少GC
    /// </summary>
    public static class PathDefine
    {
        // 根目录
        public static readonly string AssetsPath = Application.dataPath;

        // 需要打bundle的资源路径
        public static readonly string AssetsPackagePath = AssetsPath + "/AssetsPackage/";

        // bundle输出路径
        public static readonly string BundleOutPath = Application.streamingAssetsPath;

        // 只读路径
        public static string ReadPath = Application.streamingAssetsPath;

        // 可读写路径
        public static string ReadWritePath = Application.persistentDataPath;

        // Lua脚本路径
        public static string LuaPath = "Assets/AssetsPackage/LuaScripts";

        // bundle资源路径
        public static string BundleResourcePath
        {
            get
            {
                if (ConstDefine.GameMode == GameMode.UpdateMode)
                    return ReadWritePath;
                else
                    return ReadPath;
            }
        }

        // 音乐资源路径
        public static string GetMusicPath(string name) => $"Assets/AssetsPackage/Audio/Music/{name}";
        // 音效资源路径
        public static string GetSoundPath(string name) => $"Assets/AssetsPackage/Audio/Sound/{name}";
        // 特效资源路径
        public static string GetEffectPath(string name) => $"Assets/AssetsPackage/Effect/Prefabs/{name}.prefab";
        // Lua脚本资源路径
        public static string GetLuaScriptPath(string name) => $"Assets/AssetsPackage/LuaScripts/{name}.bytes";
        // 模型资源路径
        public static string GetModelPath(string name) => $"Assets/AssetsPackage/Model/Prefabs/{name}.prefab";
        // 场景资源路径
        public static string GetScenePath(string name) => $"Assets/AssetsPackage/Scenes/{name}.unity";
        // UI资源路径
        public static string GetUIPath(string name) => $"Assets/AssetsPackage/UI/Prefabs/{name}.prefab";
    }
}