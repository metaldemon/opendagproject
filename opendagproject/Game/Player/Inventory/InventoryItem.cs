using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using opendagproject.Game.RSL.Actors;

namespace opendagproject.Game.Player.Inventory
{
    class InventoryItem : Actor
    {
        public int itemID;

      

        public InventoryItem()
        {
            
        }

        public void tick()
        {

        }

        public void setVariable(string name, object value)
        {
            base.setVariable(name, value);
        }

        public object getVariable(string name)
        {
            return base.getVariable(name);
        }

    }
}
