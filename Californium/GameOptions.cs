namespace Californium
{
    public static class GameOptions
    {
        /// <summary>
        /// Window caption
        /// </summary>
        public static string Caption = "";

        /// <summary>
        /// Window icon texture
        /// </summary>
        public static string Icon = "";

        /// <summary>
        /// Default window width
        /// </summary>
        public static uint Width = 800;

        /// <summary>
        /// Default window height
        /// </summary>
        public static uint Height = 600;

        /// <summary>
        /// Allow the window to be resized by the user
        /// </summary>
        public static bool Resizable = true;

        /// <summary>
        /// Framerate limit, 0 disables
        /// </summary>
        public static uint Framerate = 60;

        /// <summary>
        /// Vsync toggle
        /// </summary>
        public static bool Vsync = false;

        /// <summary>
        /// Time between Update calls
        /// </summary>
        public static float Timestep = 1f / 60;

        /// <summary>
        /// Maximum Update calls per frame. If exceeded, Game.Lagging will be called for correction.
        /// </summary>
        public static float MaxUpdatesPerFrame = 5;

        /// <summary>
        /// Music volume from 0 (mute) to 100
        /// </summary>
        public static float MusicVolume = 50;

        /// <summary>
        /// Sound volume from 0 (mute) to 100
        /// </summary>
        public static float SoundVolume = 100;

        /// <summary>
        /// Folder to load textures from
        /// </summary>
        public static string TextureLocation = "Assets/Texture/";

        /// <summary>
        /// Folder to load sounds from
        /// </summary>
        public static string SoundLocation = "Assets/Sound/";

        /// <summary>
        /// Folder to load music from
        /// </summary>
        public static string MusicLocation = "Assets/Music/";

        /// <summary>
        /// Folder to load fonts from
        /// </summary>
        public static string FontLocation = "Assets/Font/";

        /// <summary>
        /// Size of tiles in pixels
        /// </summary>
        public static int TileSize = 16;

        /// <summary>
        /// Size of TileMap chunks (in tiles), adjusting will affect performance
        /// </summary>
        public static int TileChunkSize = 32;

        /// <summary>
        /// Size of EntityManager cells, adjusting will affect performance
        /// </summary>
        public static int EntityGridSize = 64;

        /// <summary>
        /// Overscan (in pixels) for EntityManager.InArea. Increase if larger entities disappear near view edges. Decreasing will increase performance.
        /// </summary>
        public static float EntityOverscan = 64;

        /// <summary>
        /// Size of the internal SpriteBatch texture
        /// </summary>
        public static uint SpriteBatchSize = 512;
    }
}
