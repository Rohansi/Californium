using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class AnimatedSprite : Drawable
    {
        internal BatchedSprite Sprite;
        private int frameWidth;
        private int frameHeight;
        private int totalFrames;

        private int currentFrame;
        private float elapsedTime;

        public Vector2f Position
        {
            get { return Sprite.Position; }
            set { Sprite.Position = value; }
        }

        public float Rotation
        {
            get { return Sprite.Rotation; }
            set { Sprite.Rotation = value; }
        }

        public Vector2f Scale
        {
            get { return Sprite.Scale; }
            set { Sprite.Scale = value; }
        }

        public Vector2f Origin
        {
            get { return Sprite.Origin; }
            set { Sprite.Origin = value; }
        }

        public int CurrentFrame
        { 
            get { return currentFrame; }
            set 
            {
                currentFrame = value % totalFrames;
                Sprite.TextureRect = new IntRect(currentFrame * frameWidth, 0, frameWidth, frameHeight);
                Sprite.Reset();
            } 
        }

        public float FrameTime;

        public AnimatedSprite(Texture texture, int frameWidth, int frameHeight, float frameTime)
        {
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            Sprite = new BatchedSprite(texture);
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

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Sprite);
        }
    }
}
