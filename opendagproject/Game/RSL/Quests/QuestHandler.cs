using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.RSL.Quests
{
    public class QuestHandler
    {
        public static List<Quest> questList = new List<Quest>();

        public static Quest getQuest(string name)
        {
            return questList.First(x => x.name == name);
        }

        public static void compileQuests()
        {
            questList.ForEach(x => x.compile());
        }

        public static void tick()
        {
            questList.ForEach(x => x.tick());
        }
    }
}
