using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace Framework
{
    public class LuaManager : MonoBehaviour
    {
        /// <summary>
        /// Lua路径列表
        /// </summary>
        public List<string> LuaNames = new List<string>();

        /// <summary>
        /// 缓存Lua脚本内容
        /// </summary>
        private Dictionary<string, byte[]> m_LuaScripts;

        /// <summary>
        /// Lua虚拟机
        /// </summary>
        public LuaEnv LuaEnv;

        void Awake()
        {
            LuaEnv = new LuaEnv();
            LuaEnv.AddLoader(Loader);
        }

        byte[] Loader(ref string name) => GetLuaScript(name);

        public byte[] GetLuaScript(string name)
        {
            name = name.Replace(".", "/");
            string fileName = PathDefine.GetLuaScriptPath(name);

            byte[] luaScript = null;

            if (!m_LuaScripts.TryGetValue(fileName, out luaScript))
                Debug.LogError($"lua script is not exist : {fileName}");

            return luaScript;
        }

        void LoadLuaScript()
        {
            foreach (string name in LuaNames)
            {
                Manager.ResourceManager.LoadLua(name, (Object obj) =>
                {
                    AddLuaScript(name, (obj as TextAsset).bytes);
                    if (m_LuaScripts.Count >= LuaNames.Count)
                    {
                        LuaNames.Clear();
                        LuaNames = null;
                    }
                });
            }
        }

        public void AddLuaScript(string assetName, byte[] luaScript)
        {
            m_LuaScripts[assetName] = luaScript;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器模式下 缓存Lua脚本
        /// </summary>
        void EditorLoadLuaScript()
        {
            string[] luaFiles = Directory.GetFiles(PathDefine.LuaPath, "*.bytes", SearchOption.AllDirectories);
            for (int i = 0; i < luaFiles.Length; i++)
            {
                string fileName = PathUtil.GetStandardPath(luaFiles[i]);
                byte[] file = File.ReadAllBytes(fileName);
                AddLuaScript(fileName, file);
            }
        }
#endif
    }
}