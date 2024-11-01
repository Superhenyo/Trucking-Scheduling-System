using Bunifu.Framework.UI;
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
    public partial class Logincs : Form
    {
    //    SQLConnection SQLConnection = new SQLConnection();
        public Logincs()
        {
            InitializeComponent();
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            if (bunifuDropdown1.Text == "Administrator")
            {
                string query = "select * from Administrators where username = '"+bunifuTextBox1.Text+ "' and adminPassword ='" + bunifuTextBox2.Text+"'";
                DataTable dt = SQLConnection.ExecuteQuery(query);
                if (dt.Rows.Count >= 1)
                {
                    Mainform mainform = new Mainform();
                    mainform.loginName = bunifuTextBox1.Text;
                    mainform.Show();
                }
                bunifuTextBox1.Text = "";
                bunifuTextBox2.Text = "";
                this.Hide();
            }
            else if (bunifuDropdown1.Text == "Staff")
            {
                string query = "select * from staff where username = '"+bunifuTextBox1.Text+ "' and adminPassword ='" + bunifuTextBox2.Text+"'";
                DataTable dt = SQLConnection.ExecuteQuery(query);
                if (dt.Rows.Count >= 1)
                {
                    Mainform mainform = new Mainform();
                    mainform.loginName = bunifuTextBox1.Text;
                    mainform.Show();
                }

            }
            else
            {
                MessageBox.Show("Please Check the Credentials you Entered!");
            }
        }

        private void Logincs_Load(object sender, EventArgs e)
        {
            bunifuDropdown1.SelectedIndex = 0;


        }
    }
}
