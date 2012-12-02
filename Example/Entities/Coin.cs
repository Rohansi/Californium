using System;
using Californium;
using SFML.Graphics;
using SFML.Window;

namespace Example.Entities
{
    class Coin : Entity
    {
        private AnimatedSprite sprite;

        public Coin(float x, float y)
        {
            Position = new Vector2f(x, y);
            Origin = new Vector2f(8, 8);
            Size = new Vector2f(16, 16);

            sprite = new AnimatedSprite(Assets.LoadTexture("Coin.png"), 16, 16, .075f);
            sprite.Position = Position;
            sprite.Origin = Origin;
        }

        public override void Update()
        {
            sprite.Update();
        }

        public override void Draw(RenderTarget rt)
        {
            rt.Draw(sprite);
        }
    }
}
