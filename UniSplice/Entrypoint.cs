using System;
using System.Collections;
using System.IO;
using System.Text;
using UniSplice.Preloader;
using UnityEngine;

namespace UniSplice
{
    static class Entrypoint
    {
        private static FileStream logFile;
        private static Logger _logger;
        private static GameObject _uniObject;
        
        private static bool _initialized;
        
        private static bool _CreateLogFile()
        {
            string latestLogPath = Path.Combine(GameData.LogPath, "latest.log");

            AndroidLog.Info("Preparing Logger...", "UniSplice");
            if (File.Exists(latestLogPath))
            {
                try
                {
                    // Move old File away
                    string archivedName = File.GetLastWriteTime(latestLogPath).ToString("dd.MM.yy_HH-mm-ss") + ".log";
                    File.Move(latestLogPath, Path.Combine(GameData.LogPath, archivedName));
                }
                catch
                {
                    // File is locked or can't be moved, just skip logging
                    logFile = null;
                    return false;
                }
            }
            
            try
            {
                logFile = new FileStream(latestLogPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        private static void Initialize()
        {
            AndroidLog.Info("Main Entrypoint Called!", "UniSplice");
        }

        private static void OnUnityLoad()
        {
            if (_initialized) return;
            _initialized = true;
            
            AndroidLog.Info("Loading...", "UniSplice");
            
            // Init Game Identifier
            GameData.GameIdentifier = UniSpliceHelper.GameIdentifier;

            // Init Paths
            GameData.RootPath = UniSpliceHelper.UniSplicePath;
            GameData.GamePath = UniSpliceHelper.GameFolder;

            GameData.ConfigPath = Path.Combine(GameData.GamePath, "config/");
            GameData.ModsPath = Path.Combine(GameData.GamePath, "mods/");
            GameData.LogPath = Path.Combine(GameData.GamePath, "log/");

            // Create paths if missing
            Directory.CreateDirectory(GameData.ConfigPath);
            Directory.CreateDirectory(GameData.ModsPath);
            Directory.CreateDirectory(GameData.LogPath);
            
            // Rotate log - move old latest.log to timestamped file
            string latestLogPath = Path.Combine(GameData.LogPath, "latest.log");

            if (!_CreateLogFile())
            {
                AndroidLog.Error("Logger Creation Failed! Exiting...", "UniSplice");
            }
            
            if (_logger != null) return;
            _logger = new Logger("UniSplice");
            
            AndroidLog.Info("Logging Started!", "UniSplice");
            _logger.LogInfo($"UniSplice {UniSplice.Version} Loading...");
            _logger.LogInfo($"Unity Version: {Application.unityVersion}");

            AndroidLog.Info("Creating Loader Object...");
            _uniObject = new GameObject();
            var comp = _uniObject.AddComponent<UniSpliceBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(_uniObject);

            
            try{
                // Initialize Unity Logger
                UnityLogger.Initialize();
                UniSpliceBehaviour.loadingText = "Logging";

                // Start loading mods as coroutine
                UniSpliceBehaviour.loadingText = "Mods...";
                comp.StartCoroutine(LoadModsAndFinish());
            }
            catch (Exception ex)
            {
                _logger.LogError("Initialize failed: " + ex.ToString());
                _logger.LogError("Component Null?: " + (comp == null));
                Application.Quit();
            }
        }
        
        private static IEnumerator LoadModsAndFinish()
        {
            yield return _uniObject.GetComponent<UniSpliceBehaviour>().StartCoroutine(ModLoader.LoadMods());
            
            // Done loading
            UniSpliceBehaviour.loadingText = "Done!";
            yield return null;
            
            // Cleanup
            UnityEngine.Object.Destroy(_uniObject);
        }
        
        internal static void WriteToLogFile(string value)
        {
            if (logFile == null) return;
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            logFile.Write(bytes, 0, bytes.Length);
            logFile.Flush();
        }
        
    }
}