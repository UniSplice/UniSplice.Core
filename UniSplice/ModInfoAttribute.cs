using System;

namespace UniSplice
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModInfoAttribute : Attribute
    {
        public string Name { get; }
        public string Guid { get; }
        public string Version { get; }

        public ModInfoAttribute(string name, string guid, string version)
        {
            Name = name;
            Guid = guid;
            Version = version;
        }
    }
}
