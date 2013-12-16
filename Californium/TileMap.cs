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

        /// <summary>
        /// Calculates the tile region that a view covers.
        /// </summary>
        public IntRect CalculateDrawRegion(RenderTarget rt)
        {
            var view = rt.GetView();
            var center = view.Center;
            var size = view.Size;

            var startX = (int)Math.Max(0, (center.X - (size.X / 2)) / TileSize);
            var startY = (int)Math.Max(0, (center.Y - (size.Y / 2)) / TileSize);
            var endX = (int)Math.Min(Width, (center.X + (size.X / 2)) / TileSize);
            var endY = (int)Math.Min(Height, (center.Y + (size.Y / 2)) / TileSize);

            return new IntRect(startX, startY, endX - startX, endY - startY);
        }

        public virtual void Draw(RenderTarget rt)
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
