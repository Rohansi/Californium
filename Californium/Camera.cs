using System;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class Camera
    {
        internal View View;
        public Vector2f Position;
        public bool IsSmooth = false;
        
        private Vector2f actualPosition;

        public FloatRect Bounds
        {
            get { return new FloatRect(View.Center.X - (View.Size.X / 2), View.Center.Y - (View.Size.Y / 2), View.Size.X, View.Size.Y); }
        }

        public Camera(View view)
        {
            View = new View(view);
            Position = view.Size / 2;
        }

        public void Zoom(float factor)
        {
            View.Zoom(factor);
        }

        public void Update()
        {
            // TODO: smooth camera
            if (IsSmooth)
            {
                
            }
            else
            {
                actualPosition = new Vector2f((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
            }
        }

        public void Apply(RenderTarget rt)
        {
            View.Center = actualPosition;
            rt.SetView(View);
        }
    }
}
