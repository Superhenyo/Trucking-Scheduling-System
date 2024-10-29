using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public class SQLConnection
{
    // Directly assign the connection string for troubleshooting.
    private static string connString = "Server=localhost;Port=3306;User=Yohen Nanati;Password=1234;Database=TruckingSystem;";

    public SQLConnection()
    {
        ConnectToDatabase();
    }

    public static DataTable ExecuteQuery(string query)
    {
        DataTable dataTable = new DataTable();
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection))
                {
                    dataAdapter.Fill(dataTable);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Query execution failed: " + ex.Message);
        }
        return dataTable;
    }

    public static void ConnectToDatabase()
    {
        // Use a new MySqlConnection for each connect attempt.
        using (MySqlConnection connection = new MySqlConnection(connString))
        {
            try
            {
                connection.Open();
                MessageBox.Show("Connected successfully!");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Connection failed: " + ex.Message + "\nError Code: " + ex.Number);
            }
        }
    }

    public static void ExecuteCommand(string query)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (MySqlCommand mysqlCommand = new MySqlCommand(query, connection))
                {
                    mysqlCommand.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Command execution failed: " + ex.Message);
        }
    }

    public static void ExecutePhotoCommand(string query, object photo)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (MySqlCommand mysqlCommand = new MySqlCommand(query, connection))
                {
                    mysqlCommand.Parameters.AddWithValue("@photo", photo);
                    mysqlCommand.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Photo command execution failed: " + ex.Message);
        }
    }
}
