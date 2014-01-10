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
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using opendagproject.Game.Graphics.Lighting;

namespace opendagproject.Game.Mapeditor
{
    public partial class AddLight : Form
    {
        private Vector2 position;
        private Color4 color = Color4.Gold;
        private Light.LightType lt = Light.LightType.REGULAR;
        public bool isActive = true;

        public AddLight(Vector2 position)
        {
            InitializeComponent();
            this.position = position;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int radius = 0;
            bool res = safeTextBoxParse(textBox1, out radius);
            Light.LightRenderType lrt = Light.LightRenderType.STATIC;
           
            if (radioButton2.Checked)
            {
                lrt = Light.LightRenderType.DYNAMIC;
            }
            
            Light light = new Light(this.position, radius, lrt,lt, this.color);
            if (lt == Light.LightType.POINTLIGHT)
            {
                string[] split = textBox2.Text.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                int startnode = int.Parse(split[0]);
                int conewidth = int.Parse(split[1]);
                startnode -= (conewidth / 2);
                if (startnode < 0) startnode += 360;
                if (startnode >= 360) startnode -= 360;
                light.conewidth = conewidth;
                light.startnode = startnode ;
            }
            if (lt == Light.LightType.ROTATINGCONE)
            {
                string[] split = textBox2.Text.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                int speed = int.Parse(split[0]);
                int conewidth = int.Parse(split[1]);
                light.conewidth = conewidth;
                light.rotationspeed = speed;
            }

            LightHandler.addLight(light);

            this.isActive = false;
            this.Close();
        }

        public void tick()
        {
            Application.DoEvents();
        }

        private bool safeTextBoxParse(TextBox t, out int result)
        {
            result = 0;
            try
            {
                result = int.Parse(t.Text);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SolidBrush brush = new SolidBrush(cd.Color);
                pictureBox1.CreateGraphics().FillRectangle(brush, 0, 0, pictureBox1.Width - 1, pictureBox1.Height - 1);
                this.color = new Color4(cd.Color.R, cd.Color.G, cd.Color.B, cd.Color.A);
            }
        }

        private void AddLight_Load(object sender, EventArgs e)
        {
            SolidBrush brush = new SolidBrush(Color.Gold);
            pictureBox1.CreateGraphics().FillRectangle(brush, 0, 0, pictureBox1.Width - 1, pictureBox1.Height - 1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    label3.Text = "No arguments";
                    this.lt = Light.LightType.REGULAR;
                    break;
                case 1:
                    label3.Text = "[angle], [pointwidth]";
                    this.lt = Light.LightType.POINTLIGHT;
                    break;
                case 2:
                    label3.Text = "[speed], [conewidth]";
                    this.lt = Light.LightType.ROTATINGCONE;
                    break;
                default:
                    label3.Text = "No arguments";
                    break;
            }

        }

        private void AddLight_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.isActive = false;
        }
    }
}
