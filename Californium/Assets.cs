using System;
using System.Collections.Generic;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Audio;

namespace Californium
{
    public static class Assets
    {
        static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        public static Texture LoadTexture(string name)
        {
            Texture t;

            if (textures.TryGetValue(name, out t))
                return t;

            t = new Texture(GameOptions.TextureLocation + name);
            textures.Add(name, t);

            return t;
        }

        static Dictionary<string, Font> fonts = new Dictionary<string, Font>();
        public static Font LoadFont(string name)
        {
            Font t;

            if (fonts.TryGetValue(name, out t))
                return t;

            t = new Font(GameOptions.FontLocation + name);
            fonts.Add(name, t);

            return t;
        }

        private static Dictionary<string, SoundBuffer> buffers = new Dictionary<string, SoundBuffer>();
        private static List<Sound> sounds = new List<Sound>();
        public static void PlaySound(string name)
        {
            SoundBuffer sb;

            if (!buffers.TryGetValue(name, out sb))
            {
                sb = new SoundBuffer(GameOptions.SoundLocation + name);
                buffers.Add(name, sb);
            }

            var s = new Sound(sb) { Volume = GameOptions.SoundVolume };
            s.Play();
            sounds.Add(s);

            sounds.RemoveAll(snd => snd.Status != SoundStatus.Playing);
        }

        private static Music currentMusic;
        public static void PlayMusic(string name)
        {
            var music = new Music(GameOptions.MusicLocation + name);
            var watch = new Stopwatch();
            int state = 0;
            var tween = Tween.Create(TweenType.OutQuad, 0, GameOptions.MusicVolume, 0.5f, () => state = 1);

            currentMusic = music;

            music.Volume = 0;
            music.Play();

            watch.Start();

            Timer.EveryFrame(() =>
            {
                double dt = watch.Elapsed.TotalSeconds;
                watch.Restart();

                if (music != currentMusic && state != 3)
                    state = 2;

                switch (state)
                {
                    case 0: // fade in
                        music.Volume = (float)tween(dt);
                        break;
                    case 1: // normal play
                        if (music.PlayingOffset.TotalSeconds >= music.Duration.TotalSeconds - 1)
                            state = 2;
                        break;
                    case 2: // setup fadeout
                        tween = Tween.Create(TweenType.OutQuad, music.Volume, 0, 0.5f, () => state = 10);
                        state = 3;
                        break;
                    case 3: // fade out
                        music.Volume = (float)tween(dt);

                        if (state != 3)
                            return true;

                        break;
                }

                return false;
            });
        }
    }
}
