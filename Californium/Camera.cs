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
        public bool IsSmooth = false;
        public Vector2f Position;

        private View view;
        private Vector2f actualPosition;

        public Camera(View view)
        {
            this.view = view;
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

        public void Draw(RenderTarget rt)
        {
            // if the view height is odd we need to offset the view to prevent buggy rendering
            var offset = new Vector2f();
            /*if ((int)view.Size.Y % 2 != 0)
                offset.Y = 0.5f;*/

            view.Center = actualPosition + offset;
            rt.SetView(view);
        }
    }
}
