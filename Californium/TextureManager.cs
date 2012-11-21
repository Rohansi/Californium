using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;

namespace Californium
{
    public static class TextureManager
    {
        private static Dictionary<string, Texture> textures;

        static TextureManager()
        {
            textures = new Dictionary<string, Texture>();
        }

        public static Texture Load(string fname, bool persistent = false)
        {
            Texture t;

            if (textures.TryGetValue(fname, out t))
                return t;

            t = new Texture(GameOptions.TextureLocation + fname);
            textures.Add(fname, t);

            return t;
        }
    }
}
