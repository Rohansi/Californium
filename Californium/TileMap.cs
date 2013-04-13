using System;
using SFML.Window;
using SFML.Graphics;

namespace Californium
{
    /// <summary>
    /// Simple tile. Can be inherited to have additional properties.
    /// </summary>
    public class Tile
    {
        public ushort Index;
        public bool Solid;
        public object UserData;

        public Tile()
        {
            Index = ushort.MaxValue;
            Solid = false;
            UserData = null;
        }

        public Tile(ushort index, bool solid, object userData = null)
        {
            Index = index;
            Solid = solid;
            UserData = userData;
        }
    }

    /// <summary>
    /// Base class for a tile map. Implements no rendering but can be used for collision checking.
    /// </summary>
    public class TileMap<T> where T : Tile, new()
    {
        public delegate bool CollisionCondition(T tile, FloatRect tileBounds, FloatRect collisionBounds);

        public int Width { get; private set; }
        public int Height { get; private set; }
        protected readonly int TileSize;

        protected readonly T[,] Tiles;

        protected TileMap(int width, int height, int tileSize)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;

            Tiles = new T[width,height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Tiles[x, y] = new T();
                }
            }
        }

        public virtual T this[int x, int y]
        {
            get { return Tiles[x, y]; }
            set { Tiles[x, y] = value; }
        }

        public void Draw(RenderTarget rt)
        {
            var view = rt.GetView();

            var startX = (int)Math.Max(0, (view.Center.X - (view.Size.X / 2)) / TileSize);
            var startY = (int)Math.Max(0, (view.Center.Y - (view.Size.Y / 2)) / TileSize);
            var endX = (int)Math.Ceiling(Math.Min(Width, startX + 1 + (view.Size.X / TileSize)));
            var endY = (int)Math.Ceiling(Math.Min(Height, startY + 1 + (view.Size.Y / TileSize)));

            Render(rt, startX, startY, endX, endY);
        }

        public virtual void Render(RenderTarget rt, int startX, int startY, int endX, int endY)
        {

        }

        public bool PlaceFree(FloatRect r, CollisionCondition cond = null)
        {
            var minX = Math.Max(0, ((int)r.Left / TileSize) - 1);
            var minY = Math.Max(0, ((int)r.Top / TileSize) - 1);
            var maxX = Math.Min(Width, minX + ((int)r.Width / TileSize) + 3);
            var maxY = Math.Min(Height, minY + ((int)r.Height / TileSize) + 3);
            var testRect = new FloatRect(0, 0, TileSize, TileSize);

            for (var yy = minY; yy < maxY; yy++)
            {
                for (var xx = minX; xx < maxX; xx++)
                {
                    if (!Tiles[xx, yy].Solid)
                        continue;

                    testRect.Left = xx * TileSize;
                    testRect.Top = yy * TileSize;

                    if (!r.Intersects(testRect))
                        continue;
                    if (cond == null)
                        return false;
                    if (cond(Tiles[xx, yy], testRect, r))
                        return false;
                }
            }

            return true;
        }
    }
}
