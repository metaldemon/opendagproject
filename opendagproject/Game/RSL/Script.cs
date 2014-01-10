using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using opendagproject.Game.RSL.Actors;
using opendagproject.Game.RSL.Actors.Dialogue;
using opendagproject.Game.RSL.Actors.Npc;
using opendagproject.Game.RSL.Functions;
using opendagproject.Game.RSL.Quests;
using opendagproject.Game.Particles;
using System.Threading;
using opendagproject.Game.Player.Inventory;

namespace opendagproject.Game.RSL
{
    public class Script
    {
        List<string> scriptlines = new List<string>();

        public string name = string.Empty;

        public enum internBlockType
        {
            Script,
            Actor,
            Dialogue,
            DialogueStage,
            DialogueIfBlock,
            Function,
            Quest,
            Null
        }


        private Actor bufactor = new Actor();
        private Dialogue bufdialogue = new Dialogue();
        private DialogueStage bufdialoguestage = new DialogueStage();
        private Function bufFunction = new Function();
        private Quest bufQuest = new Quest();
        private internBlockType currentBlockType = internBlockType.Null;
        private int currentline = 0;
        private int scriptstartline = 0;
        private int currentactorstartline = 0;
        private bool bufActorIsNpc = false;
        private bool bufActorIsItem = false;
        private bool bufActorIsParticle = false;
        public bool debugmessages = RSLHandler.debugmessages;



        public Script(string file)
        {
            Debug.WriteLine("-- RSL loading: " + file, ConsoleColor.Green);
            int bracketcount = 0;


            scriptlines = Loader.loadFile(file);

            for (int currentline = 0; currentline < scriptlines.Count; currentline++)
            {
                this.currentline = currentline;
                string[] split = scriptlines[currentline].Split(new string[] { " ", "\t", ";", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0)
                {
                    if (currentBlockType == internBlockType.Function)
                    {
                        handleFunctionLine(split);
                    }
                    if (currentBlockType == internBlockType.Quest)
                    {
                        handleQuestLine(split);
                    }
                    switch (split[0])
                    {
                        case "//":
                            if (this.debugmessages)
                            {
                                Debug.WriteLine("Commented line, skipping", ConsoleColor.Yellow);
                            }
                            break;
                        case "{":
                            if (functionbracketcount == 0 && questbracketcount == 0)
                            {
                                bracketcount++;
                            }
                            break;
                        case "}":
                            if (functionbracketcount == 0 && questbracketcount == 0)
                            {
                                bracketcount--;
                                checkBlocks();
                            }
                            break;
                    }
                    if (currentBlockType == internBlockType.Function || currentBlockType == internBlockType.Quest)
                    {
                        continue;
                    }
                    if (currentBlockType != internBlockType.Function && currentBlockType != internBlockType.Quest)
                    {
                        switch (split[0])
                        {
                            case "//":
                                break;
                            case "{":
                                break;
                            case "}":
                                break;
                            case "Script":
                                handleScript(split);
                                break;
                            case "var":
                                handleVar(split);
                                break;
                            case "NpcSays":
                                handleNpcSays(split);
                                break;
                            case "Reply":
                                handleReply(split);
                                break;
                            case "Stage":
                                handleStage(split);
                                break;
                            case "Dialogue":
                                handleDialogue(split);
                                break;
                            case "Actor":
                                handleActor(split);
                                break;
                            case "base":
                                handleBase(split);
                                break;
                            case "Base":
                                handleBaseBlock(split);
                                break;
                            case "npc":
                                handleNpcCommand(split);
                                break;
                            case "sound":
                                handleRuntimeDialogueCommand(split);
                                break;
                            case "Func":
                                handleFunction(split);
                                break;
                            case "Quest":
                                handleQuest(split);
                                break;
                            case "player.removeItem":
                                if (currentBlockType == internBlockType.DialogueStage || currentBlockType == internBlockType.DialogueIfBlock)
                                {
                                    this.bufdialoguestage.rslLines.Add(scriptlines[this.currentline]);
                                    if (debugmessages)
                                    {
                                        Debug.WriteLine("Runtime rsl line added: " + scriptlines[this.currentline].Replace("\t", string.Empty), ConsoleColor.Yellow);
                                    }
                                }
                                break;
                            case "player.addItem":
                                if (currentBlockType == internBlockType.DialogueStage || currentBlockType == internBlockType.DialogueIfBlock)
                                {
                                    this.bufdialoguestage.rslLines.Add(scriptlines[this.currentline]);
                                    if (debugmessages)
                                    {
                                        Debug.WriteLine("Runtime rsl line added: " + scriptlines[this.currentline].Replace("\t", string.Empty), ConsoleColor.Yellow);
                                    }
                                }
                                break;
                            case "Quest.start":
                                if (currentBlockType == internBlockType.DialogueStage || currentBlockType == internBlockType.DialogueIfBlock)
                                {
                                    this.bufdialoguestage.rslLines.Add(scriptlines[this.currentline]);
                                    if (debugmessages)
                                    {
                                        Debug.WriteLine("Runtime rsl line added: " + scriptlines[this.currentline].Replace("\t", string.Empty), ConsoleColor.Yellow);
                                    }
                                }
                                break;
                            case "if":
                                if (currentBlockType == internBlockType.DialogueStage)
                                {
                                    currentBlockType = internBlockType.DialogueIfBlock;
                                    this.bufdialoguestage.rslLines.Add(scriptlines[this.currentline]);
                                    if (debugmessages)
                                    {
                                        Debug.WriteLine("Runtime rsl line added: " + scriptlines[this.currentline].Replace("\t", string.Empty), ConsoleColor.Yellow);
                                    }
                                }
                                break;
                            default:
                                throwRSLexception(split[0] + " is not an RSL object.", currentline);
                                break;
                        }
                    }
                }
            }
        }



        #region handlers

        #region BaseBlock
        private void handleBaseBlock(string[] split)
        {
            if (currentBlockType == internBlockType.Script)
            {
                if (split.Length > 1)
                {
                    if (split[1] == "Actor")
                    {
                        currentactorstartline = currentline;
                        currentBlockType = internBlockType.Actor;

                        if (split.Length > 2)
                        {
                            bufactor.name = split[2];
                            if (debugmessages)
                            {
                                Debug.WriteLine("actor name set to: " + bufactor.name, ConsoleColor.Yellow);
                            }
                        }
                        else
                        {
                            throwRSLexception("Actor requires name.", currentline);
                        }
                    }
                }
                else
                    throwRSLexception("Base object requires blocktype.", currentline);
            }
            else
                throwRSLexception("Cannot define Base Object outside of a Script block.", currentline);
        }
        #endregion

        #region ScriptBlock
        private void handleScript(string[] split)
        {
            if (split.Length > 1)
            {
                this.name = split[1];
                currentBlockType = internBlockType.Script;
                if (debugmessages)
                {
                    Debug.WriteLine("intern block: script", ConsoleColor.Yellow);
                }
                scriptstartline = currentline;
            }
            else
                throwRSLexception("Script without a name.", currentline);
        }
        #endregion

        #region variables
        private void handleBase(string[] split)
        {
            if (currentBlockType == internBlockType.Actor)
            {
                if (split.Length > 1)
                {
                    string variable = split[1];
                    bool foundvar = false;
                    foreach (Variable v in bufactor.variableList)
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
                                            for (int a = 3; a < split.Length; a++)
                                            {
                                                string[] split2 = split[a].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
                                                bufstring += split2[0];
                                                if (a != split.Length - 1)
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
                                        else
                                        {
                                            v.setValue((object)split[3]);
                                        }
                                        if (debugmessages)
                                        {
                                            Debug.WriteLine("Set variable: " + v.name + " to: " + v.getValue().ToString() + " for actor: " + bufactor.name + " from baseactor: " + bufactor.baseActorName, ConsoleColor.Yellow);
                                        }
                                    }
                                    else
                                    {
                                        throwRSLexception("value expected", currentline);
                                    }
                                }
                                else
                                {
                                    throwRSLexception("Illegal operator \"" + split[3] + "\"", currentline);
                                }
                            }
                            else
                            {
                                throwRSLexception("Operator expected after \"base." + variable + "\"", currentline);
                            }
                            break;
                        }
                    }
                    if (!foundvar)
                    {
                        throwRSLexception("Base Actor \"" + bufactor.baseActorName + "\" does not contain a definition for variable: \"" + variable + "\"", currentline);
                    }
                }
                else
                {
                    throwRSLexception("Variable expected after \"base.\"", currentline);
                }
            }
            else
            {
                throwRSLexception("cannot assign value to base variable when outside of Actor block.", currentline);
            }
        }

        private void handleVar(string[] split)
        {
            if (currentBlockType == internBlockType.Actor)
            {
                if (split.Length > 1)
                {
                    bufactor.variableList.Add(new Variable(split[1], null, false));
                    if (debugmessages)
                    {
                        Debug.WriteLine("Created variable: " + split[1] + " for actor " + bufactor.name, ConsoleColor.Yellow);
                    }
                }
                else
                    throwRSLexception("Variable declared without a name.", currentline);
            }
            else
                throwRSLexception("Cannot assign script to non Actor Objects.", currentline);
        }
        #endregion

        #region Dialogue
        private void handleNpcSays(string[] split)
        {
            if (currentBlockType == internBlockType.DialogueStage)
            {
                if (split.Length > 1)
                {
                    if (split[1].StartsWith("\""))
                    {
                        string line = string.Empty;
                        for (int a = 1; a < split.Length; a++)
                        {
                            string[] split2 = split[a].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

                            line += split2[0];
                            if (a != split.Length - 1)
                            {
                                line += " ";
                            }
                        }
                        bufdialoguestage.npcSays = line;

                        bufdialoguestage.rslLines.Add("NpcSays " + line);
                        bufdialoguestage.rslLines.Add("ClearReplies");
                    }
                    else
                        throwRSLexception("\"" + split[0] + "\": Illegal function parameter, type of string expected.", currentline);
                }
                else
                    throwRSLexception("Arguments expected.", currentline);
            }
            else
                if (currentBlockType == internBlockType.DialogueIfBlock)
                {
                    if (split.Length > 1)
                    {
                        if (split[1].StartsWith("\""))
                        {
                            string line = string.Empty;
                            for (int a = 1; a < split.Length; a++)
                            {
                                string[] split2 = split[a].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

                                line += split2[0];
                                if (a != split.Length - 1)
                                {
                                    line += " ";
                                }
                            }
                            //bufdialoguestage.npcSays = line;
                            bufdialoguestage.rslLines.Add("NpcSays " + line);
                            bufdialoguestage.rslLines.Add("ClearReplies");
                        }
                        else
                            throwRSLexception("\"" + split[0] + "\": Illegal function parameter, type of string expected.", currentline);
                    }
                    else
                        throwRSLexception("Arguments expected.", currentline);
                }
                else
                    throwRSLexception("\"" + split[0] + "\" is an illegal statement in this type of block.", currentline);
        }

        private void handleReply(string[] split)
        {
            if (currentBlockType == internBlockType.DialogueStage)
            {
                if (split.Length > 1)
                {
                    if (split[1].StartsWith("\""))
                    {
                        string line = string.Empty;
                        for (int a = 1; a < split.Length - 1; a++)
                        {
                            string[] split2 = split[a].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

                            line += split2[0];
                            if (a != split.Length - 2)
                            {
                                line += " ";
                            }
                        }
                        int nextstage = 0;
                        try
                        {
                            nextstage = int.Parse(split[split.Length - 1]);
                        }
                        catch (Exception)
                        {
                            throwRSLexception("\"" + split[0] + "\": Illegal function parameter, type of integer expected.", currentline);
                        }

                        bufdialoguestage.replyList.Add(line, nextstage);
                        bufdialoguestage.rslLines.Add("Reply \"" + line + "\" " + nextstage);
                    }
                    else
                        throwRSLexception("\"" + split[0] + "\": Illegal function parameter, type of string expected.", currentline);
                }
                else
                    throwRSLexception("Arguments expected.", currentline);
            }
            else
                if (currentBlockType == internBlockType.DialogueIfBlock)
                {
                    if (split.Length > 1)
                    {
                        if (split[1].StartsWith("\""))
                        {
                            string line = string.Empty;
                            for (int a = 1; a < split.Length - 1; a++)
                            {
                                string[] split2 = split[a].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

                                line += split2[0];
                                if (a != split.Length - 2)
                                {
                                    line += " ";
                                }
                            }
                            int nextstage = 0;
                            try
                            {
                                nextstage = int.Parse(split[split.Length - 1]);
                            }
                            catch (Exception)
                            {
                                throwRSLexception("\"" + split[0] + "\": Illegal function parameter, type of integer expected.", currentline);
                            }

                            //bufdialoguestage.replyList.Add(line, nextstage);
                            bufdialoguestage.rslLines.Add("Reply \"" + line + "\" " + nextstage);
                        }
                        else
                            throwRSLexception("\"" + split[0] + "\": Illegal function parameter, type of string expected.", currentline);
                    }
                    else
                        throwRSLexception("Arguments expected.", currentline);
                }
                else
                    throwRSLexception("\"" + split[0] + "\" is an illegal statement in this type of block.", currentline);
        }

        private void handleDialogue(string[] split)
        {
            if (currentBlockType == internBlockType.Script)
            {
                currentBlockType = internBlockType.Dialogue;
                if (split.Length > 1)
                {
                    bufdialogue = new Dialogue();
                    bufdialogue.name = split[1];
                    if (debugmessages)
                    {
                        Debug.WriteLine("Created dialogue: " + split[1], ConsoleColor.Yellow);
                    }
                }
                else
                    throwRSLexception("Dialogue name expected.", currentline);
            }
            else
                throwRSLexception("Cannot define Dialogue Object outside of a Script block.", currentline);
        }

        private void handleStage(string[] split)
        {
            if (currentBlockType == internBlockType.Dialogue)
            {
                currentBlockType = internBlockType.DialogueStage;
                bufdialoguestage = new DialogueStage();
                if (split.Length > 1)
                {
                    try
                    {
                        int stagenr = int.Parse(split[1]);
                        bufdialoguestage.Nr = stagenr;
                        if (debugmessages)
                        {
                            Debug.WriteLine("Dialogue stage def set to: " + stagenr, ConsoleColor.Yellow);
                        }
                    }
                    catch (Exception e)
                    {
                        throwRSLexception("Dialogue Stage definition must be a number between: -1 and " + int.MaxValue + ".", currentline);
                    }
                }
                else
                    throwRSLexception("Stage number expected.", currentline);
            }
            else
                throwRSLexception("Cannot define DialogueStage outside of a Dialogue block.", currentline);
        }

        private void handleRuntimeDialogueCommand(string[] split)
        {
            if (currentBlockType == internBlockType.DialogueStage || currentBlockType == internBlockType.DialogueIfBlock)
            {
                if (split.Length > 1)
                {
                    if (split.Length > 2)
                    {
                        if (split.Length >= 3)
                        {
                            string s = string.Empty;
                            for (int a = 0; a < split.Length; a++)
                            {
                                s += split[a];
                                s += " ";
                            }
                            this.bufdialoguestage.rslLines.Add(s);
                            if (debugmessages)
                            {
                                Debug.WriteLine("Runtime rsl line added: " + s.Replace("\t", string.Empty), ConsoleColor.Yellow);
                            }
                        }
                        else
                        {
                            throwRSLexception("Variable expected after Command", currentline);
                        }
                    }
                    else
                    {
                        throwRSLexception("Command expected", currentline);
                    }
                }
                else
                {
                    throwRSLexception("Command group expected.", currentline);
                }
            }
            else
            {
                throwRSLexception("Cannot call Command outside Dialoguestage block.", currentline);
            }
        }

        private void handleNpcCommand(string[] split)
        {
            if (currentBlockType == internBlockType.DialogueStage)
            {
                if (split.Length > 1)
                {
                    if (split.Length > 2)
                    {
                        if (split.Length > 3)
                        {
                            string s = string.Empty;
                            for (int a = 0; a < split.Length; a++)
                            {
                                s += split[a];
                                s += " ";
                            }
                            this.bufdialoguestage.rslLines.Add(s);
                            if (debugmessages)
                            {
                                Debug.WriteLine("Runtime rsl line added: " + s, ConsoleColor.Yellow);
                            }
                        }
                        else
                        {
                            throwRSLexception("Variable expected after operator", currentline);
                        }
                    }
                    else
                    {
                        throwRSLexception("Operator expected after variable", currentline);
                    }
                }
                else
                {
                    throwRSLexception("Variable expected.", currentline);
                }
            }
            else
            {
                throwRSLexception("Cannot acces dialogue npc outside of dialogue stage block.", currentline);
            }
        }
        #endregion

        #region Actors
        private void handleActor(string[] split)
        {
            if (currentBlockType == internBlockType.Script)
            {
                if (split.Length > 1)
                {
                    currentactorstartline = currentline;
                    currentBlockType = internBlockType.Actor;

                    bufactor.name = split[1];
                    if (split.Length > 2)
                    {
                        if (split[2] == ":")
                        {
                            if (split.Length > 3)
                            {
                                bool foundparent = false;
                                foreach (Actor a in ActorHandler.actorList)
                                {
                                    if (a.name == split[3])
                                    {
                                        bufactor.baseActorName = a.name;
                                        foundparent = true;
                                        if (a.name == "NPC")
                                        {
                                            bufactor = new Npc();
                                            bufactor.name = split[1];
                                            bufactor.baseActorName = split[3];
                                            this.bufActorIsNpc = true;
                                            this.bufActorIsItem = false;
                                            this.bufActorIsParticle = false;
                                            if (debugmessages)
                                            {
                                                Debug.WriteLine("Npc created with name: " + bufactor.name + " from baseactor: " + bufactor.baseActorName, ConsoleColor.Yellow);
                                            }
                                        }
                                        else
                                            if (a.name == "ITEM")
                                            {
                                                bufactor = new InventoryItem();
                                                bufactor.name = split[1];
                                                bufactor.baseActorName = split[3];
                                                this.bufActorIsNpc = false;
                                                this.bufActorIsItem = true;
                                                this.bufActorIsParticle = false;
                                                if (debugmessages)
                                                {
                                                    Debug.WriteLine("Item created with name: " + bufactor.name + " from baseactor: " + bufactor.baseActorName, ConsoleColor.Yellow);
                                                }
                                            }
                                            else
                                                if (a.name == "PARTICLE")
                                                {
                                                    bufactor = new Particle();
                                                    bufactor.name = split[1];
                                                    bufactor.baseActorName = a.name;
                                                    this.bufActorIsNpc = false;
                                                    this.bufActorIsItem = false;
                                                    this.bufActorIsParticle = true;
                                                    if (debugmessages)
                                                    {
                                                        Debug.WriteLine("Particle created with name: " + bufactor.name + " from baseactor: " + bufactor.baseActorName, ConsoleColor.Yellow);
                                                    }
                                                }


                                        foreach (Variable v in a.variableList)
                                        {
                                            bufactor.variableList.Add(new Variable(v.name, v.getValue(), true));
                                        }
                                        break;
                                    }
                                }
                                if (!foundparent)
                                {
                                    throwRSLexception("Base actor: \"" + split[3] + "\" could not be found.", currentline);
                                }
                            }
                            else
                            {
                                throwRSLexception("Base actor expected for object inheritance.", currentline);
                            }
                        }
                        else
                        {
                            throwRSLexception("\":\" expected.", currentline);
                        }
                    }
                }
                else
                {
                    throwRSLexception("Actor requires name.", currentline);
                }
            }
            else
            {
                throwRSLexception("Cannot define Actor Object outside of a Script block.", currentline);
            }
        }
        #endregion

        #region Functions

        private void handleFunction(string[] split)
        {
            if (currentBlockType == internBlockType.Script)
            {
                if (split.Length > 1)
                {
                    string funcname = split[1];
                    bufFunction = new Function();
                    bufFunction.name = funcname;
                    currentBlockType = internBlockType.Function;
                }
                else
                {
                    throwRSLexception("Function requires name.", currentline);
                }
            }
            else
            {
                throwRSLexception("Cannot define Function outside of a Script block.", currentline);
            }
        }

        private int functionbracketcount = 0;

        private void handleFunctionLine(string[] split)
        {
            if (currentBlockType == internBlockType.Function)
            {
                if (split[0] == "{")
                {
                    functionbracketcount++;
                }
                if (split[0] == "}")
                {
                    functionbracketcount--;
                }
                bufFunction.rtrsl.Add(this.scriptlines[this.currentline]);
            }
            else
            {
                throwRSLexception("Cannot write to function outside of function block", currentline);
            }
        }

        #endregion

        #region Quests

        private void handleQuest(string[] split)
        {
            if (this.currentBlockType == internBlockType.Script)
            {
                if (split.Length > 1)
                {
                    this.bufQuest = new Quest(split[1]);
                    this.currentBlockType = internBlockType.Quest;
                }
                else
                {
                    throwRSLexception("Quest requires name.", currentline);
                }
            }
            else
            {
                throwRSLexception("Cannot define Quest outside of a Script block", currentline);
            }
        }

        private int questbracketcount = 0;

        private void handleQuestLine(string[] split)
        {
            if (currentBlockType == internBlockType.Quest)
            {
                if (split[0] == "{")
                {
                    questbracketcount++;
                }
                if (split[0] == "}")
                {
                    questbracketcount--;
                }
                this.bufQuest.addRSLLine(this.scriptlines[this.currentline]);
            }
            else
            {
                throwRSLexception("Cannot write to quest outside of quest block", currentline);
            }
        }

        #endregion

        #region Interpreter
        private void checkBlocks()
        {
            if (currentBlockType == internBlockType.Actor)
            {
                currentBlockType = internBlockType.Script;
                if (debugmessages)
                {
                    Debug.WriteLine("intern block: script", ConsoleColor.Yellow);
                }
                if (!this.bufActorIsNpc && !this.bufActorIsItem && !this.bufActorIsParticle)
                {
                    ActorHandler.actorList.Add(bufactor);
                    if (debugmessages)
                    {
                        Debug.WriteLine("Actor added to game", ConsoleColor.Yellow);
                    }
                }
                else if (this.bufActorIsItem)
                {
                    InventoryHandler.itemList.Add((InventoryItem)bufactor);
                }
                else if (this.bufActorIsNpc)
                {
                    NpcHandler.npcList.Add((Npc)bufactor);
                    if (debugmessages)
                    {
                        Debug.WriteLine("Npc added to game", ConsoleColor.Yellow);
                    }
                }
                else if (this.bufActorIsParticle)
                {
                    ParticleHandler.particleList.Add((Particle)bufactor);
                    if (debugmessages)
                    {
                        Debug.WriteLine("Particle added to game", ConsoleColor.Yellow);
                    }
                }
                bufactor = new Actor();
            }
            if (currentBlockType == internBlockType.Dialogue)
            {
                currentBlockType = internBlockType.Script;
                if (debugmessages)
                {
                    Debug.WriteLine("intern block: script", ConsoleColor.Yellow);
                }
                DialogueHandler.dialogueList.Add(bufdialogue);
                bufdialogue = new Dialogue();
            }
            if (currentBlockType == internBlockType.DialogueStage)
            {
                currentBlockType = internBlockType.Dialogue;
                if (debugmessages)
                {
                    Debug.WriteLine("intern block: dialogue", ConsoleColor.Yellow);
                }
                bufdialogue.stageList.Add(bufdialoguestage);
                bufdialoguestage = new DialogueStage();
            }
            if (currentBlockType == internBlockType.DialogueIfBlock)
            {
                currentBlockType = internBlockType.DialogueStage;
                if (debugmessages)
                {
                    Debug.WriteLine("intern block: dialoguestage", ConsoleColor.Yellow);
                }
                this.bufdialoguestage.rslLines.Add(scriptlines[this.currentline]);
            }
            if (currentBlockType == internBlockType.Function)
            {
                currentBlockType = internBlockType.Script;
                FunctionHandler.functionList.Add(bufFunction);
            }
            if (currentBlockType == internBlockType.Quest)
            {
                currentBlockType = internBlockType.Script;
                QuestHandler.questList.Add(this.bufQuest);
            }

        }
        #endregion

        #endregion

        private void throwRSLexception(string message)
        {
            ExceptionHandler.printException("RSL ERROR -> " + message, ConsoleColor.DarkRed, ExceptionHandler.ExceptionHandle.CLOSEONKEY);
        }
        private void throwRSLexception(string message, int linecount)
        {
            ExceptionHandler.printException("RSL ERROR at line: " + linecount + " -> " + message, ConsoleColor.DarkRed, ExceptionHandler.ExceptionHandle.CLOSEONKEY);
        }
    }
}
