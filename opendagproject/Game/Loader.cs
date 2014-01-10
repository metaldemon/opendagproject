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
using System.IO;
using System.Threading;


namespace opendagproject.Game
{
    class Loader
    {

        // THE DLL IMPORTS ARE TEMPORARY! YOU HEAR ME? TEMPORARY!!!!!!
        [DllImport("user32.dll")] // <- windows only
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)] // <- windows only
        static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        public static bool WindowOpen, ContentLoaded, Connected;

        private static int resX = 800, resY = 600;
        private static bool fullscreen = false;
        private static bool preferencesset = false;


        public static void loadGame()
        {
            string originalTitle = Console.Title;
            string uniqueTitle = Guid.NewGuid().ToString();
            Console.Title = uniqueTitle;
            IntPtr handle = FindWindowByCaption(IntPtr.Zero, uniqueTitle);

            initPreferences();
            initWindow();
            SetForegroundWindow(handle); // YES! TEMPORARY!!!!!

            loadContent();
            initScripts();
            Thread t = new Thread(loadMap);
            t.Start();
            //loadMap();
            initGameState();
            

            handle = FindWindowByCaption(IntPtr.Zero, GameUtils.windowTitle);
            SetForegroundWindow(handle);
            GameState.setState();
            //currentGameState = new GameplayState();
            Glfw.SwapInterval(false); // frame cap
            //loadSavedGame("autosave");

        }

        public static void loadSavedGame(string savename)
        {
            WorldManager.tileList = new List<Tile>();
            string currentregion = string.Empty;
            string currentactorname = string.Empty;
            StreamReader sw = new StreamReader(GameUtils.getGamePath() + "\\data\\saves\\" + savename + ".sav");
            string line = string.Empty;
            while ((line = sw.ReadLine()) != null)
            {
                string[] split = line.Split(new string[] { "(", ",", ")", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0)
                {
                    if (split[0] == "REGION")
                    {
                        if (split.Length > 1)
                        {
                            currentregion = split[1];
                            continue;
                        }
                    }
                    if (currentregion == "MAP")
                    {
                        WorldManager.tileList.Add(new Tile(new Vector2(int.Parse(split[0]), int.Parse(split[1])), split[3], int.Parse(split[2])));
                    }
                    if (currentregion == "NPC")
                    {
                        if (split[0] == "NPC")
                        {
                            if (split.Length > 1)
                            {
                                currentactorname = split[1];
                            }
                        }
                        if (split[0] == "SETVAL")
                        {
                            if (split.Length > 2)
                            {
                                NpcHandler.npcGameList.First(x => x.name == currentactorname).setVariable(split[1], (object)split[2]);
                            }
                        }
                    }
                }
            }
            WorldManager.initTiles();
        }

        public static void initPreferences()
        {
            Debug.Seperate();
            Debug.WriteLine("-- PREFERENCES --", ConsoleColor.Yellow);
            Debug.Seperate();
            Debug.WriteLine("-- loading preferences now", ConsoleColor.Green);

            try
            {
                StreamReader sr = new StreamReader(Game.GameUtils.getGamePath() + "//data//pref//preferences.ini");
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] split = line.Split(new string[] { " ", "[", "]", "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length > 0)
                        switch (split[0])
                        {
                            case "resolution":
                                resX = int.Parse(split[1]);
                                resY = int.Parse(split[2]);
                                break;
                            case "fullscreen":
                                fullscreen = bool.Parse(split[1]);
                                break;
                            default:
                                break;
                        }
                }
                sr.Close();
            }
            catch (Exception e)
            {
                ExceptionHandler.printException(e.Message, ConsoleColor.Red, ExceptionHandler.ExceptionHandle.WAIT3SECONDS);
            }

            InputManager.loadKeyConfig(Game.GameUtils.getGamePath() + "//data//pref//keyconfig.txt");

            Debug.WriteLine("-- done loading preferences", ConsoleColor.Green);
            Debug.Seperate();
            preferencesset = true;
        }

        /// <summary>
        /// sets up game window to the prefrences' settings accordingly
        /// </summary>
        public static void initWindow()
        {
            initWindow(resX, resY, fullscreen);
        }

        public static void initWindow(int width, int height, bool fullscreen)
        {
            if (!WindowOpen)
            {

                Debug.WriteLine("-- INIT --", ConsoleColor.Yellow);
                Debug.Seperate();
                try
                {
                    Glfw.Init();
                    Debug.WriteLine("-- GLFW 1.1", ConsoleColor.White);
                    WindowMode wm;
                    if (fullscreen) wm = WindowMode.FullScreen; else wm = WindowMode.Window;
                    Glfw.OpenWindow(width, height, 8, 8, 8, 8, 0, 0, wm);
                    Debug.WriteLine("-- Window setup (" + width + ", " + height + ")", ConsoleColor.White);
                    Glfw.SetWindowTitle(GameUtils.windowTitle);
                    Debug.WriteLine("-- Window title", ConsoleColor.White);
                }
                catch (Exception e)
                {
                    ExceptionHandler.printException("Could not initialize GLFW window", ConsoleColor.Red, ExceptionHandler.ExceptionHandle.CLOSEONKEY);
                }

                GL.Viewport(0, 0, width, height);
                Debug.WriteLine("-- Viewport", ConsoleColor.White);
                GL.MatrixMode(MatrixMode.Projection);
                Debug.WriteLine("-- Matrix mode", ConsoleColor.White);
                GL.LoadIdentity();

                GL.Ortho(0, width, height, 0, 10, -10);
                Debug.WriteLine("-- Ortho", ConsoleColor.White);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                Debug.WriteLine("-- Ident", ConsoleColor.White);
                Debug.Seperate();
                GL.ClearColor(0f, 0f, 0f, 0f);
                GameUtils.resolutionX = width;
                GameUtils.resolutionY = height;
            }
        }

        static void loadTextures()
        {
            ContentManager.loadAllTextures("//data//textures//");
            ContentManager.loadAllTextures("//data//textures//tiles//");
            ContentManager.loadAllTextures("//data//textures//fonts//");
            ContentManager.loadAllTextures("//data//textures//inventory//");
            ContentManager.loadAllTextures("//data//textures//weaponsprites//sword");
            ContentManager.loadAllTextures("//data//textures//hud//");
        }

        static void loadSounds()
        {
            SoundManager.loadAllSounds("//data//audio//dialogue//jack");
            SoundManager.loadAllSounds("//data//audio//general");
        }

        public static void loadContent()
        {
            Debug.WriteLine("-- LOAD --", ConsoleColor.Yellow);
            Debug.Seperate();
            Debug.WriteLine("-- Loading textures", ConsoleColor.Green);
            loadTextures();
            Debug.Seperate();
            Debug.WriteLine("-- Loading audio", ConsoleColor.Green);
            Thread t = new Thread(loadSounds);
            t.Start();
            //loadSounds();
            Debug.Seperate();

            Input.InputManager.initialize();
            InventoryHandler.initialize();
        }

        public static void loadMap()
        {
            FontManager.removeAllTexts();
            Debug.WriteLine("-- Loading map", ConsoleColor.Green);
            WorldManager.tileList = new List<Tile>();
            WorldManager.containerList = new List<Container>();
            WorldManager.loadedMap = false;
            WorldManager.nearCameraTiles = new List<int>();
            LightHandler.lightList = new List<Light>();
            NpcHandler.npcGameList = new List<Npc>();
            WorldManager.loadMap();
            Debug.WriteLine("-- Map total tiles: " + WorldManager.tileList.Count, ConsoleColor.Green);
            Debug.Seperate();
        }

        public static void initScripts()
        {
            Debug.WriteLine("-- Loading scripts", ConsoleColor.Green);
            RSL.RSLHandler.loadScripts();
            NpcHandler.initialize();
            Debug.Seperate();
        }

        public static void initGameState()
        {
            Debug.WriteLine("-- GAMESTATE --", ConsoleColor.Yellow);
            Debug.Seperate();

            while (true)
            {
                Debug.WriteLine("Are you a player or a mapeditor? (p/m)", ConsoleColor.White);
                switch ('m')
                {
                    case 'p':
                        Game.States.GameStateManager.currentGlobalGameState = GameStateManager.GlobalGameState.Game;
                        Glfw.SetWindowTitle(GameUtils.windowTitle + " - SINGLEPLAYER");
                        GameUtils.windowTitle = GameUtils.windowTitle + " - SINGLEPLAYER";
                        break;
                    case 'm':
                        Game.States.GameStateManager.currentGlobalGameState = GameStateManager.GlobalGameState.Mapeditor;
                        Mapeditor.Mapeditor.intialize();
                        Glfw.SetWindowTitle(GameUtils.windowTitle + " - EDITOR");
                        GameUtils.windowTitle = GameUtils.windowTitle + " - EDITOR";
                        break;
                    default:
                        Debug.WriteLine("Wrong input, try again", ConsoleColor.Red);
                        break;
                }
                if (Game.States.GameStateManager.currentGlobalGameState != States.GameStateManager.GlobalGameState.None) { break; }
                
            }
        }

        public static List<string> loadFile(string file)
        {
            List<string> filelines = new List<string>();

            StreamReader sr = new StreamReader(file);
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                filelines.Add(line);
            }
            sr.Close();
            return filelines;
        }
    }
}
