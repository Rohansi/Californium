using System;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class Camera
    {
        /// <summary>
        /// Center point of the camera
        /// </summary>
        public Vector2f Position;

        /// <summary>
        /// Toggle for smooth camera transition
        /// </summary>
        public bool Smooth = false;

        /// <summary>
        /// Smoothness determines how quickly the transition will take place. Higher smoothness will reach the target position faster.
        /// </summary>
        public float Smoothness = 0.33f;

        /// <summary>
        /// Toggle for automatic position rounding. Useful if pixel sizes become inconsistent or font blurring occurs.
        /// </summary>
        public bool RoundPosition = true;

        /// <summary>
        /// Gets or sets the current zoom level of the camera
        /// </summary>
        public float Zoom
        {
            get { return View.Size.X / originalSize.X; }
            set { View.Size = originalSize; View.Zoom(value); }
        }

        /// <summary>
        /// Calculates the area the camera should display
        /// </summary>
        public FloatRect Bounds
        {
            get { return new FloatRect(View.Center.X - (View.Size.X / 2), View.Center.Y - (View.Size.Y / 2), View.Size.X, View.Size.Y); }
        }

        internal View View;
        internal Vector2f ActualPosition;
        private Vector2f originalSize;

        public Camera(FloatRect rect) : this(new View(rect)) { }

        public Camera(View view)
        {
            View = new View(view);
            Position = View.Size / 2;
            originalSize = View.Size;
            ActualPosition = Position;
        }

        public void Update()
        {
            if (Smooth)
            {
                var dir = Utility.Direction(ActualPosition, Position);
                var len = Utility.Distance(ActualPosition, Position);
                ActualPosition += Utility.LengthDir(dir, len * Smoothness);
            }
            else
            {
                ActualPosition = Position;
            }
        }

        public void Apply(RenderTarget rt)
        {
            var center = ActualPosition;

            if (RoundPosition)
            {
                var pxSize = 1 * Zoom;
                center.X = Utility.RoundToNearest(ActualPosition.X, pxSize);
                center.Y = Utility.RoundToNearest(ActualPosition.Y, pxSize);
            }

            // offset fixes texture coord rounding
            var offset = 0.25f * Zoom; 
            center.X += offset;
            center.Y += offset;

            View.Center = center;
            rt.SetView(View);
        }
    }
}
