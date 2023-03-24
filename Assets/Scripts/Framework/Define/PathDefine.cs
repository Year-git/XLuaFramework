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
        public static string GetMusicPath(string name) { return string.Format("Assets/AssetsPackage/Audio/Music/{0}", name); }
        // 音效资源路径
        public static string GetSoundPath(string name) { return string.Format("Assets/AssetsPackage/Audio/Sound/{0}", name); }
        // 特效资源路径
        public static string GetEffectPath(string name) { return string.Format("Assets/AssetsPackage/Effect/Prefabs/{0}.prefab", name); }
        // Lua脚本资源路径
        public static string GetLuaScriptPath(string name) { return string.Format("Assets/AssetsPackage/LuaScripts/{0}.bytes", name); }
        // 模型资源路径
        public static string GetModelPath(string name) { return string.Format("Assets/AssetsPackage/Model/Prefabs/{0}.prefab", name); }
        // 场景资源路径
        public static string GetScenePath(string name) { return string.Format("Assets/AssetsPackage/Scenes/{0}.unity", name); }
        // UI资源路径
        public static string GetUIPath(string name) { return string.Format("Assets/AssetsPackage/UI/Prefabs/{0}.prefab", name); }
    }
}