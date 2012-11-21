using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public abstract class State
    {
        [Flags]
        public enum FrameStep
        {
            None = 0,
            Input = 1,
            Update = 2,
            Draw = 4,
            Background = Update | Draw,
            All = Input | Update | Draw
        }

        public EntityManager Entities;
        public TileMap Map;

        public FrameStep InactiveMode { get; protected set; }
        public Color ClearColor { get; protected set; }

        protected State()
        {
            Entities = new EntityManager(this);
            InactiveMode = FrameStep.All;
        }

        public bool PlaceFree(FloatRect r)
        {
            return Map.PlaceFree(r) && Entities.PlaceFree(r);
        }

        public virtual void Update(float dt)
        {
            Entities.Update(dt);
        }

        public virtual void Draw(RenderTarget rt)
        {
            rt.Draw(Entities);
        }

        public virtual bool ProcessEvent(InputArgs args)
        {
            return Entities.ProcessInput(args);
        }
    }
}
