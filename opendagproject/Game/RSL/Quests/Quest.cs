using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using opendagproject.Content;
using opendagproject.Game.Graphics;
using opendagproject.Game.Graphics.Font;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;

namespace opendagproject.Game.RSL.Quests
{
    public class Quest
    {
        public List<string> questLines = new List<string>();
        public int questFlag = 0;
        public bool active = false;
        public string name;
        public string gameName = "Unknown";
        public string description = "No description";
        public string questMessage = "I don't know what I am doing!";
        private int _currentQuestFlag = 0;

        public int currentQuestFlag
        {
            get { return _currentQuestFlag; }
            set { _currentQuestFlag = value; }
        }

        private int currentBracketCount = 0;
        private double questDelay = 0;
        public bool isCompleted = false;

        private Dictionary<string, int> pointers = new Dictionary<string, int>();

        enum QuestBlock
        {
            Quest,
            QuestInfo,
            QuestFlag
        }


        public Quest()
        {
            questLines = new List<string>();
        }

        public Quest(string name)
        {
            questLines = new List<string>();
            this.name = name;
        }

        public void addRSLLine(string line)
        {
            this.questLines.Add(line);
        }

        public void compile()
        {
            for (int a = 0; a < questLines.Count; a++)
            {
                string[] split = questLines[a].Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0)
                {
                    if (split[0] == "QuestInfo")
                    {
                        this.pointers.Add("QuestInfo", a);
                    }
                    if (split[0] == "QuestFlag")
                    {
                        if (split.Length > 1)
                        {
                            this.pointers.Add("QuestFlag " + int.Parse(split[1]), a);
                        }
                    }
                }
            }
            int questinfostartline = getPointer("QuestInfo");
            if (questinfostartline >= 0)
            {
                executeBlock(questinfostartline);
            }

        }

        private int getPointer(string name)
        {
            foreach (KeyValuePair<string, int> p in pointers)
            {
                if (p.Key == name)
                {
                    return p.Value;
                }
            }
            return -1;
        }

        private void executeBlock(int startline)
        {
            this.currentBracketCount = 0;
            bool foundblock = false;
            for (int currentline = startline; currentline < questLines.Count; currentline++)
            {
                string[] split = questLines[currentline].Split(new string[] { " ", "\t", "(", ")", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0)
                {

                    switch (split[0])
                    {
                        case "{":
                            this.currentBracketCount++;
                            foundblock = true;
                            break;
                        case "}":
                            this.currentBracketCount--;
                            if (this.currentBracketCount == 0 && foundblock)
                            {
                                return;
                            }
                            break;
                        case "Player.addItem":
                            if (split.Length > 1)
                            {
                                Player.PlayerHandler.playerList[0].addInventoryItem(getString(split, 1, 2), int.Parse(split[2]));
                            }
                            break;
                        case "Quest.displayQuestMessage":
                            if (split.Length > 1)
                            {
                                FontManager.addText(new Text(this.questMessage, "QuestMessage", new Vector2(GameUtils.resolutionX / 2, 40), 30f, "franklin2.png", true, Text.positionOriginPoint.Middle, Pencil.Gaming.Graphics.Color4.White, (float)double.Parse(split[1])));
                            }
                            break;
                        case "Quest.setName":
                            if (split.Length > 1)
                            {
                                this.gameName = getString(split, 1, split.Length);
                            }
                            break;
                        case "Quest.setDescription":
                            if (split.Length > 1)
                            {
                                this.description = getString(split, 1, split.Length);
                            }
                            break;
                        case "Quest.setQuestMessage":
                            if (split.Length > 1)
                            {
                                this.questMessage = getString(split, 1, split.Length);
                            }
                            break;
                        case "Quest.setDelay":
                            if (split.Length > 1)
                            {
                                this.questDelay = double.Parse(split[1]);
                            }
                            break;
                        case "Quest.setQuestFlag":
                            if (split.Length > 1)
                            {
                                this.currentQuestFlag = int.Parse(split[1]);
                            }
                            break;
                        case "Quest.setCompleted":
                            if (split.Length > 1)
                            {
                                this.isCompleted = bool.Parse(split[1]);
                                this.active = this.isCompleted;
                            }
                            break;
                        case "if":
                            if (this.checkIfCondition(split))
                            {

                            }
                            else
                            {
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private bool checkIfCondition(string[] split)
        {
            string value1 = string.Empty;
            string value2 = string.Empty;
            string op = string.Empty;
            int index = 1;
            value1 = getNextValue(split, index, out index);
            op = split[index];
            value2 = getNextValue(split, index++, out index);

            if (op == "==")
            {
                return (value1 == value2);
            }
            if (op == "<=")
            {
                return (double.Parse(value1) <= double.Parse(value2));
            }
            if (op == ">=")
            {
                return (double.Parse(value1) >= double.Parse(value2));
            }
            if (op == ">")
            {
                return (double.Parse(value1) > double.Parse(value2));
            }
            if (op == "<")
            {
                return (double.Parse(value1) < double.Parse(value2));
            }

            return false;
        }

        private string getNextValue(string[] split, int startindex, out int nextindex)
        {
            string value = string.Empty;
            nextindex = startindex;
            for (int a = startindex; a < split.Length; a++)
            {
                double nr;
                bool isnumber = double.TryParse(split[a], out nr);
                if (isnumber)
                {
                    value = split[a];
                    nextindex = a + 1;
                    break;
                }
                if (split[a] == "Player.checkInventory")
                {
                    value = Player.PlayerHandler.playerList[0].inventory.getItemCount(getString(split, a + 1, a + 2)).ToString();
                    nextindex = a + 2;
                    break;
                }
                if (split[a] == "Quest.getQuestDelay")
                {
                    value = this.questDelay.ToString();
                    nextindex = a + 1;
                    break;
                }
            }

            return value;
        }

        private string getString(string[] split, int startindex, int endindex)
        {
            string bufstring = string.Empty;
            for (int a = startindex; a < endindex; a++)
            {
                bufstring += split[a].Replace("\"", string.Empty).Replace(",", string.Empty);
                if (a != endindex - 1)
                {
                    bufstring += " ";
                }
            }
            return bufstring;
        }

        private void checkBlocks()
        {

        }

        public void tick()
        {
            if (this.active && !this.isCompleted)
            {
                if (this.questDelay > 0)
                {
                    this.questDelay -= Game.Tick.GameTick.delta;
                }
                for (int a = 0; a < pointers.Keys.ToList().Count; a++)
                {
                    if (pointers.Keys.ToList()[a] == ("QuestFlag " + this.currentQuestFlag))
                    {
                        executeBlock(pointers.Values.ToList()[a]);
                    }
                }
            }
        }
    }
}
