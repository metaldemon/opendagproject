using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Graphics;

namespace opendagproject.Game.Mapeditor
{
    public partial class UI : Form
    {
        private bool selectLightPos;
        private bool selectContainerPos;
        private bool selectNpcPos;

        public string walkoverScript;
        public string onfireScript;

        public Vector2 selectionPosition
        {
            get { return new Vector2(0, 0); }
            set
            {
                if (selectLightPos)
                {
                    selectLightPos = false;
                    Mapeditor.selectPosition = false;
                    Mapeditor.addLightForm = new AddLight(value);
                    Mapeditor.addLightForm.Show();
                }
                if (selectContainerPos)
                {
                    selectContainerPos = false;
                    Mapeditor.selectPosition = false;
                    Mapeditor.addContainerForm = new AddContainer(value);
                    Mapeditor.addContainerForm.Show();
                }
                if (selectNpcPos)
                {
                    selectNpcPos = false;
                    Mapeditor.selectPosition = false;
                    Mapeditor.addNpcForm = new AddNpc(value);
                    Mapeditor.addNpcForm.Show();
                }
            }
        }

        public UI()
        {
            InitializeComponent();
            trackBar1.Minimum = 0;
            trackBar1.Maximum = 100;

        }

        private void UI_Load(object sender, EventArgs e)
        {
            this.Location = new Point(GameUtils.resolutionX + 20, 0);
            this.pictureBox1.BackColor = Color.White;
        }

        public void tick()
        {
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mapeditor.editlayer = 0;
            label1.Text = "Current Layer: 0";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Mapeditor.editlayer = 1;
            label1.Text = "Current Layer: 1";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Mapeditor.editlayer = 2;
            label1.Text = "Current Layer: 2";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Mapeditor.saveMap(ofd.FileName);
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            opendagproject.Game.Graphics.Lighting.LightHandler.ambient = (float)trackBar1.Value / 100f;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Mapeditor.RSLIDE = new RSL.RSL_IDE();
            Mapeditor.RSLIDE.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (Mapeditor.selectPosition)
            {
                Mapeditor.selectPosition = false;
                this.selectNpcPos = false;
                this.selectContainerPos = false;
                this.selectLightPos = false;
                button15.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e) // npc
        {
            Mapeditor.selectPosition = true;
            this.selectNpcPos = true;
        }

        private void button6_Click(object sender, EventArgs e) // container
        {
            Mapeditor.selectPosition = true;
            this.selectContainerPos = true;
        }

        private void button7_Click(object sender, EventArgs e) // light
        {
            Mapeditor.selectPosition = true;
            this.selectLightPos = true;
            button15.Enabled = true;
        }

        private void button14_Click(object sender, EventArgs e) // set color
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (World.WorldManager.exclusiveLayer == -1)
            {
                World.WorldManager.exclusiveLayer = Mapeditor.editlayer;
                button4.Text = "Show all layers";
            }
            else
            {
                World.WorldManager.exclusiveLayer = -1;
                button4.Text = "Show only this layer";
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (Game.States.GameStateManager.currentGlobalGameState == States.GameStateManager.GlobalGameState.Mapeditor)
            {
                Loader.loadMap();
                Program.currentGameState = new States.GameplayState();
                Game.States.GameStateManager.currentGlobalGameState = States.GameStateManager.GlobalGameState.Game;
                button11.Text = "To editor";
            }
            else
            {
                Loader.loadMap();
                Program.currentGameState = new States.MapeditorState();
                Game.States.GameStateManager.currentGlobalGameState = States.GameStateManager.GlobalGameState.Mapeditor;
                button11.Text = "Test map";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Loader.loadMap();
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.walkoverScript = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.onfireScript = textBox2.Text;
        }
    }
}
