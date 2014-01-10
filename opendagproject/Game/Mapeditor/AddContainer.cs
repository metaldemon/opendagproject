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
using opendagproject.Game.Player.Inventory;
using opendagproject.Game.World;

namespace opendagproject.Game.Mapeditor
{
    public partial class AddContainer : Form
    {
        private Vector2 position;
        public bool isActive = true;

        public AddContainer(Vector2 position)
        {
            InitializeComponent();
            this.position = position;
        }

        private void AddContainer_Load(object sender, EventArgs e)
        {
            foreach (InventoryItem i in InventoryHandler.itemList)
            {
                listBox1.Items.Add((object)i.name);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            World.Container container = new World.Container(this.position, "chest.png");
            for (int a = 0; a < listBox2.Items.Count; a++)
            {
                container.addItem(listBox2.Items[a].ToString(), int.Parse(listBox3.Items[a].ToString()));
            }
            WorldManager.containerList.Add(container);
            this.Close();
        }

        public void tick()
        {
            Application.DoEvents();
        }

        private void AddContainer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.isActive = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int value = int.Parse(textBox1.Text);
            }
            catch (Exception ex)
            {
                if (textBox1.Text.Length > 0)
                {
                    textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox2.Items.Add(listBox1.SelectedItem.ToString());
            listBox3.Items.Add(textBox1.Text);
        }


    }
}
