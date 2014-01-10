using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming.Graphics;

namespace opendagproject.Game.Graphics.Lighting
{
    class LightHandler
    {
        public static List<Light> lightList = new List<Light>();
        public static float ambient = 0.5f;

        public static bool day = false;

        public static void addLight(Light l)
        {
            lightList.Add(l);
        }

        public static void draw()
        {
            /*if (Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Game)
            {
                if (!day)
                {
                    ambient -= 0.001f;
                    if (ambient <= 0)
                    {
                        day = !day;
                    }
                }
                else
                {
                    ambient += 0.001f;
                    if (ambient >= 1f)
                    {
                        day = !day;
                    }
                }
            }
            else { ambient = 0.5f; }*/

            World.WorldManager.tileList.ForEach(x =>
            {
                x.color = new Color4(0, 0, 0, ambient);
                x.lightingpasses = new List<float>();
            });
            try
            {
                lightList.ForEach(x => x.draw());
            }
            catch (Exception e) { } // due to multithreaded loading
        }
    }
}
