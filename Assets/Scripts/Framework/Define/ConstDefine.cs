using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 常量定义
    /// </summary>
    public static class ConstDefine
    {
        /// <summary>
        /// 游戏环境模式
        /// </summary>
        public static GameMode GameMode = GameMode.EditorMode;

        /// <summary>
        /// bundle后缀
        /// </summary>
        public const string BundleExtension = ".ab";

        /// <summary>
        /// 版本文件名
        /// </summary>
        public const string FileListName = "filelist.txt";

        /// <summary>
        /// 热更新资源地址
        /// </summary>
        public static string ResourceUrl = "http://game-yxy.gyyx.cn/HeroBallZ/TestAssetBundles"; //"http://127.0.0.1/AssetBundle";
    }
}