using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Graphics;
using opendagproject.Game;
using opendagproject.Game.Player;
using opendagproject.Game.World;
using opendagproject.Game.RSL;
using opendagproject.Game.RSL.Actors;
using opendagproject.Game.RSL.Actors.Npc;
using opendagproject.Game.Graphics.Font;
using System.Threading;

namespace opendagproject.Game.Saver
{
    class GameSaver
    {
        private List<string> saveLines = new List<string>();
        private string saveFileName = string.Empty;

        public GameSaver(string savename)
        {
            this.saveFileName = savename;
        }

        public void addSaveRegion(string name)
        {
            addSaveLine(Environment.NewLine);
            addSaveLine("REGION " + name);
            addSaveLine(Environment.NewLine);
        }

        public void addSaveLine(string line)
        {
            this.saveLines.Add(line);
        }
        public void addSaveLine(List<string> lines)
        {
            this.saveLines.AddRange(lines);
        }

        private Thread saveThread;

        private void _save()
        {
            while (true)
            {
                try
                {

                    FontManager.addText(new Text("Saving game...", "GAMESAVE", new Vector2(5, 5), 25f, "franklin2.png", true, Text.positionOriginPoint.UpperLeft, Color4.White));
                    StreamWriter sw = new StreamWriter(GameUtils.getGamePath() + "\\data\\saves\\" + saveFileName + ".sav");
                    foreach (string s in saveLines)
                    {
                        sw.WriteLine(s);
                    }
                    sw.Close();
                    FontManager.addText(new Text("Game saved!", "GAMESAVE", new Vector2(5, 5), 25f, "franklin2.png", true, Text.positionOriginPoint.UpperLeft, Color4.White, 3f));
                    break;
                }
                catch (Exception e) { }
            }
        }

        public void save()
        {
            saveThread = new Thread(_save);
            saveThread.Start();
        }
    }
}
