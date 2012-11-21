using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            get { return Window.DefaultView; }
        }

        private static List<State> states;

        static Game()
        {
            states = new List<State>();
        }

        public static void Initialize()
        {
            Window = new RenderWindow(new VideoMode(GameOptions.Width, GameOptions.Height), GameOptions.Caption, Styles.Close);
            Window.SetFramerateLimit(GameOptions.Framerate);
            Window.SetVerticalSyncEnabled(GameOptions.Vsync);

            Window.Closed += (sender, args) => Window.Close();
            Window.KeyPressed += (sender, args) => DispatchEvent(new KeyInputArgs(args.Code, true, args.Control, args.Shift));
            Window.KeyReleased += (sender, args) => DispatchEvent(new KeyInputArgs(args.Code, false, args.Control, args.Shift));
            Window.MouseButtonPressed += (sender, args) => DispatchEvent(new MouseButtonInputArgs(args.Button, true, args.X, args.Y));
            Window.MouseButtonReleased += (sender, args) => DispatchEvent(new MouseButtonInputArgs(args.Button, false, args.X, args.Y));
            Window.MouseWheelMoved += (sender, args) => DispatchEvent(new MouseWheelInputArgs(args.Delta, args.X, args.Y));
            Window.MouseMoved += (sender, args) => DispatchEvent(new MouseMoveInputArgs(args.X, args.Y));
        }

        public static void Run()
        {
            var timer = new Stopwatch();

            while (Window.IsOpen())
            {
                float dt = (float)timer.ElapsedMilliseconds / 1000;
                timer.Restart();

                Timer.Update(dt);

                Window.DispatchEvents();

                for (int i = 0; i < states.Count; i++)
                {
                    var state = states[i];

                    if (i == states.Count - 1 || state.InactiveMode.HasFlag(State.FrameStep.Update))
                        state.Update(dt);

                    if (i == states.Count - 1 || state.InactiveMode.HasFlag(State.FrameStep.Draw))
                    {
                        if (i == 0)
                            Window.Clear(state.ClearColor);

                        state.Draw(Window);
                    }
                }

                Window.Display();
            }
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
            for (int i = states.Count - 1; i >= 0; i--)
            {
                if (states[i].ProcessEvent(args)) return;
            }
        }
    }
}
