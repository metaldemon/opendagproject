using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.Player
{
    class PlayerHandler
    {
        public static List<Player> playerList = new List<Player>();

        public static void initPlayers()
        {

        }

        public static void tick()
        {
            playerList.ForEach(x => x.tick());
        }

        public static void draw()
        {
            playerList.ForEach(x => x.draw());
        }

        public static void drawHud()
        {
            try
            {
                playerList.ForEach(x => x.drawHud());
            }
            catch (Exception) { }
        }
    }
}
