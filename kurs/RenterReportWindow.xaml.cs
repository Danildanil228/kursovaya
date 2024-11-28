using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarRentalApp
{
    public partial class RenterReportWindow : Window
    {
        private SQLiteConnection _connection;

        public RenterReportWindow()
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

                string query = "SELECT RenterID, FirstName, LastName FROM Renter";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RenterComboBox.Items.Add(new Renter
                    {
                        RenterID = Convert.ToInt32(reader["RenterID"]),
                        FirstName = reader["FirstName"].ToString()
                        
                    });
                }

                RenterComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private void ShowRenterReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (RenterComboBox.SelectedItem is Renter selectedRenter)
            {
                LoadRenterReport(selectedRenter.RenterID);
            }
        }

        private void LoadRenterReport(int renterId)
        {
            RenterReportStackPanel.Children.Clear();

            string databasePath = @"C:\Users\d_sil\OneDrive\Рабочий стол\kurs\kurs\identifier.sqlite";
            string connectionString = $"Data Source={databasePath}";

            try
            {
                _connection = new SQLiteConnection(connectionString);
                _connection.Open();

                string renterQuery = @"SELECT FirstName, LastName, ContactInfo FROM Renter WHERE RenterID = @RenterID";
                SQLiteCommand renterCommand = new SQLiteCommand(renterQuery, _connection);
                renterCommand.Parameters.AddWithValue("@RenterID", renterId);
                SQLiteDataReader renterReader = renterCommand.ExecuteReader();

                if (renterReader.Read())
                {
                    TextBlock renterInfo = new TextBlock
                    {
                        Text = $"Email: {renterReader["LastName"]}\nКонтактная информация: {renterReader["ContactInfo"]}",
                        Margin = new Thickness(5),
                        FontSize = 16,
                        Foreground = Brushes.Black
                    };
                    RenterReportStackPanel.Children.Add(renterInfo);

                    string rentalQuery = @"SELECT c.Model, c.Make, r.StartDate, r.EndDate, r.Cost
                                           FROM Rental r
                                           JOIN Car c ON r.CarID = c.CarID
                                           WHERE r.RenterID = @RenterID";
                    SQLiteCommand rentalCommand = new SQLiteCommand(rentalQuery, _connection);
                    rentalCommand.Parameters.AddWithValue("@RenterID", renterId);
                    SQLiteDataReader rentalReader = rentalCommand.ExecuteReader();

                    while (rentalReader.Read())
                    {
                        TextBlock rentalInfo = new TextBlock
                        {
                            Text = $"Модель: {rentalReader["Model"]}\n"+
                                   $"Дата начала аренды: {rentalReader["StartDate"]}\n" +
                                   $"Дата окончания аренды: {rentalReader["EndDate"]}\n" +
                                   $"Стоимость: {rentalReader["Cost"]} руб.",
                            Margin = new Thickness(5),
                            FontSize = 14,
                            Foreground = Brushes.Black
                        };
                        RenterReportStackPanel.Children.Add(rentalInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }

    public class Renter
    {
        public int RenterID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
