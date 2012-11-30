using System;
using Californium;
using SFML.Graphics;
using SFML.Window;
using Example.Entities;

namespace Example.States
{
    class Example : State
    {
        private Player player;

        public Example()
        {
            ClearColor = new Color(100, 149, 237);

            const int width = 100;
            const int height = 100;

            player = new Player(new Vector2f(100, 100));

            Entities.Add(player);

            var random = new Random();
            Map = new TileMap(width, height, "Tiles.png");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y == 0 || x == 0 || x == width - 1 || y == height - 1)
                    {
                        Map[x, y] = new Tile(0, true);
                    }
                    else if (x > 20 || y > 20)
                    {
                        if (random.NextDouble() > 0.9)
                            Map[x, y] = new Tile(1 + random.Next(3), true);
                    }
                }
            }

            Input.MouseButton[Mouse.Button.Left] = args =>
            {
                int x = (int)args.Position.X / GameOptions.TileSize;
                int y = (int)args.Position.Y / GameOptions.TileSize;

                if (x >= 0 && x < Map.Width && y >= 0 && y < Map.Height)
                    Map[x, y] = new Tile(500, false);

                return true;
                
            };
        }

        public override void Update()
        {
            Camera.Position = player.Position;
        }

        public override void InitializeCamera()
        {
            base.InitializeCamera();
            Camera.Zoom(0.5f);
        }
    }
}
