using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Graphics;
using opendagproject.Game.Graphics.Font;
using opendagproject.Game.Input;
using opendagproject.Game.Player.Inventory;
using opendagproject.Content;

namespace opendagproject.Game.World
{
    class Container : Tile
    {
        public Dictionary<InventoryItem, int[]> items = new Dictionary<InventoryItem, int[]>();
        private readonly int inventoryWidth = 6, inventoryHeight = 4;
        private readonly float dimX = 256, dimY = 129;
        public bool isOpened = false;


        public Container(Vector2 position, string texturename)
            : base(position, texturename, 1)
        {
            base.mouseControls = true;
            this.sprite = new Content.Sprite(texturename, this.position);
        }

        public void addItem(string itemname, int count)
        {
            InventoryItem item = InventoryHandler.getItemByName(itemname);
            for (int a = 0; a < items.Keys.ToList().Count; a++)
            {
                if (items.Keys.ToList()[a].getVariable("name").ToString() == itemname)
                {
                    items.Values.ToList()[a][2] += count;
                    return;
                }
            }
            items.Add(item, new int[] { items.Keys.ToList().Count % this.inventoryWidth, items.Keys.ToList().Count / this.inventoryWidth, count });
        }

        public override void tick(double delta)
        {
            FontManager.removeText("containerinfo");
            base.tick(delta);
            if (this.isOpened)
            {
                Vector2 inventorypos = new Vector2(-dimX / 2, -dimY * 1.5f) + new Vector2(GameUtils.resolutionX, GameUtils.resolutionY) - Graphics.Graphics.cameraPosition;
                for (int a = 0; a < items.Keys.ToList().Count; a++)
                {
                    Vector2 itempos = ((inventorypos - new Vector2(dimX / 2, dimY / 2)) + new Vector2(32 * items.Values.ToList()[a][0], 32 * items.Values.ToList()[a][1])) + new Vector2(49, 21);
                    float distance = (float)GameUtils.getDistance(Graphics.Graphics.screenPositionToGamePosition(InputManager.getCurrentMousePosition() + new Vector2(GameUtils.resolutionX / 2, GameUtils.resolutionY / 2)), itempos);
                    if (distance <= 16 && InputManager.clicked())
                    {
                        Player.PlayerHandler.playerList[0].delay = 100;
                        InventoryItem item = items.Keys.ToList()[a];
                        int count = items.Values.ToList()[a][2];
                        this.removeItem(item, count);
                        Player.PlayerHandler.playerList[0].addInventoryItem(item.name, count);
                    }
                }
                if (GameUtils.getDistance(this.position, Player.PlayerHandler.playerList[0].position) > 64)
                {
                    this.isOpened = false;
                }
            }
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
                if (items.Keys.ToList()[a].name == itemname)
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

        public override void onHover()
        {
            FontManager.addText(new Text("Container", "containerinfo", InputManager.getCurrentMousePosition() - new Vector2(0, 32), 30f, "franklin2.png", true, Text.positionOriginPoint.Middle, Color4.Green));
        }

        public override void onClick()
        {
            Player.PlayerHandler.playerList[0].delay = 100;
            this.isOpened = true;
        }

        public override string getSaveData()
        {
            string savedata = string.Empty;
            for (int a = 0; a < items.Keys.ToList().Count; a++)
            {
                savedata += items.Keys.ToList()[a].name + " " + items.Values.ToList()[a][2] + " ";
            }
            return base.getSaveData() + " contains " + savedata;
        }

        public void drawHud()
        {
            for (int a = 0; a < 24; a++)
            {
                FontManager.removeText("containercount" + a);
            }
            if (this.isOpened)
            {
                Vector2 inventorypos = new Vector2(-dimX / 2, -dimY * 1.5f) + new Vector2(GameUtils.resolutionX, GameUtils.resolutionY) - Graphics.Graphics.cameraPosition;
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
                    Vector2 textpos = new Vector2(GameUtils.resolutionX - dimX + 53 + (32 * items.Values.ToList()[a][0]), GameUtils.resolutionY - (dimY * 2) + 5 + (32 * items.Values.ToList()[a][1]));
                    FontManager.addText(new Text(items.Values.ToList()[a][2].ToString(), "containercount" + a, textpos, 30, "franklin2.png", true, Text.positionOriginPoint.Middle, Color4.Red));
                }
            }
        }
    }
}
