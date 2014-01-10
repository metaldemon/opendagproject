using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming;
using opendagproject.Content;

namespace opendagproject.Game.Graphics
{
    /// <summary>
    ///  add this to the using block atop of your class to have your standard graphics functions
    /// </summary>
    class Graphics
    {
        public static Vector2 cameraPosition = new Vector2(0, 0);

        public static void drawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color4 color)
        {
            GL.Color4(color);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(p1.X, p1.Y);
            GL.Vertex2(p2.X, p2.Y);
            GL.Vertex2(p3.X, p3.Y);
            GL.End();
        }

        public static void drawQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color4 color)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Color4(color);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(p1.X, p1.Y);
            GL.Vertex2(p2.X, p2.Y);
            GL.Vertex2(p3.X, p3.Y);
            GL.Vertex2(p4.X, p4.Y);
            GL.End();
        }

        public static void drawQuad(Vector2 p1, Vector2 dim, Color4 color)
        {
            dim.X /= 2;
            dim.Y /= 2;
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Color4(color);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(p1.X - dim.X, p1.Y - dim.Y);
            GL.Vertex2(p1.X + dim.X, p1.Y - dim.Y);
            GL.Vertex2(p1.X + dim.X, p1.Y + dim.Y);
            GL.Vertex2(p1.X - dim.X, p1.Y + dim.Y);
            GL.End();
            GL.Disable(EnableCap.Blend);// test test test :D
        }

        public static void drawLineQuad(Vector2 p1, Vector2 dim, Color4 color, float width)
        {
            dim.X /= 2;
            dim.Y /= 2;
            GL.Color4(color);
            GL.LineWidth(width);

            GL.PushMatrix();
            GL.Translate(p1.X + Graphics.cameraPosition.X, p1.Y + Graphics.cameraPosition.Y, 0);
            GL.Begin(BeginMode.Lines);
            {
                GL.Vertex2(-dim.X, -dim.Y);
                GL.Vertex2(dim.X, -dim.Y);

                GL.Vertex2(dim.X, -dim.Y);
                GL.Vertex2(dim.X, dim.Y);

                GL.Vertex2(dim.X, dim.Y);
                GL.Vertex2(-dim.X, dim.Y);

                GL.Vertex2(-dim.X, dim.Y);
                GL.Vertex2(-dim.X, -dim.Y);


            }
            GL.End();
            GL.PopMatrix();
        }


        public static void drawSprite(Vector2 origin, Vector2 dimensions, Sprite sprite)
        {
            if (!quadOnCanvas(origin, dimensions)) return;
            dimensions.X /= 2;
            dimensions.Y /= 2;

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            sprite.bind();
            GL.Color4(Color4.White);

            GL.PushMatrix();
            GL.Translate(cameraPosition.X + origin.X, cameraPosition.Y + origin.Y, 0);
            GL.Rotate(sprite.rotation, new Vector3(0, 0, 1));

            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(0, 0);
                GL.Vertex2(-dimensions.X, -dimensions.Y); 
                GL.TexCoord2(1, 0);
                GL.Vertex2(dimensions.X, -dimensions.Y); 
                GL.TexCoord2(1, 1);
                GL.Vertex2(dimensions.X, dimensions.Y); 
                GL.TexCoord2(0, 1);
                GL.Vertex2(-dimensions.X, dimensions.Y);

            }
            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);


        }


        public static bool pointOnCanvas(Vector2 p)
        {
            if (p.X + cameraPosition.X > 0 && p.X + cameraPosition.X < GameUtils.resolutionX &&
                p.Y + cameraPosition.Y > 0 && p.Y + cameraPosition.Y < GameUtils.resolutionY)
                return true;
            return false;
        }


        /// <summary>
        /// determines wether a quad is on the canvas
        /// </summary>
        /// <param name="p"></param> origin of the quad
        /// <param name="d"></param> dimensions of the quad
        /// <returns></returns>
        public static bool quadOnCanvas(Vector2 p, Vector2 d)
        {
            if (pointOnCanvas(p + new Vector2(-(d.X / 2), -(d.Y / 2))) || pointOnCanvas(p + new Vector2((d.X / 2), -(d.Y / 2))) ||
                pointOnCanvas(p + new Vector2((d.X / 2), (d.Y / 2))) || pointOnCanvas(p + new Vector2(-(d.X / 2), (d.Y / 2))))
                return true;
            return false;
        }

        public static void clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public static void update()
        {
            Glfw.SwapBuffers();
        }


        public static Vector2 screenPositionToGamePosition(Vector2 pos)
        {
            return pos - new Vector2(GameUtils.resolutionX / 2, GameUtils.resolutionY / 2) -
                   cameraPosition;
        }

        public static Vector2 screenPositionToGamePosition(Vector2 pos, bool grid)
        {
            return new Vector2((int)(pos.X / 32) * 32, (int)(pos.Y / 32) * 32) - new Vector2(GameUtils.resolutionX / 2, GameUtils.resolutionY / 2) -
                    new Vector2((int)(cameraPosition.X / 32) * 32, (int)(cameraPosition.Y / 32) * 32);
        }
    }
}
