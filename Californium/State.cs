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

        public Camera Camera;
        public EntityManager Entities;
        public TileMap Map;

        public FrameStep InactiveMode { get; protected set; }
        public Color ClearColor { get; protected set; }

        protected State()
        {
            InitializeCamera();
            Entities = new EntityManager(this);
            InactiveMode = FrameStep.All;
        }

        public bool PlaceFree(FloatRect r)
        {
            return Map.PlaceFree(r) && Entities.PlaceFree(r);
        }

        internal void UpdateInternal(float dt)
        {
            Entities.Update(dt);
            Update(dt);
            Camera.Update(dt);
        }

        public virtual void Update(float dt)
        {
            
        }

        public virtual void Draw(RenderTarget rt)
        {
            Camera.Apply(rt);

            if (Map != null)
                Map.Draw(rt);

            rt.Draw(Entities);
        }

        public virtual bool ProcessEvent(InputArgs args)
        {
            return Entities.ProcessInput(args);
        }

        public virtual void InitializeCamera()
        {
            Camera = new Camera(Game.DefaultView);
        }
    }
}
