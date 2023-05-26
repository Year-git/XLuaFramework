using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Manager : MonoBehaviour
    {
        private static HotUpdateManager _hotUpdateManager;
        public static HotUpdateManager HotUpdateManager => _hotUpdateManager;

        private static ResourceManager _resourceManager;
        public static ResourceManager ResourceManager => _resourceManager;

        private static LuaManager _luaManager;
        public static LuaManager LuaManager => _luaManager;

        void Awake()
        {
            _hotUpdateManager = gameObject.AddComponent<HotUpdateManager>();
            _resourceManager = gameObject.AddComponent<ResourceManager>();
            _luaManager = gameObject.AddComponent<LuaManager>();
        }
    }
}