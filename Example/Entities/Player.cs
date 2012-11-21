using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Californium;
using SFML.Graphics;
using SFML.Window;

namespace Example.Entities
{
    class Player : Entity
    {
        private const int size = 8;

        private RectangleShape shape;

        private float hSave, vSave;
        private bool keyW, keyA, keyS, keyD;

        public Player(Vector2f position)
        {
            Solid = true;
            Position = position;
            Origin = new Vector2f(size / 2, size / 2);
            Size = new Vector2f(size, size);

            shape = new RectangleShape(Size);
            shape.Origin = Origin;

            Input.Key[Keyboard.Key.W] = args => keyW = args.Pressed;
            Input.Key[Keyboard.Key.A] = args => keyA = args.Pressed;
            Input.Key[Keyboard.Key.S] = args => keyS = args.Pressed;
            Input.Key[Keyboard.Key.D] = args => keyD = args.Pressed;
        }

        public override void Update(float dt)
        {
            const float speed = 500;

            float hMove = speed * Direction(keyA, keyD) * dt;
            float vMove = speed * Direction(keyW, keyS) * dt;

            int hRep = (int)Math.Floor(Math.Abs(hMove));
            int vRep = (int)Math.Floor(Math.Abs(vMove));

            hSave += (float)(Math.Abs(hMove) - Math.Floor(Math.Abs(hMove)));
            vSave += (float)(Math.Abs(vMove) - Math.Floor(Math.Abs(vMove)));

            while (hSave >= 1.0)
            {
                --hSave;
                ++hRep;
            }

            while (vSave >= 1.0)
            {
                --vSave;
                ++vRep;
            }

            var testRect = BoundingBox;
            while (hRep-- > 0)
            {
                testRect.Left += Math.Sign(hMove);
                if (!Parent.PlaceFree(testRect))
                {
                    hSave = 0;
                    break;
                }

                Position.X += Math.Sign(hMove);
            }

            testRect = BoundingBox;
            while (vRep-- > 0)
            {
                testRect.Top += Math.Sign(vMove);
                if (!Parent.PlaceFree(testRect))
                {
                    vSave = 0;
                    break;
                }

                Position.Y += Math.Sign(vMove);
            }
        }

        public override void Draw(RenderTarget rt)
        {
            shape.Position = Position;
            rt.Draw(shape);
        }

        private static float Direction(bool neg, bool pos)
        {
            float res = 0;
            if (neg) res -= 1;
            if (pos) res += 1;
            return res;
        }
    }
}
