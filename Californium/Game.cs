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

        public static Action Lagging;

        private static readonly List<State> StateStack;
        private static readonly bool[] KeyStates;

        private static Vector2f size;

        static Game()
        {
            StateStack = new List<State>();
            KeyStates = new bool[(int)Keyboard.Key.KeyCount];
        }

        public static void Initialize()
        {
            var style = Styles.Titlebar | Styles.Close;
            if (GameOptions.Resizable)
                style |= Styles.Resize;

            size = new Vector2f(GameOptions.Width, GameOptions.Height);

            Window = new RenderWindow(new VideoMode(GameOptions.Width, GameOptions.Height), GameOptions.Caption, style);
            Window.SetFramerateLimit(GameOptions.Framerate);
            Window.SetVerticalSyncEnabled(GameOptions.Vsync);

            if (!string.IsNullOrWhiteSpace(GameOptions.Icon))
            {
                var icon = Assets.LoadTexture(GameOptions.Icon);
                Window.SetIcon(icon.Size.X, icon.Size.Y, icon.CopyToImage().Pixels);
            }
            
            Window.Closed += (sender, args) => Window.Close();
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
        }

        public static void Run()
        {
            var timer = new Stopwatch();
            double accumulator = 0;

            while (Window.IsOpen())
            {
                var time = timer.Elapsed.TotalSeconds;
                timer.Restart();
                accumulator += time;
                
                // Spiral of death fix
                if (accumulator > (GameOptions.Timestep * GameOptions.MaxUpdatesPerFrame))
                {
                    if (Lagging != null)
                        Lagging();

                    accumulator = GameOptions.Timestep * GameOptions.MaxUpdatesPerFrame;
                }

                // Update
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

                // Draw
                var clearState = StateStack.FindIndex(s => s.InactiveMode.HasFlag(State.UpdateMode.Draw)); 
                for (var i = 0; i < StateStack.Count; i++)
                {
                    var state = StateStack[i];

                    if (i == clearState)
                        Window.Clear(state.ClearColor);

                    if (i != StateStack.Count - 1 && !state.InactiveMode.HasFlag(State.UpdateMode.Draw))
                        continue;

                    state.Draw(Window);
                }

                Window.Display();
            }
        }

        public static void Exit()
        {
            Window.Close();
        }

        public static void SetState(State state)
        {
            foreach (var s in StateStack)
            {
                s.Leave();
            }

            StateStack.Clear();
            PushState(state);
        }

        public static void PushState(State state)
        {
            state.Enter();
            StateStack.Add(state);
        }

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
