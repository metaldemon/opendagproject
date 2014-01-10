using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;


namespace opendagproject
{
    class Debug
    {
        static readonly int linedelay = 0;
        static public int chardelay = 0;

        static readonly bool log = false;


        public static void WriteLine(string line, ConsoleColor color)
        {
            if (Game.States.GameStateManager.currentGlobalGameState == Game.States.GameStateManager.GlobalGameState.Mapeditor && opendagproject.Game.Mapeditor.Mapeditor.isEditingRSL())
            {
                opendagproject.Game.Mapeditor.Mapeditor.getEditor().addDebugLine(line);
            }
            Write(line, color);
            Write("\n", color);
            
            Thread.Sleep(linedelay);
            if (log)
            {
                StreamWriter sw = new StreamWriter(@"D:\LOG.txt", true);
                sw.WriteLine(line);
                sw.Close();
            }
        }

        public static void Write(string text, ConsoleColor color, int x, int y)
        {
            Console.ForegroundColor = color;
            int bufx = Console.CursorLeft, bufy = Console.CursorTop;
            Console.SetCursorPosition(x, y);
            if (text.Length > 1)
            {
                for (int a = 0; a < text.Length; a++)
                {
                    Write(text[a].ToString(), color);
                    Thread.Sleep(chardelay);
                }
            }
            else
            {
                Console.Write(text);
                Thread.Sleep(chardelay);
            }
        }

        public static void Write(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            if (text.Length > 1)
            {
                for (int a = 0; a < text.Length; a++)
                {
                    Write(text[a].ToString(), color);
                    
                    Thread.Sleep(chardelay);
                }
            }
            else
            {
                Console.Write(text);
                Thread.Sleep(chardelay);
            }
        }

        public static void Seperate()
        {
            for (int a = 0; a < 80; a++)
            {
                Write("*", ConsoleColor.White);
                Thread.Sleep(chardelay / 4);
            }
            Write("\n", ConsoleColor.White);
        }
    }
}
