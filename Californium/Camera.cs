using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void Update(float dt)
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
            // if the view height is odd we need to offset the view to prevent buggy rendering
            var offset = new Vector2f();
            if ((int)View.Size.Y % 2 != 0)
                offset.Y = 0.5f;

            View.Center = actualPosition + offset;
            rt.SetView(View);
        }
    }
}
