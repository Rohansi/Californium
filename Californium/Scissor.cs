using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class Scissor
    {
        private const int GL_SCISSOR_TEST = 0x0C11;

        [DllImport("opengl32")]
        private static extern void glEnable(int cap);

        [DllImport("opengl32")]
        private static extern void glDisable(int cap);

        [DllImport("opengl32")]
        private static extern void glScissor(int x, int y, int width, int height);

        private readonly RenderTarget target;
        private readonly Stack<IntRect> scissorStack; 

        public Scissor(RenderTarget renderTarget)
        {
            target = renderTarget;
            scissorStack = new Stack<IntRect>();
        }

        public ScissorRegion Create(FloatRect rect)
        {
            var topLeftCoord = new Vector2f(rect.Left, rect.Top);
            var bottomRightCoord = topLeftCoord + new Vector2f(rect.Width, rect.Height);

            var topLeftScreen = target.MapCoordsToPixel(topLeftCoord);
            var bottomRightScreen = target.MapCoordsToPixel(bottomRightCoord);

            var screenRect = new IntRect(topLeftScreen.X, topLeftScreen.Y, bottomRightScreen.X - topLeftScreen.X, bottomRightScreen.Y - topLeftScreen.Y);
            var top = scissorStack.Count > 0 ? scissorStack.Peek() : new IntRect(0, 0, (int)target.Size.X, (int)target.Size.Y);

            // Clamp screenRect to top

            Apply(screenRect);
            return new ScissorRegion(this);
        }

        internal void Pop()
        {
            scissorStack.Pop();

            if (scissorStack.Count == 0)
            {
                glDisable(GL_SCISSOR_TEST);
                return;
            }

            var rect = scissorStack.Peek();
            glScissor(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        private void Apply(IntRect rect)
        {
            if (scissorStack.Count == 0)
                glEnable(GL_SCISSOR_TEST);

            glScissor(rect.Left, rect.Top, rect.Width, rect.Height);
            scissorStack.Push(rect);
        }
    }

    public class ScissorRegion : IDisposable
    {
        private readonly Scissor scissor;

        public ScissorRegion(Scissor scissor)
        {
            this.scissor = scissor;
        }

        public void Dispose()
        {
            scissor.Pop();
        }
    }
}
