using Californium;
using SFML.Graphics;
using SFML.Window;

namespace Example.States
{
    public class Pause : State
    {
        public Pause()
        {
            IsOverlay = false;

            Input.Key[Keyboard.Key.Space] = args =>
            {
                if (!args.Pressed)
                    return true;

                Game.PopState();
                return true;
            };
        }

        public override void Draw(RenderTarget rt)
        {
            base.Draw(rt);

            var textBackSize = new Vector2f(150, 54);
            var textBack = new RectangleShape(textBackSize);
            textBack.FillColor = new Color(0, 0, 0, 150);
            textBack.Origin = textBackSize / 2;
            textBack.Position = rt.GetView().Center;

            var text = new Text("PAUSED", Assets.LoadFont("OpenSans-Regular.ttf"), 32);
            text.Color = Color.White;
            text.Position = rt.GetView().Center;
            text.Center();

            rt.Draw(textBack);
            rt.Draw(text);
        }
    }
}
