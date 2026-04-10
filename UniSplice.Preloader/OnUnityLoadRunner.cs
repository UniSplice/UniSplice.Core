using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UniSplice.Preloader
{
    public class OnUnityLoadRunner : MonoBehaviour
    {
        public static bool Exists;
        void Start()
        {
            Exists = true;
            AndroidLog.Info("OnUnityLoadRunner is Working!");
        }
        
        void Update()
        {
            RunQueued();
        }

        public void RunQueued()
        {
            // Snapshot in case methods enqueue more methods
            var methods = new List<MethodInfo>(AssemblyLoader.OnUnityLoadMethods);
            AssemblyLoader.OnUnityLoadMethods.Clear();

            foreach (var info in methods)
            {
                try
                {
                    info.Invoke(null, null);
                }
                catch (Exception e)
                {
                    AndroidLog.Error($"Exception running OnUnityLoad method {info.DeclaringType}.{info.Name}: {e}");
                }
            }

            AndroidLog.Info("Done loading OnUnityLoad!");

            Destroy(gameObject);
        }
    }
}