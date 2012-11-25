using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Californium;
using SFML.Graphics;
using SFML.Window;
using Example.Entities;

namespace Example.States
{
    class Test : State
    {
        private Player player;

        public Test()
        {
            const int width = 100;
            const int height = 100;

            player = new Player(new Vector2f(100, 100));

            Entities.Add(player);

            var random = new Random(0);
            Map = new TileMap(width, height, "Tiles.png");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y == 0 || x == 0 || x == width - 1 || y == height - 1)
                    {
                        Map[x, y] = new Tile(0, false);
                    }
                    else if (x > 20 || y > 20)
                    {
                        if (random.NextDouble() > 0.8)
                            Map[x, y] = new Tile(1 + random.Next(3), true);
                    }
                }
            }
        }

        public override void Update(float dt)
        {
            Camera.Position = player.Position;
        }

        public override bool ProcessEvent(InputArgs args)
        {
            var e = args as MouseButtonInputArgs;
            if (e != null && e.Button == Mouse.Button.Left && e.Pressed)
            {
                int x = (int)e.Position.X / GameOptions.TileSize;
                int y = (int)e.Position.Y / GameOptions.TileSize;

                if (x >= 0 && x < Map.Width && y >= 0 && y < Map.Height)
                    Map[x, y] = new Tile(500, false);

                return true;
            }
            
            return base.ProcessEvent(args);
        }
    }
}
