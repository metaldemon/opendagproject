using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace opendagproject.Game.Graphics.Font
{
    class FontManager
    {
        public static List<Text> textList = new List<Text>();

        public static void draw()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            int begin = (int)' ';
            foreach (Text t in textList)
            {
                if (t.time > 0)
                {
                    t.time -= (float)Game.Tick.GameTick.deltaseconds;
                }
                Vector2 pos = t.position;
                if (t.pop == Text.positionOriginPoint.Middle)
                {
                    pos -= new Vector2((getTextLength(t) / 2) * t.size, 0);
                }
                float current_x_pos = 0;
                GL.BindTexture(TextureTarget.Texture2D, Content.ContentManager.getTexture(t.fontName).textureID);
                for (int a = 0; a < t.text.Length; a++)
                {
                    int charnr = (int)t.text[a];
                    int ssnr = charnr - begin;
                    float x = (float)(ssnr % 10) / 10f;
                    float y = (float)(ssnr / 10) / 10f;

                    GL.Color4(t.color);

                    GL.Begin(BeginMode.Quads);

                    if (t.withCamera)
                    {

                        GL.TexCoord2(x + 0.01f, y);
                        GL.Vertex2(pos.X + current_x_pos, pos.Y);

                        GL.TexCoord2(x + 0.01f + 0.09f, y);
                        GL.Vertex2(pos.X + current_x_pos + t.size, pos.Y);

                        GL.TexCoord2(x + 0.01f + 0.09f, y + 0.1f);
                        GL.Vertex2(pos.X + current_x_pos + t.size, pos.Y + t.size * 1.2f);

                        GL.TexCoord2(x + 0.01f, y + 0.1f);
                        GL.Vertex2(pos.X + current_x_pos, pos.Y + t.size * 1.2f);
                    }
                    else
                    {

                        GL.TexCoord2(x, y);
                        GL.Vertex2((GameUtils.resolutionX / 2) + Graphics.cameraPosition.X + pos.X + current_x_pos, (GameUtils.resolutionY / 2) + Graphics.cameraPosition.Y + pos.Y);

                        GL.TexCoord2(x + 0.1f, y);
                        GL.Vertex2((GameUtils.resolutionX / 2) + Graphics.cameraPosition.X + pos.X + current_x_pos + t.size, (GameUtils.resolutionY / 2) + Graphics.cameraPosition.Y + pos.Y);

                        GL.TexCoord2(x + 0.1f, y + 0.1f);
                        GL.Vertex2((GameUtils.resolutionX / 2) + Graphics.cameraPosition.X + pos.X + current_x_pos + t.size, (GameUtils.resolutionY / 2) + Graphics.cameraPosition.Y + pos.Y + t.size * 1.2f);

                        GL.TexCoord2(x, y + 0.1f);
                        GL.Vertex2((GameUtils.resolutionX / 2) + Graphics.cameraPosition.X + pos.X + current_x_pos, (GameUtils.resolutionY / 2) + Graphics.cameraPosition.Y + pos.Y + t.size * 1.2f);
                    }
                    GL.End();
                    if (t.text[a] != ' ')
                        current_x_pos += t.size / 2;
                    else
                        current_x_pos += t.size / 2;
                }
            }
            textList = textList.Where(x => x.time > 0 || x.time == float.MinValue).ToList();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }

        public static void addText(Text t)
        {
            textList = textList.Where(x => x.name != t.name).ToList();
            textList.Add(t);
        }

        public static void removeText(string name)
        {
            textList = textList.Where(x => x.name != name).ToList();
        }

        public static void removeAllTexts()
        {
            textList = new List<Text>();
        }

        static float getTextLength(Text t)
        {
            float length = 0;
            for (int a = 0; a < t.text.Length; a++)
                if (t.text[a] != ' ')
                    length += 0.5f;
                else
                    length += 0.5f;
            return length;
        }
    }

    public class Text
    {
        public string text;
        public string name;
        public Vector2 position;
        public float size = 1f;
        public string fontName = string.Empty;
        public bool withCamera = false;
        public positionOriginPoint pop;
        public Color4 color;

        public float time = float.MinValue;


        public enum positionOriginPoint
        {
            UpperLeft,
            Middle,
            LowerLeft
        }

        public Text(string text, string name, Vector2 pos, float size, string fontname, bool withcamera, positionOriginPoint pop, Color4 color)
        {
            this.text = text;
            this.name = name;
            this.position = pos;
            this.size = size;
            this.fontName = fontname;
            this.withCamera = withcamera;
            this.pop = pop;
            this.color = color;
        }
        public Text(string text, string name, Vector2 pos, float size, string fontname, bool withcamera, positionOriginPoint pop, Color4 color, float time)
        {
            this.text = text;
            this.name = name;
            this.position = pos;
            this.size = size;
            this.fontName = fontname;
            this.withCamera = withcamera;
            this.pop = pop;
            this.color = color;
            this.time = time;
        }
    }

}
