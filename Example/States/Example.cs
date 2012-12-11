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

            // Create our player and add it to the state
            player = new Player(new Vector2f(100, 100));
            Entities.Add(player);

            var random = new Random();
            
            const int width = 100;
            const int height = 100;

            // Initialize the state's tilemap
            Map = new TileMap(width, height, Assets.LoadTexture("Tiles.png"));

            // Fill the TileMap with random tiles
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (y == 0 || x == 0 || x == width - 1 || y == height - 1)
                    {
                        // Border tiles so the player can't accidentally leave the map
                        Map[x, y] = new Tile(0, true);
                    }
                    else if (x > 20 || y > 20)
                    {
                        // Leave an empty start region so the player doesn't get stuck
                        var tile = random.Next(3);

                        // 20% chance of a tile being non-air, if the tile is red (zero), we give it
                        // a non-null userdata which will make it jumpthrough in this example
                        if (random.NextDouble() > 0.8)
                            Map[x, y] = new Tile(1 + tile, true, tile == 0 ? (object)1 : null);
                    }
                }
            }

            // Create a bunch of ramdom coins around the map
            for (var i = 0; i < 250; i++)
            {
                Entities.Add(new Coin(random.Next(width * GameOptions.TileSize), random.Next(height * GameOptions.TileSize)));
            }

            // Pressing LMB will erase the tile that was clicked
            Input.MouseButton[Mouse.Button.Left] = args =>
            {
                if (!args.Pressed) return true;

                var x = (int)args.Position.X / GameOptions.TileSize;
                var y = (int)args.Position.Y / GameOptions.TileSize;

                if (x >= 0 && x < Map.Width && y >= 0 && y < Map.Height)
                    Map[x, y] = new Tile(500, false);

                // Tile 500 does not exist on the tile atlas and will render nothing

                return true;
                
            };
        }

        public override void Update()
        {
            // Make the camera follow the player
            Camera.Position = player.Position;
        }

        public override void InitializeCamera()
        {
            base.InitializeCamera();
            Camera.Zoom(0.5f);
        }
    }
}
