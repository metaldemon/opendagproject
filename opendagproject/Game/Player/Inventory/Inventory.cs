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
using opendagproject.Game.Graphics.Font;
using opendagproject.Game.Input;
using opendagproject.Game.RSL.Actors;
using opendagproject.Game.RSL.Functions;

namespace opendagproject.Game.Player.Inventory
{
    class Inventory
    {
        public Dictionary<InventoryItem, int[]> items = new Dictionary<InventoryItem, int[]>();

        private readonly int inventoryWidth = 6, inventoryHeight = 4;
        private readonly float dimX = 256, dimY = 129;

        private Player parentPlayer;

        public Inventory(Player parent)
        {
            this.parentPlayer = parent;
        }

        public void addItem(InventoryItem item)
        {
            addItem(item, 1);
        }
        public void addItem(InventoryItem item, int count)
        {
            if (item != null)
            {
                for (int a = 0; a < items.Keys.ToList().Count; a++)
                {
                    if (items.Keys.ToList()[a].name == item.name)
                    {
                        items.Values.ToList()[a][2] += count;
                        return;
                    }
                }
                items.Add(item, new int[] { items.Keys.ToList().Count % this.inventoryWidth, items.Keys.ToList().Count / this.inventoryWidth, count });
            }
            else
            {
                ExceptionHandler.printException("Could not add item: \"" + item.name + "\" to players inventory.", ConsoleColor.Red, ExceptionHandler.ExceptionHandle.WAIT3SECONDS);
            }
        }
        public void addItem(string itemname)
        {
            if (InventoryHandler.getItemByName(itemname) != null)
            {
                addItem(InventoryHandler.getItemByName(itemname), 1);
            }
        }
        public void addItem(string itemname, int count)
        {
            if (InventoryHandler.getItemByName(itemname) != null)
            {
                addItem(InventoryHandler.getItemByName(itemname), count);
            }
        }


        public int getItemCount(string name)
        {
            for (int a = 0; a < items.Keys.Count; a++)
            {
                if (items.Keys.ToList()[a].name == name)
                {
                    return items.Values.ToList()[a][2];
                }
            }
            return 0;
        }

        public void removeItem(InventoryItem item, int count)
        {
            for (int a = 0; a < items.Keys.ToList().Count; a++)
            {
                if (items.Keys.ToList()[a] == item)
                {
                    items.Values.ToList()[a][2] -= count;
                    if (items.Values.ToList()[a][2] <= 0)
                    {
                        items.Remove(items.Keys.ToList()[a]);
                        return;
                    }
                }

            }
        }
        public void removeItem(string itemname, int count)
        {
            for (int a = 0; a < items.Keys.ToList().Count; a++)
            {
                if (items.Keys.ToList()[a].name == itemname )
                {
                    items.Values.ToList()[a][2] -= count;
                    if (items.Values.ToList()[a][2] <= 0)
                    {
                        items.Remove(items.Keys.ToList()[a]);
                        return;
                    }
                }

            }
        }

        public void tick()
        {
            foreach (InventoryItem I in items.Keys.ToList())
            {
                I.tick();
            }
            if (this.parentPlayer.control)
            {

                if (InputManager.currentKeyState.mouseState.LeftButton && !InputManager.previousKeyState.mouseState.LeftButton)
                {
                    Vector2 mouseposition = new Vector2(InputManager.currentKeyState.mouseState.X, InputManager.currentKeyState.mouseState.Y) -
                            Graphics.Graphics.cameraPosition;
                    Vector2 inventorypos = new Vector2(-dimX / 2, -dimY / 2) + new Vector2(GameUtils.resolutionX, GameUtils.resolutionY) - Graphics.Graphics.cameraPosition;
                    for (int a = 0; a < items.Keys.ToList().Count; a++)
                    {
                        Vector2 itempos = ((inventorypos - new Vector2(dimX / 2, dimY / 2)) + new Vector2(32 * items.Values.ToList()[a][0], 32 * items.Values.ToList()[a][1])) + new Vector2(49, 21);
                        if (GameUtils.getDistance(mouseposition, itempos) <= 16)
                        {
                            if (!World.WorldManager.isContainerOpen())
                            {
                                if (items.Keys.ToList()[a].getVariable("activefunc") != null)
                                {
                                    FunctionHandler.executeFunction((string)items.Keys.ToList()[a].getVariable("activefunc"));
                                }
                                if ((string)items.Keys.ToList()[a].getVariable("equipable") == Boolean.TrueString.ToLower())
                                {
                                    equipItem(items.Keys.ToList()[a].name, Boolean.TrueString.ToLower());
                                    FontManager.addText(new Text(items.Keys.ToList()[a].getVariable("name").ToString() + " equipped", "equippedinfo", new Vector2(0, 0), 30f, "franklin2.png", true, Text.positionOriginPoint.UpperLeft, Color4.White, 3));
                                    parentPlayer.delay = 100;
                                }
                                else
                                {
                                    equipItem(items.Keys.ToList()[a].name, Boolean.FalseString.ToLower());
                                    FontManager.addText(new Text(items.Keys.ToList()[a].getVariable("name").ToString() + " cannot be equipped", "equippedinfo", new Vector2(0, 0), 30f, "franklin2.png", true, Text.positionOriginPoint.UpperLeft, Color4.White, 3));
                                }
                            }
                            else
                            {
                                string name = items.Keys.ToList()[a].name;
                                int count = items.Values.ToList()[a][2];
                                this.removeItem(name, count);
                                World.WorldManager.getOpenedContainer().addItem(name, count);
                            }
                        }
                    }
                }
            }
        }

        public void equipItem(string name, string value)
        {
            for (int a = 0; a < items.Keys.ToList().Count; a++)
            {
                if (items.Keys.ToList()[a].name == name)
                {
                    items.Keys.ToList()[a].setVariable("isequipped", (object)value);
                }
                else
                {
                    if (value == Boolean.TrueString.ToLower())
                    {
                        items.Keys.ToList()[a].setVariable("isequipped", "false");
                    }
                }
            }
        }

        public void getEquippedWeapon(out InventoryItem item)
        {
            foreach (InventoryItem i in items.Keys.ToList())
            {
                if (i.getVariable("type").ToString() == "weaponry" || i.getVariable("type").ToString() == "ranged weaponry")
                {
                    if (i.getVariable("isequipped").ToString() == Boolean.TrueString.ToLower())
                    {
                        item = i;
                        return;
                    }
                }
            }
            item = null;
        }

        public void draw()
        {
            if (this.parentPlayer.control)
            {
                for (int a = 0; a < 24; a++)
                {
                    FontManager.removeText("inventorycount" + a);
                }
                Vector2 inventorypos = new Vector2(-dimX / 2, -dimY / 2) + new Vector2(GameUtils.resolutionX, GameUtils.resolutionY) - Graphics.Graphics.cameraPosition;
                Graphics.Graphics.drawSprite(inventorypos, new Vector2(dimX, dimY), InventoryHandler.inventorySprite);
                for (int a = 0; a < items.Keys.ToList().Count; a++)
                {
                    Vector2 itempos = ((inventorypos - new Vector2(dimX / 2, dimY / 2)) + new Vector2(32 * items.Values.ToList()[a][0], 32 * items.Values.ToList()[a][1])) + new Vector2(49, 21);
                    Graphics.Graphics.drawSprite(itempos
                        , new Vector2(22, 22),
                        new Sprite((string)items.Keys.ToList()[a].getVariable("inventorysprite"),
                            new Vector2(0, 0)));
                    object equipped = items.Keys.ToList()[a].getVariable("isequipped");
                    if (equipped != null && equipped.ToString() == Boolean.TrueString.ToLower())
                    {
                        Graphics.Graphics.drawLineQuad(itempos, new Vector2(32, 32), Color4.Red, 3f);
                    }
                    Vector2 textpos = new Vector2(GameUtils.resolutionX - dimX + 53 + (32 * items.Values.ToList()[a][0]), GameUtils.resolutionY - dimY + 5 + (32 * items.Values.ToList()[a][1]));
                    FontManager.addText(new Text(items.Values.ToList()[a][2].ToString(), "inventorycount" + a, textpos, 30, "franklin2.png", true, Text.positionOriginPoint.Middle, Color4.Red));
                }
            }
        }
    }
}
