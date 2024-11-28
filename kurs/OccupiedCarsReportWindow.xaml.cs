using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CarRentalApp
{
    public partial class OccupiedCarsReportWindow : Window
    {
        private SQLiteConnection _connection;

        public OccupiedCarsReportWindow()
        {
            InitializeComponent();
            LoadOccupiedCars();
        }

        private void LoadOccupiedCars()
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

                string query = @"
                    SELECT c.CarID, c.Model, c.Make, c.Year, c.Price, r.EndDate 
                    FROM Car c
                    JOIN Rental r ON c.CarID = r.CarID
                    WHERE c.Status = 'Занята'";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    StackPanel carPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

                    int carId = (int)(long)reader["CarID"];
                    string imageName = $"{carId}.jpg";
                    Image carImage = new Image
                    {
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(5),
                        Source = new BitmapImage(new Uri($"/Images/{imageName}", UriKind.Relative))
                    };
                    carPanel.Children.Add(carImage);

                    TextBlock carInfo = new TextBlock
                    {
                        Text = $"Модель: {reader["Model"]}\n" +
                               $"Марка: {reader["Make"]}\n" +
                               $"Год: {reader["Year"]}\n" +
                               $"Цена за день: {reader["Price"]} руб.\n" +
                               $"Дата окончания аренды: {Convert.ToDateTime(reader["EndDate"]).ToShortDateString()}",
                        Margin = new Thickness(5),
                        FontSize = 14,
                        Foreground = Brushes.White
                    };
                    carPanel.Children.Add(carInfo);

                    CarStackPanel.Children.Add(carPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }
}
