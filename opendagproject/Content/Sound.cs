using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming.Audio;

namespace opendagproject.Content
{
    class Sound
    {
        private Pencil.Gaming.Audio.Sound snd;
        public string name;

        public Sound(string filename, string name)
        {
            this.snd = new Pencil.Gaming.Audio.Sound(filename);
            this.name = name;
            
        }

        public void play()
        {
            snd.Play();
        }

        public void stop()
        {
            snd.Stop();
        }

        public void dispose()
        {
            snd.Dispose();
        }
    }
}
