using System;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace UniSplice.Preloader
{
    public static class Entry
    {
        private static bool _initialized;
        
        public static void Initialize()
        {
            // No Multi-Initialization
            if (_initialized) return;
            _initialized = true;
            AndroidLog.Info("Loading...");
            
            // Set up Basic Paths
            UniSpliceHelper.UniSplicePath = "/sdcard/UniSplice/";
            UniSpliceHelper.GameIdentifier = Application.identifier;

            AndroidLog.Info("Setting up Game...");
            
            // Add as Game Assembly
            AssemblyLoader.AddAssembly("UniSplice.Preloader.dll", Assembly.GetCallingAssembly());
            
            // Create Game Folder /sdcard/UniSplice/games/GAME_PACKAGE
            CreateGameFolder();
            
            LoadModules();

            StartOnUnityLoadThread();
        }

        private static void StartOnUnityLoadThread()
        {
            // Here, spamming unity a lot shouldn't cause issues.
            // The Game has not loaded in any way, i.e. this shouldn't cause a negative performance hit.
            new Thread(() =>
            {
                while (true)
                {
                    if (ThreadWaitForUnityEarly())
                        break;
                    Thread.Sleep(100); // Wait a short time before loading
                }
                
                AndroidLog.Info("Unity is almost Ready!");
                Thread.Sleep(300); // Avoid Race Conditions, especially with Splash Screens.
                
                AndroidLog.Info("Unity should be done Loading.");
                AndroidLog.Info("Preparing Loading of OnUnityLoad Functions!");
                AndroidLog.Info("Game Path: " + UniSpliceHelper.GameFolder);

                // Create our Preloader Checking Object
                var obj = new GameObject("TempUniSplicePreloaderObject", typeof(OnUnityLoadRunner));
                UnityEngine.Object.DontDestroyOnLoad(
                    obj
                );

                Thread.Sleep(500); // Avoid Splash Screen race conditions

                while (true)
                {
                    // Yes, we spam unity full of GameObject Existence checks.
                    Thread.Sleep(80);
                    if (OnUnityLoadRunner.Exists) break;
                    
                    // Ensure that the object exists
                    if (!obj)
                    {
                        AndroidLog.Info("Preloader: Object does not exist! Recreating!");
                        obj = new GameObject("TempUniSplicePreloaderObject", typeof(OnUnityLoadRunner));
                        UnityEngine.Object.DontDestroyOnLoad(
                            obj
                        );
                    }
                }

            }).Start();
        }

        
        /// <returns><b>true</b> once Unity is loaded, <b>false</b> if not</returns>
        private static bool ThreadWaitForUnityEarly()
        {
            try
            {
                GameObject.Find("");
                return true;
            }
            catch (MissingMethodException) { } // MissingMethodException = Unity is definitely not ready
            catch
            {
                return true;
            }

            return false;
        }

        private static void CreateGameFolder()
        {
            UniSpliceHelper.GameFolder = Path.Combine(UniSpliceHelper.UniSplicePath + "games", UniSpliceHelper.GameIdentifier);
            if(!Directory.Exists(UniSpliceHelper.GameFolder))
                Directory.CreateDirectory(UniSpliceHelper.GameFolder);
        }

        private static void LoadModules()
        {
            var versionInfo = Path.Combine(UniSpliceHelper.GameFolder, "config/UniSpliceVersion.txt");
            if (File.Exists(versionInfo))
            {
                var data = File.ReadAllText(versionInfo);
                var versionString = data.Trim();
                UniSpliceHelper.LoadedModules = AssemblyLoader.LoadAllModules(versionString) ?? 0;
                AndroidLog.Info("Preloader: Loaded Modules (Version Override): " + UniSpliceHelper.LoadedModules);
            }
            else
            {
                UniSpliceHelper.LoadedModules = AssemblyLoader.LoadAllModules(null) ?? 0;
                AndroidLog.Info("Preloader: Loaded Modules: " + UniSpliceHelper.LoadedModules);
            }
        }
        
    }
}