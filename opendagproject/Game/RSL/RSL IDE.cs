using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace opendagproject.Game.RSL
{
    public partial class RSL_IDE : Form
    {
        private readonly string rslTemplate = "Script myScript" + Environment.NewLine + "{" + Environment.NewLine + "// Here you can add your script stuffies" + Environment.NewLine + "}";
        public bool isActive = true;
        private string currentEditFile = string.Empty;

        private readonly Color textColor = Color.LightGray;

        public RSL_IDE()
        {
            InitializeComponent();
            RichTextBox.CheckForIllegalCrossThreadCalls = false;
            ProgressBar.CheckForIllegalCrossThreadCalls = false;
            editor.ReadOnly = false;
            editor.Text = "Load or create a new file :)";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.editor.Text = this.rslTemplate;
            this.currentEditFile = string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "*.rsl";
            ofd.InitialDirectory = GameUtils.getGamePath() + "\\data\\scripts\\";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                loadFile(ofd.FileName);
            }
        }

        public void tick()
        {
            label1.Text = "Loaded: " + RSLHandler.scriptsLoaded + " script(s)";
            try
            {
                Application.DoEvents();
                editor.Update();
            }
            catch (Exception e) { }
        }
        
        private void _loadFile()
        {
            string file = this.currentEditFile;
            editor.Text = "";
            
            List<string> lines = Loader.loadFile(file);
            progressBar1.Maximum = lines.Count;
            progressBar1.Value = 0;
            foreach (string line in lines) 
            {
                progressBar1.Value++;
                string[] split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string[] split2 = line.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (split2.Length > 0)
                {
                    if (split2.Length > 0 && split2[0].StartsWith("//"))
                    {
                        AppendText(this.editor, line, Color.Green);
                    }
                    else
                    {
                        for (int a = 0; a < split2.Length; a++)
                        {
                            for (int b = 0; b < split.Length; b++)
                            {
                                if (b == a && split[b].StartsWith("\t"))
                                {
                                    for (int c = 0; c < split[b].Count(x => x == '\t'); c++)
                                    {
                                        AppendText(this.editor, "\t", this.textColor);
                                    }
                                }
                            }
                            if (split2[a] == "Script" || split2[a] == "Dialogue" || split2[a] == "NPC" || split2[a] == "ITEM" || split2[a] == "PARTICLE")
                            {
                                AppendText(this.editor, split2[a], Color.Blue);
                                if (a != split2.Length - 1)
                                {
                                    AppendText(this.editor, " ", this.textColor);
                                }
                            }
                            else
                                if (split2[a] == "Func")
                                {
                                    AppendText(this.editor, split2[a], Color.FromArgb(255, 200, 0, 200));
                                    if (a != split2.Length - 1)
                                    {
                                        AppendText(this.editor, " ", this.textColor);
                                    }
                                }
                                else
                                    if (split2[a] == "Particle")
                                    {
                                        AppendText(this.editor, split2[a], Color.Yellow);
                                        if (a != split2.Length - 1)
                                        {
                                            AppendText(this.editor, " ", this.textColor);
                                        }
                                    }
                                    else
                                        if (split2[a] == "Spawn" && split2[a - 1] == "Particle")
                                        {
                                            AppendText(this.editor, split2[a], Color.Green);
                                            if (a != split2.Length - 1)
                                            {
                                                AppendText(this.editor, " ", this.textColor);
                                            }
                                        }
                                        else
                                            if (split2[a].StartsWith("SELF") || split2[a].StartsWith("TARGET"))
                                            {
                                                for (int b = 0; b < split2[a].Length; b++)
                                                {
                                                    if (split2[a][b].ToString() != ";")
                                                    {
                                                        AppendText(this.editor, split2[a][b].ToString(), Color.Red);
                                                    }
                                                    else
                                                    {
                                                        AppendText(this.editor, split2[a][b].ToString(), this.textColor);
                                                    }
                                                }

                                                if (a != split2.Length - 1)
                                                {
                                                    AppendText(this.editor, " ", this.textColor);
                                                }
                                            }
                                            else
                                                if (split2[a] == "Actor")
                                                {
                                                    AppendText(this.editor, split2[a], Color.Blue);
                                                    if (a != split2.Length - 1)
                                                    {
                                                        AppendText(this.editor, " ", this.textColor);
                                                    }
                                                }
                                                else
                                                    if (Actors.ActorHandler.actorWithName(split2[a]))
                                                    {
                                                        AppendText(this.editor, split2[a], Color.Blue);
                                                        if (a != split2.Length - 1)
                                                        {
                                                            AppendText(this.editor, " ", this.textColor);
                                                        }
                                                    }
                                                    else
                                                        if (split2[a] == "Base")
                                                        {
                                                            AppendText(this.editor, split2[a], Color.DarkBlue);
                                                            if (a != split2.Length - 1)
                                                            {
                                                                AppendText(this.editor, " ", this.textColor);
                                                            }
                                                        }
                                                        else
                                                            if (split2[a] == "base" || split2[a] == "false" || split2[a] == "true" || split2[a] == "var")
                                                            {
                                                                AppendText(this.editor, split2[a], Color.LightBlue);
                                                                if (a != split2.Length - 1)
                                                                {
                                                                    AppendText(this.editor, " ", this.textColor);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                AppendText(this.editor, split2[a], this.textColor);
                                                                if (a != split2.Length - 1)
                                                                {
                                                                    AppendText(this.editor, " ", this.textColor);
                                                                }
                                                            }

                        }
                    }
                }
                AppendText(this.editor, Environment.NewLine, this.textColor);
            }
        }

        private void loadFile(string file)
        {
            currentEditFile = file;
            Thread t = new Thread(_loadFile);
            t.Start();
        }

        public void setProgressbarMax(int max)
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = max;
        }

        public void addProgressbarValue()
        {
            progressBar1.Value++;
        }

        private void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = Color.Black;
        }

        private void RSL_IDE_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.isActive = false;
        }

        private void editor_KeyPress(object sender, KeyPressEventArgs e)
        {
            editor.SelectionColor = this.textColor;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RSLHandler.debugmessages = true;
            Loader.initScripts();
            Debug.WriteLine("-----------------------------------------------------------", ConsoleColor.White);
            Debug.WriteLine("Build complete!", ConsoleColor.White);
            Debug.WriteLine(RSLHandler.rslErrors + " Error(s) during build.", ConsoleColor.White);
            RSLHandler.debugmessages = false;
            RSLHandler.rslErrors = 0;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.currentEditFile != string.Empty)
            {
                save(this.currentEditFile);
            }
            else { saveAs(); }
        }

        public void addDebugLine(string line)
        {
            richTextBox1.AppendText(line + Environment.NewLine);
            richTextBox1.ScrollToCaret();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Title = "Merge";
            ofd.DefaultExt = "*.rsl";
            ofd.InitialDirectory = GameUtils.getGamePath() + "\\data\\scripts\\";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(ofd.FileName, true);
                sw.Write(editor.Text);
                sw.Close();
                File.Delete(this.currentEditFile);
                this.editor.Text = string.Empty;
                this.loadFile(ofd.FileName);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            saveAs();
        }

        private void saveAs()
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "*.rsl";
            ofd.InitialDirectory = GameUtils.getGamePath() + "\\data\\scripts\\";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                save(ofd.FileName);
            }
        }

        private void save(string file)
        {
            StreamWriter sw = new StreamWriter(file);
            sw.Write(editor.Text);
            sw.Close();
            loadFile(file);
        }

        private void RSL_IDE_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
