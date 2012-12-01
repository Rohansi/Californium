using System;
using SFML.Window;
using SFML.Graphics;

namespace Californium
{
    public struct Tile
    {
        public int Index;
        public bool Solid;

        public Tile(int index, bool solid)
        {
            Index = index;
            Solid = solid;
        }
    }

    public class TileMap
    {
        private class Chunk : Drawable
        {
            public bool Dirty;

            private VertexArray vertexArray;
            private int chunkX, chunkY;
            private Tile[,] tiles;
            private Texture texture;
            private int lastTile;

            public Chunk(int chunkX, int chunkY, Tile[,] tiles, Texture texture)
            {
                this.tiles = tiles;
                this.chunkX = chunkX;
                this.chunkY = chunkY;
                this.texture = texture;

                lastTile = (int)((texture.Size.X / GameOptions.TileSize) * (texture.Size.Y / GameOptions.TileSize) - 1);

                Dirty = true;
            }

            private void Rebuild()
            {
                vertexArray = new VertexArray(PrimitiveType.Quads);

                int mapWidth = tiles.GetUpperBound(0) + 1;
                int mapHeight = tiles.GetUpperBound(1) + 1;

                int tileSize = GameOptions.TileSize;
                int tileMapWidth = (int)texture.Size.X / tileSize;
                int chunkSize = GameOptions.TileChunkSize;

                int startX = chunkX * chunkSize;
                int startY = chunkY * chunkSize;
                int endX = Math.Min(startX + chunkSize, mapWidth);
                int endY = Math.Min(startY + chunkSize, mapHeight);

                for (int y = startY; y < endY; y++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        if (tiles[x, y].Index >= lastTile) continue;

                        uint last = vertexArray.VertexCount;
                        vertexArray.Resize(vertexArray.VertexCount + 4);

                        int itexX = (tiles[x, y].Index % tileMapWidth) * tileSize;
                        int itexY = (tiles[x, y].Index / tileMapWidth) * tileSize;

                        // HACK: SFML's weird rendering
                        float texX = itexX + 0.01f;
                        float texY = itexY - 0.01f;

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
        private Texture texture;

        public TileMap(int width, int height, string image)
        {
            texture = TextureManager.Load(image);

            Width = width;
            Height = height;

            int lastTile = (int)((texture.Size.X / GameOptions.TileSize) * (texture.Size.Y / GameOptions.TileSize) - 1);
            tiles = new Tile[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y].Index = lastTile;
                    tiles[x, y].Solid = false;
                }
            }

            int chunkWidth = (width / GameOptions.TileChunkSize) + 1;
            int chunkHeight = (height / GameOptions.TileChunkSize) + 1;

            chunks = new Chunk[chunkWidth, chunkHeight];

            for (int y = 0; y < chunkHeight; y++)
            {
                for (int x = 0; x < chunkWidth; x++)
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
            View view = rt.GetView();

            int tileSize = GameOptions.TileSize;
            int chunkSize = GameOptions.TileChunkSize;

            int startX = (int)Math.Max(0, ((view.Center.X - (view.Size.X / 2)) / tileSize) / chunkSize);
            int startY = (int)Math.Max(0, ((view.Center.Y - (view.Size.Y / 2)) / tileSize) / chunkSize);
            int endX = (int)Math.Ceiling(Math.Min(chunks.GetUpperBound(0) + 1, startX + 1 + ((view.Size.X / tileSize) / chunkSize)));
            int endY = (int)Math.Ceiling(Math.Min(chunks.GetUpperBound(1) + 1, startY + 1 + ((view.Size.Y / tileSize) / chunkSize)));

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    rt.Draw(chunks[x, y]);
                }
            }
        }

        // NOTE: these don't treat outside the map as solid
        public bool PlaceFree(FloatRect r)
        {
            int tileSize = GameOptions.TileSize;

            int minX = Math.Max(0, ((int)r.Left / tileSize) - 1);
            int minY = Math.Max(0, ((int)r.Top / tileSize) - 1);
            int maxX = Math.Min(Width, minX + ((int)r.Width / tileSize) + 3);
            int maxY = Math.Min(Height, minY + ((int)r.Height / tileSize) + 3);
            FloatRect testRect = new FloatRect(0, 0, tileSize, tileSize);

            for (int yy = minY; yy < maxY; yy++)
            {
                for (int xx = minX; xx < maxX; xx++)
                {
                    if (tiles[xx, yy].Solid)
                    {
                        testRect.Left = xx * tileSize;
                        testRect.Top = yy * tileSize;

                        if (r.Intersects(testRect))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool PlaceFree(Vector2f p)
        {
            int xx = (int)p.X / GameOptions.TileSize;
            int yy = (int)p.Y / GameOptions.TileSize;

            if (xx < 0 || xx > Width || yy < 0 || yy > Height)
                return true;

            return !tiles[xx, yy].Solid;
        }
    }
}
