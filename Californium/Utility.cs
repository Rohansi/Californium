using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Window;

namespace Californium
{
    public static class Utility
    {
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

        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> match)
        {
            foreach (var cur in dict.Where(match).ToList())
            {
                dict.Remove(cur.Key);
            }
        }
    }
}
