using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Graphics;
using opendagproject.Game.Graphics.Font;
using opendagproject.Game.RSL.Actors.Npc;
using opendagproject.Content;

namespace opendagproject.Game.RSL.Actors.Dialogue
{
    public class Dialogue
    {
        public string name = string.Empty;
        public List<DialogueStage> stageList = new List<DialogueStage>();

        private int currentStage = 0;
        private bool active = false;

        private readonly Vector2 dialoguePosition = new Vector2(GameUtils.resolutionX / 2, (float)GameUtils.resolutionY * 0.75f);

        private Npc.Npc parentNpc = null;

        public void setActive(bool active, Npc.Npc parentnpc)
        {
            this.active = active;
            this.parentNpc = parentnpc;
        }

        public bool getActive()
        {
            return this.active;
        }

        public void setStage(int stage)
        {
            this.currentStage = stage;
        }

        public void tick()
        {
            FontManager.removeText("DialogueNpcSays");
            FontManager.removeText("DialogueNpcName");
            for (int a = 0; a < 5; a++)
            {
                FontManager.removeText("DialogueReply" + a.ToString());
            }
            if (!NpcHandler.npcGameList.Contains(this.parentNpc) || this.parentNpc.getVariable("hostile").ToString() == Boolean.TrueString.ToLower())
            {

                this.active = false;
                return;
            }
            FontManager.addText(new Text(stageList[this.currentStage].npcSays, "DialogueNpcSays", dialoguePosition, DialogueHandler.textSize, "franklin2.png", true, Text.positionOriginPoint.Middle, Color4.Green));
            FontManager.addText(new Text(name, "DialogueNpcName", dialoguePosition - new Vector2(0, 30), DialogueHandler.textSize, "franklin2.png", true, Text.positionOriginPoint.Middle, Color4.Green));

            for (int a = 0; a < stageList[this.currentStage].replyList.Count; a++)
            {
                Vector2 currentpos = dialoguePosition + new Vector2(0, (30f * (a + 1)));
                Vector2 mousepos = new Vector2(Input.InputManager.currentKeyState.mouseState.X, Input.InputManager.currentKeyState.mouseState.Y - 25);
                if (mousepos.Y > currentpos.Y - 15 && mousepos.Y < currentpos.Y + 15)
                {
                    FontManager.addText(new Text(stageList[this.currentStage].replyList.Keys.ToList()[a], "DialogueReply" + a.ToString(), currentpos, DialogueHandler.textSize, "franklin2.png", true, Text.positionOriginPoint.Middle, Color4.Red));
                    if (Input.InputManager.currentKeyState.mouseState.LeftButton && !Input.InputManager.previousKeyState.mouseState.LeftButton)
                    {
                        if (stageList[this.currentStage].replyList.Values.ToList()[a] >= 0)
                        {
                            this.currentStage = stageList[this.currentStage].replyList.Values.ToList()[a];
                            stageList[this.currentStage].executeRSL(this.parentNpc);
                        }
                        else
                        {
                            this.active = false;
                            FontManager.removeText("DialogueNpcSays");
                            FontManager.removeText("DialogueNpcName");
                            for (int b = 0; b < 5; b++)
                            {
                                FontManager.removeText("DialogueReply" + b.ToString());
                            }

                            this.currentStage = 0;
                        }
                        return;
                    }
                }
                else
                    FontManager.addText(new Text(stageList[this.currentStage].replyList.Keys.ToList()[a], "DialogueReply" + a.ToString(), currentpos, DialogueHandler.textSize, "franklin2.png", true, Text.positionOriginPoint.Middle, Color4.Yellow));

            }
        }

        public void draw()
        {

        }

    }

    public class DialogueStage
    {

        public string npcSays = string.Empty;
        public Dictionary<string, int> replyList = new Dictionary<string, int>();
        public int Nr = -2;

        public List<string> rslLines = new List<string>();

        public void executeRSL(Npc.Npc buffernpc)
        {
            bool findnextbracket = false;
            for (int a = 0; a < rslLines.Count; a++)
            {
                string[] split = rslLines[a].Split(new string[] { "\t", " ", "(", ")", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (findnextbracket && split[0] != "}")
                {
                    continue;
                }
                if (!findnextbracket && split[0] == "}")
                {
                    if (split.Length > 1 && split[1] == "else")
                    {
                        findnextbracket = true;
                    }
                    continue;
                }
                if (findnextbracket && split[0] == "}")
                {
                    findnextbracket = !findnextbracket;
                }

                if (split.Length > 0)
                {
                    if (split[0] == "NpcSays")
                    {
                        string line = string.Empty;
                        for (int b = 1; b < split.Length; b++)
                        {
                            line += split[b];
                            if (b != split.Length - 1)
                                line += " ";
                        }
                        this.npcSays = line;
                    }
                    if (split[0] == "ClearReplies")
                    {
                        this.replyList = new Dictionary<string, int>();
                    }
                    if (split[0] == "Reply")
                    {
                        string line = string.Empty;
                        for (int b = 1; b < split.Length - 1; b++)
                        {
                            string[] split2 = split[b].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

                            line += split2[0];
                            if (b != split.Length - 2)
                            {
                                line += " ";
                            }
                        }
                        int nextstage = int.Parse(split[split.Length - 1]);
                        replyList.Add(line, nextstage);
                    }

                    if (split[0] == "if")
                    {
                        if (split[1].StartsWith("npc"))
                        {
                            string[] split2 = split[1].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                            object var = buffernpc.getVariable(split2[1]);
                            object var2;
                            if (split[3].StartsWith("npc"))
                            {
                                string[] split3 = split[3].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                var2 = buffernpc.getVariable(split3[1]);
                            }
                            else if (split[3].StartsWith("player"))
                            {
                                string[] split3 = split[3].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                                var2 = (object)Player.PlayerHandler.playerList.First(x => x.control).inventory.getItemCount(split3[1]);
                            }
                            else
                                var2 = (object)split[3];
                            if (split[2] == "==")
                            {
                                if (var == var2) { }
                                else
                                    findnextbracket = true;
                            }
                            else
                                if (split[2] == "!=")
                                {
                                    if (var != var2) { }
                                    else
                                        findnextbracket = true;
                                }
                                else
                                    if (split[2] == "<")
                                    {
                                        if (double.Parse((string)var) < double.Parse((string)var2)) { }
                                        else
                                            findnextbracket = true;
                                    }
                                    else
                                        if (split[2] == ">")
                                        {
                                            if (double.Parse((string)var) > double.Parse((string)var2)) { }
                                            else
                                                findnextbracket = true;
                                        }
                        }
                        else
                            if (split[1].StartsWith("player")) // [0]if [1] player.gold [2]operator [3]val
                            {
                                string[] split2 = split[1].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                object var = (object)(Player.PlayerHandler.playerList.First(x => x.control).inventory.getItemCount(split2[1]).ToString());
                                object var2;
                                if (split[3].StartsWith("npc"))
                                {
                                    string[] split3 = split[3].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                    var2 = buffernpc.getVariable(split3[1]);
                                }
                                else if (split[3].StartsWith("player"))
                                {
                                    string[] split3 = split[3].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                                    var2 = (object)Player.PlayerHandler.playerList.First(x => x.control).inventory.getItemCount(split3[1]);
                                }
                                else
                                    var2 = (object)split[3];
                                if (split[2] == "==")
                                {
                                    if (var == var2) { }
                                    else
                                        findnextbracket = true;
                                }
                                else
                                    if (split[2] == "!=")
                                    {
                                        if (var != var2) { }
                                        else
                                            findnextbracket = true;
                                    }
                                    else
                                        if (split[2] == "<")
                                        {
                                            if (double.Parse((string)var) < double.Parse((string)var2)) { }
                                            else
                                                findnextbracket = true;
                                        }
                                        else
                                            if (split[2] == ">")
                                            {
                                                if (double.Parse((string)var) > double.Parse((string)var2)) { }
                                                else
                                                    findnextbracket = true;
                                            }
                            }
                            else if (split[1] == "Quest.getFlag")
                            {
                                string[] split2 = split[1].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                object var = Quests.QuestHandler.getQuest(split[2].Replace("\"", string.Empty)).currentQuestFlag.ToString();
                                object var2;
                                if (split[4].StartsWith("npc"))
                                {
                                    string[] split3 = split[4].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                    var2 = buffernpc.getVariable(split3[1]);
                                }
                                else if (split[4].StartsWith("player"))
                                {
                                    string[] split3 = split[4].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                                    var2 = (object)Player.PlayerHandler.playerList.First(x => x.control).inventory.getItemCount(split3[1]);
                                }
                                else
                                    var2 = (object)split[4];
                                if (split[3] == "==")
                                {
                                    if (var.ToString() == var2.ToString()) { }
                                    else
                                        findnextbracket = true;
                                }
                                else
                                    if (split[3] == "!=")
                                    {
                                        if (var != var2) { }
                                        else
                                            findnextbracket = true;
                                    }
                                    else
                                        if (split[3] == "<")
                                        {
                                            if (double.Parse((string)var) < double.Parse((string)var2)) { }
                                            else
                                                findnextbracket = true;
                                        }
                                        else
                                            if (split[3] == ">")
                                            {
                                                if (double.Parse((string)var) > double.Parse((string)var2)) { }
                                                else
                                                    findnextbracket = true;
                                            }
                            }

                    }


                    if (split[0] == "player.removeItem")
                    {
                        Player.PlayerHandler.playerList.First(x => x.control).inventory.removeItem(split[1].Replace("\"", string.Empty), int.Parse(split[2]));
                    }
                    if (split[0] == "player.addItem")
                    {
                        string item = string.Empty;
                        for (int b = 1; b < split.Length - 1; b++)
                        {
                            item += split[b];
                            if (b != split.Length - 2)
                            {
                                item += " ";
                            }
                        }
                        item.Replace("\"", string.Empty);

                        Player.PlayerHandler.playerList.First(x => x.control).inventory.addItem(item, int.Parse(split[split.Length - 1]));
                    }
                    if (split[0] == "Quest.start")
                    {
                        string quest = string.Empty;
                        for (int b = 1; b < split.Length - 1; b++)
                        {
                            quest += split[b];
                            if (b != split.Length - 2)
                            {
                                quest += " ";
                            }
                        }
                        quest = quest.Replace("\"", string.Empty).Replace(",", string.Empty);
                        Quests.QuestHandler.getQuest(quest).currentQuestFlag = int.Parse(split[split.Length - 1]);
                        Quests.QuestHandler.getQuest(quest).active = true;
                    }

                    if (split[0] == "sound")
                    {
                        if (split[1] == "play")
                        {
                            SoundManager.playSound(split[2].Replace("\"", string.Empty));
                        }
                    }


                    if (split[0] == "npc")
                    {
                        if (split.Length > 1)
                        {
                            string variable = split[1];
                            bool foundvar = false;
                            foreach (Variable v in buffernpc.variableList)
                            {
                                if (v.name == variable)
                                {
                                    foundvar = true;
                                    if (split.Length > 2)
                                    {
                                        if (split[2] == "=")
                                        {
                                            if (split.Length > 3)
                                            {

                                                if (split[3].StartsWith("\""))
                                                {
                                                    string bufstring = string.Empty;
                                                    for (int b = 3; b < split.Length; b++)
                                                    {
                                                        string[] split2 = split[b].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
                                                        bufstring += split2[0];
                                                        if (b != split.Length - 1)
                                                            bufstring += " ";
                                                    }
                                                    v.setValue((object)bufstring);
                                                }
                                                else if (split[3] == bool.TrueString)
                                                {
                                                    v.setValue((object)true);
                                                }
                                                else if (split[3] == bool.FalseString)
                                                {
                                                    v.setValue((object)false);
                                                }
                                                else if (split[3] == "playerID")
                                                {
                                                    buffernpc.setVariable(variable, Player.PlayerHandler.playerList.First(x => x.control).playerID);
                                                }
                                                else
                                                    v.setValue((object)split[3]);


                                            }
                                            else
                                                throwRSLexception("value expected");
                                        }
                                        else
                                            if (split[2] == "+=")
                                            {
                                                if (split.Length > 3)
                                                {
                                                    if (split[3].StartsWith("\""))
                                                    {
                                                        string bufstring = string.Empty;
                                                        for (int b = 3; b < split.Length; b++)
                                                        {
                                                            string[] split2 = split[b].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
                                                            bufstring += split2[0];
                                                            if (b != split.Length - 1)
                                                                bufstring += " ";
                                                        }
                                                        try
                                                        {
                                                            v.setValue((double.Parse(v.getValue().ToString().Replace("\"", string.Empty)) + double.Parse(bufstring.Replace("\"", string.Empty))));
                                                            return;
                                                        }
                                                        catch (Exception)
                                                        {
                                                            v.setValue((string)v.getValue() + (object)bufstring);
                                                        }

                                                    }
                                                    else if (split[3] == bool.TrueString)
                                                    {
                                                        v.setValue((object)true);
                                                    }
                                                    else if (split[3] == bool.FalseString)
                                                    {
                                                        v.setValue((object)false);
                                                    }
                                                    else
                                                        v.setValue((object)split[3]);
                                                }
                                                else
                                                    throwRSLexception("value expected");
                                            }
                                            else
                                                if (split[2] == "-=")
                                                {
                                                    if (split.Length > 3)
                                                    {
                                                        if (split[3].StartsWith("\""))
                                                        {
                                                            string bufstring = string.Empty;
                                                            for (int b = 3; b < split.Length; b++)
                                                            {
                                                                string[] split2 = split[b].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
                                                                bufstring += split2[0];
                                                                if (b != split.Length - 1)
                                                                    bufstring += " ";
                                                            }
                                                            try
                                                            {
                                                                v.setValue((double.Parse(v.getValue().ToString().Replace("\"", string.Empty)) - double.Parse(bufstring.Replace("\"", string.Empty))));
                                                                return;
                                                            }
                                                            catch (Exception)
                                                            {
                                                                v.setValue((string)(v.getValue()).ToString().Replace(bufstring, string.Empty));
                                                            }
                                                        }
                                                        else if (split[3] == bool.TrueString)
                                                        {
                                                            v.setValue((object)true);
                                                        }
                                                        else if (split[3] == bool.FalseString)
                                                        {
                                                            v.setValue((object)false);
                                                        }
                                                        else
                                                            v.setValue((object)split[3]);
                                                    }
                                                    else
                                                        throwRSLexception("value expected");
                                                }
                                                else
                                                    throwRSLexception("Illegal operator \"" + split[3] + "\"");
                                    }
                                    else
                                        throwRSLexception("Operator expected after \"base." + variable + "\"");
                                    break;
                                }
                            }
                            if (!foundvar)
                                throwRSLexception("Base Actor \"" + buffernpc.baseActorName + "\" does not contain a definition for variable: \"" + variable + "\"");

                        }
                    }
                }
            }
        }


        private void throwRSLexception(string message)
        {
            ExceptionHandler.printException("RSL ERROR -> " + message, ConsoleColor.DarkRed, ExceptionHandler.ExceptionHandle.CLOSEONKEY);
        }

    }
}
