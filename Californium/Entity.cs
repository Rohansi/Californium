using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace Californium
{
    public abstract class Entity
    {
        internal LinkedListNode<Entity> Node;
        internal Vector2i GridCoordinate;
        internal float DepthRandomize = 0;
        internal Input InputInstance;

        /// <summary>
        /// Gets the entity's parent state
        /// </summary>
        public State Parent { get; internal set; }

        /// <summary>
        /// Gets the entity's associated Input instance. Handlers should return true if the event was used or false if it was ignored. 
        /// </summary>
        public Input Input
        {
            get
            {
                if (InputInstance == null)
                {
                    InputInstance = new Input();

                    if (Parent != null)
                        Parent.Entities.AddInput(this);
                }
                
                return InputInstance;
            }
        }

        public bool Solid = false;
        public short Depth = 0;
        public Vector2f Position = new Vector2f();
        public Vector2f Origin = new Vector2f();
        public Vector2f Size = new Vector2f();
        public Vector2f Scale = new Vector2f(1, 1);

        /// <summary>
        /// Returns a bounding box based on Position, Origin, Size and Scale.
        /// </summary>
        public virtual FloatRect BoundingBox
        {
            get
            {
                return new FloatRect(
                    Position.X - (Origin.X * Scale.X),
                    Position.Y - (Origin.Y * Scale.Y),
                    Size.X * Scale.X, 
                    Size.Y * Scale.Y
                );
            }
        }

        /// <summary>
        /// Called when the Entity is being added to an EntityManager.
        /// Unlike the constructor, Parent is valid when Create is called.
        /// </summary>
        public virtual void Create()
        {
            
        }

        /// <summary>
        /// Called when the Entity is being removed from an EntityManager.
        /// Parent is valid when Destroy is called.
        /// </summary>
        public virtual void Destroy()
        {
            
        }

        /// <summary>
        /// Called one or more times before Draw depending on the current delta.
        /// This will be called as many times as required to keep the update loop
        /// in sync with expected time.
        /// </summary>
        public virtual void Update()
        {
            
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        /// <param name="rt">RenderTarget, passed from engine.</param>
        public virtual void Draw(RenderTarget rt)
        {
            
        }

        /// <summary>
        /// Draws a rectangle around the entity as its computed bounding box.
        /// If any collision is detected the box will be opaque, otherwise it is drawn transparently.
        /// </summary>
        /// <param name="rt">RenderTarget, passed from engine.</param>
        /// <param name="color">Color of bounding box.</param>
        protected void DrawBoundingBox(RenderTarget rt, Color color)
        {
            RectangleShape boundingBox = new RectangleShape(new Vector2f(BoundingBox.Width, BoundingBox.Height))
            {
                Position = new Vector2f(BoundingBox.Left, BoundingBox.Top),
                FillColor = Parent.Entities.PlaceFree(BoundingBox) ? color : new Color(color.R, color.G, color.B, 128)
            };

            rt.Draw(boundingBox);
        }
    }
}
