using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CarRentalApp
{
    public partial class UpdateRenterContactInfoWindow : Window
    {
        private SQLiteConnection _connection;

        public UpdateRenterContactInfoWindow()
        {
            InitializeComponent();
            LoadRenters();
        }

        private void LoadRenters()
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

                string query = "SELECT RenterID, FirstName FROM Renter";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RenterComboBox.Items.Add(new ComboBoxItem { Content = reader["FirstName"], Tag = (long)reader["RenterID"] });
                }

                if (RenterComboBox.Items.Count > 0)
                {
                    RenterComboBox.SelectedIndex = 0;
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

        private void RenterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidationMessage.Text = string.Empty;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (RenterComboBox.SelectedItem == null)
            {
                ValidationMessage.Text = "Пожалуйста, выберите арендатора.";
                return;
            }

            if (string.IsNullOrEmpty(NewContactInfoTextBox.Text))
            {
                ValidationMessage.Text = "Пожалуйста, введите новую контактную информацию.";
                return;
            }

            long renterId = (long)((ComboBoxItem)RenterComboBox.SelectedItem).Tag;
            string newContactInfo = NewContactInfoTextBox.Text;

            string databasePath = @"C:\Users\d_sil\OneDrive\Рабочий стол\kurs\kurs\identifier.sqlite";
            string connectionString = $"Data Source={databasePath}";

            try
            {
                _connection = new SQLiteConnection(connectionString);
                _connection.Open();

                string query = "UPDATE Renter SET ContactInfo = @ContactInfo WHERE RenterID = @RenterID";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                command.Parameters.AddWithValue("@ContactInfo", newContactInfo);
                command.Parameters.AddWithValue("@RenterID", renterId);
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Контактная информация успешно обновлена.");
                    NewContactInfoTextBox.Clear();
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
