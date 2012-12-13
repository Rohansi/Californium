using System;
using SFML.Window;
using SFML.Graphics;

namespace Californium
{
    public struct Tile
    {
        public ushort Index;
        public bool Solid;
        public object UserData;

        public Tile(ushort index, bool solid, object userData = null)
        {
            Index = index;
            Solid = solid;
            UserData = userData;
        }
    }

    public class TileMap
    {
        public delegate bool CollisionCondition(Tile tile, FloatRect tileBounds, FloatRect collisionBounds);

        private class Chunk : Drawable
        {
            public bool Dirty;

            private VertexArray vertexArray;
            private int chunkX, chunkY;
            private Tile[,] tiles;
            private Texture texture;
            private ushort lastTile;

            public Chunk(int chunkX, int chunkY, Tile[,] tiles, Texture texture)
            {
                this.tiles = tiles;
                this.chunkX = chunkX;
                this.chunkY = chunkY;
                this.texture = texture;

                lastTile = texture == null ? ushort.MaxValue : (ushort)((texture.Size.X / GameOptions.TileSize) * (texture.Size.Y / GameOptions.TileSize) - 1);

                Dirty = true;
            }

            private void Rebuild()
            {
                vertexArray = new VertexArray(PrimitiveType.Quads);

                var mapWidth = tiles.GetUpperBound(0) + 1;
                var mapHeight = tiles.GetUpperBound(1) + 1;

                var tileSize = GameOptions.TileSize;
                var tileMapWidth = (int)texture.Size.X / tileSize;
                var chunkSize = GameOptions.TileChunkSize;

                var startX = chunkX * chunkSize;
                var startY = chunkY * chunkSize;
                var endX = Math.Min(startX + chunkSize, mapWidth);
                var endY = Math.Min(startY + chunkSize, mapHeight);

                for (var y = startY; y < endY; y++)
                {
                    for (var x = startX; x < endX; x++)
                    {
                        if (tiles[x, y].Index >= lastTile) continue;

                        var last = vertexArray.VertexCount;
                        vertexArray.Resize(vertexArray.VertexCount + 4);

                        var itexX = (tiles[x, y].Index % tileMapWidth) * tileSize;
                        var itexY = (tiles[x, y].Index / tileMapWidth) * tileSize;

                        // HACK: SFML's weird rendering
                        var texX = itexX + 0.01f;
                        var texY = itexY - 0.01f;

                        vertexArray[last + 0] = new Vertex(new Vector2f(x * tileSize, y * tileSize),
                                                           new Vector2f(texX, texY));

                        vertexArray[last + 1] = new Vertex(new Vector2f((x * tileSize) + tileSize, y * tileSize),
                                                           new Vector2f(texX + tileSize, texY));

                        vertexArray[last + 2] = new Vertex(new Vector2f((x * tileSize) + tileSize, (y * tileSize) + tileSize),
                                                           new Vector2f(texX + tileSize, texY + tileSize));

                        vertexArray[last + 3] = new Vertex(new Vector2f(x * tileSize, (y * tileSize) + tileSize),
                                                           new Vector2f(texX, texY + tileSize));
                    }
                }

                Dirty = false;
            }

            public void Draw(RenderTarget target, RenderStates states)
            {
                if (Dirty)
                    Rebuild();

                states.Texture = texture;
                vertexArray.Draw(target, states);
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Tile[,] tiles;
        private Chunk[,] chunks;

        public TileMap(int width, int height, Texture texture)
        {
            Width = width;
            Height = height;

            var lastTile = texture == null ? ushort.MaxValue : (ushort)((texture.Size.X / GameOptions.TileSize) * (texture.Size.Y / GameOptions.TileSize) - 1);

            tiles = new Tile[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    tiles[x, y].Index = lastTile;
                    tiles[x, y].Solid = false;
                }
            }

            var chunkWidth = (width / GameOptions.TileChunkSize) + 1;
            var chunkHeight = (height / GameOptions.TileChunkSize) + 1;

            chunks = new Chunk[chunkWidth, chunkHeight];

            for (var y = 0; y < chunkHeight; y++)
            {
                for (var x = 0; x < chunkWidth; x++)
                {
                    chunks[x, y] = new Chunk(x, y, tiles, texture);
                }
            }
        }

        public Tile this[int x, int y]
        {
            get { return tiles[x, y]; }
            set
            {
                tiles[x, y] = value;
                chunks[x / GameOptions.TileChunkSize, y / GameOptions.TileChunkSize].Dirty = true;
            }
        }

        public void Draw(RenderTarget rt)
        {
            var view = rt.GetView();

            var tileSize = GameOptions.TileSize;
            var chunkSize = GameOptions.TileChunkSize;

            var startX = (int)Math.Max(0, ((view.Center.X - (view.Size.X / 2)) / tileSize) / chunkSize);
            var startY = (int)Math.Max(0, ((view.Center.Y - (view.Size.Y / 2)) / tileSize) / chunkSize);
            var endX = (int)Math.Ceiling(Math.Min(chunks.GetUpperBound(0) + 1, startX + 1 + ((view.Size.X / tileSize) / chunkSize)));
            var endY = (int)Math.Ceiling(Math.Min(chunks.GetUpperBound(1) + 1, startY + 1 + ((view.Size.Y / tileSize) / chunkSize)));

            for (var y = startY; y < endY; y++)
            {
                for (var x = startX; x < endX; x++)
                {
                    rt.Draw(chunks[x, y]);
                }
            }
        }

        public bool PlaceFree(FloatRect r, CollisionCondition cond = null)
        {
            var tileSize = GameOptions.TileSize;

            var minX = Math.Max(0, ((int)r.Left / tileSize) - 1);
            var minY = Math.Max(0, ((int)r.Top / tileSize) - 1);
            var maxX = Math.Min(Width, minX + ((int)r.Width / tileSize) + 3);
            var maxY = Math.Min(Height, minY + ((int)r.Height / tileSize) + 3);
            var testRect = new FloatRect(0, 0, tileSize, tileSize);

            for (var yy = minY; yy < maxY; yy++)
            {
                for (var xx = minX; xx < maxX; xx++)
                {
                    if (!tiles[xx, yy].Solid)
                        continue;

                    testRect.Left = xx * tileSize;
                    testRect.Top = yy * tileSize;

                    if (!r.Intersects(testRect))
                        continue;
                    if (cond == null)
                        return false;
                    if (cond(tiles[xx, yy], testRect, r))
                        return false;
                }
            }

            return true;
        }
    }
}
