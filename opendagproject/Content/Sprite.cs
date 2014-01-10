using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace opendagproject.Content
{
    public class Sprite
    {
        public Vector2 position;
        public Vector2 additionalrotation;
        private string textureName;

        public float width, height, rotation;

        private int displaynr = -1;
        private bool displaylistenabled = false;

        public Sprite(string texturename, Vector2 position)
        {
            this.position = position;
            this.textureName = texturename;
            this.width = Content.ContentManager.getTexture(this.textureName).width;
            this.height = Content.ContentManager.getTexture(this.textureName).height;
        }

        public Sprite(string texturename, Vector2 position, float w, float h)
        {
            this.position = position;
            this.textureName = texturename;
            this.width = w;
            this.height = h;

        }

        public string getTextureName()
        {
            return this.textureName;
        }

        public void createDisplayList()
        {
            displaynr = GL.GenLists(1);
            GL.NewList(this.displaynr, ListMode.Compile);
            this.draw();
            GL.EndList();
            displaylistenabled = true;
        }

        public void bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, Content.ContentManager.getTexture(this.textureName).textureID);
        }

        public void draw()
        {
            if (!displaylistenabled)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                GL.BindTexture(TextureTarget.Texture2D, Content.ContentManager.getTexture(this.textureName).textureID);
                GL.Color4(Color4.White);

                GL.PushMatrix();
                GL.Translate(new Vector3(this.position.X, this.position.Y, 0f));
                GL.Rotate(this.rotation, new Vector3(0, 0, 1));

                GL.Begin(BeginMode.Quads);

                GL.TexCoord2(0, 0);
                GL.Vertex2(-(width / 2), -(height / 2));
                GL.TexCoord2(1, 0);
                GL.Vertex2((width / 2), -(height / 2));
                GL.TexCoord2(1, 1);
                GL.Vertex2((width / 2), (height / 2));
                GL.TexCoord2(0, 1);
                GL.Vertex2(-(width / 2), (height / 2));

                GL.End();

                GL.PopMatrix();

                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Blend);
            }
            else
            {
                GL.CallList(this.displaynr);
            }
        }

        public void drawwocamera()
        {
            if (!displaylistenabled)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                GL.BindTexture(TextureTarget.Texture2D, Content.ContentManager.getTexture(this.textureName).textureID);
                GL.Color4(Color4.White);

                GL.PushMatrix();
                GL.Translate(new Vector3(this.position.X + Game.Graphics.Graphics.cameraPosition.X + Game.GameUtils.resolutionX / 2, this.position.Y + Game.Graphics.Graphics.cameraPosition.Y + Game.GameUtils.resolutionY / 2, 0f));
                GL.Rotate(this.rotation, new Vector3(0, 0, 1));

                GL.Begin(BeginMode.Quads);

                GL.TexCoord2(0, 0);
                GL.Vertex2(-(width / 2), -(height / 2));
                GL.TexCoord2(1, 0);
                GL.Vertex2((width / 2), -(height / 2));
                GL.TexCoord2(1, 1);
                GL.Vertex2((width / 2), (height / 2));
                GL.TexCoord2(0, 1);
                GL.Vertex2(-(width / 2), (height / 2));

                GL.End();

                GL.PopMatrix();

                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Blend);
            }
            else
            {
                GL.CallList(this.displaynr);
            }
        }

        public void drawwocamera(Color4 color)
        {
            if (!displaylistenabled)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                GL.BindTexture(TextureTarget.Texture2D, Content.ContentManager.getTexture(this.textureName).textureID);
                

                GL.PushMatrix();
                GL.Translate(new Vector3(this.position.X + Game.Graphics.Graphics.cameraPosition.X + Game.GameUtils.resolutionX / 2, this.position.Y + Game.Graphics.Graphics.cameraPosition.Y + Game.GameUtils.resolutionY / 2, 0f));
                GL.Rotate(this.rotation, new Vector3(0, 0, 1));

                GL.Begin(BeginMode.Quads);

                GL.Color4(color);

                GL.TexCoord2(0, 0);
                GL.Vertex2(-(width / 2), -(height / 2));
                GL.TexCoord2(1, 0);
                GL.Vertex2((width / 2), -(height / 2));
                GL.TexCoord2(1, 1);
                GL.Vertex2((width / 2), (height / 2));
                GL.TexCoord2(0, 1);
                GL.Vertex2(-(width / 2), (height / 2));

                GL.End();

                GL.PopMatrix();

                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Blend);
            }
            else
            {
                GL.CallList(this.displaynr);
            }
        }

        public void drawWithOrigin()
        {
            if (!displaylistenabled)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                GL.BindTexture(TextureTarget.Texture2D, Content.ContentManager.getTexture(this.textureName).textureID);
                GL.Color4(Color4.White);

                GL.PushMatrix();
                GL.Translate(new Vector3(this.position.X + this.additionalrotation.X, this.position.Y + this.additionalrotation.Y, 0f));
                GL.Rotate(this.rotation, new Vector3(0, 0, 1));
                GL.Begin(BeginMode.Quads);

                GL.TexCoord2(0, 0);
                GL.Vertex2(-(width / 2), -(height / 2));
                GL.TexCoord2(1, 0);
                GL.Vertex2((width / 2), -(height / 2));
                GL.TexCoord2(1, 1);
                GL.Vertex2((width / 2), (height / 2));
                GL.TexCoord2(0, 1);
                GL.Vertex2(-(width / 2), (height / 2));

                GL.End();

                GL.PopMatrix();

                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Blend);
            }
            else
            {
                GL.CallList(this.displaynr);
            }
        }

        public void drawWithOriginWoCamera()
        {
            if (!displaylistenabled)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                GL.BindTexture(TextureTarget.Texture2D, Content.ContentManager.getTexture(this.textureName).textureID);
                GL.Color4(Color4.White);

                GL.PushMatrix();
                GL.Translate(new Vector3(this.position.X + this.additionalrotation.X + Game.Graphics.Graphics.cameraPosition.X + Game.GameUtils.resolutionX / 2, this.position.Y + this.additionalrotation.Y + Game.Graphics.Graphics.cameraPosition.Y + Game.GameUtils.resolutionY / 2, 0f));
                GL.Rotate(this.rotation, new Vector3(0, 0, 1));
                GL.Begin(BeginMode.Quads);

                GL.TexCoord2(0, 0);
                GL.Vertex2(-(width / 2), -(height / 2));
                GL.TexCoord2(1, 0);
                GL.Vertex2((width / 2), -(height / 2));
                GL.TexCoord2(1, 1);
                GL.Vertex2((width / 2), (height / 2));
                GL.TexCoord2(0, 1);
                GL.Vertex2(-(width / 2), (height / 2));

                GL.End();

                GL.PopMatrix();

                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Blend);
            }
            else
            {
                GL.CallList(this.displaynr);
            }
        }

        public void drawFrame()
        {
            GL.Color4(Color4.Green);

            GL.LineWidth(5);

            GL.PushMatrix();
            GL.Translate(new Vector3(this.position.X + Game.Graphics.Graphics.cameraPosition.X + Game.GameUtils.resolutionX / 2, this.position.Y + Game.Graphics.Graphics.cameraPosition.Y + Game.GameUtils.resolutionY / 2, 0f));
            GL.Rotate(this.rotation, new Vector3(0, 0, 1));

            GL.Begin(BeginMode.Lines);

            GL.Vertex2(-(width / 2), -(height / 2));
            GL.Vertex2((width / 2), -(height / 2));

            GL.Vertex2((width / 2), -(height / 2));
            GL.Vertex2((width / 2), (height / 2));

            GL.Vertex2((width / 2), (height / 2));
            GL.Vertex2(-(width / 2), (height / 2));

            GL.Vertex2(-(width / 2), (height / 2));
            GL.Vertex2(-(width / 2), -(height / 2));

            GL.End();

            GL.PopMatrix();
        }
    }
}
