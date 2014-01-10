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
using opendagproject.Game.Graphics.Font;
using opendagproject.Game.Tick;
using opendagproject.Game.PathFinding;
using opendagproject.Game.Player.Inventory;

namespace opendagproject.Game.RSL.Actors.Npc
{
    public class Npc : Actor
    {

        private Path path;
        private Vector2 target = new Vector2(0, 0);
        private AttackState attackState;


        public Npc()
        {

        }

        public void initialize()
        {
            string texturefile = (string)base.getVariable("texture");
            int x = Convert.ToInt32(base.getVariable("positionX"));
            int y = Convert.ToInt32(base.getVariable("positionY"));

            this.sprite = new Sprite(texturefile, new Vector2(x, y));
            base.variableList.First(u => u.name == "texture").getValue();
            this.target = this.sprite.position;
            this.initAttackState();
        }

        private void initAttackState()
        {
            if (this.getVariable("hostile").ToString() == Boolean.TrueString.ToLower())
            {
                InventoryItem weapon = InventoryHandler.getItemByName(getVariable("equippedweapon").ToString());
                this.attackState = new AttackState(this);
                string sprite = weapon.getVariable("ingamesprite").ToString();
                int attackspeed = Convert.ToInt32(weapon.getVariable("attackspeed"));
                this.attackState.weapon = new Sprite(sprite, this.sprite.position);
                this.attackState.attackspeed = (float)attackspeed;
            }
        }

        float followtime = 1000;

        public override void onRemove()
        {
            FontManager.removeText(this.name + "NpcName");
        }

        public new void tick()
        {
            base.actorUpdate();
            if (Game.States.GameStateManager.currentGlobalGameState != States.GameStateManager.GlobalGameState.Mapeditor)
            {
                float x = Convert.ToSingle(base.getVariable("positionX"));
                float y = Convert.ToSingle(base.getVariable("positionY"));
                int maxhealth = Convert.ToInt32(base.getVariable("maxhealth"));
                int health = Convert.ToInt32(base.getVariable("health"));
                if (this.getVariable("hostile").ToString() == Boolean.TrueString.ToLower())
                {
                    FontManager.addText(new Text(this.name + " " + health + " / " + maxhealth, this.name + "NpcName", new Vector2(x, y - 50), 25f, "franklin2.png", false, Text.positionOriginPoint.Middle, Color4.Red));
                }
                else
                {
                    FontManager.addText(new Text(this.name + " " + health + " / " + maxhealth, this.name + "NpcName", new Vector2(x, y - 50), 25f, "franklin2.png", false, Text.positionOriginPoint.Middle, Color4.Green));

                }
                if (World.WorldManager.loadedMap)
                {

                    followtime -= (float)GameTick.delta;
                    bool followsplayer = Convert.ToBoolean(base.getVariable("followsplayer"));
                    if (followsplayer && GameUtils.getDistance(this.sprite.position, Player.PlayerHandler.playerList[0].position) > 32)
                    {
                        bool hostile = (this.getVariable("hostile").ToString() == Boolean.TrueString.ToLower());
                        if (!hostile || hostile && canSeePlayer(Player.PlayerHandler.playerList[0]))
                        {

                            float followspeed = (float)Convert.ToDouble(base.getVariable("followspeed"));
                            if (path == null || (path != null && path.currentPathState == Path.PathState.COMPLETEDRUN))
                            {
                                if (Player.PlayerHandler.playerList[0].control)
                                {
                                    this.path = new Path(this.sprite.position, Player.PlayerHandler.playerList[0].position);
                                }
                                else
                                    this.path = new Path(this.sprite.position, Player.PlayerHandler.playerList[0].sprite.position);
                            }
                            Vector2 supposedtarget = this.path.getNextTarget();
                            if (supposedtarget != new Vector2(float.MaxValue, float.MaxValue) && supposedtarget != this.target)
                                this.target = supposedtarget;
                            if ((float)GameUtils.getDistance(this.sprite.position, this.target) > 16 && (float)GameUtils.getDistance(this.sprite.position, this.path.endPosition) > 32)
                            {
                                this.sprite.position = GameUtils.moveVector(this.sprite.position, followspeed, this.target, true, new Vector2(this.sprite.width, this.sprite.height));
                            }
                            else
                            {
                                if ((float)GameUtils.getDistance(this.sprite.position, this.path.endPosition) <= 32)
                                {
                                    this.path.currentPathState = Path.PathState.COMPLETEDRUN;
                                }
                            }
                            path.checkPosition(this.sprite.position);
                            if (path != null && path.currentPathState != Path.PathState.BUSY)
                            {
                                this.path.checkPosition(this.sprite.position);
                            }
                            else if (path != null && path.currentPathState == Path.PathState.BUSY)
                            {
                            }
                        }
                    }
                    base.setVariable("positionX", this.sprite.position.X);
                    base.setVariable("positionY", this.sprite.position.Y);
                    if (this.getVariable("hostile").ToString() == Boolean.TrueString.ToLower())
                    {
                        if (GameUtils.getDistance(this.sprite.position, Player.PlayerHandler.playerList.First(u => u.control).position) < 32)
                        {
                            if (this.attackState == null)
                            {
                                this.initAttackState();
                            }
                            this.attackState.attack();
                        }
                    }

                    if (this.attackState != null)
                    {
                        this.attackState.tick();
                    }
                }
            }
        }

        private bool canSeePlayer(Player.Player p)
        {
            Vector2 playerpos = Graphics.Graphics.screenPositionToGamePosition(p.sprite.position);
            if (!p.control)
            {
                playerpos = p.sprite.position;
            }
            if (GameUtils.getDistance(playerpos, this.sprite.position) < 400)
            {
                float distance = 0;
                float maxdistance = 400f;
                float speedperstep = 8f;
                float angle = (float)GameUtils.getRotation(this.sprite.position, playerpos) - 90;
                Vector2 currentpos = this.sprite.position;
                while (distance < maxdistance)
                {
                    currentpos += Vector2.Transform(new Vector2(0, speedperstep), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), (float)MathHelper.ToRadians(angle)));
                    distance += speedperstep;
                    if (GameUtils.getCollision(currentpos, new Vector2(1, 1)))
                    {
                        break;
                    }
                    if (GameUtils.getDistance(currentpos, playerpos) < 32)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void onClick()
        {
            string dialoguefile = (string)this.getVariable("dialoguefile");
            if (dialoguefile != null)
            {
                Dialogue.DialogueHandler.setDialogue(dialoguefile, this);
            }
        }

        public void draw()
        {
            if (this.attackState != null)
            {
                this.attackState.draw();
            }
            this.sprite.drawwocamera();
        }

        public Npc clone()
        {
            return (Npc)this.MemberwiseClone();
        }

        public List<string> getSaveData()
        {
            List<string> range = new List<string>();

            range.Add("NPC " + base.name);
            base.variableList.ForEach(x => range.Add(x.getSaveData()));

            return range;
        }
    }


    class AttackState
    {
        public enum AttackType
        {
            RANGED, MELEE
        }

        public enum AttackStage
        {
            IDLE,
            SWING,
            FIRE,
            DELAY
        }

        public AttackStage attackStage = AttackStage.IDLE;
        public AttackType attackType = AttackType.MELEE;

        private readonly float maxhalfswing = 90f;

        private float swingrotation;
        public float attackspeed = 600f;

        public Sprite weapon = null;

        public Npc parentNpc;

        public AttackState(Npc p)
        {
            this.parentNpc = p;
            this.swingrotation = -maxhalfswing;
        }

        public void attack()
        {
            if (this.attackStage == AttackStage.IDLE)
            {
                if (this.attackType == AttackType.MELEE)
                {
                    SoundManager.playSound("swordswing.ogg");
                    this.attackStage = AttackStage.SWING;
                    this.swingrotation = -maxhalfswing;
                }
                if (this.attackType == AttackType.RANGED)
                {
                    this.attackStage = AttackStage.FIRE;
                }
            }
        }

        public InventoryItem getWeapon()
        {
            return InventoryHandler.getItemByName(parentNpc.getVariable("equippedweapon").ToString());
        }

        public void tick()
        {
            if (this.attackStage == AttackStage.SWING)
            {
                this.swingrotation += attackspeed * (float)GameTick.deltaseconds;

                Vector2 weaponposition = this.weapon.position + Vector2.Transform(new Vector2(0, -32), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), MathHelper.ToRadians(this.swingrotation)));

                foreach (Player.Player n in Player.PlayerHandler.playerList)
                {
                    if (GameUtils.getDistance(weaponposition, n.position) < 32)
                    {
                        n.health -= Convert.ToInt32(getWeapon().getVariable("attackdamage"));
                        this.attackStage = AttackStage.DELAY;
                        SoundManager.playSound("hit.ogg");
                    }
                }


                if (this.swingrotation >= maxhalfswing)
                {
                    this.attackStage = AttackStage.DELAY;
                }
            }
            if (this.attackStage == AttackStage.DELAY)
            {
                this.attackStage = AttackStage.IDLE;

            }
        }

        public void draw()
        {
            if (this.attackStage == AttackStage.SWING)
            {
                this.weapon.position = parentNpc.sprite.position;
                this.weapon.rotation = parentNpc.sprite.rotation + this.swingrotation;
                this.weapon.additionalrotation = new Vector2(-32 * (float)Math.Sin(MathHelper.ToRadians(-this.weapon.rotation)), -32 * (float)Math.Cos(MathHelper.ToRadians(-this.weapon.rotation)));
                this.weapon.drawWithOriginWoCamera();
            }
            if (this.attackStage == AttackStage.FIRE)
            {


            }
        }
    }

}
