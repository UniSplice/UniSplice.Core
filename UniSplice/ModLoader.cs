using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UniSplice.Preloader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniSplice
{
    public static class ModLoader
    {
        private static GameObject _sharedModObject;
        private static readonly Dictionary<Type, Logger> PendingLoggers = new Dictionary<Type, Logger>();
        private static bool loaded;
        
        public static IEnumerator LoadMods()
        {
            if (loaded) yield break;
            _sharedModObject = new GameObject("UniSpliceMods");
            Object.DontDestroyOnLoad(_sharedModObject);

            var modsPath = GameData.ModsPath;
            if (!Directory.Exists(modsPath))
            {
                yield return null;
            }

            var modFiles = Directory.GetFiles(modsPath, "*.dll", SearchOption.AllDirectories);
            var loadedAssemblies = new Dictionary<string, Assembly>();
            int totalMods = modFiles.Length;

            // First pass: load all assemblies into resolver
            foreach (var file in modFiles)
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    var asm = Assembly.Load(File.ReadAllBytes(file));
                    Preloader.AssemblyLoader.AddAssembly(fileName, asm);
                    loadedAssemblies[file] = asm;
                    AndroidLog.Info($"[ModLoader] Loaded assembly: {asm.GetName().Name} from {fileName}", "UniSplice");
                }
                catch (Exception ex)
                {
                    AndroidLog.Info($"[ModLoader] Failed to load assembly {file}: {ex.Message}", "UniSplice");
                }
            }

            yield return null;

            // Second pass: find and instantiate mod classes
            int loadedCount = 0;

            foreach (var kvp in loadedAssemblies)
            {
                var file = kvp.Key;
                var asm = kvp.Value;

                try
                {
                    Type[] types;
                    try
                    {
                        types = asm.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        types = ex.Types;
                    }

                    foreach (var type in types)
                    {
                        if (type == null || !type.IsClass || type.IsAbstract || !type.IsSubclassOf(typeof(UniSpliceMod)))
                            continue;
                        
                        var modInfo = (ModInfoAttribute)Attribute.GetCustomAttribute(type, typeof(ModInfoAttribute));
                        if (modInfo == null)
                            continue;

                        AndroidLog.Info($"[ModLoader] Found mod: {modInfo.Name} ({type.FullName})", "UniSplice");

                        var logger = new Logger(modInfo.Name);
                        PendingLoggers[type] = logger;

                        _sharedModObject.AddComponent(type);

                        PendingLoggers.Remove(type);

                        loadedCount++;
                        UniSpliceBehaviour.loadingText = $"Mods {loadedCount}/{totalMods}";
                    }
                }
                catch (Exception ex)
                {
                    AndroidLog.Info($"[ModLoader] Failed to load mod from {file}: {ex.Message}", "UniSplice");
                }

                yield return null;
            }

            loaded = true;
        }

        internal static bool TryGetLogger(Type modType, out Logger logger)
        {
            return PendingLoggers.TryGetValue(modType, out logger);
        }
    }
}
