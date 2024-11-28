using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarRentalApp
{
    public partial class RentersReportWindow : Window
    {
        private SQLiteConnection _connection;

        public RentersReportWindow()
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

                string query = @"SELECT r.RenterID, r.FirstName, r.LastName, r.ContactInfo, c.Model, re.StartDate, re.EndDate, re.Cost
                                FROM Renter r
                                LEFT JOIN Rental re ON r.RentalID = re.RentalID
                                LEFT JOIN Car c ON re.CarID = c.CarID";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    StackPanel renterPanel = new StackPanel { Margin = new Thickness(5), Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48)) };

                    TextBlock renterInfo = new TextBlock
                    {
                        Text = $"Имя: {reader["FirstName"]} {reader["LastName"]}\n" +
                               $"Контактная информация: {reader["ContactInfo"]}\n" +
                               $"Модель: {reader["Model"]}\n" +
                               $"Дата начала аренды: {reader["StartDate"]}\n" +
                               $"Дата окончания аренды: {reader["EndDate"]}\n" +
                               $"Стоимость аренды: {reader["Cost"]} руб.",
                        Margin = new Thickness(5),
                        FontSize = 14,
                        Foreground = Brushes.White
                    };
                    renterPanel.Children.Add(renterInfo);

                    RenterStackPanel.Children.Add(renterPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }
}
