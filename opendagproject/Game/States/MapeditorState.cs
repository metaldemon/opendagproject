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
using opendagproject.Game.Saver;

namespace opendagproject.Game.States
{
    class MapeditorState : GameState
    {
        public MapeditorState()
        {

        }

        public override void tick()
        {
            if (Mapeditor.Mapeditor.isEditingRSL()) // nasty workaround. find better fix
            {
                while (Mapeditor.Mapeditor.isEditingRSL())
                {
                    Mapeditor.Mapeditor.tick();
                }
            }
            if (Mapeditor.Mapeditor.isAddingLight())
            {
                while (Mapeditor.Mapeditor.isAddingLight())
                {
                    Mapeditor.Mapeditor.tick();
                }
            }
            if (Mapeditor.Mapeditor.isAddingContainer())
            {
                while (Mapeditor.Mapeditor.isAddingContainer())
                {
                    Mapeditor.Mapeditor.tick();
                }
            }
            Mapeditor.Mapeditor.tick();
            WorldManager.tick();
        }

        public override void draw(byte layer)
        {
            WorldManager.draw(0);
            NpcHandler.draw();
            WorldManager.applyLighting(0);
            LightHandler.draw();
            WorldManager.draw(1);
            WorldManager.applyLighting(1);
            Mapeditor.Mapeditor.draw();
        }
    }
}
