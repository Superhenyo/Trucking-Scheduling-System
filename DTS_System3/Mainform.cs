using Bunifu.UI.WinForms;
using System;
using System.Collections;
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

            LoadYesterdayTrips();
            LoadTodayTrips();
            QueueTrips();

        }

        private void LoadYesterdayTrips()
        {
            string query = "SELECT T.truckID AS DT, CONCAT(D1.firstName, ' ', D1.lastName) AS driver1, " +
                  "CONCAT(D2.firstName, ' ', D2.lastName) AS driver2 " +
                  "FROM Trips T " +
                  "LEFT JOIN tripDriver TD ON T.tripDriverID = TD.tripDriverID " +
                  "LEFT JOIN Drivers D1 ON TD.driver1ID = D1.driverID " +
                  "LEFT JOIN Drivers D2 ON TD.driver2ID = D2.driverID " +
                  "WHERE tripDate = CURDATE() - INTERVAL 1 DAY AND status = 'In Progress';"; // Change CURDATE() to CURDATE() - INTERVAL 1 DAY

            DataTable dt = SQLConnection.ExecuteQuery(query);
            bunifuDataGridView3.DataSource = dt;
            bunifuDataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            bunifuDataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            bunifuDataGridView3.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            bunifuDataGridView3.Columns[0].Width = 50;
            bunifuDataGridView3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void LoadTodayTrips()
        {
            string query = "SELECT T.truckID AS DT, CONCAT(D1.firstName, ' ', D1.lastName) AS driver1, " +
                    "CONCAT(D2.firstName, ' ', D2.lastName) AS driver2 " +
                    "FROM Trips T " +
                    "LEFT JOIN tripDriver TD ON T.tripDriverID = TD.tripDriverID " +
                    "LEFT JOIN Drivers D1 ON TD.driver1ID = D1.driverID " +
                    "LEFT JOIN Drivers D2 ON TD.driver2ID = D2.driverID " +
                    "WHERE tripDate = CURDATE() AND status = 'In Progress';"; // This is correct if you want today's trips

            DataTable dt = SQLConnection.ExecuteQuery(query);
            bunifuDataGridView2.DataSource = dt;
            bunifuDataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            bunifuDataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            bunifuDataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            bunifuDataGridView2.Columns[0].Width = 50;
            bunifuDataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void QueueTrips()
        {
            string query = "SELECT T.truckID AS DT " +
           "FROM Trips T " +
           "WHERE tripDate < CURDATE() AND status = 'Queued';";
            DataTable dt = SQLConnection.ExecuteQuery(query);
            bunifuDataGridView1.DataSource = dt;
            bunifuDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            bunifuDataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            bunifuDataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            bunifuDataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
            CreateSchedule createSchedule = new CreateSchedule();
            createSchedule.Show();
        }

        private void oTPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewOTP addNewOTP = new AddNewOTP();
            addNewOTP.Show();
        }
    }
}
