using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class BatchedSprite : Sprite
    {
        internal IntRect? SheetRect = null;

        public BatchedSprite(Texture texture)
        {
            Texture = texture;
        }

        public BatchedSprite(Texture texture, IntRect textureRect)
        {
            Texture = texture;
            TextureRect = textureRect;
        }

        public void Reset()
        {
            SheetRect = null;
        }
    }

    public class SpriteBatch
    {
        private static TextureSheet sheet;
        private static TextureSheet Sheet
        {
            get { return sheet ?? (sheet = new TextureSheet(GameOptions.SpriteBatchSize)); }
        }

        private Vertex[] vertices;
        private bool drawing;

        private List<Tuple<int, BatchedSprite>> sprites;

        public SpriteBatch()
        {
            vertices = new Vertex[0];
            sprites = new List<Tuple<int, BatchedSprite>>();
        }

        public void Begin()
        {
            if (drawing) throw new Exception("End() must be called first");
            sprites.Clear();
            drawing = true;
        }

        public void End(RenderTarget rt, RenderStates states, bool depthSort = false)
        {
            if (!drawing) throw new Exception("Begin() must be called first");

            if (sprites.Count * 4 > vertices.Length)
                Array.Resize(ref vertices, sprites.Count * 4);

            if (depthSort)
                sprites.Sort((i, j) => i.Item1 - i.Item1);

            for (var i = 0; i < sprites.Count; i++)
            {
                var s = sprites[i].Item2;

                if (!s.SheetRect.HasValue)
                    s.SheetRect = Sheet.Get(s.Texture, s.TextureRect);

                WriteQuad(i * 4, s.Position, s.SheetRect.Value, s.Color, s.Scale, s.Origin, s.Rotation);
            }

            states.Texture = Sheet.Texture;
            rt.Draw(vertices, 0, (uint)sprites.Count * 4, PrimitiveType.Quads, states);
            drawing = false;
        }

        public void Draw(BatchedSprite sprite, int depth = 0)
        {
            if (!drawing) throw new Exception("Begin() must be called first");

            sprites.Add(Tuple.Create(depth, sprite));
        }

        public void Draw(AnimatedSprite sprite, int depth = 0)
        {
            Draw(sprite.Sprite, depth);
        }

        // http://en.sfml-dev.org/forums/index.php?topic=8660.msg68104#msg68104
        public void WriteQuad(int offset, Vector2f position, IntRect rect, Color color, Vector2f scale, Vector2f origin, float rotation)
        {
            rotation = rotation * (float)Math.PI / 180;

            var sin = (float)Math.Sin(rotation);
            var cos = (float)Math.Cos(rotation);

            origin.X *= scale.X;
            origin.Y *= scale.Y;
            scale.X *= rect.Width;
            scale.Y *= rect.Height;

            var v = new Vertex { Color = color };

            var pX = -origin.X;
            var pY = -origin.Y;

            v.Position.X = pX * cos - pY * sin + position.X;
            v.Position.Y = pX * sin + pY * cos + position.Y;
            v.TexCoords.X = rect.Left;
            v.TexCoords.Y = rect.Top;
            vertices[offset + 0] = v;

            pX += scale.X;
            v.Position.X = pX * cos - pY * sin + position.X;
            v.Position.Y = pX * sin + pY * cos + position.Y;
            v.TexCoords.X += rect.Width;
            vertices[offset + 1] = v;

            pY += scale.Y;
            v.Position.X = pX * cos - pY * sin + position.X;
            v.Position.Y = pX * sin + pY * cos + position.Y;
            v.TexCoords.Y += rect.Height;
            vertices[offset + 2] = v;

            pX -= scale.X;
            v.Position.X = pX * cos - pY * sin + position.X;
            v.Position.Y = pX * sin + pY * cos + position.Y;
            v.TexCoords.X -= rect.Width;
            vertices[offset + 3] = v;
        }
    }

    internal class TextureSheet
    {
        struct TextureInfo
        {
            private readonly Texture texture;
            private readonly IntRect subrect;

            public TextureInfo(Texture texture, IntRect subrect)
            {
                this.texture = texture;
                this.subrect = subrect;
            }

            public override int GetHashCode()
            {
                return (subrect.Left | ((subrect.Top & 0xFFF) << 8) | ((subrect.Width & 0xFF) << 16) | ((subrect.Height & 0xFF) << 24)) ^ texture.CPointer.ToInt32();
            }
        }

        public Texture Texture
        {
            get { return renderTexture.Texture; }
        }

        public int Count
        {
            get { return textureLocations.Count; }
        }

        private Dictionary<TextureInfo, IntRect> textureLocations;
        private List<IntRect> freeRects;
        private RenderTexture renderTexture;

        public TextureSheet(uint size)
        {
            textureLocations = new Dictionary<TextureInfo, IntRect>();
            freeRects = new List<IntRect>();
            renderTexture = new RenderTexture(size, size);

            freeRects.Add(new IntRect(0, 0, (int)size, (int)size));
        }

        public IntRect Get(Texture texture, IntRect subrect)
        {
            var info = new TextureInfo(texture, subrect);

            IntRect res;
            if (textureLocations.TryGetValue(info, out res))
                return res;

            IntRect r;

            try
            {
                r = freeRects.First(rect => rect.Width >= subrect.Width && rect.Height >= subrect.Height);
            }
            catch
            {
                throw new Exception("SpriteBatch size is too small");
            }

            freeRects.Remove(r);

            res = new IntRect(r.Left, r.Top, subrect.Width, subrect.Height);
            textureLocations.Add(info, res);

            var rightSector = new IntRect(r.Left + subrect.Width, r.Top, r.Width - subrect.Width, subrect.Height);
            var bottomSector = new IntRect(r.Left, r.Top + subrect.Height, r.Width, r.Height - subrect.Height);

            if (rightSector.Width != 0 && rightSector.Height != 0)
                freeRects.Add(rightSector);

            if (bottomSector.Width != 0 && bottomSector.Height != 0)
                freeRects.Add(bottomSector);

            freeRects.Sort((i, j) => i.Height - j.Height);

            var spr = new Sprite(texture, subrect) { Position = new Vector2f(res.Left, res.Top) };

            renderTexture.Draw(spr, new RenderStates(BlendMode.None));
            renderTexture.Display();

            return res;
        }
    }
}
