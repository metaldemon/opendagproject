using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Graphics;
using opendagproject.Game.Graphics;
using opendagproject.Game.Tick;
using opendagproject.Content;
using System.IO;
using System.Windows.Forms;
using opendagproject.Game.RSL.Actors.Npc;
using opendagproject.Game.Graphics.Lighting;

namespace opendagproject.Game.World
{
    class WorldManager
    {
        public static List<Tile> tileList = new List<Tile>();
        public static List<Container> containerList = new List<Container>();

        public static List<int> nearCameraTiles = new List<int>();

        public static bool loadedMap = false;
        public static int exclusiveLayer = -1;


        public static void loadMap()
        {
            /*OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {*/
            StreamReader sr = new StreamReader(GameUtils.getGamePath() + "\\data\\maps\\map01.txt"); // ofd.FileName
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {

                if (line.StartsWith("//")) continue;
                string[] split = line.Split(new string[] { "(", ",", ")", " ", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (!line.StartsWith("NPC") && !line.StartsWith("Light"))
                {
                    if (!line.Contains("contains"))
                    {
                        WorldManager.tileList.Add(new Tile(new Vector2(int.Parse(split[0]), int.Parse(split[1])), split[3], int.Parse(split[2])));
                    }
                    else
                    {
                        Container c = new Container(new Vector2(int.Parse(split[0]), int.Parse(split[1])), split[3]);

                        for (int a = 5; a < int.MaxValue; a++)
                        {
                            if (split.Length > a)
                            {
                                string item = split[a];
                                if (split.Length > a++)
                                {
                                    int count = int.Parse(split[a]);
                                    c.addItem(item, count);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        float val = LightHandler.ambient;
                        c.color = new Color4(val, val, val, val);
                        WorldManager.containerList.Add(c);
                    }
                }
                else if (line.StartsWith("NPC"))
                {
                    Npc bufnpc = NpcHandler.npcList.First(x => x.name == split[1]).clone();
                    bufnpc.initialize();
                    bufnpc.setVariable("positionX", int.Parse(split[2]));
                    bufnpc.setVariable("positionY", int.Parse(split[3]));
                    bufnpc.sprite.position = new Vector2(int.Parse(split[2]), int.Parse(split[3]));
                    NpcHandler.npcGameList.Add(bufnpc);
                    Debug.WriteLine("Added npc", ConsoleColor.Green);
                }
                else if (line.StartsWith("Light"))
                {
                    Vector2 position = new Vector2(int.Parse(split[1]), int.Parse(split[2]));
                    int radius = int.Parse(split[3]);
                    float r = float.Parse(split[4]), g = float.Parse(split[5]), b = float.Parse(split[6]), a = float.Parse(split[7]);
                    string rendertype = split[8];
                    string lighttype = split[9];
                    int startnode = int.Parse(split[10]);
                    int conewidth = int.Parse(split[11]);
                    int rotationspeed = int.Parse(split[12]);
                    Light.LightRenderType rt = (rendertype == "STATIC") ? Light.LightRenderType.STATIC : Light.LightRenderType.DYNAMIC;
                    Light.LightType lt = Light.LightType.REGULAR;
                    if (lighttype == "POINTLIGHT")
                    {
                        lt = Light.LightType.POINTLIGHT;
                    }
                    if (lighttype == "ROTATINGCONE")
                    {
                        lt = Light.LightType.ROTATINGCONE;
                    }
                    Light l = new Light(position, radius, rt, lt, new Color4(r, g, b, a));
                    l.startnode = startnode;
                    l.conewidth = conewidth;
                    l.rotationspeed = rotationspeed;
                    LightHandler.lightList.Add(l);
                    Debug.WriteLine("Added light " + LightHandler.lightList.Count, ConsoleColor.Green);
                }
            }
            sr.Close();
            //}
            initTiles();
            foreach (var x in LightHandler.lightList)
            {
                System.Threading.Thread t = new System.Threading.Thread(x.render);
                t.Start();
            }
            loadedMap = true;
        }

        public static void initTiles()
        {
            for (int a = 0; a < tileList.Count; a++)
            {
                if (tileList[a].layer == 0)
                {
                    for (int b = 0; b < tileList.Count; b++)
                    {
                        if (tileList[b].layer == 0)
                        {
                            if (a != b && GameUtils.getDistance(tileList[a].position, tileList[b].position) <= 32)
                            {
                                tileList[a].surroundingtiles.Add(b);
                            }
                        }
                    }
                }
            }
        }

        public static void tick()
        {
            nearCameraTiles = new List<int>();
            for (int a = 0; a < tileList.Count; a++)
            {
                tileList[a].tick(GameTick.delta);
                if (tileList[a].layer == 0)
                {
                    if (GameUtils.getDistance(tileList[a].position, -Graphics.Graphics.cameraPosition) < 300)
                    {
                        nearCameraTiles.Add(a);
                    }
                }
            }
            for (int a = 0; a < containerList.Count; a++)
            {
                containerList[a].tick(GameTick.delta);
            }

        }

        public static void draw(int layer)
        {
            if (exclusiveLayer >= 0 && layer != exclusiveLayer) { return; }
            foreach (var x in tileList.Where(x => x.layer == layer))
            {
                x.draw();
            }
            if (layer == 1)
            {
                foreach (var x in containerList)
                {
                    x.draw();
                }
            }
            if (layer == 3)
            {
                foreach (var x in containerList)
                {
                    x.drawHud();
                }
            }
        }

        public static void applyLighting(int layer)
        {
            if (exclusiveLayer >= 0 && layer != exclusiveLayer) { return; }
            foreach (var x in tileList.Where(x => x.layer == layer))
            {
                x.applyLighting();
            }
            if (layer == 1)
            {
                foreach (var x in containerList)
                {
                    x.applyLighting();
                }
            }
        }

        public static bool isContainerOpen()
        {
            foreach (var x in containerList)
            {
                if (x.isOpened)
                {
                    return true;
                }
            }
            return false;
        }

        public static Container getOpenedContainer()
        {
            return containerList.First(x => x.isOpened);
        }
    }
}

