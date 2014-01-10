using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using opendagproject.Game.Graphics;
using opendagproject.Game.World;

namespace opendagproject.Game.Graphics.Lighting
{
    class Light
    {
        public float radius = 0;
        public Vector2 position;
        public Vector2 previousPosition;
        private readonly float ambient = LightHandler.ambient;
        private Color4 color;

        public enum LightRenderType
        {
            STATIC,
            DYNAMIC
        }

        public enum LightType
        {
            REGULAR,
            POINTLIGHT,
            ROTATINGCONE
        }



        private LightRenderType lightRenderType = LightRenderType.STATIC;
        private LightType lightType = LightType.REGULAR;

        private List<int> tilesInView = new List<int>();

        private List<Vector2> endNodes = new List<Vector2>();


        public Light(Vector2 position, float radius, LightRenderType rendertype, LightType type, Color4 color)
        {
            
            color.R /= 2;
            color.G /= 2;
            color.B /= 2;
            this.color = color;
            this.position = position;
            this.previousPosition = position;
            this.radius = radius;
            this.lightRenderType = rendertype;
            this.lightType = type;
            //this.render();
        }

        public void render()
        {
            this.renderLight();
        }


        public int startnode = 0;
        public int conewidth = 30;
        public int rotationspeed = 1;

        private void renderLight()
        {
            if ((this.lightRenderType == LightRenderType.DYNAMIC && GameUtils.getDistance(this.position, this.previousPosition) > 1) || (this.lightRenderType == LightRenderType.STATIC && this.endNodes.Count == 0))
            {
                this.endNodes = new List<Vector2>();
                for (double a = 0; a < MathHelper.Tau; a += MathHelper.Tau / 360)
                {
                    Ray r = new Ray(this.position, a, this.radius);
                    endNodes.Add(r.endposition);
                }
            }
        }

        private void _renderLight(){

            }

        public void draw()
        {
            this.renderLight();
            if (this.endNodes.Count > 0)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.OneMinusDstColor, BlendingFactorDest.One);
                GL.PushMatrix();
                GL.Translate(new Vector3(Graphics.cameraPosition.X + GameUtils.resolutionX / 2, Graphics.cameraPosition.Y + GameUtils.resolutionY / 2, 0));

                GL.Begin(BeginMode.TriangleFan);
                
                GL.Color4(color);
                GL.Vertex2(this.position.X, this.position.Y);
                if (this.lightType == LightType.REGULAR)
                {
                   
                    for (int a = 0; a < this.endNodes.Count; a++)
                    {

                        Vector2 tv = this.endNodes[a];
                        Vector2 tvt = tv - this.position;
                        float val = 1f - (tvt.Length / this.radius);
                        //float val = 1f - (tvt.Length * tvt.Length / (this.radius * this.radius));
                        GL.Color4(new Color4(val * color.R, val * color.G, val * color.B, val * color.A));

                        GL.Vertex2(tv.X, tv.Y);
                    }
                    GL.Vertex2(this.endNodes[0].X, this.endNodes[0].Y);
                }
                else if (this.lightType == LightType.ROTATINGCONE || this.lightType == LightType.POINTLIGHT)
                {
                    int b = 0;
                    for (int a = startnode; b < conewidth + 1; a++)
                    {
                        b++;
                        if (a >= this.endNodes.Count)
                        {
                            a %= this.endNodes.Count;
                        }
                        Vector2 tv = this.endNodes[a];
                        Vector2 tvt = tv - this.position;
                        float val = 1f - (tvt.Length / this.radius);
                        GL.Color4(new Color4(val * color.R, val * color.G, val * color.B, val * color.A));

                        GL.Vertex2(tv.X, tv.Y);
                        
                    }
                    if (this.lightType == LightType.ROTATINGCONE)
                    {
                        startnode += rotationspeed;
                    }
                }
                GL.End();
                GL.PopMatrix();

                GL.Disable(EnableCap.Blend);

               
            }
            if (GameUtils.getDistance(this.position, this.previousPosition) > 1)
            {
                this.previousPosition = this.position;
            }
        }

        public string getSaveData()
        {
            return "Light " + this.position.ToString() + " "
                + this.radius.ToString() + " "
                + (this.color.R * 2).ToString() + " "
                + (this.color.G * 2).ToString() + " "
                + (this.color.B * 2).ToString() + " "
                + (this.color.A * 2).ToString() + " "
                + this.lightRenderType.ToString() + " "
                + this.lightType.ToString() + " "
                + this.startnode.ToString() + " "
                + this.conewidth.ToString() + " "
                + this.rotationspeed.ToString();
        }

    }

    class Ray
    {
        private readonly float speed = 1f;
        public float distance = 0f;
        public Vector2 endposition;
        public float distancetotarget = 0f;


        public Ray(Vector2 startpos, double angle, float maxdistance)
        {
            /*float xoffset = 32 - (startpos.X % 32);
            float yoffset = (float)Math.Tan(angle) * (xoffset);
            startpos += new Vector2(xoffset, yoffset);
            Vector2 addvector = new Vector2(32f / (float)Math.Tan(angle), (float)Math.Tan(angle) * 32f);
            while (distance <= maxdistance)
            {
                if (GameUtils.getCollision(startpos, new Vector2(1, 1)))
                {
                    break;
                }
                distance += speed;
                startpos += addvector;
            }*/
            Vector2 addvector = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
            while (distance <= maxdistance)
            {
                startpos += addvector;
                if (GameUtils.getCollision(startpos, new Vector2(1, 1)))
                {
                    break;
                }
                distancetotarget -= speed;
                distance += speed;
            }
            this.endposition = startpos;

        }
    }
}
