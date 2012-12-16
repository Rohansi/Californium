using System;
using System.Collections.Generic;
using System.Threading;

namespace Californium
{
    public class AssetPreloader : State
    {
        private List<string> textures;
        private List<string> sounds;
        private List<string> fonts;

        private Thread thread;
        private int totalResources;
        private int completedResources;

        public Action Completed;

        public float Progress
        {
            get { return (float)completedResources / totalResources; }
        }

        public AssetPreloader(List<string> textures, List<string> sounds = null, List<string> fonts = null)
        {
            this.textures = textures;
            this.sounds = sounds;
            this.fonts = fonts;

            totalResources = (textures != null ? textures.Count : 0) +
                             (sounds != null ? sounds.Count : 0) +
                             (fonts != null ? fonts.Count : 0);
            completedResources = 0;

            thread = new Thread(ThreadMethod);
            thread.Start();
        }

        private void ThreadMethod()
        {
            Preload(textures, Assets.LoadTexture);
            Preload(sounds, Assets.LoadSound);
            Preload(fonts, Assets.LoadFont);
            Completed();
        }

        private void Preload(IEnumerable<string> resources, Func<string, object> loader)
        {
            if (resources == null)
                return;

            foreach (var res in resources)
            {
                loader(res);
                completedResources++;
            }
        }
    }
}
