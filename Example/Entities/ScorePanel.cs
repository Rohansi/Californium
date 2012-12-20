using Californium;
using SFML.Graphics;
using SFML.Window;

namespace Example.Entities
{
    class ScorePanel : Entity
    {
        private bool dragging;
        private Vector2f dragOffset;
        private Text text;

        public ScorePanel()
        {
            Size = new Vector2f(100, 100);
            Position = new Vector2f(10, 10);

            text = new Text("", Assets.LoadFont("OpenSans-Regular.ttf"))
                   { CharacterSize = 18, Color = Color.Black };
            
            // Begin/end drag on press/release
            Input.MouseButton[Mouse.Button.Left] = args =>
            {
                var pos = args.Position;
                if (!BoundingBox.Contains(pos.X, pos.Y))
                    return false;

                if (args.Pressed && !dragging)
                {
                    dragging = true;
                    dragOffset = new Vector2f(Position.X - pos.X, Position.Y - pos.Y);
                }
                else if (!args.Pressed && dragging)
                {
                    dragging = false;
                }

                return true;
            };

            // Update position if still dragging
            Input.MouseMove = args =>
            {
                var pos = args.Position;

                if (!dragging)
                    return false;

                Position = new Vector2f(pos.X, pos.Y) + dragOffset;
                return true;
            };
        }

        public override void Update()
        {
            var bounds = Parent.Camera.Bounds;

            // Clamp the panel to the display
            if (Position.X < bounds.Left) Position.X = bounds.Left;
            if (Position.Y < bounds.Top) Position.Y = bounds.Top;
            if (Position.X > bounds.Left + bounds.Width - 100) Position.X = bounds.Left + bounds.Width - 100;
            if (Position.Y > bounds.Top + bounds.Height - 100) Position.Y = bounds.Top + bounds.Height - 100;

            // Update the displayed text
            text.DisplayedString = "Score: " + Program.Score;
            text.Position = Position + new Vector2f(50, 50);
            text.Center();
        }

        public override void Draw(RenderTarget rt, SpriteBatch spriteBatch)
        {
            var shape = new RectangleShape(Size)
                        { FillColor = new Color(255, 255, 255, 128), Position = Position };

            rt.Draw(shape);
            rt.Draw(text);
        }
    }
}
