using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public static class PathUtil
    {
        /// <summary>
        /// 获取Unity的相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUnityPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return path.Substring(path.IndexOf("Assets"));
        }

        /// <summary>
        /// 获取标准路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetStandardPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return path.Trim().Replace("\\", "/");
        }
    }
}