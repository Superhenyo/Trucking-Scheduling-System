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
        }
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            btnCreateSchedule_Click(sender, e);
        }

        private void btnCreateSchedule_Click(object sender, EventArgs e)
        {
            // Get input values from the BunifuTextBox
            string tonsInput = bunifuTextBox1.Text.Trim();
            string idInput = bunifuTextBox4.Text.Trim();
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

            // Save OTP to the database
            SaveOTPToDatabase(tonsToDeliver, otpDate, otpExpiryDate, idInput);
        }

        private void SaveOTPToDatabase(int tonsToDeliver, DateTime otpDate, DateTime otpExpiryDate, string idInput)
        {
            string query = "INSERT INTO OTP (otpID, totalTons, startDate, endDate) VALUES (@idInput, @tonsToDeliver, @otpDate, @otpExpiryDate);";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@idInput", idInput),
                new MySqlParameter("@tonsToDeliver", tonsToDeliver),
                new MySqlParameter("@otpDate", otpDate),
                new MySqlParameter("@otpExpiryDate", otpExpiryDate)
            };

            SQLConnection.ExecuteCommand(query, parameters);
            MessageBox.Show("OTP saved successfully!");
        }

        private void AddNewOTP_Load(object sender, EventArgs e)
        {

        }
    }
}
