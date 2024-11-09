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
    public partial class AvailableTruck : Form
    {
        DataTable dtAvailable;
        DataTable dtUnavailable;

        public AvailableTruck()
        {
            InitializeComponent();
        }
        private void AvailableTruck_Load(object sender, EventArgs e)
        {
            LoadAvailableTrucks();
            LoadUnavailableTrucks();
        }

        private void LoadAvailableTrucks()
        {
            dtAvailable = new DataTable();
            dtAvailable.Columns.Add("DT", typeof(int));
            dtAvailable.Columns.Add("Last Trip", typeof(DateTime));
            dtAvailable.Columns.Add("Status", typeof(string));

            LoadTruckData(dtAvailable, "Available");
            bunifuDataGridView1.DataSource = dtAvailable;
            SetupDataGridView(bunifuDataGridView1);
        }

        private void LoadUnavailableTrucks()
        {
            dtUnavailable = new DataTable();
            dtUnavailable.Columns.Add("DT", typeof(int));
            dtUnavailable.Columns.Add("Last Trip", typeof(DateTime));
            dtUnavailable.Columns.Add("Status", typeof(string));

            LoadTruckData(dtUnavailable, "Unavailable");
            bunifuDataGridView2.DataSource = dtUnavailable;
            SetupDataGridView(bunifuDataGridView2);
        }

        private void LoadTruckData(DataTable dt, string status)
        {
            // Modify the query to parameterize the status value to prevent SQL injection
            string query = "SELECT truckId AS DT, lastUsedDate AS Last_trip, status FROM Trucks WHERE status = @status";
            MySqlParameter[] parameters = { new MySqlParameter("@status", status) };

            // Execute query and get truck data
            DataTable truckData = SQLConnection.ExecuteQuery(query, parameters);

            // Populate the provided DataTable (dtAvailable or dtUnavailable) with the results
            foreach (DataRow row in truckData.Rows)
            {
                dt.Rows.Add(row["DT"], row["Last_trip"], row["status"]);
            }
        }


        private void SetupDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgv.Columns[0].Width = 50;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            EditTruckAvailability editTruckAvailability = new EditTruckAvailability(); 
            editTruckAvailability.ShowDialog();
        }
    }
}