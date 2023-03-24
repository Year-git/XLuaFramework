using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public static class FileUtil
    {
        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsExists(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Exists;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void WriteFile(string path, byte[] data)
        {
            path = PathUtil.GetStandardPath(path);
            string dir = path.Substring(0, path.LastIndexOf("/"));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileInfo fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(data, 0, data.Length);
                    fileStream.Close();
                }
            }
            catch (IOException e)
            {

                Debug.LogError(e.Message);
            }
        }
    }
}