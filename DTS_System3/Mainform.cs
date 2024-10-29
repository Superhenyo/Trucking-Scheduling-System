using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;





namespace DTS_System3
{
    public partial class Mainform : Form
    {

        private Resizer rs = new Resizer();
        public string loginName = "";

        public Mainform()
        {
            InitializeComponent();
            this.Load += Mainform_Load;
            this.Resize += Mainform_Resize;
        }

        private void Mainform_Load(object sender, EventArgs e)
        {

            rs.FindAllControls(this);
            bunifuLabel6.Text = this.loginName;
        }

        private void Mainform_Resize(object sender, EventArgs e)
        {
            rs.ResizeAllControls(this, this.Width, this.Height);
        }

        private void trucksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListOfTrucks listOfTrucks = new ListOfTrucks();
            listOfTrucks.Show();
        }

        private void driversToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListOfDrivers listOfDrivers = new ListOfDrivers();
            listOfDrivers.Show();
        }

        private void truckOwnersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListOfTruckOwners listOfTruckOwners = new ListOfTruckOwners();
            listOfTruckOwners.Show();
        }

        private void scheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
