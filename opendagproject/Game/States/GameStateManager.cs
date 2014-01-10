using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.States
{
    class GameStateManager
    {
        public enum GlobalGameState
        {
            None,
            Game,
            Mapeditor
        }
        public static GlobalGameState currentGlobalGameState = GlobalGameState.None;
    }
}
