using System;
using SFML.Graphics;

namespace Californium
{
    public abstract class State
    {
        [Flags]
        public enum UpdateMode
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

        public UpdateMode InactiveMode { get; protected set; }
        public Color ClearColor { get; protected set; }

        private Input input;
        public Input Input
        {
            get { return input ?? (input = new Input()); }
        }

        /// <summary>
        /// Returns true if this State is at the top of the State stack.
        /// </summary>
        public bool Active
        {
            get { return Game.IsActive(this); }
        }

        protected State()
        {
            Entities = new EntityManager(this);
            InactiveMode = UpdateMode.All;
        }

        /// <summary>
        /// Called when the State's Camera needs to be (re)initialized.
        /// </summary>
        public virtual void InitializeCamera()
        {
            Camera = new Camera(Game.DefaultView);
        }

        /// <summary>
        /// Called when a State is added to the game. InitializeCamera is called here by default.
        /// </summary>
        public virtual void Enter()
        {
            InitializeCamera();
        }

        /// <summary>
        /// Called when a State is removed from the game.
        /// </summary>
        public virtual void Leave()
        {
            
        }

        /// <summary>
        /// Update is called once every Timestep. Game logic should be handled here (movement, animations, etc).
        /// </summary>
        public virtual void Update()
        {
            
        }

        /// <summary>
        /// Called once per frame, right after Update. Avoid putting game login in here.
        /// </summary>
        /// <param name="rt"></param>
        public virtual void Draw(RenderTarget rt)
        {
            Camera.Apply(rt);

            if (Map != null)
                Map.Draw(rt);

            Entities.Draw(rt);
        }

        public bool ProcessEvent(InputArgs args)
        {
            if (input != null && input.ProcessInput(args))
                return true;
            return Entities.ProcessInput(args);
        }

        internal void UpdateInternal()
        {
            Entities.Update();
            Update();
            Camera.Update();
        }
    }
}
