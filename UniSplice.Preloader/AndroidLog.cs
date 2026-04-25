using System.Runtime.InteropServices;

namespace UniSplice.Preloader
{
    public static class AndroidLog
    {
        private const int ANDROID_LOG_INFO = 4;
        private const int ANDROID_LOG_ERROR = 6;

        [DllImport("log")] // liblog.so
        private static extern int __android_log_print(
            int priority,
            string tag,
            string fmt,
            string msg
        );

        public static void Info(string message, string tag="UniSplice.Preloader")
        {
            __android_log_print(ANDROID_LOG_INFO, tag, "%s", message);
        }

        public static void Error(string message, string tag="UniSplice.Preloader")
        {
            __android_log_print(ANDROID_LOG_ERROR, tag, "%s", message);
        }
    }
}