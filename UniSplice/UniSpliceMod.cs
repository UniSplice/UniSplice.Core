using UnityEngine;

namespace UniSplice
{
    public class UniSpliceMod : MonoBehaviour
    {
        public Logger Logger { get; internal set; }

        protected void Awake()
        {
            if (Logger == null)
            {
                ModLoader.TryGetLogger(GetType(), out var logger);
                Logger = logger;
            }
            ModAwake();
        }

        public virtual void ModAwake() { }
    }
}