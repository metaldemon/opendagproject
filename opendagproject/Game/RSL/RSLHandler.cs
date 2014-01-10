using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using opendagproject.Game.RSL.Actors;
using opendagproject.Game.RSL.Actors.Npc;
using opendagproject.Game.RSL.Actors.Dialogue;

namespace opendagproject.Game.RSL
{
    class RSLHandler
    {
        static List<Script> scriptList = new List<Script>();
        public static int scriptsLoaded = 0;
        public static bool debugmessages = false;
        public static int rslErrors = 0;

        public static void loadScripts()
        {
            rslErrors = 0;
            scriptsLoaded = 0;
            ActorHandler.actorList = new List<Actor>();
            NpcHandler.npcList = new List<Npc>();
            DialogueHandler.dialogueList = new List<Dialogue>();
            Particles.ParticleHandler.particleList = new List<Particles.Particle>();
            Quests.QuestHandler.questList = new List<Quests.Quest>();
            scriptList = new List<Script>();

            DateTime t1 = DateTime.Now;
            List<string> basescripts = Directory.GetFiles(GameUtils.getGamePath() + "\\data\\scripts\\base\\").ToList();
            List<string> scripts = Directory.GetFiles(GameUtils.getGamePath() + "\\data\\scripts\\").ToList();
            if (Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Mapeditor && Mapeditor.Mapeditor.isEditingRSL())
            {
                Mapeditor.Mapeditor.getEditor().setProgressbarMax(basescripts.Count + scripts.Count);
            }
            basescripts.ForEach(x =>
            {
                scriptList.Add(new Script(x));
                if (Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Mapeditor && Mapeditor.Mapeditor.isEditingRSL())
                {
                    Mapeditor.Mapeditor.getEditor().addProgressbarValue();
                }
            });
            scripts.ForEach(x =>
            {
                scriptList.Add(new Script(x));
                if (Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Mapeditor && Mapeditor.Mapeditor.isEditingRSL())
                {
                    Mapeditor.Mapeditor.getEditor().addProgressbarValue();
                }
            });
            Particles.ParticleHandler.initialize();
            Quests.QuestHandler.compileQuests();
            DateTime t2 = DateTime.Now;
            TimeSpan ts = t2 - t1;
            scriptsLoaded = scriptList.Count;
            Debug.WriteLine("-- RSL load time: " + ts.TotalMilliseconds + " milliseconds", ConsoleColor.Green);
        }
    }
}
