using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarRentalApp
{
    public partial class UpdateCarStatusWindow : Window
    {
        private SQLiteConnection _connection;

        public UpdateCarStatusWindow()
        {
            InitializeComponent();
            LoadCarIds();
        }

        private void LoadCarIds()
        {
            string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "identifier.sqlite");
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

                string query = "SELECT CarID, Make, Model FROM Car";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    CarComboBox.Items.Add(new
                    {
                        CarID = (long)reader["CarID"],
                        CarDisplay = $"{reader["Make"]} {reader["Model"]} (ID: {reader["CarID"]})"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (CarComboBox.SelectedValue == null || StatusComboBox.SelectedItem == null)
            {
                ValidationMessage.Text = "Пожалуйста, выберите автомобиль и статус.";
                return;
            }

            string selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            if (selectedStatus != "Свободна" && selectedStatus != "Занята")
            {
                ValidationMessage.Text = "Недопустимый статус. Выберите 'Свободна' или 'Занята'.";
                return;
            }

            long carId = (long)CarComboBox.SelectedValue; 
            string databasePath = @"C:\Users\d_sil\OneDrive\Рабочий стол\kurs\kurs\identifier.sqlite";
            string connectionString = $"Data Source={databasePath}";

            try
            {
                _connection = new SQLiteConnection(connectionString);
                _connection.Open();

                string query = "UPDATE Car SET Status = @Status WHERE CarID = @CarID";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                command.Parameters.AddWithValue("@Status", selectedStatus);
                command.Parameters.AddWithValue("@CarID", carId);
                command.ExecuteNonQuery();

                ValidationMessage.Text = "Статус автомобиля обновлен.";
                ValidationMessage.Foreground = new SolidColorBrush(Colors.Green);
            }
            catch (Exception ex)
            {
                ValidationMessage.Text = "Ошибка при обновлении статуса: " + ex.Message;
                ValidationMessage.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
