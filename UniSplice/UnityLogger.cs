using UnityEngine;

namespace UniSplice
{
    internal static class UnityLogger
    {
        private static Logger _logger;
        internal static void Initialize()
        {
            _logger = new Logger("Unity Log");

            Application.logMessageReceived += (message, stackTrace, type) =>
            {
                switch (type)
                {
                    case LogType.Log:
                        _logger.LogInfo(message);
                        break;
                    case LogType.Warning:
                        _logger.LogWarning(message);
                        break;
                    case LogType.Exception:
                    case LogType.Error:
                        _logger.LogError(message +"\n"+stackTrace);
                        break;
                }
            };
        }
    }
}