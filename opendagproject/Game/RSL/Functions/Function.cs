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


namespace opendagproject.Game.RSL.Functions
{
    class Function
    {
        public string name;

        public List<string> rtrsl = new List<string>();

        private int currentFunctionLine = 0;

        public enum FunctionState
        {
            FUNCTION,
            IFBLOCK,
            ELSEBLOCK,
            WHILEBLOCK,
            FORBLOCK
        }

        private FunctionState currentFunctionState = FunctionState.FUNCTION;

        public void parseRSL(string[] split)
        {
            if (split.Length > 0)
            {
                if (split[0] == "Sound.Play")
                {
                    Content.SoundManager.playSound(split[1].Replace("\"",string.Empty));
                }
                if (split[0] == "Particle.Spawn")
                {
                    string particlename = split[1].Replace("\"", string.Empty);
                    Particles.Particle p = new Particles.Particle();
                    foreach (var x in Particles.ParticleHandler.particleList.First(x => x.name == particlename).getVariableList())
                    {
                        p.addVariable(new Variable(x.name, x.getValue(), x.baseValue));
                    }
                    p.init();
                    string from = split[2];
                    string to = split[3];
                    if (from == "SELF")
                    {
                        p.position = PlayerHandler.playerList[0].position;
                    }
                    if (to == "TARGET")
                    {
                        p.rotation = (float)GameUtils.getRotation(p.position, Graphics.Graphics.screenPositionToGamePosition(new Vector2(InputManager.currentKeyState.mouseState.X, InputManager.currentKeyState.mouseState.Y))) - 90;
                    }
                    Particles.ParticleHandler.particleGameList.Add(p);
                }
                if (split[0].StartsWith("Player"))
                {
                    if (split[0] == "Player.RemoveItem")
                    {
                        string itemname = split[1].Replace("\"",string.Empty);
                        int count = int.Parse(split[2]);
                        Player.PlayerHandler.playerList[0].inventory.removeItem(InventoryHandler.getItemByName(itemname), count);
                    }
                    if (split[0] == "Player.AddItem")
                    {
                        string itemname = split[1].Replace("\"", string.Empty);
                        int count = int.Parse(split[2]);
                        Player.PlayerHandler.playerList[0].inventory.addItem(InventoryHandler.getItemByName(itemname), count);
                    }
                    if (split[0] == "Player.delay")
                    {
                        string op = split[1];
                        int val = int.Parse(split[2]);
                        if (op == "=")
                        {
                            PlayerHandler.playerList.First(x => x.control).delay = val;
                        }
                        else if (op == "+=")
                        {
                            PlayerHandler.playerList.First(x => x.control).delay += val;
                        }
                        else if (op == "-=")
                        {
                            PlayerHandler.playerList.First(x => x.control).delay -= val;
                        }
                        else if (op == "*=")
                        {
                            PlayerHandler.playerList.First(x => x.control).delay *= val;
                        }
                        else if (op == "/=")
                        {
                            PlayerHandler.playerList.First(x => x.control).delay /= val;
                        }
                    }

                    if (split[0] == "Player.health")
                    {
                        string op = split[1];
                        int val = int.Parse(split[2]);
                        if (op == "=")
                        {
                            PlayerHandler.playerList.First(x => x.control).health = val;
                        }
                        else if (op == "+=")
                        {
                            PlayerHandler.playerList.First(x => x.control).health += val;
                        }
                        else if (op == "-=")
                        {
                            PlayerHandler.playerList.First(x => x.control).health -= val;
                        }
                        else if (op == "*=")
                        {
                            PlayerHandler.playerList.First(x => x.control).health *= val;
                        }
                        else if (op == "/=")
                        {
                            PlayerHandler.playerList.First(x => x.control).health /= val;
                        }
                    }
                    if (split[0] == "Player.speed")
                    {
                        string op = split[1];
                        int val = int.Parse(split[2]);
                        if (op == "=")
                        {
                            PlayerHandler.playerList.First(x => x.control).speed = val;
                        }
                        else if (op == "+=")
                        {
                            PlayerHandler.playerList.First(x => x.control).speed += val;
                        }
                        else if (op == "-=")
                        {
                            PlayerHandler.playerList.First(x => x.control).speed -= val;
                        }
                        else if (op == "*=")
                        {
                            PlayerHandler.playerList.First(x => x.control).speed *= val;
                        }
                        else if (op == "/=")
                        {
                            PlayerHandler.playerList.First(x => x.control).speed /= val;
                        }
                    }
                }
                if (split[0] == "Npc")
                {
                    string name = split[1];
                    Npc npc = NpcHandler.npcGameList.First(x => x.name == name);

                    string var = split[2];
                    string op = split[3];
                    string val = split[4];
                    if (op == "=")
                    {
                        npc.setVariable(var, (object)val);
                    }
                    else if (op == "+=")
                    {
                        npc.setVariable(var, (int)npc.getVariable(var) + int.Parse(val));
                    }
                    else if (op == "-=")
                    {
                        npc.setVariable(var, (int)npc.getVariable(var) - int.Parse(val));
                    }
                    else if (op == "*=")
                    {
                        npc.setVariable(var, (int)npc.getVariable(var) * int.Parse(val));
                    }
                    else if (op == "/=")
                    {
                        npc.setVariable(var, (int)npc.getVariable(var) / int.Parse(val));
                    }
                }
            }
        }

        public void parseIfLine(string[] split)
        {
            if (split[1] == "Player.CheckInventory")
            {
                int count = PlayerHandler.playerList[0].inventory.getItemCount(split[2].Replace("\"", string.Empty));
                string op = split[3];
                int value = int.Parse(split[4]);
                switch (op)
                {
                    case "==":
                        if (count == value)
                        {
                            lookfornextclosebracket = false;
                            this.currentFunctionState = FunctionState.IFBLOCK;
                        }
                        else
                        {
                            this.lookfornextclosebracket = true;
                            this.lookforelseblock = true;
                        }
                        break;
                    case ">=":
                        if (count >= value)
                        {
                            lookfornextclosebracket = false;
                            this.currentFunctionState = FunctionState.IFBLOCK;
                        }
                        else
                        {
                            this.lookfornextclosebracket = true;
                            this.lookforelseblock = true;
                        }
                        break;
                    case "<=":
                        if (count <= value)
                        {
                            lookfornextclosebracket = false;
                            this.currentFunctionState = FunctionState.IFBLOCK;
                        }
                        else
                        {
                            this.lookfornextclosebracket = true;
                            this.lookforelseblock = true;
                        }
                        break;
                    case ">":
                        if (count > value)
                        {
                            lookfornextclosebracket = false;
                            this.currentFunctionState = FunctionState.IFBLOCK;
                        }
                        else
                        {
                            this.lookfornextclosebracket = true;
                            this.lookforelseblock = true;
                        }
                        break;
                    case "<":
                        if (count < value)
                        {
                            lookfornextclosebracket = false;
                            this.currentFunctionState = FunctionState.IFBLOCK;
                        }
                        else
                        {
                            this.lookfornextclosebracket = true;
                            this.lookforelseblock = true;
                        }
                        break;
                    case "!=":
                        if (count != value)
                        {
                            lookfornextclosebracket = false;
                            this.currentFunctionState = FunctionState.IFBLOCK;
                        }
                        else
                        {
                            this.lookfornextclosebracket = true;
                            this.lookforelseblock = true;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private bool lookfornextopenbracket = false;
        private bool lookfornextclosebracket = false;
        private bool lookforelseblock = false;

        public void execute()
        {
            for(this.currentFunctionLine = 0; this.currentFunctionLine < rtrsl.Count; this.currentFunctionLine++)
            {
                string[] split = rtrsl[this.currentFunctionLine].Split(new string[] { " ", "(", ")", ";", "\t" , ","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (lookfornextclosebracket)
                {
                    if (split[0] != "}")
                    {
                        continue;
                    }
                    else
                    {
                        if (this.currentFunctionState != FunctionState.FUNCTION)
                        {
                            this.currentFunctionState = FunctionState.FUNCTION;
                        }
                        lookfornextclosebracket = false;
                        continue;
                    }
                }
                if (lookforelseblock)
                {
                    if (this.currentFunctionState == FunctionState.FUNCTION && split[0] == "else")
                    {
                        this.currentFunctionState = FunctionState.ELSEBLOCK;
                        this.currentFunctionLine++;
                        lookforelseblock = false;
                        continue;
                    }
                    
                }
                if (lookfornextopenbracket)
                {
                    if (split[0] != "{")
                    {
                        continue;
                    }
                    else
                    {
                        lookfornextopenbracket = false;
                        continue;
                    }
                }
                if (split[0] == "}" && currentFunctionState == FunctionState.IFBLOCK)
                {
                    lookfornextclosebracket = true;
                    continue;
                }
                switch (split[0])
                {
                    case "if":
                        parseIfLine(split);
                        break;
                    default:
                        parseRSL(split);
                        break;
                }
            }
        }
    }
}
