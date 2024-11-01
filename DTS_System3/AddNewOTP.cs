using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DTS_System3
{
    public partial class AddNewOTP : Form
    {
        public AddNewOTP()
        {
            InitializeComponent();
            SQLConnection conn = new SQLConnection();
        }

        private void bunifuLabel4_Click(object sender, EventArgs e)
        {
            btnCreateSchedule_Click(sender, e);
        }

        private void AddNewOTP_Load(object sender, EventArgs e)
        {
        }

        private void btnCreateSchedule_Click(object sender, EventArgs e)
        {
            // Get input values from the BunifuTextBox
            string tonsInput = bunifuTextBox1.Text.Trim();
            int tonsToDeliver;

            // Convert input to integer
            if (!int.TryParse(tonsInput, out tonsToDeliver) || tonsToDeliver <= 0)
            {
                MessageBox.Show("Please enter a valid positive integer for tons to deliver.");
                return;
            }

            DateTime otpDate = bunifuDatePicker1.Value;
            DateTime otpExpiryDate = bunifuDatePicker2.Value;

            // Validate dates
            if (otpDate > otpExpiryDate)
            {
                MessageBox.Show("OTP expiry date must be after OTP date.\nOTP Date: " + otpDate.ToString("yyyy-MM-dd") + "\nOTP Expiry Date: " + otpExpiryDate.ToString("yyyy-MM-dd"));
                return;
            }

            // Create the delivery schedule
            CreateDeliverySchedule(tonsToDeliver, otpDate, otpExpiryDate);
        }

        private void CreateDeliverySchedule(int tonsToDeliver, DateTime otpDate, DateTime otpExpiryDate)
        {
            const int truckCapacity = 18;
            int batchesNeeded = (int)Math.Ceiling((double)tonsToDeliver / truckCapacity);

            for (int i = 0; i < batchesNeeded; i++)
            {
                DateTime tripDate = otpDate.AddDays(i);

                if (tripDate > otpExpiryDate)
                {
                    break; // Exit the loop if the trip date exceeds the expiry date
                }

                AssignTrucksForBatch(truckCapacity, ref tonsToDeliver, tripDate);
            }

            // Optionally refresh the DataGrids here to reflect the scheduled trips
        }

        private void AssignTrucksForBatch(int truckCapacity, ref int remainingTons, DateTime tripDate)
        {
            HashSet<int> usedTruckIDs = new HashSet<int>(); // To ensure trucks are only used once in the queue

            while (remainingTons > 0)
            {
                int truckID = GetAvailableTruckID(tripDate);
                if (truckID == -1 || usedTruckIDs.Contains(truckID))
                {
                    // Break if no available trucks or truck has already been used
                    break;
                }

                usedTruckIDs.Add(truckID); // Track the truck ID used in this batch

                int tonsToCarry = Math.Min(remainingTons, truckCapacity);
                remainingTons -= tonsToCarry;

                InsertTripIntoDatabase(tripDate, tonsToCarry, truckID);
            }
        }

        private int GetAvailableTruckID(DateTime tripDate)
        {
            string query = @"
                SELECT truckID 
                FROM Trucks 
                WHERE lastUsedDate IS NULL 
                    OR TIMESTAMPDIFF(HOUR, lastUsedDate, @tripDate) >= 24 
                ORDER BY lastUsedDate ASC 
                LIMIT 1";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@tripDate", tripDate)
            };

            object result = SQLConnection.ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : -1;
        }

        private void InsertTripIntoDatabase(DateTime tripDate, int tonsToCarry, int truckID)
        {
            string query = "INSERT INTO Trips (tripDate, tonsCarry, truckID, status) VALUES (@tripDate, @tonsCarry, @truckID, 'Queued');";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@tripDate", tripDate),
                new MySqlParameter("@tonsCarry", tonsToCarry),
                new MySqlParameter("@truckID", truckID)
            };

            SQLConnection.ExecuteCommand(query, parameters);

            UpdateTruckLastUsedDate(truckID, tripDate);
        }

        private void UpdateTruckLastUsedDate(int truckID, DateTime tripDate)
        {
            string updateQuery = "UPDATE Trucks SET lastUsedDate = @tripDate WHERE truckID = @truckID";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@tripDate", tripDate),
                new MySqlParameter("@truckID", truckID)
            };

            SQLConnection.ExecuteCommand(updateQuery, parameters);
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            btnCreateSchedule_Click(sender, e);
        }
    }
}
