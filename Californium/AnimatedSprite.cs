using SFML.Graphics;

namespace Californium
{
    class AnimatedSprite : Drawable
    {
        Sprite sprite;
        int frameWidth;
        int frameHeight;
        int totalFrames;

        int currentFrame;
        float elapsedTime;

        public int CurrentFrame
        { 
            get { return currentFrame; }
            set 
            {
                currentFrame = value % totalFrames;
                sprite.TextureRect = new IntRect(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            } 
        }

        public float FrameTime;

        public AnimatedSprite(Texture texture, int frameWidth, int frameHeight, float frameTime)
        {
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            sprite = new Sprite(texture);
            totalFrames = (int)(texture.Size.X / frameWidth) - 1;
            elapsedTime = 0;

            FrameTime = frameTime;
            CurrentFrame = 0;
        }

        public void Update(float dt)
        {
            elapsedTime += dt;

            while (elapsedTime >= FrameTime)
            {
                currentFrame++;
                elapsedTime -= FrameTime;
            }

            CurrentFrame = currentFrame;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(sprite);
        }
    }
}
