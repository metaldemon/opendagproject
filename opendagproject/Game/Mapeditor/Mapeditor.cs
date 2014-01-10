using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.Audio;
using opendagproject.Content;
using opendagproject.Game.Graphics;
using opendagproject.Game.Input;
using opendagproject.Game.Player;
using opendagproject.Game.States;
using opendagproject.Game.Tick;
using opendagproject.Game.World;
using System.Windows.Forms;


namespace opendagproject.Game.Mapeditor
{
    public class Mapeditor
    {

        static Sprite editSprite;
        public static byte editlayer = 0;

        static List<string> texturelist = new List<string>();
        static int textureIndex = 0;

        public static AddNpc addNpcForm;
        public static RSL.RSL_IDE RSLIDE;
        public static AddLight addLightForm;
        public static AddContainer addContainerForm;
        private static UI userInterface;
        public static bool selectPosition = false;

        public static void intialize()
        {
            userInterface = new UI();
            userInterface.Show();
            foreach (Texture t in ContentManager.textureList)
            {
                if (t.file.Contains("tiles"))
                    texturelist.Add(t.name);
            }
            editSprite = new Sprite(texturelist[textureIndex], new Vector2(0, 0));
        }

        public static RSL.RSL_IDE getEditor()
        {
            return RSLIDE;
        }

        public static void tick()
        {
            if (Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Mapeditor)
            {
                userInterface.tick();
                Vector2 mouseposition = new Vector2(InputManager.currentKeyState.mouseState.X, InputManager.currentKeyState.mouseState.Y);
                float scrollspeed = 5f;
                if (mouseposition.X < 25)
                {
                    Graphics.Graphics.cameraPosition.X += scrollspeed;
                }
                if (mouseposition.X > GameUtils.resolutionX - 25)
                {
                    Graphics.Graphics.cameraPosition.X -= scrollspeed;
                }
                if (mouseposition.Y < 25)
                {
                    Graphics.Graphics.cameraPosition.Y += scrollspeed;
                }
                if (mouseposition.Y > GameUtils.resolutionY - 25)
                {
                    Graphics.Graphics.cameraPosition.Y -= scrollspeed;
                }

                if (InputManager.currentKeyState.mouseState.LeftButton)
                {
                    if (!selectPosition)
                    {
                        bool containssametile = false;
                        foreach (Tile t in WorldManager.tileList)
                        {
                            if (t.position == editSprite.position && t.layer == editlayer)
                            {
                                containssametile = true; break;
                            }
                        }
                        if (!containssametile)
                        {
                            Tile t = new Tile(editSprite.position, editSprite.getTextureName(), editlayer);
                            t.onwalkoverFunction = userInterface.walkoverScript;
                            t.onfireFunction = userInterface.onfireScript;
                            WorldManager.tileList.Add(t);
                        }
                    }
                    else
                    {
                        userInterface.selectionPosition = editSprite.position;
                    }
                }
                if (InputManager.currentKeyState.mouseState.RightButton)
                {
                    foreach (Game.Graphics.Lighting.Light l in Game.Graphics.Lighting.LightHandler.lightList)
                    {
                        if (GameUtils.getDistance(l.position, editSprite.position) <= 16)
                        {
                            DialogResult dr = MessageBox.Show("There is a light placed on this tile. " + Environment.NewLine, "Remove this light?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                Game.Graphics.Lighting.LightHandler.lightList = Game.Graphics.Lighting.LightHandler.lightList.Where(x => GameUtils.getDistance(x.position, editSprite.position) > 16).ToList();
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    WorldManager.tileList = WorldManager.tileList.Where(x => x.position != editSprite.position).ToList();
                }

                if (InputManager.currentKeyState.mouseState.ScrollWheel < InputManager.previousKeyState.mouseState.ScrollWheel)
                {
                    if (textureIndex < texturelist.Count - 1)
                        textureIndex++;
                }
                else if (InputManager.currentKeyState.mouseState.ScrollWheel > InputManager.previousKeyState.mouseState.ScrollWheel)
                {
                    if (textureIndex > 0)
                        textureIndex--;
                }

                if (InputManager.currentKeyState.keyState['1'])
                {
                    editlayer = 0;
                }
                if (InputManager.currentKeyState.keyState['2'])
                {
                    editlayer = 1;
                }
                if (InputManager.currentKeyState.keyState['3'])
                {
                    editlayer = 2;
                }

                

                editSprite = new Sprite(texturelist[textureIndex], new Vector2(0, 0));
                editSprite.position = Graphics.Graphics.screenPositionToGamePosition(mouseposition, true);

                if (InputManager.currentKeyState.keyState['N'] && !InputManager.previousKeyState.keyState['N'])
                {
                    addNpcForm = new AddNpc(editSprite.position);
                    addNpcForm.Show();
                }
                if (InputManager.currentKeyState.keyState['L'] && !InputManager.previousKeyState.keyState['L'])
                {
                    addLightForm = new AddLight(editSprite.position);
                    addLightForm.Show();
                }
                if (InputManager.currentKeyState.keyState['M'] && !InputManager.previousKeyState.keyState['M'])
                {
                    RSLIDE = new RSL.RSL_IDE();
                    RSLIDE.Show();
                }
                if (InputManager.currentKeyState.keyState['C'] && !InputManager.previousKeyState.keyState['C'])
                {
                    addContainerForm = new AddContainer(editSprite.position);
                    addContainerForm.Show();
                }
                if (addContainerForm != null)
                {
                    addContainerForm.tick();
                    if (!addContainerForm.isActive)
                    {
                        addContainerForm = null;
                    }
                }
                if (RSLIDE != null)
                {
                    RSLIDE.tick();
                    if (!RSLIDE.isActive)
                    {
                        RSLIDE = null;
                    }
                }
                if (addLightForm != null)
                {
                    addLightForm.tick();
                    if (!addLightForm.isActive)
                    {
                        addLightForm = null;
                    }
                }
               

                if (InputManager.currentKeyState.keyState[Key.LeftControl] && InputManager.currentKeyState.keyState['S'])
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        saveMap(ofd.FileName);
                    }
                }

            }
        }

        public static void saveMap(string path)
        {
            Debug.WriteLine("Saving map...", ConsoleColor.Green);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
            {
                sw.WriteLine("//layer 0");
                WorldManager.tileList.Where(x => x.layer == 0).ToList().ForEach(x => sw.WriteLine(x.getSaveData())); sw.WriteLine("//layer 1");
                WorldManager.tileList.Where(x => x.layer == 1).ToList().ForEach(x => sw.WriteLine(x.getSaveData())); sw.WriteLine("//layer 2");
                WorldManager.tileList.Where(x => x.layer == 2).ToList().ForEach(x => sw.WriteLine(x.getSaveData())); sw.WriteLine("//containers");
                WorldManager.containerList.ForEach(x => sw.WriteLine(x.getSaveData()));
                sw.WriteLine("//Non-playable characters");
                opendagproject.Game.RSL.Actors.Npc.NpcHandler.npcGameList.ForEach(x => sw.WriteLine("NPC " + x.name + " " + x.sprite.position.ToString()));
                sw.WriteLine("//Lighting");
                opendagproject.Game.Graphics.Lighting.LightHandler.lightList.ForEach(x => sw.WriteLine(x.getSaveData()));
                sw.Close();
            }
            Debug.WriteLine("Saved map...", ConsoleColor.Green);
        }

        public static bool isEditingRSL()
        {
            return (RSLIDE != null);
        }

        public static bool isAddingLight()
        {
            return (addLightForm != null);
        }

        public static bool isAddingContainer()
        {
            return (addContainerForm != null);
        }

        public static void draw()
        {
            if (Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Mapeditor)
            {
                editSprite.drawwocamera();
                editSprite.drawFrame();
            }
        }
    }
}
