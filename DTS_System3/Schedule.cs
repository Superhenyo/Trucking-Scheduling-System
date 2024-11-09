using MySql.Data.MySqlClient;
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
    public partial class Schedule : Form
    {
        public DataTable dtschedule;
        public Schedule()
        {
            InitializeComponent();
        }
        private void SetupDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.AllowUserToAddRows = false;
        }

        private void Schedule_Load(object sender, EventArgs e)
        {
            LoadOtp();
            if (bunifuDropdown1.Items.Count > 0)
            {
                bunifuDropdown1.SelectedIndex = 0; // Select the first item if available
            }
            LoadScheduleTable();
        }

        private void LoadOtp()
        {

            string query = "select otpID from OTP";
            DataTable dt = SQLConnection.ExecuteQuery(query);

            bunifuDropdown1.DataSource = dt;
            bunifuDropdown1.DisplayMember = "otpID";
            bunifuDropdown1.ValueMember = "otpID";
            bunifuDropdown1.SelectedIndex = -1;

            bunifuDropdown1.SelectedIndexChanged += bunifuDropdown1_SelectedIndexChanged;
        }


        private void LoadScheduleTable()
        {
            dtschedule = new DataTable();
            dtschedule.Columns.Add("DT", typeof(int));
            dtschedule.Columns.Add("Driver1", typeof(string));
            dtschedule.Columns.Add("Driver2", typeof(string));
            dtschedule.Columns.Add("Tons", typeof(string));
            dtschedule.Columns.Add("Status", typeof(string));
            dtschedule.Columns.Add("Batch", typeof(string));
            dtschedule.Columns.Add("tripDate", typeof(string));
            dtschedule.Columns.Add("Action", typeof(string));

            if (bunifuDropdown1?.SelectedValue != null)
            {
                string otpID = bunifuDropdown1.SelectedValue.ToString();
                LoadTripDate(dtschedule, otpID);
            }


            bunifuDataGridView1.DataSource = dtschedule;
            SetupDataGridView(bunifuDataGridView1);
        }

        private void LoadTripDate(DataTable dt, string otpID)
        {
            string query = @"SELECT 
            t.truckID AS DT, 
      CONCAT(D1.firstName, ' ', D1.lastName) AS Driver1,
      CONCAT(D2.firstName, ' ', D2.lastName) AS Driver2,
      t.tonsCarry AS Tons, 
      t.status AS Status,
      tb.batchNumber AS Batch,
      t.tripDate 
 FROM Trips t
 LEFT JOIN tripBatch tb ON t.tripBatchID = tb.batchID
 LEFT JOIN Drivers D1 ON t.Driver1ID = D1.driverID
 LEFT JOIN Drivers D2 ON t.Driver2ID = D2.driverID
 WHERE tb.otpID = @otpID";

            Console.WriteLine("otpID: " + otpID); // Make sure otpID has a valid value here

            MySqlParameter[] sp = { new MySqlParameter("@otpID", otpID)};
            DataTable tripData = SQLConnection.ExecuteQuery(query, sp);

            foreach (DataRow row in tripData.Rows)
            {
                dt.Rows.Add(row["DT"], row["Driver1"], row["Driver2"], row["Tons"], row["Status"], row["Batch"], row["tripDate"]);
            }
        }

        private void bunifuDropdown1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bunifuDropdown1.SelectedItem != null)
            {
                LoadScheduleTable(); 
            }
            
        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
