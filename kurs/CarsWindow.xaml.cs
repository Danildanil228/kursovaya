using System.Data;
using System.Windows;
using System.Data.SQLite;
using System.Windows.Controls;
using System;
using System.IO;
using Microsoft.Win32;

namespace CarRentalApp
{
    public partial class CarsWindow : Window
    {
        private DataTable _carsTable;
        private SQLiteDataAdapter _adapter;
        private SQLiteConnection _connection;

        public CarsWindow()
        {
            InitializeComponent();
            _carsTable = new DataTable();
            _adapter = new SQLiteDataAdapter();
            _connection = new SQLiteConnection();
            LoadCars();
        }

        private void LoadCars()
        {
            string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "identifier.sqlite");

            if (!File.Exists(databasePath))
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

                string query = "SELECT * FROM Car";
                _adapter = new SQLiteDataAdapter(query, _connection);
                _adapter.Fill(_carsTable);
                CarsDataGrid.ItemsSource = _carsTable.DefaultView;

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

        private void CarsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_carsTable != null)
            {
                _adapter.Update(_carsTable);
            }
        }

        private void CarsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (_carsTable != null)
            {
                _adapter.Update(_carsTable);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_carsTable != null)
            {
                _adapter.Update(_carsTable);
                MessageBox.Show("Изменения сохранены.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CarsDataGrid.SelectedItems.Count > 0)
            {
                var itemsToDelete = new List<DataRowView>();
                foreach (var item in CarsDataGrid.SelectedItems)
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

                _adapter.Update(_carsTable);
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

