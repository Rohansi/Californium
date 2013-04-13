using System;
using SFML.Graphics;

namespace Californium
{
    public abstract class State
    {
        /// <summary>
        /// Flags that determine the enabled functionality of a state.
        /// </summary>
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

        private Input input;

        public Camera Camera;
        public EntityManager Entities;
        public TileMap<Tile> Map;

        /// <summary>
        /// Functions to call for this state when it is not the active state.
        /// </summary>
        public UpdateMode InactiveMode { get; protected set; }

        /// <summary>
        /// Background color of the state. Only valid for the bottom most state with rendering enabled (UpdateMode.Draw).
        /// </summary>
        public Color ClearColor { get; protected set; }

        /// <summary>
        /// Gets the state's associated Input instance. Handlers should return true if the event was used or false if it was ignored. 
        /// </summary>
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
            var o = Camera;
            Camera = new Camera(Game.DefaultView);

            if (o != null)
            {
                // Restore old positions. Fixes an issue with smooth cameras and window resizing
                Camera.Position = o.Position;
                Camera.ActualPosition = o.ActualPosition;
            }
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
        public virtual void Leave() { }

        /// <summary>
        /// Update is called once every Timestep. Game logic should be handled here (movement, animations, etc).
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Called once per frame, right after Update. Avoid putting game logic in here.
        /// </summary>
        public virtual void Draw(RenderTarget rt)
        {
            if (Map != null)
                Map.Draw(rt);

            Entities.Draw(rt);
        }

        /// <summary>
        /// Called when the user attempts to close the game. If all states return true, Game.Exit will be called.
        /// </summary>
        public virtual bool ExitRequested()
        {
            return true;
        }

        internal bool ProcessEvent(InputArgs args)
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

        internal void DrawInternal(RenderTarget rt)
        {
            Camera.Apply(rt);
            Draw(rt);
        }
    }
}
