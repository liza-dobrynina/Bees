using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bees
{
    public partial class Form1 : Form
    {
        World world = new World();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Random rnd = new Random();
            for (int i = 0; i < 5; i++)
                world.AddObject(new Bees(world, rnd.NextDouble() * world.WorldOptions.WorldWidth, rnd.NextDouble() * world.WorldOptions.WorldHeight));
        }

        private void worldTimer_Tick(object sender, EventArgs e)
        {
            world.Action();
            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            world.Draw(e.Graphics);
        }
    }
}
