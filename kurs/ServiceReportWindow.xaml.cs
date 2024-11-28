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
    public partial class ServiceReportWindow : Window
    {
        private SQLiteConnection _connection;

        public ServiceReportWindow()
        {
            InitializeComponent();
            LoadServiceData();
        }

        private void LoadServiceData()
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
                    SELECT 
                        c.CarID, c.Model, c.Make, c.Year, c.Status, c.Price,
                        m.FirstName, m.LastName, m.ContactInfo, m.ContactInfo as Email,
                        r.EndDate
                    FROM Car c
                    JOIN Manager m ON c.CarID = m.CarID
                    LEFT JOIN Rental r ON c.CarID = r.CarID";

                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    StackPanel servicePanel = new StackPanel
                    {
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48)),
                        Orientation = Orientation.Horizontal
                    };

                    int carId = Convert.ToInt32(reader["CarID"]);
                    string imageName = $"{carId}.jpg";
                    Image carImage = new Image
                    {
                        Width = 200,
                        Height = 160,
                        Margin = new Thickness(5),
                        Source = new BitmapImage(new Uri($"/Images/{imageName}", UriKind.Relative))
                    };
                    servicePanel.Children.Add(carImage);

                    StackPanel infoPanel = new StackPanel { Margin = new Thickness(5) };

                    TextBlock carInfo = new TextBlock
                    {
                        Text = $"Марка: {reader["Make"]}, Модель: {reader["Model"]}, Год: {reader["Year"]}, Цена за день: {reader["Price"]} руб.",
                        FontSize = 16,
                        Foreground = Brushes.White
                    };
                    infoPanel.Children.Add(carInfo);

                    TextBlock managerInfo = new TextBlock
                    {
                        Text = $"Менеджер: {reader["FirstName"]} {reader["LastName"]}, Email: {reader["Email"]}, Контактная информация: {reader["ContactInfo"]}",
                        FontSize = 16,
                        Foreground = Brushes.White
                    };
                    infoPanel.Children.Add(managerInfo);

                    if (reader["Status"].ToString() == "Занята")
                    {
                        TextBlock statusInfo = new TextBlock
                        {
                            Text = $"Статус: {reader["Status"]}, Дата окончания: {reader["EndDate"]}",
                            FontSize = 16,
                            Foreground = Brushes.White
                        };
                        infoPanel.Children.Add(statusInfo);
                    }

                    servicePanel.Children.Add(infoPanel);
                    ServiceStackPanel.Children.Add(servicePanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }
}
