using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Graphics;
using opendagproject.Game.Saver;
using opendagproject.Game.World;
using opendagproject.Game.RSL.Actors.Npc;


namespace opendagproject.Game.Tick
{
    class GameTick
    {
        public static double totalGameTime = 0;

        public static double saveTime = 5; // in minutes

        public static double delta = 0;
        public static double deltaseconds = 0;

        public static void tick()
        {
            delta = (Glfw.GetTime() - totalGameTime) * 1000.0;
            deltaseconds = delta / 1000.0;
            if (Math.Floor(totalGameTime) != Math.Floor(Glfw.GetTime()))
                secondUpdate();
            firstUpdate();
            totalGameTime = Glfw.GetTime();
        }

        static void secondUpdate()
        {
            if (Game.States.GameStateManager.currentGlobalGameState != States.GameStateManager.GlobalGameState.Mapeditor)
            {
                if (Math.Round(totalGameTime, 0) % (saveTime * 60) == 0 && totalGameTime > 5)
                {
                    GameSaver gs = new GameSaver("autosave");
                    gs.addSaveRegion("MAP");
                    WorldManager.tileList.ForEach(x => gs.addSaveLine(x.getSaveData()));
                    gs.addSaveRegion("NPC");
                    NpcHandler.npcGameList.ForEach(x => gs.addSaveLine(x.getSaveData()));
                    gs.save();
                    Debug.WriteLine("Game saved!", ConsoleColor.Yellow);
                }                
            }
        }

        static void firstUpdate()
        {
            
        }
    }
}
