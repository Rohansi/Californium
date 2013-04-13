using System;
using System.Linq;
using Californium;
using SFML.Graphics;
using SFML.Window;

namespace Example.Entities
{
    class Player : Entity
    {
        private Sprite sprite;

        private float hSave, vSave;
        private bool keyW, keyA, keyS, keyD;

        private float hSpeed, vSpeed;
        private bool canJump;
        private float jumpEnergy;
        private bool fallThrough;

        public Player(Vector2f position)
        {
            Solid = true;
            Position = position;

            sprite = new Sprite(Assets.LoadTexture("Player.png"));

            var size = (int)sprite.Texture.Size.X;
            var center = sprite.Texture.Size.X / 2;

            Origin = new Vector2f(center, center);
            Size = new Vector2f(size, size);
            sprite.Origin = Origin;

            // Player moves with WASD. Jumping is not continuous so there is a bit more work there
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

        public override void Update()
        {
            Movement();

            // Request nearby coins and check if we collided with them
            var coins = Parent.Entities.InArea(BoundingBox).OfType<Coin>();
            foreach (var c in coins.Where(c => c.BoundingBox.Intersects(BoundingBox)))
            {
                Parent.Entities.Remove(c);
                Program.Score++;
                Assets.PlaySound("PickupCoin.wav");
            }
        }

        public override void Draw(RenderTarget rt)
        {
            sprite.Position = Position;
            rt.Draw(sprite);
        }

        private void Movement()
        {
            const float maxHSpeed = 2.5f;
            const float maxVSpeed = 6;
            const float acceleration = 1;
            const float jumpPotential = 10;
            const float jumpSpeed = 4;
            const float gravity = 0.7f;
            const float friction = 0.7f;

            // Left/right acceleration
            if (keyA) hSpeed -= acceleration;
            if (keyD) hSpeed += acceleration;

            hSpeed *= friction;

            // Done with horizontal math, clamp to maximum speed
            hSpeed = Utility.Clamp(hSpeed, -maxHSpeed, maxHSpeed);

            vSpeed += gravity;

            // Reset fallThrough for the upcoming PlaceFree call
            fallThrough = false;

            // Check if we are on a solid surface
            var bounds = BoundingBox;
            bounds.Top++;
            var onGround = !PlaceFree(bounds);

            // fallThrough's actual value
            fallThrough = keyS && onGround;

            if (canJump)
            {
                // Begin a jump by setting jumpEnergy
                if (onGround)
                    jumpEnergy = jumpPotential;
                canJump = false;
            }

            // As long as you continue to hold W and still have energy to jump you will rise
            if (keyW && jumpEnergy > 0)
            {
                // Drain energy so you can't float forever
                float usedEnergy = Math.Min(jumpSpeed + vSpeed, jumpEnergy);
                jumpEnergy -= usedEnergy;
                vSpeed -= usedEnergy;
            }

            // Done with vertical math, clamp to maximum speed
            vSpeed = Utility.Clamp(vSpeed, -maxVSpeed, maxVSpeed);

            var hMove = hSpeed;
            var vMove = vSpeed;

            // My movement code only works with whole pixel movements (no partial)
            // Figure out how many pixels to move on each axis
            var hRep = (int)Math.Floor(Math.Abs(hMove));
            var vRep = (int)Math.Floor(Math.Abs(vMove));

            // Save whatever fraction of a pixel we can not move this frame
            hSave += (float)(Math.Abs(hMove) - Math.Floor(Math.Abs(hMove)));
            vSave += (float)(Math.Abs(vMove) - Math.Floor(Math.Abs(vMove)));

            // The fraction can add up so we make sure to include any whole pixels that have accumulated
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

            // Now the actual movement
            // Loop for every pixel we need to move
            while (hRep-- > 0)
            {
                // If moving one pixel in that direction creates a collision, stop moving
                testRect.Left += Math.Sign(hMove);
                if (!PlaceFree(testRect))
                {
                    hSave = 0;
                    hSpeed = 0;
                    break;
                }

                // Otherwise we continue to move
                Position.X += Math.Sign(hMove);
            }

            testRect = BoundingBox;

            // Same thing here but for the Y axis
            while (vRep-- > 0)
            {
                testRect.Top += Math.Sign(vMove);
                if (!PlaceFree(testRect))
                {
                    vSave = 0;
                    vSpeed = 0;
                    break;
                }

                Position.Y += Math.Sign(vMove);
            }
        }

        private bool PlaceFree(FloatRect r)
        {
            // This PlaceFree function is used over the default because jumpthrough tiles create an
            // extra condition for it to consider.
            TileMap<Tile>.CollisionCondition cond = (tile, bounds, collisionBounds) =>
            {
                // If the tile is not a jumpthrough tile then there was a collision
                if (tile.UserData == null)
                    return true;

                // If the player wants to fall through jumpthrough tiles we override the folliwing condition
                if (fallThrough)
                    return false;

                // If we are moving down (gravity) and our feet are on the top of the tile, we collide with it
                return vSpeed > 0 && collisionBounds.Top + collisionBounds.Height - 1 <= bounds.Top;
            };

            // Now call TileMap.PlaceFree with our extra conditions
            return Parent.Map.PlaceFree(r, cond);
        }
    }
}
