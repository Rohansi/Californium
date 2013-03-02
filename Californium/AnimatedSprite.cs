using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class AnimatedSprite : Drawable
    {
        internal Sprite Sprite;
        private readonly int frameWidth;
        private readonly int frameHeight;
        private readonly int totalFrames;

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

        public Color Color
        {
            get { return Sprite.Color; }
            set { Sprite.Color = value; }
        }

        /// <summary>
        /// Gets or sets the index of current displayed frame.
        /// </summary>
        public int CurrentFrame
        { 
            get { return currentFrame; }
            set 
            {
                currentFrame = value % totalFrames;
                Sprite.TextureRect = new IntRect(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            } 
        }

        /// <summary>
        /// Time spent on a frame before switching to the next one.
        /// </summary>
        public float FrameTime;

        /// <summary>
        /// Toggle for automatic frame changes based on FrameTime.
        /// </summary>
        public bool Enabled = true;

        public AnimatedSprite(Texture texture, int frameWidth, int frameHeight, float frameTime)
        {
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            Sprite = new Sprite(texture);
            totalFrames = (int)(texture.Size.X / frameWidth) - 1;
            elapsedTime = 0;

            FrameTime = frameTime;
            CurrentFrame = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (!Enabled)
                return;

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
