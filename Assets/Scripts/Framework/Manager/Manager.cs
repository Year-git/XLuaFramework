using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Manager : MonoBehaviour
    {
        private static HotUpdateManager _hotUpdateManager;
        public static HotUpdateManager HotUpdateManager { get { return _hotUpdateManager; } }

        private static ResourceManager _resourceManager;
        public static ResourceManager ResourceManager { get { return _resourceManager; } }

        void Awake()
        {
            _hotUpdateManager = this.gameObject.AddComponent<HotUpdateManager>();
            _resourceManager = this.gameObject.AddComponent<ResourceManager>();
        }
    }
}