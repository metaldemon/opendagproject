using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using opendagproject.Content;
using opendagproject.Game.Graphics;
using opendagproject.Game.World;

namespace opendagproject.Game.World
{
    class Tile
    {
        public Vector2 position;
        public Sprite sprite;
        public int layer;

        public float lightLevel = 0.5f;

        public Color4 color = Color4.White;
        public List<float> lightingpasses = new List<float>();

        public List<int> surroundingtiles = new List<int>();

        public List<Vector2> corners = new List<Vector2>();

        public bool mouseControls = false;

        public string onwalkoverFunction;
        public string onfireFunction;

        public Tile(Vector2 position, string texturename, int layer)
        {
            this.position = position;
            this.sprite = new Sprite(texturename, this.position);
            this.layer = layer;
            this.corners.Add(this.sprite.position + new Vector2(-16, -16));
            this.corners.Add(this.sprite.position + new Vector2(16, -16));
            this.corners.Add(this.sprite.position + new Vector2(16, 16));
            this.corners.Add(this.sprite.position + new Vector2(-16, 16));
        }

        public virtual void tick(double delta)
        {
            if (this.mouseControls)
            {
                if (GameUtils.getDistance(Graphics.Graphics.screenPositionToGamePosition(Input.InputManager.getCurrentMousePosition()), this.position) < 16)
                {
                    this.onHover();
                    if (Input.InputManager.clicked())
                    {
                        this.onClick();
                    }
                }
            }
        }

        public virtual void onHover()
        {

        }

        public virtual void onClick()
        {

        }

        public virtual string getSaveData()
        {
            return position.ToString() + " " + layer + " " + sprite.getTextureName();
        }

        public virtual void draw()
        {
            sprite.drawwocamera(Color4.White);
        }

        public void applyLighting()
        {
            Graphics.Graphics.drawQuad(this.position + Graphics.Graphics.cameraPosition + new Vector2(GameUtils.resolutionX / 2, GameUtils.resolutionY / 2), new Vector2(32, 32), this.color);
        }
    }
}
