using System.Collections.Generic;
using System.Diagnostics;
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

        private static List<State> states;
        private static bool[] keyStates;
        private static Vector2f size;

        static Game()
        {
            states = new List<State>();
            keyStates = new bool[(int)Keyboard.Key.KeyCount];
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
            
            Window.Closed += (sender, args) => Window.Close();
            Window.Resized += (sender, args) => Resize(new Vector2f(args.Width, args.Height));
            Window.MouseButtonPressed += (sender, args) => DispatchEvent(new MouseButtonInputArgs(args.Button, true, args.X, args.Y));
            Window.MouseButtonReleased += (sender, args) => DispatchEvent(new MouseButtonInputArgs(args.Button, false, args.X, args.Y));
            Window.MouseWheelMoved += (sender, args) => DispatchEvent(new MouseWheelInputArgs(args.Delta, args.X, args.Y));
            Window.MouseMoved += (sender, args) => DispatchEvent(new MouseMoveInputArgs(args.X, args.Y));
            Window.TextEntered += (sender, args) => DispatchEvent(new TextInputArgs(args.Unicode));

            Window.KeyPressed += (sender, args) =>
            {
                if (args.Code == Keyboard.Key.Unknown || keyStates[(int)args.Code]) // repeated key press
                    return; 
                keyStates[(int)args.Code] = true;
                DispatchEvent(new KeyInputArgs(args.Code, true, args.Control, args.Shift));
            };

            Window.KeyReleased += (sender, args) =>
            {
                if (args.Code != Keyboard.Key.Unknown)
                    keyStates[(int)args.Code] = false;
                DispatchEvent(new KeyInputArgs(args.Code, false, args.Control, args.Shift));
            };
        }

        public static void Run()
        {
            var timer = new Stopwatch();
            float accumulator = 0;

            while (Window.IsOpen())
            {
                var time = (float)timer.Elapsed.TotalSeconds;
                timer.Restart();

                accumulator += time;

                // Update
                while (accumulator >= GameOptions.Timestep)
                {
                    Window.DispatchEvents();
                    Timer.Update();

                    for (var i = 0; i < states.Count; i++)
                    {
                        var state = states[i];

                        if (i == states.Count - 1 || state.InactiveMode.HasFlag(State.UpdateMode.Update))
                            state.UpdateInternal();
                    }

                    accumulator -= GameOptions.Timestep;
                }

                // Draw
                for (var i = 0; i < states.Count; i++)
                {
                    var state = states[i];

                    if (i != states.Count - 1 && !state.InactiveMode.HasFlag(State.UpdateMode.Draw))
                        continue;

                    if (i == 0)
                        Window.Clear(state.ClearColor);

                    state.Draw(Window);
                }

                Window.Display();
            }
        }

        public static void Stop()
        {
            Window.Close();
        }

        public static void SetState(State state)
        {
            states.Clear();
            states.Add(state);
        }

        public static void PushState(State state)
        {
            states.Add(state);
        }

        public static void PopState()
        {
            states.RemoveAt(states.Count - 1);
        }

        private static void DispatchEvent(InputArgs args)
        {
            for (var i = states.Count - 1; i >= 0; i--)
            {
                var state = states[i];

                args.View = state.Camera.View;
                if (states[i].ProcessEvent(args))
                    return;
            }
        }

        private static void Resize(Vector2f newSize)
        {
            size = newSize;

            foreach (var state in states)
            {
                state.InitializeCamera();
            }
        }
    }
}
