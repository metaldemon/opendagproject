using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Pencil.Gaming;

namespace opendagproject.Game.Input
{
    class KeyConfig
    {
        private List<KeyBinding> keyBindings = new List<KeyBinding>();

        public void addKeyBinding(KeyBinding key)
        {
            this.keyBindings.Add(key);
        }
        public void addKeyBinding(string name, int key)
        {
            this.keyBindings.Add(new KeyBinding(name, key));
        }

        public void save(string path)
        {
            StreamWriter sw = new StreamWriter(path);
            KeyboardState state = KeyboardState.GetState();
            for (int a = 0; a < 190; a++)
            {
                if (char.IsLetterOrDigit((char)a) || char.IsPunctuation((char)a))
                {
                    sw.WriteLine("// key: " + a + " default: " + (char)a);
                }
            }
            keyBindings.ForEach(x => sw.WriteLine("Key " + x.getName() + " [" + x.getKey().ToString() + "]"));
            sw.Close();
        }

        public int getKey(string name, out bool result)
        {
            try
            {
                result = true;
                return keyBindings.First(x => x.getName() == name).getKey();                    
            }
            catch (Exception e)
            {
                result = false;
                return -1;
            }
        }


    }
    class KeyBinding
    {
        private string name;
        private int key;

        public KeyBinding(string name, int key)
        {
            this.name = name;
            this.key = key;
        }

        public string getName()
        {
            return this.name;
        }

        public int getKey()
        {
            return this.key;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public void setKey(int key)
        {
            this.key = key;
        }
    }
}
