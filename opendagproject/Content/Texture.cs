using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Content
{
    class Texture
    {
        public int width, height, textureID;
        public string name;
        public string file;

        public Texture(int w, int h, int TID, string name, string file)
        {
            this.width = w;
            this.height = h;
            this.textureID = TID;
            this.name = name;
            this.file = file;
        }
    }
}
