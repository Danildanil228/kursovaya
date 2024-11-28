using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CarRentalApp
{
    public partial class DeleteCarByIdWindow : Window
    {
        private SQLiteConnection _connection;

        public DeleteCarByIdWindow()
        {
            InitializeComponent();
            LoadCarIds();
        }

        private void LoadCarIds()
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

                string query = "SELECT CarID FROM Car";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    CarIdComboBox.Items.Add(reader["CarID"]);
                }

                if (CarIdComboBox.Items.Count > 0)
                {
                    CarIdComboBox.SelectedIndex = 0;
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

        private void CarIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidationMessage.Text = string.Empty;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CarIdComboBox.SelectedItem == null)
            {
                ValidationMessage.Text = "Пожалуйста, выберите ID автомобиля.";
                return;
            }

            int carId = Convert.ToInt32(CarIdComboBox.SelectedItem);

            string databasePath = @"C:\Users\d_sil\OneDrive\Рабочий стол\kurs\kurs\identifier.sqlite";
            string connectionString = $"Data Source={databasePath}";

            try
            {
                _connection = new SQLiteConnection(connectionString);
                _connection.Open();

                string query = "DELETE FROM Car WHERE CarID = @CarID";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                command.Parameters.AddWithValue("@CarID", carId);
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Автомобиль успешно удален.");
                    CarIdComboBox.Items.Remove(carId);

                    if (CarIdComboBox.Items.Count > 0)
                    {
                        CarIdComboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    MessageBox.Show("Автомобиль с таким ID не найден.");
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
