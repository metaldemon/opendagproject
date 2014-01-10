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

namespace opendagproject
{
    class Program
    {
        public static GameState currentGameState;

        [STAThread]
        static void Main(string[] args)
        {
            Loader.loadGame();
            while (Glfw.GetWindowParam(WindowParam.Opened) == 1)
            {
                if (InputManager.currentKeyState.keyState[Key.Escape]) break;

                // TICK
                GameTick.tick();
                InputManager.tick();
                currentGameState.tick();
                InputManager.lateTick();

                // DRAW
                Graphics.clear();
                currentGameState.draw(0);
                currentGameState.draw(1);
                FontManager.draw();
                Graphics.update();
            }
            // CLEANUP
            SoundManager.dispose();
            Environment.Exit(0); // not much of a cleanup is it :)
        }
    }
}
