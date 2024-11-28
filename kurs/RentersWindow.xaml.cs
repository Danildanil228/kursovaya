using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace CarRentalApp
{
    public partial class RentersWindow : Window
    {
        private DataTable _rentersTable;
        private SQLiteDataAdapter _adapter;
        private SQLiteConnection _connection;

        public RentersWindow()
        {
            InitializeComponent();
            _rentersTable = new DataTable();
            _adapter = new SQLiteDataAdapter();
            _connection = new SQLiteConnection();
            LoadRenters();
        }

        private void LoadRenters()
        {
            string databasePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "identifier.sqlite");

            if (!System.IO.File.Exists(databasePath))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "SQLite Database (*.sqlite)|*.sqlite";
                openFileDialog.Title = "Выберите файл базы данных";

                if (openFileDialog.ShowDialog() == true)
                {
                    databasePath = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Файл базы данных не выбран!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            string connectionString = $"Data Source={databasePath}";

            try
            {
                _connection = new SQLiteConnection(connectionString);
                _connection.Open();

                string query = "SELECT * FROM Renter";
                _adapter = new SQLiteDataAdapter(query, _connection);
                _adapter.Fill(_rentersTable);
                RentersDataGrid.ItemsSource = _rentersTable.DefaultView;

                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.DeleteCommand = builder.GetDeleteCommand();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private void RentersDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_rentersTable != null)
            {
                _adapter.Update(_rentersTable);
            }
        }

        private void RentersDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (_rentersTable != null)
            {
                _adapter.Update(_rentersTable);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_rentersTable != null)
            {
                _adapter.Update(_rentersTable);
                MessageBox.Show("Изменения сохранены.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RentersDataGrid.SelectedItems.Count > 0)
            {
                var itemsToDelete = new List<DataRowView>();
                foreach (var item in RentersDataGrid.SelectedItems)
                {
                    if (item is DataRowView dataRowView)
                    {
                        itemsToDelete.Add(dataRowView);
                    }
                }

                foreach (var item in itemsToDelete)
                {
                    item.Row.Delete();
                }

                _adapter.Update(_rentersTable);
                MessageBox.Show("Выбранные строки удалены.");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _connection?.Close();
        }
    }
}
