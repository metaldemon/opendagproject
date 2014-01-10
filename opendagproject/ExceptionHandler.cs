using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace opendagproject
{
    class ExceptionHandler
    {
        public enum ExceptionHandle {
            CLOSEONKEY,
            WAIT3SECONDS,
            CONTINUE
        }

        public static void printException(string message, ConsoleColor color, ExceptionHandle exh)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            if (message.StartsWith("RSL ERROR") && Game.States.GameStateManager.currentGlobalGameState == Game.States.GameStateManager.GlobalGameState.Mapeditor && Game.Mapeditor.Mapeditor.isEditingRSL())
            {
                opendagproject.Game.RSL.RSLHandler.rslErrors++;
                Game.Mapeditor.Mapeditor.getEditor().addDebugLine(message);
                return;
            }
            switch (exh)
            {
                case ExceptionHandle.CLOSEONKEY:
                    Console.ReadKey();
                    Environment.Exit(0);
                    break;
                case ExceptionHandle.WAIT3SECONDS:
                    Thread.Sleep(500);
                    break;
                case ExceptionHandle.CONTINUE:
                    break;
                default:
                    break;
            }
        }
    }
}
