using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System.IO;

namespace opendagproject.Game.Input
{
    class InputManager
    {
        public static KeyState currentKeyState = new KeyState();
        public static KeyState previousKeyState = new KeyState();

        public static KeyConfig currentKeyConfig = null;

        public static void initialize()
        {
            currentKeyState.keyState = KeyboardState.GetState();
            currentKeyState.mouseState = MouseState.GetMouseState();
            previousKeyState.keyState = KeyboardState.GetState();
            previousKeyState.mouseState = MouseState.GetMouseState();
        }

        public static void loadKeyConfig(string file)
        {
            currentKeyConfig = new KeyConfig();
            StreamReader sr = new StreamReader(file);
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                string[] split = line.Split(new string[] { " ", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                switch (split[0])
                {
                    case "Key":
                        int key = 0;
                        bool result = safeParse(split[2], out key);
                        if (result)
                        {
                            currentKeyConfig.addKeyBinding(split[1], key);
                        }
                        break;
                    default:
                        break;
                }
            }
            sr.Close();
        }

        private static bool safeParse(string str, out int nr)
        {
            nr = 0;
            try
            {
                nr = int.Parse(str);
                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.printException("Could not parse int in keyconfig", ConsoleColor.Red, ExceptionHandler.ExceptionHandle.CLOSEONKEY);
            }
            return false;
        }

        public static void tick()
        {
            if (Game.States.GameStateManager.currentGlobalGameState != States.GameStateManager.GlobalGameState.Mapeditor || Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Mapeditor && !Mapeditor.Mapeditor.isEditingRSL())
            {
                currentKeyState.keyState = KeyboardState.GetState();
                currentKeyState.mouseState = MouseState.GetMouseState();
            }
        }

        public static Vector2 getCurrentMousePosition()
        {
            return new Vector2(currentKeyState.mouseState.X, currentKeyState.mouseState.Y);
        }

        public static bool clicked()
        {
            return (currentKeyState.mouseState.LeftButton && !previousKeyState.mouseState.LeftButton);
        }

        public static void lateTick()
        {
            previousKeyState = currentKeyState.copy();
        }
        public static char getkey()
        {
            for (int a = 33; a < 187; a++)
            {
                try
                {
                    if (currentKeyState.keyState[(char)a])
                    {
                        return (char)a;
                    }
                }
                catch (Exception e)
                {

                }
            }
            return (char)301;
        }
    }

    class KeyState
    {
        public MouseState mouseState;
        public KeyboardState keyState;

        public bool isKeyDown(string name)
        {
            bool result = false;
            int key = InputManager.currentKeyConfig.getKey(name, out result);
            if (result)
            {
                return keyState[(char)key];
            }
            else return false;
        }

        public KeyState copy()
        {
            return (KeyState)this.MemberwiseClone();
        }
    }
}
