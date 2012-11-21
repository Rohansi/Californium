using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace Californium
{
    public static class FontManager
    {
        private static Dictionary<string, Font> fonts;

        static FontManager()
        {
            fonts = new Dictionary<string, Font>();
        }

        public static Font Load(string fname, bool persistent = false)
        {
            Font t;

            if (fonts.TryGetValue(fname, out t))
                return t;

            t = new Font(GameOptions.TextureLocation + fname);
            fonts.Add(fname, t);

            return t;
        }
    }
}
