using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using opendagproject.Game;
using opendagproject.Game.Graphics;
using opendagproject.Game.Tick;
using opendagproject.Content;
using opendagproject.Game.World;
using opendagproject.Game.States;
using opendagproject.Game.Input;
using System.Runtime.InteropServices;
using opendagproject.Game.Mapeditor;
using opendagproject.Game.RSL.Actors;
using opendagproject.Game.RSL.Actors.Npc;
using opendagproject.Game.Graphics.Font;
using opendagproject.Game.PathFinding;
using opendagproject.Game.RSL.Actors.Dialogue;
using opendagproject.Game.Player;
using opendagproject.Game.Player.Inventory;
using opendagproject.Game.Graphics.Lighting;
using opendagproject.Game.Saver;
using opendagproject.Game.RSL.Quests;


namespace opendagproject.Game.States
{
    class GameplayState : GameState
    {
        public GameplayState()
        {
            PlayerHandler.playerList = new List<Player.Player>();
            PlayerHandler.playerList.Add(new Player.Player(new Vector2(GameUtils.resolutionX / 2, GameUtils.resolutionY / 2), "player.png"));
            PlayerHandler.playerList[0].control = true;
        }

        public override void tick()
        {
            ActorHandler.tick();
            NpcHandler.tick();
            QuestHandler.tick();
            DialogueHandler.tick();
            WorldManager.tick();
            PlayerHandler.tick();
            Game.Particles.ParticleHandler.tick();
            if (Input.InputManager.currentKeyState.keyState[Key.F5] && !Input.InputManager.previousKeyState.keyState[Key.F5])
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

        public override void draw(byte layer)
        {
            if (layer == 0)
            {
                opendagproject.Game.Graphics.Lighting.LightHandler.lightList.ForEach(x => x.render());
                WorldManager.draw(0);
                PlayerHandler.draw();
                Game.Particles.ParticleHandler.draw();
                NpcHandler.draw();
                WorldManager.applyLighting(0);
                LightHandler.draw();
                WorldManager.draw(1);
                WorldManager.applyLighting(1);
                WorldManager.draw(2);
                WorldManager.draw(3);
            }
            else
            {
                PlayerHandler.drawHud();
                PlayerHandler.playerList.First(x => x.control).inventory.draw();
            }
        }
    }
}
