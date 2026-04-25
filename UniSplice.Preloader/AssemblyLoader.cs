using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UniSplice.Preloader
{
    public static class AssemblyLoader
    {
        private static Dictionary<string, Assembly> _loadedAssemblies = new();

        internal static List<MethodInfo> OnUnityLoadMethods = new();
        
        public static int? LoadAllModules(string version)
        {
            var dir = Path.Combine(UniSpliceHelper.UniSplicePath, "core/");

            if (version == null)
            {
                var versions = Directory.GetDirectories(dir)
                    .Select(Path.GetFileName)
                    .Select(TryParseVersion)
                    .Where(v=>v!=null)
                    .OrderByDescending(v=>v)
                    .ToList();
                
                // Get Highest Version and make sure to discard anything without X.Y.Z format

                if (!versions.Any()) return null;

                var highestVersion = versions.First();

                return LoadModulesIn(Path.Combine(dir, highestVersion.OriginalString));
            }else{
                return LoadModulesIn(Path.Combine(dir,version));
            }
        }

        private static int LoadModulesIn(string path)
        {
            var allFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            
            // Sort so that UniSplice.dll comes first
            allFiles = allFiles
                .OrderBy(f => Path.GetFileName(f).Equals("UniSplice.dll", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(f => f, StringComparer.OrdinalIgnoreCase) // optional: keep consistent order for others
                .ToArray();
            
            // Load Assemblies
            foreach (var file in allFiles)
            {
                var fileName = Path.GetFileName(file);
                
                // Skip if already loaded
                if (_loadedAssemblies.ContainsKey(fileName))
                    continue;
                    
                var asm = Assembly.Load(File.ReadAllBytes(file));
                _loadedAssemblies[fileName] = asm;
                
                var entrypoint = asm.GetType("Entrypoint");
                if (entrypoint == null)
                {
                    // fallback: scan all types
                    entrypoint = asm.GetTypes()
                        .FirstOrDefault(t => t.Name == "Entrypoint" &&
                                             t.GetMethod("Initialize",
                                                 BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic) != null);
                }

                if (entrypoint == null)
                    continue;
                
                var method = entrypoint.GetMethod("Initialize",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                
                method?.Invoke(null, null);

                var onUnityLoad = entrypoint.GetMethod("OnUnityLoad",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (onUnityLoad != null)
                    OnUnityLoadMethods.Add(onUnityLoad);

            }

            return allFiles.Length;
        }

        public static void AddAssembly(string name, Assembly assembly)
        {
            _loadedAssemblies[name] = assembly;
        }

        public static void LinkResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var requestedName = new AssemblyName(args.Name).Name;
                
                foreach (var asm in _loadedAssemblies.Values)
                {
                    if (asm.GetName().Name == requestedName)
                    {
                        return asm;
                    }
                }
                

                return null;
            };
        }
        
        
        private class ParsedVersion : IComparable<ParsedVersion>
        {
            public int Major, Minor, Patch;
            public string OriginalString;

            public int CompareTo(ParsedVersion other)
            {
                if (Major != other.Major) return Major.CompareTo(other.Major);
                if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
                return Patch.CompareTo(other.Patch);
            }
        }

        private static ParsedVersion TryParseVersion(string s)
        {
            var parts = s.Split('.');
            if (parts.Length != 3) return null;

            if (int.TryParse(parts[0], out var major) &&
                int.TryParse(parts[1], out var minor) &&
                int.TryParse(parts[2], out var patch))
            {
                return new ParsedVersion { Major = major, Minor = minor, Patch = patch, OriginalString = s };
            }

            return null;
        }
    }
}