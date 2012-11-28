using System;
using Californium;
using SFML.Graphics;
using SFML.Window;

namespace Example.Entities
{
    class Player : Entity
    {
        private const int SpriteSize = 8;

        private Sprite sprite;

        private float hSave, vSave;
        private bool keyW, keyA, keyS, keyD;

        private float hSpeed, vSpeed;
        private bool canJump;
        private float jumpEnergy;

        public Player(Vector2f position)
        {
            Solid = true;
            Position = position;
            Origin = new Vector2f(SpriteSize / 2, SpriteSize / 2);
            Size = new Vector2f(SpriteSize, SpriteSize);

            sprite = new Sprite(TextureManager.Load("Player.png")) { Origin = Origin };

            Input.Key[Keyboard.Key.W] = args =>
            {
                keyW = args.Pressed;
                if (args.Pressed)
                    canJump = true;
                else
                    jumpEnergy = 0;
                return true;
            };

            Input.Key[Keyboard.Key.A] = args => { keyA = args.Pressed; return true; };
            Input.Key[Keyboard.Key.S] = args => { keyS = args.Pressed; return true; };
            Input.Key[Keyboard.Key.D] = args => { keyD = args.Pressed; return true; };
        }

        public override void Update(float dt)
        {
            const float maxHSpeed = 500;
            const float maxVSpeed = 750;
            const float acceleration = 5000;
            const float jumpPotential = 600;
            const float jumpSpeed = 200;
            const float gravity = 2000;
            const float friction = 25;

            if (keyA) hSpeed -= acceleration * dt;
            if (keyD) hSpeed += acceleration * dt;

            hSpeed *= (float)Math.Exp(-friction * dt);
            hSpeed = Utility.Clamp(hSpeed, -maxHSpeed, maxHSpeed);

            vSpeed += gravity * dt;

            if (canJump)
            {
                var bounds = BoundingBox;
                bounds.Top++;

                if (!Parent.PlaceFree(bounds))
                    jumpEnergy = jumpPotential;
                canJump = false;
            }

            if (keyW && jumpEnergy > 0)
            {
                float usedEnergy = Math.Min(jumpSpeed + vSpeed, jumpEnergy);
                jumpEnergy -= usedEnergy;
                vSpeed -= usedEnergy;
            }

            vSpeed = Utility.Clamp(vSpeed, -maxVSpeed, maxVSpeed);

            float hMove = hSpeed * dt;
            float vMove = vSpeed * dt;

            int hRep = (int)Math.Floor(Math.Abs(hMove));
            int vRep = (int)Math.Floor(Math.Abs(vMove));

            hSave += (float)(Math.Abs(hMove) - Math.Floor(Math.Abs(hMove)));
            vSave += (float)(Math.Abs(vMove) - Math.Floor(Math.Abs(vMove)));

            while (hSave >= 1)
            {
                --hSave;
                ++hRep;
            }

            while (vSave >= 1)
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
                    hSpeed = 0;
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
                    vSpeed = 0;
                    break;
                }

                Position.Y += Math.Sign(vMove);
            }
        }

        public override void Draw(RenderTarget rt)
        {
            sprite.Position = Position;
            rt.Draw(sprite);
        }
    }
}
