using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using opendagproject.Game.RSL.Actors;

namespace opendagproject.Game.RSL.Actors
{
    public class ActorHandler
    {
        public static List<Actor> actorList = new List<Actor>();

        public static void tick()
        {
            actorList.ForEach(x => x.tick());
        }

        public static bool actorWithName(string name)
        {
            foreach (Actor a in actorList)
            {
                if (a.name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
