using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.States
{
    class GameState
    {
        public static void setState()
        {
            if (Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Mapeditor)
            {
                Program.currentGameState = new MapeditorState();
            }
            else
            {
                Program.currentGameState = new GameplayState();
            }
        }

        public GameState()
        {

        }


        public virtual void tick()
        {

        }

        public virtual void draw(byte layer)
        {

        }
    }
}
