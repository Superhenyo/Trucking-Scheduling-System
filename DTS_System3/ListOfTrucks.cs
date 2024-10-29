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
    public partial class ListOfTrucks : Form
    {
        private Resizer rs = new Resizer();
        public ListOfTrucks()
        {
            InitializeComponent();
            this.Load += ListOfTrucks_Load;
            this.Resize += ListOfTrucks_Resize;
        }

        private void ListOfTrucks_Load(object sender, EventArgs e)
        {
            rs.FindAllControls(this);
        }

        private void ListOfTrucks_Resize(object sender, EventArgs e)
        {
            rs.ResizeAllControls(this, this.Width, this.Height);
        }
    }
}
