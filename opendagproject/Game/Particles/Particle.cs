using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using opendagproject.Game.Graphics;
using opendagproject.Content;
using opendagproject.Game.RSL.Actors;
using opendagproject.Game.RSL.Actors.Npc;
using opendagproject.Game.Player;

namespace opendagproject.Game.Particles
{
    class Particle : Actor
    {
        private Sprite sprite;

        public float rotation = 0f;
        private float speed = 0f;
        public Vector2 position;

        public bool remove = false;

        public enum ParticleTarget
        {
            PLAYER,
            NPC
        }

        public ParticleTarget target;



        public Particle()
        {
        }

        public void init()
        {
            this.sprite = new Sprite(base.getVariable("sprite").ToString(), this.position);
            this.speed = Convert.ToInt32(base.getVariable("speed"));

        }

        public void tick()
        {
            bool collision = false;
            this.position = GameUtils.moveVector(this.position, this.speed, this.rotation, out collision, new Vector2(8, 8));
            this.sprite.position = this.position;
            this.sprite.rotation = this.rotation;
            if (collision) { onWallHit(); }
            if (playerHit()) { onPlayerHit(); }
            Npc n = getNpcHit(); if (n != null) { onNpcHit(ref n); }
        }

        private Npc getNpcHit()
        {
            foreach (Npc n in NpcHandler.npcGameList)
            {
                if (GameUtils.getDistance(this.position, n.sprite.position) < 16)
                {
                    return n;
                }
            }

            return null;
        }

        private bool playerHit()
        {
            if (GameUtils.getDistance(this.position, PlayerHandler.playerList[0].position) < 16)
            {
                return true;
            }
            return false;
        }

        public virtual void onWallHit()
        {
            this.remove = true;
        }

        public virtual void onPlayerHit()
        {

        }

        public virtual void onNpcHit(ref Npc n)
        {
            n.setVariable("health", Convert.ToInt32(n.getVariable("health")) - Convert.ToInt32(base.getVariable("npcdamage")));
            this.remove = true;
        }

        public List<Game.RSL.Variable> getVariableList()
        {
            return base.variableList;
        }

        public void addVariable(Game.RSL.Variable var)
        {
            base.variableList.Add(var);
        }

        public Particle copy()
        {
            return (Particle)this.MemberwiseClone();
        }

        public void draw()
        {
            if (this.sprite != null)
            {
                this.sprite.drawwocamera();
            }
        }
    }
}
