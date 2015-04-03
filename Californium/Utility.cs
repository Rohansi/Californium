using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public static class Utility
    {
        public static FloatRect Translate(this FloatRect value, float x, float y)
        {
            return new FloatRect(value.Left + x, value.Top + y, value.Width, value.Height);
        }

        public static Vector2f Rotate(this Vector2f value, Vector2f origin, float theta)
        {
            return new Vector2f(
                (float)Math.Cos(theta) * (value.X - origin.X) - (float)Math.Sin(theta) * (value.Y - origin.Y) + origin.X,
                (float)Math.Sin(theta) * (value.X - origin.X) + (float)Math.Cos(theta) * (value.Y - origin.Y) + origin.Y
            );
        }

        public static Vector2f ToFloat(this Vector2u value)
        {
            return new Vector2f(value.X, value.Y);
        }

        public static Vector2u ToUnsigned(this Vector2f value)
        {
            return new Vector2u((uint)value.X, (uint)value.Y);
        }

        public static float Lerp(float a, float b, float w)
        {
            return a + w * (b - a);
        }

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : ((value > max) ? max : value);
        }

        public static float ToDegrees(float dir)
        {
            return dir * (180 / (float)Math.PI);
        }

        public static float ToRadians(float dir)
        {
            return dir * ((float)Math.PI / 180);
        }

        public static float Distance(Vector2f p1, Vector2f p2)
        {
            return (float)Math.Sqrt(((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y)));
        }

        public static float Direction(Vector2f p1, Vector2f p2)
        {
            var r = (float)Math.Atan2(p1.Y - p2.Y, p2.X - p1.X);
            return r < 0 ? r + (2 * (float)Math.PI) : r;
        }

        public static Vector2f LengthDir(float dir, float len)
        {
            return new Vector2f((float)Math.Cos(dir) * len, (float)-Math.Sin(dir) * len);
        }

        public static float RoundToNearest(float value, float factor)
        {
            return (float)Math.Round(value / factor) * factor;
        }

        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> match)
        {
            foreach (var cur in dict.Where(match).ToList())
            {
                dict.Remove(cur.Key);
            }
        }

        public static void Center(this Text text, bool horizontal = true, bool vertical = true)
        {
            text.Origin = new Vector2f();

            var bounds = text.GetGlobalBounds();
            bounds.Left -= text.Position.X;
            bounds.Top -= text.Position.Y;

            var x = 0f;
            var y = 0f;

            if (horizontal)
            {
                x = bounds.Left / text.Scale.X;
                x += (bounds.Width / text.Scale.X) / 2;
            }

            if (vertical)
            {
                y = bounds.Top / text.Scale.Y;
                y += (bounds.Height / text.Scale.Y) / 2;
            }

            text.Origin = new Vector2f(x, y);
        }

        public static void Round(this Transformable trans)
        {
            trans.Position = new Vector2f((float)Math.Round(trans.Position.X), (float)Math.Round(trans.Position.Y));
            trans.Origin = new Vector2f((float)Math.Round(trans.Origin.X), (float)Math.Round(trans.Origin.Y));
        }

        public static Vector2f Round(this Vector2f value)
        {
            return new Vector2f(
                (float)Math.Round(value.X),
                (float)Math.Round(value.Y)
            );
        }
    }
}
