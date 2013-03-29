using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public static class Game
    {
        public static RenderWindow Window;

        public static View DefaultView
        {
            get
            {
                var view = Window.DefaultView;
                view.Size = size;
                return view;
            }
        }

        public static Stack<State> States
        {
            get { return new Stack<State>(StateStack); }
        }

        /// <summary>
        /// Function to call when the game is lagging. Provides the amount of time that was dropped.
        /// </summary>
        public static Action<float> Lagging;

        private static readonly List<State> StateStack;
        private static readonly bool[] KeyStates;

        private static Vector2f size;
        private static bool running;

        static Game()
        {
            StateStack = new List<State>();
            KeyStates = new bool[(int)Keyboard.Key.KeyCount];
        }

        /// <summary>
        /// Initialize the game. Call once after modifying GameOptions.
        /// </summary>
        public static void Initialize()
        {
            var style = Styles.Titlebar | Styles.Close;
            if (GameOptions.Resizable)
                style |= Styles.Resize;

            size = new Vector2f(GameOptions.Width, GameOptions.Height);
            running = true;

            Window = new RenderWindow(new VideoMode(GameOptions.Width, GameOptions.Height), GameOptions.Caption, style);
            Window.SetFramerateLimit(GameOptions.Framerate);
            Window.SetVerticalSyncEnabled(GameOptions.Vsync);

            if (!string.IsNullOrWhiteSpace(GameOptions.Icon))
            {
                var icon = Assets.LoadTexture(GameOptions.Icon);
                Window.SetIcon(icon.Size.X, icon.Size.Y, icon.CopyToImage().Pixels);
            }
            
            #region Event Wrappers
            Window.Closed += (sender, args) => Exit(true);
            Window.Resized += (sender, args) => Resize(new Vector2f(args.Width, args.Height));
            Window.MouseButtonPressed += (sender, args) => DispatchEvent(new MouseButtonInputArgs(args.Button, true, args.X, args.Y));
            Window.MouseButtonReleased += (sender, args) => DispatchEvent(new MouseButtonInputArgs(args.Button, false, args.X, args.Y));
            Window.MouseWheelMoved += (sender, args) => DispatchEvent(new MouseWheelInputArgs(args.Delta, args.X, args.Y));
            Window.MouseMoved += (sender, args) => DispatchEvent(new MouseMoveInputArgs(args.X, args.Y));
            Window.TextEntered += (sender, args) => DispatchEvent(new TextInputArgs(args.Unicode));

            Window.KeyPressed += (sender, args) =>
            {
                if (args.Code == Keyboard.Key.Unknown || KeyStates[(int)args.Code]) // repeated key press
                    return; 
                KeyStates[(int)args.Code] = true;
                DispatchEvent(new KeyInputArgs(args.Code, true, args.Control, args.Shift));
            };

            Window.KeyReleased += (sender, args) =>
            {
                if (args.Code != Keyboard.Key.Unknown)
                    KeyStates[(int)args.Code] = false;
                DispatchEvent(new KeyInputArgs(args.Code, false, args.Control, args.Shift));
            };
            #endregion
        }

        /// <summary>
        /// Runs the game. This method will return when Exit is called.
        /// </summary>
        public static void Run()
        {
            var timer = new Stopwatch();
            double accumulator = 0;

            while (running)
            {
                var time = timer.Elapsed.TotalSeconds;
                timer.Restart();
                accumulator += time;
                
                // Spiral of death fix
                var lagThreshold = GameOptions.Timestep * GameOptions.MaxUpdatesPerFrame;
                if (accumulator > lagThreshold)
                {
                    if (Lagging != null)
                        Lagging((float)accumulator - lagThreshold);

                    accumulator = lagThreshold;
                }

                #region Update
                while (accumulator >= GameOptions.Timestep)
                {
                    Window.DispatchEvents();
                    Timer.Update();

                    for (var i = 0; i < StateStack.Count; i++)
                    {
                        var state = StateStack[i];

                        if (i == StateStack.Count - 1 || state.InactiveMode.HasFlag(State.UpdateMode.Update))
                            state.UpdateInternal();
                    }

                    accumulator -= GameOptions.Timestep;
                }
                #endregion

                #region Draw

                // Find bottom most state that renders. This state will provide the color to clear to.
                var clearState = StateStack.FindIndex(s => s.InactiveMode.HasFlag(State.UpdateMode.Draw)); 

                for (var i = 0; i < StateStack.Count; i++)
                {
                    var state = StateStack[i];

                    if (i == clearState)
                        Window.Clear(state.ClearColor);
                    
                    if (i != StateStack.Count - 1 && !state.InactiveMode.HasFlag(State.UpdateMode.Draw))
                        continue;

                    state.DrawInternal(Window);
                }
                #endregion

                Window.Display();
            }

            Window.Close();
        }

        /// <summary>
        /// Exit the game. State.ExitRequested will only be called if userTriggered is true.
        /// </summary>
        public static void Exit(bool userTriggered = false)
        {
            if (userTriggered)
            {
                if (StateStack.All(s => s.ExitRequested()))
                    running = false;
            }
            else
            {
                running = false;
            }
        }

        /// <summary>
        /// Pops all states off the stack and pushes one onto it.
        /// </summary>
        public static void SetState(State state)
        {
            foreach (var s in StateStack)
            {
                s.Leave();
            }

            StateStack.Clear();
            PushState(state);
        }

        /// <summary>
        /// Pushes a state onto the state stack.
        /// </summary>
        public static void PushState(State state)
        {
            StateStack.Add(state);
            state.Enter();
        }

        /// <summary>
        /// Pops a state off the state stack.
        /// </summary>
        public static void PopState()
        {
            var last = StateStack.Count - 1;
            StateStack[last].Leave();
            StateStack.RemoveAt(last);
        }

        private static void DispatchEvent(InputArgs args)
        {
            for (var i = StateStack.Count - 1; i >= 0; i--)
            {
                var state = StateStack[i];

                args.View = state.Camera.View;
                if (StateStack[i].ProcessEvent(args))
                    return;
            }
        }

        private static void Resize(Vector2f newSize)
        {
            size = newSize;

            foreach (var state in StateStack)
            {
                state.InitializeCamera();
            }
        }

        internal static bool IsActive(State state)
        {
            return StateStack.IndexOf(state) == (StateStack.Count - 1);
        }
    }
}
