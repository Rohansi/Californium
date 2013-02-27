using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public abstract class Entity
    {
        internal LinkedListNode<Entity> Node;
        internal Vector2i GridCoordinate;
        internal float DepthRandomize = 0;
        private Input input;

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
                if (input == null)
                {
                    input = new Input();

                    if (Parent != null)
                        Parent.Entities.AddInput(this);
                }
                
                return input;
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
            get { return new FloatRect(Position.X - (Origin.X * Scale.X), Position.Y - (Origin.Y * Scale.Y), Size.X * Scale.X, Size.Y * Scale.Y); }
        }

        /// <summary>
        /// Called when the Entity is being added to an EntityManager. Unlike the constructor, Parent is valid when Create is called.
        /// </summary>
        public virtual void Create() { }

        /// <summary>
        /// Called when the Entity is being removed from an EntityManager.
        /// </summary>
        public virtual void Destroy() { }

        public virtual void Update() { }
        public virtual void Draw(RenderTarget rt) { }
    }
}
