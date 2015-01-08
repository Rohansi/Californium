using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class Input
    {
        public delegate bool KeyEvent(KeyInputArgs args);
        public delegate bool TextEvent(TextInputArgs args);
        public delegate bool MouseButtonEvent(MouseButtonInputArgs args);
        public delegate bool MouseWheelEvent(MouseWheelInputArgs args);
        public delegate bool MouseMoveEvent(MouseMoveInputArgs args);

        /// <summary>
        /// Keyboard button event handlers
        /// </summary>
        public readonly Dictionary<Keyboard.Key, KeyEvent> Key;

        /// <summary>
        /// Mouse button event handlers
        /// </summary>
        public readonly Dictionary<Mouse.Button, MouseButtonEvent> MouseButton;

        /// <summary>
        /// Text event handler. This event should always used for text input.
        /// </summary>
        public TextEvent Text;

        /// <summary>
        /// Mouse wheel handler. Will not occur when outside of the window.
        /// </summary>
        public MouseWheelEvent MouseWheel;
        
        /// <summary>
        /// Mouse move handler. Will not occur when outside of the window.
        /// </summary>
        public MouseMoveEvent MouseMove;

        public Input()
        {
            Key = new Dictionary<Keyboard.Key, KeyEvent>();
            MouseButton = new Dictionary<Mouse.Button, MouseButtonEvent>();
            Text = null;
            MouseWheel = null;
            MouseMove = null;
        }

        internal bool ProcessInput(InputArgs args)
        {
            if (MouseMove != null)
            {
                var eArgs = args as MouseMoveInputArgs;

                if (eArgs != null)
                    return MouseMove(eArgs);
            }

            if (MouseWheel != null)
            {
                var eArgs = args as MouseWheelInputArgs;

                if (eArgs != null)
                    return MouseWheel(eArgs);
            }

            if (Text != null)
            {
                var eArgs = args as TextInputArgs;

                if (eArgs != null)
                    return Text(eArgs);
            }

            var keyArgs = args as KeyInputArgs;
            if (keyArgs != null)
            {
                KeyEvent e;
                if (Key.TryGetValue(keyArgs.Key, out e))
                    return e(keyArgs);
            }

            var mouseArgs = args as MouseButtonInputArgs;
            if (mouseArgs != null)
            {
                MouseButtonEvent e;
                if (MouseButton.TryGetValue(mouseArgs.Button, out e))
                    return e(mouseArgs);
            }

            return false;
        }
    }

    public abstract class InputArgs
    {
        internal View View;
    }

    public class KeyInputArgs : InputArgs
    {
        public Keyboard.Key Key { get; protected set; }
        public bool Pressed { get; protected set; }
        public bool Control { get; private set; }
        public bool Shift { get; private set; }

        public KeyInputArgs(Keyboard.Key key, bool pressed, bool control, bool shift)
        {
            Key = key;
            Pressed = pressed;
            Control = control;
            Shift = shift;
        }
    }

    public class TextInputArgs : InputArgs
    {
        public string Text { get; protected set; }

        public TextInputArgs(string text)
        {
            Text = text;
        }
    }

    public class MouseButtonInputArgs : InputArgs
    {
        public Mouse.Button Button { get; protected set; }
        public bool Pressed { get; protected set; }
        public Vector2f Position { get { return Game.Window.MapPixelToCoords(screenPosition, View); } }
        private readonly Vector2i screenPosition;

        public MouseButtonInputArgs(Mouse.Button button, bool pressed, int x, int y)
        {
            Button = button;
            Pressed = pressed;
            screenPosition = new Vector2i(x, y);
        }
    }

    public class MouseWheelInputArgs : InputArgs
    {
        public int Delta { get; protected set; }
        public Vector2f Position { get { return Game.Window.MapPixelToCoords(screenPosition, View); } }
        private readonly Vector2i screenPosition;

        public MouseWheelInputArgs(int delta, int x, int y)
        {
            Delta = delta;
            screenPosition = new Vector2i(x, y);
        }
    }

    public class MouseMoveInputArgs : InputArgs
    {
        public Vector2f Position { get { return Game.Window.MapPixelToCoords(screenPosition, View); } }
        private readonly Vector2i screenPosition;

        public MouseMoveInputArgs(int x, int y)
        {
            screenPosition = new Vector2i(x, y);
        }
    }
}
