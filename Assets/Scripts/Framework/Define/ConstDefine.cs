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
        // bundle后缀
        public const string BundleExtension = ".ab";

        // 版本文件名
        public const string FileListName = "filelist.txt";

        // 游戏环境模式
        public static GameMode GameMode = GameMode.EditorMode;
    }
}