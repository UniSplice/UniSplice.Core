namespace UniSplice
{
    public static class GameData
    {
        /// <summary>
        /// The Root Path of UniSplice (usually /sdcard/UniSplice)
        /// </summary>
        public static string RootPath { get; internal set; }
        
        /// <summary>
        /// The Path of your Actual Game (<see cref="RootPath"/>/games/<see cref="GameIdentifier"/>)
        /// </summary>
        public static string GamePath { get; internal set; }
        
        /// <summary>
        /// The Path of the Game Mods (<see cref="GamePath"/>/mods)
        /// </summary>
        public static string ModsPath { get; internal set; }
        
        /// <summary>
        /// The Path of the Mod Configuration (<see cref="GamePath"/>/config)
        /// </summary>
        public static string ConfigPath { get; internal set; }
        
        /// <summary>
        /// The Path of the Game Logs (<see cref="GamePath"/>/logs)
        /// </summary>
        public static string LogPath { get; internal set; }
        
        public static string GameIdentifier { get; internal set; }
        
    }
}