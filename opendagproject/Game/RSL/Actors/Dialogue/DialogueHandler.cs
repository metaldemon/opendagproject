using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.RSL.Actors.Dialogue
{
    public class DialogueHandler
    {
        public static List<Dialogue> dialogueList = new List<Dialogue>();

        public static readonly float textSize = 32.5f;

        public static void setDialogue(string dialogue, Npc.Npc parentnpc)
        {
            dialogueList.ForEach(x => x.setActive(false, parentnpc));
            dialogueList.First(x => x.name == dialogue).setActive(true, parentnpc);
        }

        public static void tick()
        {
            for (int a = 0; a < dialogueList.Count; a++)
            {
                if (dialogueList[a].getActive())
                {
                    dialogueList[a].tick();
                }
            }

        }

        public static void draw()
        {

        }
    }
}
