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
    public partial class AvailableCarsWindow : Window
    {
        private SQLiteConnection _connection;

        public AvailableCarsWindow()
        {
            InitializeComponent();
        }

        private void ShowAvailableCarsButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            if (startDate == null || endDate == null)
            {
                ValidationMessage.Text = "Пожалуйста, выберите начальную и конечную даты.";
                return;
            }

            if (startDate > endDate)
            {
                ValidationMessage.Text = "Начальная дата не может быть больше конечной даты.";
                return;
            }

            
            ValidationMessage.Text = string.Empty;
            LoadAvailableCars(startDate.Value, endDate.Value);
        }

        private void LoadAvailableCars(DateTime startDate, DateTime endDate)
        {
            CarStackPanel.Children.Clear();

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

                string query = @"SELECT c.CarID, c.Model, c.Make, c.Year, c.Price, m.FirstName, m.LastName, m.ContactInfo
                                FROM Car c
                                LEFT JOIN Rental r ON c.CarID = r.CarID AND (r.StartDate <= @EndDate AND r.EndDate >= @StartDate)
                                LEFT JOIN Manager m ON c.CarID = m.CarID
                                WHERE r.CarID IS NULL";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    StackPanel carPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(10),
                        Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48)),

                    };

                    int carId = (int)(long)reader["CarID"];
                    string imageName = $"{carId}.jpg";
                    Image carImage = new Image
                    {
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(5),
                        Source = new BitmapImage(new Uri($"/Images/{imageName}", UriKind.Relative))
                    };

                    Button showDetailsButton = new Button
                    {
                        Content = "Открыть\nхарактеристики",
                        Tag = carId,
                        Margin = new Thickness(5),
                        FontSize = 14,
                        Width = carImage.Width
                    };
                    showDetailsButton.Click += ShowDetailsButton_Click;

                    StackPanel imagePanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    imagePanel.Children.Add(carImage);
                    imagePanel.Children.Add(showDetailsButton);

                    TextBlock carInfo = new TextBlock
                    {
                        Text = $"Модель: {reader["Model"]}\n" +
                               $"Марка: {reader["Make"]}\n" +
                               $"Год: {reader["Year"]}\n" +
                               $"Цена за день: {reader["Price"]} руб.",
                        Margin = new Thickness(5),
                        FontSize = 14,
                        Foreground = Brushes.White
                    };

                    carPanel.Children.Add(imagePanel);
                    carPanel.Children.Add(carInfo);
                    CarStackPanel.Children.Add(carPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private void ShowDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int carId)
            {
                CarDetailsWindow carDetailsWindow = new CarDetailsWindow(carId);
                carDetailsWindow.Show();
            }
        }
    }
}
