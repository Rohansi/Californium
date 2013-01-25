using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SFML.Graphics;
using SFML.Audio;

namespace Californium
{
    public static class Assets
    {
        private static readonly Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
        public static Texture LoadTexture(string name)
        {
            Texture texture;

            if (Textures.TryGetValue(name, out texture))
                return texture;

            texture = new Texture(Path.Combine(GameOptions.TextureLocation, name));
            Textures.Add(name, texture);

            return texture;
        }

        private static readonly Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
        public static Font LoadFont(string name)
        {
            Font font;

            if (Fonts.TryGetValue(name, out font))
                return font;

            font = new Font(Path.Combine(GameOptions.FontLocation, name));
            Fonts.Add(name, font);

            return font;
        }

        private static readonly Dictionary<string, SoundBuffer> Buffers = new Dictionary<string, SoundBuffer>();
        public static SoundBuffer LoadSound(string name)
        {
            SoundBuffer soundBuffer;

            if (!Buffers.TryGetValue(name, out soundBuffer))
            {
                soundBuffer = new SoundBuffer(Path.Combine(GameOptions.SoundLocation, name));
                Buffers.Add(name, soundBuffer);
            }

            return soundBuffer;
        }

        private static readonly List<Sound> Sounds = new List<Sound>();
        public static void PlaySound(string name)
        {
            var sound = new Sound(LoadSound(name));
            sound.Volume = GameOptions.SoundVolume;
            
            sound.Play();
            Sounds.Add(sound);

            Sounds.RemoveAll(snd => snd.Status != SoundStatus.Playing);
        }

        private static Music currentMusic;
        public static void PlayMusic(string name)
        {
            var state = 0;
            var tween = Tween.Create(TweenType.OutQuad, 0, GameOptions.MusicVolume, 0.5f, () => state = 1);
            var music = new Music(Path.Combine(GameOptions.MusicLocation, name));
            var watch = new Stopwatch();

            currentMusic = music;

            music.Volume = 0;
            music.Play();

            watch.Start();

            Timer.EveryFrame(() =>
            {
                var dt = watch.Elapsed.TotalSeconds;
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
