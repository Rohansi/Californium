using System;
using SFML.Graphics;
using SFML.System;

namespace Californium
{
    /// <summary>
    /// A simple TileMap great for large maps with small tiles or maps that are mostly static.
    /// </summary>
    public class ChunkedTileMap : TileMap<Tile>
    {
        private class Chunk : Drawable
        {
            public bool Dirty;

            private VertexArray vertexArray;
            private readonly int chunkX, chunkY;
            private readonly int tileSize, chunkSize;
            private readonly Tile[,] tiles;
            private readonly Texture texture;
            private readonly ushort lastTile;

            public Chunk(int chunkX, int chunkY, Tile[,] tiles, int tileSize, int chunkSize, Texture texture)
            {
                this.tiles = tiles;
                this.chunkX = chunkX;
                this.chunkY = chunkY;
                this.tileSize = tileSize;
                this.chunkSize = chunkSize;
                this.texture = texture;

                lastTile = texture == null ? ushort.MaxValue : (ushort)((texture.Size.X / tileSize) * (texture.Size.Y / tileSize) - 1);

                Dirty = true;
            }

            private void Rebuild()
            {
                vertexArray = new VertexArray(PrimitiveType.Quads);

                var mapWidth = tiles.GetUpperBound(0) + 1;
                var mapHeight = tiles.GetUpperBound(1) + 1;
                var tileMapWidth = (int)texture.Size.X / tileSize;

                var startX = chunkX * chunkSize;
                var startY = chunkY * chunkSize;
                var endX = Math.Min(startX + chunkSize, mapWidth);
                var endY = Math.Min(startY + chunkSize, mapHeight);

                for (var y = startY; y < endY; y++)
                {
                    for (var x = startX; x < endX; x++)
                    {
                        if (tiles[x, y].Index > lastTile) continue;

                        var last = vertexArray.VertexCount;
                        vertexArray.Resize(vertexArray.VertexCount + 4);

                        var itexX = (tiles[x, y].Index % tileMapWidth) * tileSize;
                        var itexY = (tiles[x, y].Index / tileMapWidth) * tileSize;

                        // HACK: SFML's weird rendering
                        var texX = itexX/* + 0.01f*/;
                        var texY = itexY/* - 0.01f*/;

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

        private readonly int chunkSize;
        private readonly int chunksWidth;
        private readonly int chunksHeight;
        private readonly Chunk[,] chunks;

        public ChunkedTileMap(int width, int height, Texture texture, int tileSize, int chunkSize = 32)
            : base(width, height, tileSize)
        {
            this.chunkSize = chunkSize;
            chunksWidth = (width / chunkSize) + 1;
            chunksHeight = (height / chunkSize) + 1;

            chunks = new Chunk[chunksWidth, chunksHeight];

            for (var y = 0; y < chunksHeight; y++)
            {
                for (var x = 0; x < chunksWidth; x++)
                {
                    chunks[x, y] = new Chunk(x, y, Tiles, TileSize, chunkSize, texture);
                }
            }
        }

        public override Tile this[int x, int y]
        {
            set
            {
                chunks[x / chunkSize, y / chunkSize].Dirty = true;
                base[x, y] = value;
            }
        }

        public override void Draw(RenderTarget rt)
        {
            var region = CalculateDrawRegion(rt);

            var startX = Math.Max(0, region.Left / chunkSize);
            var startY = Math.Max(0, region.Top / chunkSize);
            var endX = Math.Min(chunksWidth, startX + (region.Width / chunkSize) + 2);
            var endY = Math.Min(chunksHeight, startY + (region.Height / chunkSize) + 2);

            for (var y = startY; y < endY; y++)
            {
                for (var x = startX; x < endX; x++)
                {
                    rt.Draw(chunks[x, y]);
                }
            }
        }
    }
}
