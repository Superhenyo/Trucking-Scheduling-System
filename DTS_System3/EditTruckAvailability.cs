using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DTS_System3
{
    public partial class EditTruckAvailability : Form
    {
        DataTable dtAvailable;
        DataTable dtUnavailable;

        public EditTruckAvailability()
        {
            InitializeComponent();
        }

        private void TruckAvailability_Load(object sender, EventArgs e)
        {
            LoadAvailableTrucks();
            LoadUnavailableTrucks();

            bunifuDataGridView1.CellValueChanged += bunifuDataGridView1_CellValueChanged;
            bunifuDataGridView2.CellValueChanged += bunifuDataGridView2_CellValueChanged;
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
            bunifuDataGridView1.CurrentCellDirtyStateChanged += bunifuDataGridView1_CurrentCellDirtyStateChanged;
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

            bunifuDataGridView2.CurrentCellDirtyStateChanged += bunifuDataGridView2_CurrentCellDirtyStateChanged;
        }

        private void LoadTruckData(DataTable dt, string status)
        {
            string query = $"SELECT truckId AS DT, lastUsedDate, status FROM Trucks WHERE status = '{status}'";
            DataTable truckData = SQLConnection.ExecuteQuery(query);

            foreach (DataRow row in truckData.Rows)
            {
                dt.Rows.Add(row["DT"], row["lastUsedDate"], row["status"]);
            }

          
        }

        private void SetupDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewComboBoxColumn statusColumn = new DataGridViewComboBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataSource = new List<string> { "Available", "Unavailable" },
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                DataPropertyName = "Status"
            };

            if (dgv.Columns["Status"] != null)
            {
                dgv.Columns.Remove("Status");
            }

            dgv.Columns.Add(statusColumn);
        }

        private void bunifuDataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == bunifuDataGridView1.Columns["Status"].Index && e.RowIndex >= 0)
            {
                var statusValue = bunifuDataGridView1.Rows[e.RowIndex].Cells["Status"].Value;
                if (statusValue != null && statusValue.ToString() == "Unavailable")
                {
                    MoveRowBetweenDataTables(dtAvailable, dtUnavailable, e.RowIndex, "Unavailable");
                    bunifuDataGridView1.EndEdit();
                }
            }
        }

        private void bunifuDataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == bunifuDataGridView2.Columns["Status"].Index && e.RowIndex >= 0)
            {
                var statusValue = bunifuDataGridView2.Rows[e.RowIndex].Cells["Status"].Value;
                if (statusValue != null && statusValue.ToString() == "Available")
                {
                    MoveRowBetweenDataTables(dtUnavailable, dtAvailable, e.RowIndex, "Available");
                }
            }
        }

        private void MoveRowBetweenDataTables(DataTable source, DataTable target, int rowIndex, string newStatus)
        {
            DataRow row = source.Rows[rowIndex];
            DataRow newRow = target.NewRow();
            newRow["DT"] = row["DT"];
            newRow["Last Trip"] = row["Last Trip"];
            newRow["Status"] = newStatus;

            target.Rows.Add(newRow);
            source.Rows.Remove(row);

            // Refresh the DataGridViews to show updated data
            RefreshDataGrids();
        }

        private void RefreshDataGrids()
        {
            // Rebind the DataSource to update the views
            bunifuDataGridView1.DataSource = dtAvailable;
            bunifuDataGridView2.DataSource = dtUnavailable;

            // Optional: You can call Refresh() if you want to ensure the UI refresh
            bunifuDataGridView1.Refresh();
            bunifuDataGridView2.Refresh();
        }

        private void SaveTrucks(DataTable dt, string status)
        {
            foreach (DataRow row in dt.Rows)
            {
                string lastUsedDateFormatted = row["Last Trip"] == DBNull.Value ? "NULL" : $"'{Convert.ToDateTime(row["Last Trip"]).ToString("yyyy-MM-dd")}'";
                string query = $"INSERT INTO Trucks (truckId, lastUsedDate, status) VALUES ({row["DT"]}, {lastUsedDateFormatted}, '{status}') ON DUPLICATE KEY UPDATE lastUsedDate = {lastUsedDateFormatted}, status = '{status}'";
                SQLConnection.ExecuteCommand(query);
            }
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            SaveTrucks(dtAvailable, "Available");
            SaveTrucks(dtUnavailable, "Unavailable");
            MessageBox.Show("Changes saved successfully!");
        }

        private void bunifuDataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (bunifuDataGridView1.IsCurrentCellDirty)
            {
                bunifuDataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void bunifuDataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (bunifuDataGridView2.IsCurrentCellDirty)
            {
                bunifuDataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
