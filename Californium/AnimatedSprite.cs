using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class AnimatedSprite
    {
        BatchedSprite sprite;
        int frameWidth;
        int frameHeight;
        int totalFrames;

        int currentFrame;
        float elapsedTime;

        public Vector2f Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        public float Rotation
        {
            get { return sprite.Rotation; }
            set { sprite.Rotation = value; }
        }

        public Vector2f Scale
        {
            get { return sprite.Scale; }
            set { sprite.Scale = value; }
        }

        public Vector2f Origin
        {
            get { return sprite.Origin; }
            set { sprite.Origin = value; }
        }

        public int CurrentFrame
        { 
            get { return currentFrame; }
            set 
            {
                currentFrame = value % totalFrames;
                sprite.TextureRect = new IntRect(currentFrame * frameWidth, 0, frameWidth, frameHeight);
                sprite.Reset();
            } 
        }

        public float FrameTime;

        public AnimatedSprite(Texture texture, int frameWidth, int frameHeight, float frameTime)
        {
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            sprite = new BatchedSprite(texture);
            totalFrames = (int)(texture.Size.X / frameWidth) - 1;
            elapsedTime = 0;

            FrameTime = frameTime;
            CurrentFrame = 0;
        }

        public void Update()
        {
            elapsedTime += GameOptions.Timestep;

            while (elapsedTime >= FrameTime)
            {
                currentFrame++;
                elapsedTime -= FrameTime;
            }

            CurrentFrame = currentFrame;
        }

        public void Draw(SpriteBatch spriteBatch, int depth = 0)
        {
            spriteBatch.Draw(sprite, depth);
        }
    }
}
