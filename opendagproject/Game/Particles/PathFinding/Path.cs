using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using opendagproject.Game.World;
using System.Threading;

namespace opendagproject.Game.PathFinding
{
    public class Path
    {
        public enum PathState
        {
            IDLE,
            BUSY,
            COMPLETED,
            COMPLETEDRUN
        }

        public PathState currentPathState = PathState.IDLE;

        private Thread thread;

        private List<Vector2> nodes = new List<Vector2>();
        private int currentnode = 0;

        private List<Node> openlist = new List<Node>();
        private List<Node> closedlist = new List<Node>();

        private int starttile = 0;
        private int endtile = 0;
        private int currenttile = 0;

        private readonly int maxMarginLength = 32;

        private Vector2 startPosition;
        public Vector2 endPosition;

        public Path(Vector2 startpos, Vector2 endpos)
        {
            findPath(startpos, endpos);

        }

        public void findPath(Vector2 startpos, Vector2 endpos)
        {
            this.currentPathState = PathState.BUSY;
            this.startPosition = startpos;
            this.endPosition = endpos;
            this.currentnode = 0;
            this.nodes = new List<Vector2>();
            if (this.thread != null)
                this.thread.Abort();
            this.thread = new Thread(findPath);
            this.thread.Start();
        }

        private void findPath()
        {
            try
            {
                openlist = new List<Node>();
                closedlist = new List<Node>();

                float pathlength = 0;

                starttile = getTile(this.startPosition);
                endtile = getTile(this.endPosition);
                if (starttile == -1 || endtile == -1)
                {
                    return;
                }

                currenttile = starttile;
                openlist.Add(new Node(currenttile, 0, 0, endtile));

                while (currenttile != endtile)
                {
                    openlist.AddRange(getSurrounding(currenttile, pathlength));
                    float lowestFscore = float.MaxValue;
                    int index = -1;
                    for (int a = 0; a < openlist.Count; a++)
                    {
                        if (openlist[a].fScore < lowestFscore)
                        {
                            lowestFscore = openlist[a].fScore;
                            index = a;
                        }
                    }
                    currenttile = openlist[index].tileNR;

                    closedlist.Add(openlist.First(x => x.tileNR == currenttile));
                    openlist.Remove(openlist.First(x => x.tileNR == currenttile));
                }
                int currentnode = endtile;
                while (currentnode != starttile)
                {
                    foreach (Node n in closedlist)
                    {
                        if (n.tileNR == currentnode)
                        {
                            this.nodes.Add(WorldManager.tileList[n.tileNR].position);
                            currentnode = n.parentNode;
                            break;
                        }
                    }
                }
                this.nodes.Add(WorldManager.tileList[starttile].position);
                currentPathState = PathState.COMPLETED;
                this.currentnode = this.nodes.Count - 1;
            }
            catch (Exception e)
            {

            }
        }

        public Vector2 getNextTarget()
        {
            if (currentnode < nodes.Count)
                return nodes[currentnode];
            else
                return new Vector2(float.MaxValue,float.MaxValue);
        }

        public void checkPosition(Vector2 position)
        {
            if (GameUtils.getDistance(position, getNextTarget()) < 16)
            {
                if (getNextTarget() == WorldManager.tileList[getTile(endPosition)].position) this.currentPathState = PathState.COMPLETEDRUN;
                if (currentnode > 0)
                    currentnode--;
            }
        }



        private void showPath()
        {
            foreach (Tile t in WorldManager.tileList) t.color = Color4.White;
            int currentnode = endtile;
            while (currentnode != starttile)
            {
                foreach (Node n in closedlist)
                {
                    if (n.tileNR == currentnode)
                    {
                        WorldManager.tileList[n.tileNR].color = Color4.Green;
                        currentnode = n.parentNode;
                        break;
                    }
                }
            }
            WorldManager.tileList[starttile].color = Color4.Red;
            WorldManager.tileList[endtile].color = Color4.Blue;
        }

        private List<Node> getSurrounding(int tile, float pathlength)
        {
            List<Node> surrounding = new List<Node>();
            WorldManager.tileList[tile].surroundingtiles.
                Where(x => !listContainsNode(x, closedlist) && !listContainsNode(x, openlist) && !positionClips(WorldManager.tileList[x].position)).ToList().
                ForEach(y => surrounding.Add(new Node(y, pathlength + (float)GameUtils.getDistance(WorldManager.tileList[tile].position, WorldManager.tileList[y].position), currenttile,endtile)));

            WorldManager.tileList[tile].surroundingtiles.
                Where(x => listContainsNode(x, openlist) && !positionClips(WorldManager.tileList[x].position)).ToList().
                Where(y => getNodeByTileNr(y,openlist).pathLength > pathlength + (float)GameUtils.getDistance(WorldManager.tileList[tile].position, WorldManager.tileList[y].position)).ToList().
                ForEach(z => getNodeByTileNr(z,openlist).parentNode = currenttile);
            return surrounding;
        }



        private Node getNodeByTileNr(int nr, List<Node> nodelist)
        {
            return nodelist.First(x => x.tileNR == nr);
        }

        private bool positionClips(Vector2 pos)
        {
            for (int a = 0; a < WorldManager.tileList.Count; a++)
            {
                if (WorldManager.tileList[a].layer == 1 && WorldManager.tileList[a].position == pos)
                {
                    return true;
                }
            }
            return false;
        }

        private bool listContainsNode(int node, List<Node> nodelist)
        {
            try
            {
                foreach (Node n in nodelist)
                {
                    if (n.tileNR == node)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private int getTile(Vector2 pos)
        {
            int bestpos = -1;
            float closest = float.MaxValue;
            for (int a = 0; a < WorldManager.tileList.Count; a++)
            {
                if (GameUtils.getDistance(WorldManager.tileList[a].position, pos) <= closest)
                {
                    closest = (float)GameUtils.getDistance(WorldManager.tileList[a].position, pos);
                    bestpos = a;
                    
                }
            }
            return bestpos;
        }
    }

    public class Node
    {
        public int tileNR = -1;
        public float pathLength = 0;
        public float distToTarget = 0f;
        public float fScore = 0;
        public int parentNode = -1;

        public Node(int nr, float pathlength, int parentNode, int endtile)
        {
            this.tileNR = nr;
            this.pathLength = pathlength;
            this.distToTarget = (float)GameUtils.getDistance(WorldManager.tileList[this.tileNR].position, WorldManager.tileList[endtile].position);
            this.parentNode = parentNode;
            this.fScore = this.pathLength + distToTarget;
        }
    }
}
