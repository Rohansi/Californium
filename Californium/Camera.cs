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
                actualPosition = Position;
            }
        }

        public void Draw(RenderTarget rt)
        {
            view.Center = actualPosition;
            rt.SetView(view);
        }
    }
}
