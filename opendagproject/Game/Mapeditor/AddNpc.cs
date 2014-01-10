using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using opendagproject.Game.RSL.Actors.Npc;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;

namespace opendagproject.Game.Mapeditor
{
    public partial class AddNpc : Form
    {
        private Vector2 position;
        public AddNpc(Vector2 position)
        {
            InitializeComponent();
            this.position = position;

        }

        private void AddNpc_Load(object sender, EventArgs e)
        {
            NpcHandler.npcList.ForEach(x => comboBox1.Items.Add(x.name));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Npc bufnpc = NpcHandler.npcList.First(x => x.name == comboBox1.SelectedItem.ToString()).clone();
            bufnpc.setVariable("positionX", (int)position.X);
            bufnpc.setVariable("positionY", (int)position.Y);
            bufnpc.initialize();
            NpcHandler.npcGameList.Add(bufnpc);
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Npc bufnpc = NpcHandler.npcList.First(x => x.name == comboBox1.SelectedItem.ToString());
            this.label1.Text = "Name: " + bufnpc.name + Environment.NewLine +
                "Base Actor: " + bufnpc.baseActorName + Environment.NewLine +
                "MaxHealth: " + bufnpc.getVariable("maxhealth").ToString() + Environment.NewLine +
                "Hostile: " + bufnpc.getVariable("hostile").ToString() + Environment.NewLine +
                "Karma: " + bufnpc.getVariable("karma").ToString() + Environment.NewLine +
                "Weapon: " + bufnpc.getVariable("equippedweapon").ToString();
        }
    }
}
