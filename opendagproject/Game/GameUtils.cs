using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using opendagproject.Game.Tick;
using opendagproject.Game.World;

namespace opendagproject.Game
{
    class GameUtils
    {
        public static int resolutionX, resolutionY;

        public static string windowTitle = "Open dag project - graafschap 2014 - Ynze Hettema 4S1D";

        public static string getGamePath()
        {
            return Convert.ToString(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).CodeBase)).Split(new string[] { "file:\\" }, StringSplitOptions.None)[1];
        }

        public static Vector2 moveVector(Vector2 org, float speedpersecond, float angle)
        {
            return org += Vector2.Transform(new Vector2(0, speedpersecond * (float)GameTick.deltaseconds), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), (float)MathHelper.ToRadians(angle)));
        }

        /// <summary>
        /// Moves a vector without delta
        /// </summary>
        /// <param name="org"></param>
        /// <param name="speed"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 moveVectorND(Vector2 org, float speed, float angle)
        {
            return org += Vector2.Transform(new Vector2(0, speed), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), (float)MathHelper.ToRadians(angle)));
        }
        public static Vector2 moveVectorND(Vector2 org, float speed, float angle, bool collision)
        {
            Vector2 buf = org + Vector2.Transform(new Vector2(0, speed), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), (float)MathHelper.ToRadians(angle)));
            if (getCollision(buf, new Vector2(1, 1)))
            {
                return org;
            }
            return buf;
        }

        public static Vector2 moveVector(Vector2 org, float speedpersecond, Vector2 tar)
        {
            return org += Vector2.Transform(new Vector2(0, speedpersecond * (float)GameTick.deltaseconds), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), (float)MathHelper.ToRadians(getRotation(org, tar) - 90)));
        }

        public static Vector2 moveVector(Vector2 org, float speedpersecond, Vector2 tar, bool collision, Vector2 dim)
        {
            if (getDistance(org, tar) > speedpersecond * (float)GameTick.deltaseconds)
            {

                Vector2 bufferposition = org + Vector2.Transform(new Vector2(0, speedpersecond * (float)GameTick.deltaseconds), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), (float)MathHelper.ToRadians(getRotation(org, tar) - 90)));

                bool collisionX = false, collisionY = false;

                if (getCollision(new Vector2(bufferposition.X, org.Y), new Vector2((float)dim.X * 0.9f, (float)dim.Y * 0.9f))) collisionX = true;
                if (getCollision(new Vector2(org.X, bufferposition.Y), new Vector2((float)dim.X * 0.9f, (float)dim.Y * 0.9f))) collisionY = true;

                if (!collisionX && !collisionY)
                {
                    org = bufferposition;
                }
                else if (collisionY && !collisionX)
                {
                    org = new Vector2(bufferposition.X, org.Y);
                }
                else if (!collisionY && collisionX)
                {
                    org = new Vector2(org.X, bufferposition.Y);
                }
            }
            return org;
        }

        public static Vector2 moveVector(Vector2 org, float speedpersecond, float angle, out bool collision, Vector2 dim)
        {
            collision = false;

            Vector2 bufferposition = org + Vector2.Transform(new Vector2(0, speedpersecond * (float)GameTick.deltaseconds), Quaternion.FromAxisAngle(new Vector3(0, 0, 1), (float)MathHelper.ToRadians(angle)));

            bool collisionX = false, collisionY = false;

            if (getCollision(new Vector2(bufferposition.X, org.Y), new Vector2((float)dim.X * 0.9f, (float)dim.Y * 0.9f))) collisionX = true;
            if (getCollision(new Vector2(org.X, bufferposition.Y), new Vector2((float)dim.X * 0.9f, (float)dim.Y * 0.9f))) collisionY = true;

            if (collisionX || collisionY)
            {
                collision = true;
            }

            return bufferposition;
        }


        public static Vector2 moveVector(Vector2 org, Vector2 tar, bool collision, Vector2 dim)
        {
            Vector2 bufferposition = tar;
            bool collisionX = false, collisionY = false;

            if (getCollision(new Vector2(bufferposition.X, org.Y), new Vector2((float)dim.X * 0.9f, (float)dim.Y * 0.9f))) collisionX = true;
            if (getCollision(new Vector2(org.X, bufferposition.Y), new Vector2((float)dim.X * 0.9f, (float)dim.Y * 0.9f))) collisionY = true;

            if (!collisionX && !collisionY)
            {
                org = bufferposition;
            }
            else if (collisionY && !collisionX)
            {
                org = new Vector2(bufferposition.X, org.Y);
            }
            else if (!collisionY && collisionX)
            {
                org = new Vector2(org.X, bufferposition.Y);
            }

            return org;
        }

        public static bool getCollision(Vector2 pos, Vector2 dim)
        {

            foreach (Tile t in WorldManager.tileList)
            {
                if (t.layer == 1)
                {
                    Vector2 tilepos = t.position;
                    Vector2 playerpos = pos;

                    if (playerpos.X + (dim.X / 2) >= tilepos.X - 16 && playerpos.X - (dim.X / 2) < tilepos.X + 16 &&
                        playerpos.Y + (dim.X / 2) >= tilepos.Y - 16 && playerpos.Y - (dim.X / 2) < tilepos.Y + 16)
                    {
                        return true;
                    }
                }
            }
            foreach (Container t in WorldManager.containerList)
            {
                if (t.layer == 1)
                {
                    Vector2 tilepos = t.position;
                    Vector2 playerpos = pos;

                    if (playerpos.X + (dim.X / 2) >= tilepos.X - 16 && playerpos.X - (dim.X / 2) < tilepos.X + 16 &&
                        playerpos.Y + (dim.X / 2) >= tilepos.Y - 16 && playerpos.Y - (dim.X / 2) < tilepos.Y + 16)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int getTile(Vector2 pos)
        {
            int bestpos = -1;
            float closest = float.MaxValue;
            for (int a = 0; a < WorldManager.tileList.Count; a++)
            {
                if (WorldManager.tileList[a].layer == 0)
                    if (GameUtils.getDistance(WorldManager.tileList[a].position, pos) <= closest)
                    {
                        closest = (float)GameUtils.getDistance(WorldManager.tileList[a].position, pos);
                        bestpos = a;

                    }
            }
            return bestpos;
        }

        public static double getDistance(Vector2 startpnt, Vector2 endpnt)
        {
            return (endpnt - startpnt).Length;
        }

        public static double getRotation(Vector2 startpnt, Vector2 endpnt)
        {
            Vector2 temp = endpnt - startpnt;
            float opposite = temp.Y;
            float adjacent = temp.X;
            double temp2 = MathHelper.ToDegrees((float)Math.Atan2(opposite, adjacent));
            return temp2;
        }
    }
}
