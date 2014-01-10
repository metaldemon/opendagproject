using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.RSL.Actors.Npc
{
    public class NpcHandler
    {
        public static List<Npc> npcList = new List<Npc>();
        public static List<Npc> npcGameList = new List<Npc>();


        public static void initialize()
        {
            npcList.ForEach(x => x.initialize());
            npcGameList.ForEach(x => x.initialize());
        }

        public static void tick()
        {
            try
            {
                npcGameList.ForEach(x =>
                {
                    x.tick();
                    if (Convert.ToInt32(x.getVariable("health")) <= 0)
                    {
                        x.onRemove();
                    }
                });
                npcGameList = npcGameList.Where(x => Convert.ToInt32(x.getVariable("health")) > 0).ToList();
            }
            catch (Exception e) { } // due to multithreaded loading
        }

        public static void draw()
        {
            try
            {
                npcGameList.ForEach(x => x.draw());
            }
            catch (Exception e) { } // due to multithreaded loading
        }
    }
}
