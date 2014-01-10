using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Pencil.Gaming.Audio;
using opendagproject.Game;

namespace opendagproject.Content
{
    class SoundManager
    {
        private static List<Sound> soundList = new List<Sound>();


        public static void loadSound(string filepath, string name)
        {
            try
            {
                soundList.Add(new Sound(filepath, name));
                //Debug.WriteLine("loaded sound: \"" + name + "\"", ConsoleColor.Yellow);
            }
            catch (Exception e)
            {
                ExceptionHandler.printException("Failed to load sound: \"" + filepath + "\"", ConsoleColor.Red, ExceptionHandler.ExceptionHandle.WAIT3SECONDS);
            }
        }

        public static void playSound(string name)
        {
            try
            {
                soundList.First(x => x.name == name).play();
            }
            catch (Exception e)
            {
                ExceptionHandler.printException("Could not play audio file with name \"" + name + "\"", ConsoleColor.Red, ExceptionHandler.ExceptionHandle.CONTINUE);
            }
        }

        public static void stopSound(string name)
        {
            soundList.First(x => x.name == name).stop();
        }

        /// <summary>
        /// loads all the textures in specified folder, default texturename is filename minus extention
        /// </summary>
        /// <param name="folderpath"></param>
        public static void loadAllSounds(string folderpath)
        {
            DirectoryInfo di = new DirectoryInfo(GameUtils.getGamePath() + folderpath);
            List<FileInfo> fi = di.GetFiles().ToList();
            fi.ForEach(x => loadSound(x.FullName, x.Name));
        }

        public static void dispose()
        {
            soundList.ForEach(x => x.dispose());
        }
    }
}
