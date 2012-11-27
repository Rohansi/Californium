using System.Collections.Generic;
using System.Diagnostics;
using SFML.Audio;

namespace Californium
{
    public static class SoundManager
    {
        private static Dictionary<string, SoundBuffer> buffers;
        private static List<Sound> sounds;
        private static Music currentMusic;

        static SoundManager()
        {
            buffers = new Dictionary<string, SoundBuffer>();
            sounds = new List<Sound>();
        }

        public static void Play(string fname)
        {
            SoundBuffer sb;

            if (!buffers.TryGetValue(fname, out sb))
            {
                sb = new SoundBuffer(GameOptions.SoundLocation + fname);
                buffers.Add(fname, sb);
            }

            var s = new Sound(sb) { Volume = GameOptions.SoundVolume };
            s.Play();
            sounds.Add(s);

            sounds.RemoveAll(snd => snd.Status != SoundStatus.Playing);
        }

        public static void PlayMusic(string fname)
        {
            var music = new Music(GameOptions.MusicLocation + fname);
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
