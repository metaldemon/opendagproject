using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System.IO;
using opendagproject.Game;

namespace opendagproject.Content
{
    class ContentManager
    {
        public static List<Texture> textureList = new List<Texture>();
        
        public static Texture getTexture(string name)
        {
            return textureList.First(x => x.name == name);
        }

        public static void loadTexture(string filepath, string name)
        {
            try
            {
                int glid = GL.Utils.LoadImage(filepath, true);
                GL.BindTexture(TextureTarget.Texture2D, glid);

                int width, height;
                GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, out width);
                GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight, out height);

                textureList.Add(new Texture(width, height, glid, name, filepath));
                Debug.WriteLine("loaded texture: \"" + name + "\"", ConsoleColor.Yellow); 
            }
            catch (Exception e)
            {
                ExceptionHandler.printException("Failed to load texture: \"" + filepath + "\"", ConsoleColor.Red, ExceptionHandler.ExceptionHandle.WAIT3SECONDS);
            }
        }


        /// <summary>
        /// loads all the textures in specified folder, default texturename is filename minus extention
        /// </summary>
        /// <param name="folderpath"></param>
        public static void loadAllTextures(string folderpath)
        {
            DirectoryInfo di = new DirectoryInfo(GameUtils.getGamePath() + folderpath);
            List<FileInfo> fi = di.GetFiles().ToList();
            fi.ForEach(x => loadTexture(x.FullName,x.Name));
        }
    }
}
