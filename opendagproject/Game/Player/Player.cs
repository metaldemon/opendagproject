using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using opendagproject.Game.Graphics;
using opendagproject.Game.Tick;
using opendagproject.Content;
using opendagproject.Game.World;
using opendagproject.Game.Input;
using opendagproject.Game.PathFinding;
using opendagproject.Game.Player.Inventory;
using opendagproject.Game.Tick;
using opendagproject.Game.Graphics.Font;
using opendagproject.Game.RSL.Actors.Npc;

namespace opendagproject.Game.Player
{
    class Player
    {
        public Vector2 position;
        public Sprite sprite;
        public Sprite healthbar;
        public AttackState attackState;

        public float speed = 256;

        public bool control = false; // le very importante variable

        private Path path;

        public int playerID;


        public float health = 150;
        public float maxhealth = 100;

        public float delay = 0;

        public Inventory.Inventory inventory;

        public Player(Vector2 position, string filename)
        {
            this.position = position;
            this.sprite = new Sprite(filename, this.position);
            this.healthbar = new Sprite("barframe.png", new Vector2(128, GameUtils.resolutionY - 16));
            this.attackState = new AttackState(this);
            this.inventory = new Inventory.Inventory(this);
        }

        public Player(Vector2 position, string filename, int ID)
        {
            this.position = position;
            this.sprite = new Sprite(filename, this.position);
            this.healthbar = new Sprite("barframe.png", new Vector2(128, GameUtils.resolutionY - 16));
            this.playerID = ID;
            this.attackState = new AttackState(this);
            this.inventory = new Inventory.Inventory(this);
        }

        public void addInventoryItem(string itemname, int count)
        {
            this.inventory.addItem(itemname, count);
        }

        public void tick()
        {
            inventory.tick();
            KeyboardState keystate = KeyboardState.GetState();
            if (this.control)
            {
                if (this.health < this.maxhealth)
                {
                    this.health += ((this.maxhealth - this.health) / this.maxhealth) * 0.5f;
                }
                if (this.health < 0)
                {
                    Environment.Exit(0);
                }
                if (this.health > this.maxhealth)
                {
                    this.health = this.maxhealth;
                }
                Vector2 bufferposition = this.position;

                if (InputManager.currentKeyState.isKeyDown("FORWARD"))
                {
                    bufferposition.Y -= speed * (float)GameTick.deltaseconds;
                }
                if (InputManager.currentKeyState.isKeyDown("BACKWARD"))
                {
                    bufferposition.Y += speed * (float)GameTick.deltaseconds;
                }
                if (InputManager.currentKeyState.isKeyDown("LEFT"))
                {
                    bufferposition.X -= speed * (float)GameTick.deltaseconds;
                }
                if (InputManager.currentKeyState.isKeyDown("RIGHT"))
                {
                    bufferposition.X += speed * (float)GameTick.deltaseconds;
                }

                this.position = GameUtils.moveVector(this.position, bufferposition, true, new Vector2(this.sprite.width * 0.9f, this.sprite.height * 0.9f));



                this.sprite.rotation = (float)GameUtils.getRotation(this.sprite.position, new Vector2(InputManager.currentKeyState.mouseState.X, InputManager.currentKeyState.mouseState.Y)) + 90;
                Graphics.Graphics.cameraPosition = -this.position;
                if (this.delay <= 0)
                {
                    if (InputManager.currentKeyState.mouseState.LeftButton)
                    {
                        InventoryItem equippeditem = null;
                        inventory.getEquippedWeapon(out equippeditem);
                        if (equippeditem != null)
                        {
                            bool automatic = (equippeditem.getVariable("automatic").ToString() == Boolean.TrueString.ToLower());
                            if (automatic || !automatic && !InputManager.previousKeyState.mouseState.LeftButton)
                            {
                                if (equippeditem.getVariable("weapontype").ToString() == "melee")
                                {
                                    this.attackState.attackType = AttackState.AttackType.MELEE;
                                    string sprite = equippeditem.getVariable("ingamesprite").ToString();
                                    int attackspeed = Convert.ToInt32(equippeditem.getVariable("attackspeed"));
                                    this.attackState.weapon = new Sprite(sprite, this.position);
                                    this.attackState.attackspeed = (float)attackspeed;
                                    this.attackState.attack();
                                }
                                if (equippeditem.getVariable("weapontype").ToString() == "ranged")
                                {
                                    this.attackState.attackType = AttackState.AttackType.RANGED;
                                    string sprite = equippeditem.getVariable("ingamesprite").ToString();
                                    int attackspeed = Convert.ToInt32(equippeditem.getVariable("attackspeed"));
                                    this.attackState.weapon = new Sprite(sprite, this.position);
                                    this.attackState.attackspeed = (float)attackspeed;
                                    this.attackState.attack();
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.delay -= (float)GameTick.delta;
                }
            }
            this.attackState.parentPlayer = this;
            this.attackState.tick();
        }

        public void draw()
        {

            if (control)
            {
                sprite.draw();
                this.attackState.draw();
            }
            else
            {
                sprite.drawwocamera();
                this.attackState.draw();
            }
        }

        public void drawHud()
        {
            if (control)
            {
                float percentage = this.health / this.maxhealth;
                Graphics.Graphics.drawQuad(this.healthbar.position + new Vector2(-128, -16),
                    this.healthbar.position + new Vector2(-128 + (256 * percentage), -16),
                    this.healthbar.position + new Vector2(-128 + (256 * percentage), 16),
                    this.healthbar.position + new Vector2(-128, 16), Color4.Red);
                this.healthbar.draw();
            }
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

        private AttackStage _attackStage = AttackStage.IDLE;

        private int attackDelay = 1000;
        private float currentAttackDelay = 0;

        public AttackStage attackStage
        {
            get
            {
                return this._attackStage;
            }
            set
            {
                this._attackStage = value;
            }
        }
        public AttackType attackType = AttackType.MELEE;

        private readonly float maxhalfswing = 90f;

        private float swingrotation;
        public float attackspeed = 600f;

        public Sprite weapon = null;

        public Player parentPlayer;

        public AttackState(Player p)
        {
            this.parentPlayer = p;
            this.swingrotation = -maxhalfswing;
        }

        public void attack()
        {
            if (this.attackStage == AttackStage.IDLE)
            {
                InventoryItem weapon;
                parentPlayer.inventory.getEquippedWeapon(out weapon);
                this.attackDelay = Convert.ToInt32(weapon.getVariable("attackdelay"));
                this.currentAttackDelay = this.attackDelay;
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
            InventoryItem item;
            this.parentPlayer.inventory.getEquippedWeapon(out item);
            return item;
        }

        public void tick()
        {
            if (this.attackStage == AttackStage.SWING && this.weapon != null)
            {
                this.swingrotation += attackspeed * (float)GameTick.deltaseconds;

                Vector2 weaponposition = this.weapon.position + Vector2.Transform(new Vector2(0, -32), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), MathHelper.ToRadians(this.weapon.rotation)));

                if (this.parentPlayer.control)
                {
                    foreach (Game.RSL.Actors.Npc.Npc n in Game.RSL.Actors.Npc.NpcHandler.npcGameList)
                    {
                        Vector2 npcposition = n.sprite.position + Graphics.Graphics.cameraPosition + new Vector2(GameUtils.resolutionX / 2, GameUtils.resolutionY / 2);
                        if (GameUtils.getDistance(weaponposition, npcposition) < 32)
                        {
                            object originalhealth = n.getVariable("health");
                            int nexthealth = Convert.ToInt32(originalhealth) - Convert.ToInt32(getWeapon().getVariable("attackdamage"));
                            n.setVariable("health", (object)nexthealth);
                            FontManager.removeText(n.name + "NpcName");
                            this.attackStage = AttackStage.DELAY;
                            SoundManager.playSound("hit.ogg");
                        }
                    }
                }


                if (this.swingrotation >= maxhalfswing)
                {
                    this.attackStage = AttackStage.DELAY;
                }
            }
            if (this.attackStage == AttackStage.DELAY)
            {
                if (this.currentAttackDelay > 0)
                {
                    this.currentAttackDelay -= (float)GameTick.delta;
                }
                else
                {
                    this.attackStage = AttackStage.IDLE;
                }
            }
        }

        public void draw()
        {
            if (this.weapon != null)
            {
                if (this.attackStage == AttackStage.SWING)
                {
                    this.weapon.position = parentPlayer.sprite.position;
                    this.weapon.rotation = parentPlayer.sprite.rotation + this.swingrotation;
                    this.weapon.additionalrotation = new Vector2(-32 * (float)Math.Sin(MathHelper.ToRadians(-this.weapon.rotation)), -32 * (float)Math.Cos(MathHelper.ToRadians(-this.weapon.rotation)));
                    if (this.parentPlayer.control)
                    {
                        this.weapon.drawWithOrigin();
                    }
                    else
                    {
                        this.weapon.drawWithOriginWoCamera();
                    }
                }
                if (this.attackStage == AttackStage.FIRE)
                {
                    InventoryItem item;
                    parentPlayer.inventory.getEquippedWeapon(out item);
                    if (item != null)
                    {
                        opendagproject.Game.RSL.Functions.FunctionHandler.executeFunction(item.getVariable("firefunc").ToString());
                    }
                    this.attackStage = AttackStage.DELAY;
                }
            }
            else
            {
                InventoryItem equippeditem;
                this.parentPlayer.inventory.getEquippedWeapon(out equippeditem);
                if (equippeditem != null)
                {
                    string sprite = equippeditem.getVariable("ingamesprite").ToString();
                    int attackspeed = Convert.ToInt32(equippeditem.getVariable("attackspeed"));
                    this.weapon = new Sprite(sprite, this.parentPlayer.sprite.position);
                    this.attackspeed = (float)attackspeed;
                }
            }
        }
    }
}
