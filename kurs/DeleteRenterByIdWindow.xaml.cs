using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CarRentalApp
{
    public partial class DeleteRenterByIdWindow : Window
    {
        private SQLiteConnection _connection;

        public DeleteRenterByIdWindow()
        {
            InitializeComponent();
            LoadRenterIds();
        }

        private void LoadRenterIds()
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

                string query = "SELECT RenterID FROM Renter";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RenterIdComboBox.Items.Add(reader["RenterID"]);
                }

                if (RenterIdComboBox.Items.Count > 0)
                {
                    RenterIdComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
            finally
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }
        }

        private void RenterIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidationMessage.Text = string.Empty;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RenterIdComboBox.SelectedItem == null)
            {
                ValidationMessage.Text = "Пожалуйста, выберите ID арендатора.";
                return;
            }

            int renterId = Convert.ToInt32(RenterIdComboBox.SelectedItem);

            string databasePath = @"C:\Users\d_sil\OneDrive\Рабочий стол\kurs\kurs\identifier.sqlite";
            string connectionString = $"Data Source={databasePath}";

            try
            {
                _connection = new SQLiteConnection(connectionString);
                _connection.Open();

                string query = "DELETE FROM Renter WHERE RenterID = @RenterID";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                command.Parameters.AddWithValue("@RenterID", renterId);
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Арендатор успешно удален.");
                    RenterIdComboBox.Items.Remove(renterId);

                    if (RenterIdComboBox.Items.Count > 0)
                    {
                        RenterIdComboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    MessageBox.Show("Арендатор с таким ID не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
            finally
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }
        }
    }
}
