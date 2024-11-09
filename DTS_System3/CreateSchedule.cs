using Bunifu.UI.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DTS_System3
{
    public partial class CreateSchedule : Form
    {
        private Resizer rs = new Resizer();
        private DataTable dtAvailable;
        private DataTable dtUnavailable;
        private DataTable driverPool;
        private List<int> selectedDrivers = new List<int>();
        private Dictionary<int, (int? Driver1ID, int? Driver2ID)> previousAssignments = new Dictionary<int, (int? Driver1ID, int? Driver2ID)>();

        public CreateSchedule()
        {
            InitializeComponent();
            this.Load += CreateSchedule_Load;
            this.Resize += CreateSchedule_Resize;
        }

        private void CreateSchedule_Load(object sender, EventArgs e)
        {
            rs.FindAllControls(this);

            LoadUnavailableTrucks();
            LoadAvailableTrucks();
            LoadDriverPool();
            LoadDropdown();


        }

        private void LoadAvailableTrucks()
        {
            dtAvailable = new DataTable();
            dtAvailable.Columns.Add("TruckID", typeof(int));
            dtAvailable.Columns.Add("Driver1", typeof(string));
            dtAvailable.Columns.Add("Driver2", typeof(string));

            string query = @"
        SELECT 
            t.truckID AS TruckID,
            NULL AS Driver1,
            NULL AS Driver2
        FROM 
            Trucks t
        WHERE 
            t.status = 'Available'";

            DataTable availableTrucks = SQLConnection.ExecuteQuery(query);

            foreach (DataRow row in availableTrucks.Rows)
            {
                dtAvailable.Rows.Add(row["TruckID"], row["Driver1"], row["Driver2"]);
            }

            bunifuDataGridView1.DataSource = dtAvailable;
            SetupDataGridView(bunifuDataGridView1);

            // Call LoadTruckDropdown to populate bunifuDropdown2
            LoadTruckDropdown(availableTrucks);
        }
        private void LoadDropdown()
        {
            string query = "SELECT otpID FROM OTP WHERE endDate > CURDATE()";
            DataTable result = SQLConnection.ExecuteQuery(query);

            bunifuDropdown1.Items.Clear();
            foreach (DataRow row in result.Rows)
            {
                bunifuDropdown1.Items.Add(row["otpID"].ToString());
            }

            bunifuDropdown1.SelectedIndexChanged += bunifuDropdown1_SelectedIndexChanged;
        }

        private void bunifuDropdown1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bunifuDropdown1.SelectedItem != null)
            {
                string selectedOtpID = bunifuDropdown1.SelectedItem.ToString();
                LoadOtpDetails(selectedOtpID);
            }
        }

        private void LoadOtpDetails(string otpID)
        {
            string query = $"SELECT startDate, endDate, totalTons FROM OTP WHERE otpID = {otpID}";
            DataTable result = SQLConnection.ExecuteQuery(query);

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                bunifuLabel7.Text = Convert.ToDateTime(row["startDate"]).ToString("yyyy-MM-dd");
                bunifuLabel10.Text = Convert.ToDateTime(row["endDate"]).ToString("yyyy-MM-dd");
                bunifuLabel8.Text = row["totalTons"].ToString();
            }
        }

     
        private void bunifuDropdown3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDropdown(bunifuDropdown3, bunifuDropdown4);
        }

        private void bunifuDropdown4_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDropdown(bunifuDropdown4, bunifuDropdown3);
        }

        private void FilterDropdown(Bunifu.UI.WinForms.BunifuDropdown sourceDropdown, Bunifu.UI.WinForms.BunifuDropdown targetDropdown)
        {
            if (sourceDropdown.SelectedValue != null && int.TryParse(sourceDropdown.SelectedValue.ToString(), out int selectedDriverID))
            {
                // Clone the driver pool and exclude both selected drivers and drivers already assigned in selectedDrivers list
                DataTable filteredDriverPool = driverPool.Clone();
                foreach (DataRow row in driverPool.Rows)
                {
                    int driverID = (int)row["driverID"];
                    if (driverID != selectedDriverID && !selectedDrivers.Contains(driverID)) // Exclude selected and previously assigned drivers
                    {
                        filteredDriverPool.ImportRow(row);
                    }
                }

                // Re-bind target dropdown with the filtered data
                targetDropdown.DataSource = filteredDriverPool;
                targetDropdown.DisplayMember = "DriverName";
                targetDropdown.ValueMember = "driverID";
                targetDropdown.SelectedIndex = -1; // Ensure no default selection to avoid issues
            }
        }



        private void LoadTruckDropdown(DataTable availableTrucks)
        {
            bunifuDropdown2.Items.Clear();
            foreach (DataRow row in availableTrucks.Rows)
            {
                bunifuDropdown2.Items.Add(row["TruckID"].ToString());
            }

            if (bunifuDropdown2.Items.Count > 0)
            {
                // Set no item selected in bunifuDropdown2
                bunifuDropdown2.SelectedIndex = -1;
                // Set the first item as the default selected item
            }
        }


        private void LoadUnavailableTrucks()
        {
            dtUnavailable = new DataTable();
            dtUnavailable.Columns.Add("TruckID", typeof(int));
            dtUnavailable.Columns.Add("TruckOwner", typeof(string));
            dtUnavailable.Columns.Add("Contacts", typeof(string));

            string query = @"
                SELECT 
                    t.truckID AS TruckID,
                    CONCAT(o.firstName, ' ', o.lastName) AS TruckOwner,
                    o.contacts AS Contacts
                FROM 
                    Trucks t
                JOIN 
                    Truck_Owners towner ON t.truckID = towner.truckID
                JOIN 
                    Owners o ON towner.ownerID = o.ownerID
                WHERE 
                    t.status = 'Unavailable'";

            DataTable unavailableTrucks = SQLConnection.ExecuteQuery(query);

            foreach (DataRow row in unavailableTrucks.Rows)
            {
                dtUnavailable.Rows.Add(row["TruckID"], row["TruckOwner"], row["Contacts"]);
            }

            bunifuDataGridView2.DataSource = dtUnavailable;
            SetupDataGridView(bunifuDataGridView2);
        }

        private void AssignDrivers(int truckID, int driver1ID, int driver2ID)
        {
            // Step 1: Update DataTable for available trucks
            foreach (DataRow row in dtAvailable.Rows)
            {
                if ((int)row["TruckID"] == truckID)
                {
                    // Clear previous driver assignments
                    if (row["Driver1"] != DBNull.Value && row["Driver2"] != DBNull.Value)
                    {
                        int previousDriver1ID = GetDriverIDFromName(row["Driver1"].ToString());
                        int previousDriver2ID = GetDriverIDFromName(row["Driver2"].ToString());

                        // Remove previous drivers from selectedDrivers list
                        selectedDrivers.Remove(previousDriver1ID);
                        selectedDrivers.Remove(previousDriver2ID);
                    }

                    // Step 2: Set new driver names in DataTable
                    row["Driver1"] = GetDriverNameByID(driver1ID);
                    row["Driver2"] = GetDriverNameByID(driver2ID);

                    // Step 3: Update selectedDrivers list with the new selections
                    selectedDrivers.Add(driver1ID);
                    selectedDrivers.Add(driver2ID);

                    break;
                }
            }

            // Step 4: Refresh the driver pool and bind to the dropdowns
            LoadDriverPool();
        }

        private int GetDriverIDFromName(string driverName)
        {
            foreach (DataRow row in driverPool.Rows)
            {
                if (row["DriverName"].ToString() == driverName)
                    return (int)row["driverID"];
            }
            return -1; // Driver not found
        }

        private string GetDriverNameByID(int driverID)
        {
            foreach (DataRow row in driverPool.Rows)
            {
                if ((int)row["driverID"] == driverID)
                    return row["DriverName"].ToString();
            }
            return string.Empty; // Driver ID not found
        }

        private void LoadDriverPool()
        {
            string query = "SELECT driverID, CONCAT(firstName, ' ', lastName) AS DriverName FROM Drivers";
            driverPool = SQLConnection.ExecuteQuery(query);
            // Clone the driver pool, excluding currently assigned drivers
            DataTable filteredDriverPool = driverPool.Clone();
            foreach (DataRow row in driverPool.Rows)
            {
                int driverID = (int)row["driverID"];
                if (!selectedDrivers.Contains(driverID))
                {
                    filteredDriverPool.ImportRow(row);
                }
            }

            // Re-bind driver dropdowns with the updated pool
            bunifuDropdown3.DataSource = filteredDriverPool.Copy();
            bunifuDropdown3.DisplayMember = "DriverName";
            bunifuDropdown3.ValueMember = "driverID";
            bunifuDropdown3.SelectedIndex = -1;

            bunifuDropdown4.DataSource = filteredDriverPool.Copy();
            bunifuDropdown4.DisplayMember = "DriverName";
            bunifuDropdown4.ValueMember = "driverID";
            bunifuDropdown4.SelectedIndex = -1;
        }



        private void SetupDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.AllowUserToAddRows = false;
        }

     


        private void CreateSchedule_Resize(object sender, EventArgs e)
        {
            rs.ResizeAllControls(this, this.Width, this.Height);
        }

        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            if (bunifuDropdown2.SelectedItem != null && bunifuDropdown3.SelectedValue != null && bunifuDropdown4.SelectedValue != null)
            {
                int selectedTruckID = int.Parse(bunifuDropdown2.SelectedItem.ToString());
                int driver1ID = (int)bunifuDropdown3.SelectedValue;
                int driver2ID = (int)bunifuDropdown4.SelectedValue;

                // Call AssignDrivers method
                AssignDrivers(selectedTruckID, driver1ID, driver2ID);

                bunifuDataGridView1.Refresh();
                MessageBox.Show("Drivers assigned successfully!");
            }
            else
            {
                MessageBox.Show("Please select a truck and both drivers.");
            }
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
