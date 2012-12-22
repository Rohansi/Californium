using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public abstract class Entity
    {
        internal LinkedListNode<Entity> Node;
        internal Vector2i GridCoordinate;

        public State Parent;

        private Input input;
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
        public Vector2f Position = new Vector2f();
        public Vector2f Origin = new Vector2f();
        public Vector2f Size = new Vector2f();
        public Vector2f Scale = new Vector2f(1, 1);

        public virtual FloatRect BoundingBox
        {
            get { return new FloatRect(Position.X - (Origin.X * Scale.X), Position.Y - (Origin.Y * Scale.Y), Size.X * Scale.X, Size.Y * Scale.Y); }
        }

        public virtual void Update() { }
        public virtual void Draw(RenderTarget rt) { }
    }
}
