namespace UniSplice.Preloader
{
    public static class UniSpliceHelper
    {
        public static string UniSplicePath { get; internal set; }

        public static string GameIdentifier { get; internal set; }
        
        public static string GameFolder { get; internal set; }
        
        public static int LoadedModules { get; internal set; }
        
        public static bool IsUnityLoaded { get; internal set; }
    }
}