using System;
using UniSplice.Preloader;

namespace UniSplice
{
    public class Logger
    {
        private string _caller;
        private string _callerUsed;
        
        public Logger(string caller)
        {
            _caller = caller;
            _callerUsed = caller.PadRight(12);
        }

        private void DoLog(string message, string type)
        {
            string result = $"[ {_callerUsed} : {type,12} ] {message}";
            AndroidLog.Info(result, "UniSplice");
            Entrypoint.WriteToLogFile(result + "\n");
        }

        public void LogInfo(string message)
        {
            DoLog(message, "Info");
        }

        public void LogWarning(string message)
        {
            DoLog(message, "Warning");
        }

        public void LogError(string message)
        {
            DoLog(message, "Error");
        }
        
        public void LogFatal(string message)
        {
            DoLog(message, "Fatal");
        }
    }
}