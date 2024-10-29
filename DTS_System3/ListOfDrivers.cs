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
    public partial class ListOfDrivers : Form
    {
        private Resizer rs = new Resizer();
        public ListOfDrivers()
        {
            InitializeComponent();
            this.Load += ListOfDrivers_Load;
            this.Resize += ListOfDrivers_Resize;
        }
     

        private void bunifuTileButton1_Click(object sender, EventArgs e)
        {

        }

        private void ListOfDrivers_Load(object sender, EventArgs e)
        {
            rs.FindAllControls(this);
            string query = "select * from drivers";
            DataTable dt = SQLConnection.ExecuteQuery(query);
            bunifuDataGridView1.DataSource = dt;
        }

        private void ListOfDrivers_Resize(object sender, EventArgs e)
        {
            rs.ResizeAllControls(this, this.Width, this.Height);
        }
    }
}
