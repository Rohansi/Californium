using Californium;

namespace Example
{
    class Program
    {
        static void Main()
        {
            GameOptions.TileSize = 8;

            GameOptions.Width = 1280;
            GameOptions.Height = 720;

            Game.Initialize();
            Game.SetState(new States.Example());
            Game.PushState(new States.UserInterface());
            Game.Run();
        }
    }
}
