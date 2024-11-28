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
    public partial class RentalsWindow : Window
    {
        private DataTable _rentalsTable;
        private SQLiteDataAdapter _adapter;
        private SQLiteConnection _connection;

        public RentalsWindow()
        {
            InitializeComponent();
            _rentalsTable = new DataTable();
            _adapter = new SQLiteDataAdapter();
            _connection = new SQLiteConnection();
            LoadRentals();
        }

        private void LoadRentals()
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

                string query = "SELECT * FROM Rental";
                _adapter = new SQLiteDataAdapter(query, _connection);
                _adapter.Fill(_rentalsTable);
                RentalsDataGrid.ItemsSource = _rentalsTable.DefaultView;

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

        private void RentalsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_rentalsTable != null)
            {
                _adapter.Update(_rentalsTable);
            }
        }

        private void RentalsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (_rentalsTable != null)
            {
                _adapter.Update(_rentalsTable);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_rentalsTable != null)
            {
                _adapter.Update(_rentalsTable);
                MessageBox.Show("Изменения сохранены.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RentalsDataGrid.SelectedItems.Count > 0)
            {
                var itemsToDelete = new List<DataRowView>();
                foreach (var item in RentalsDataGrid.SelectedItems)
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

                _adapter.Update(_rentalsTable);
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