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

namespace opendagproject.Game.Player.Inventory
{
    class InventoryHandler
    {
        public static List<InventoryItem> itemList = new List<InventoryItem>();

        public static Sprite inventorySprite;

        public static void initialize()
        {
            inventorySprite = new Sprite("inventory2.png",new Vector2(0,0));
        }





        public static InventoryItem getItemByName(string name)
        {
            foreach (InventoryItem I in itemList)
            {
                if (I.name == name)
                {
                    return I;
                }
            }
            return null;
        }
    }
}
