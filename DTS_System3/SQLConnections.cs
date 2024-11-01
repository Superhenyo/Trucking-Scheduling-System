using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public class SQLConnection
{
    private static string connString = "Server=localhost;Port=3306;User=Yohen Nanati;Password=1234;Database=TruckingSystem;";

    public SQLConnection()
    {
        ConnectToDatabase();
    }

    public static DataTable ExecuteQuery(string query, MySqlParameter[] parameters = null)
    {
        DataTable dataTable = new DataTable();
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Add parameters if provided
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Query execution failed: " + ex.Message);
        }
        return dataTable;
    }

    public static object ExecuteScalar(string query, MySqlParameter[] parameters = null)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Add parameters if provided
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return command.ExecuteScalar();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Scalar query execution failed: " + ex.Message);
            return null;
        }
    }

    public static void ConnectToDatabase()
    {
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

    public static void ExecuteCommand(string query, MySqlParameter[] parameters = null)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Add parameters if provided
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    command.ExecuteNonQuery();
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
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@photo", photo);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Photo command execution failed: " + ex.Message);
        }
    }
}
