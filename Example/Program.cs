using Californium;

namespace Example
{
    class Program
    {
        // Score needs to be accessed by Player and ScorePanel, so lets put it here
        public static int Score = 0;

        static void Main()
        {
            // Tiles are 8x8
            GameOptions.TileSize = 8;

            // 720p window size by default
            GameOptions.Width = 1280;
            GameOptions.Height = 720;

            // Done setting options, initialize
            Game.Initialize();

            // Base state is the actual game
            Game.SetState(new States.Example());

            // HUD goes above the game, so we push it on top
            Game.PushState(new States.UserInterface());

            // Finally start the game loop
            Game.Run();
        }
    }
}
