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
    public partial class CarReportWindow : Window
    {
        private SQLiteConnection _connection;

        public CarReportWindow()
        {
            InitializeComponent();
            LoadCarData();
        }

        private void LoadCarData()
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

                string query = @"SELECT CarID, Model, Make, Year, Status, Price FROM Car";
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Grid carGrid = new Grid
                    {
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48)),
                        
                    };
                    carGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    carGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

                    int carId = Convert.ToInt32(reader["CarID"]);
                    string imageName = $"{carId}.jpg";
                    StackPanel imagePanel = new StackPanel
                    {
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Image carImage = new Image
                    {
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(5),
                        Source = new BitmapImage(new Uri($"/Images/{imageName}", UriKind.Relative))
                    };
                    Button showDetailsButton = new Button
                    {
                        Content = "Детали",
                        Tag = carId,
                        Margin = new Thickness(5),
                        Width = 100,
                        Height= 30,
                        Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48)),
                        Foreground = Brushes.White
                    };
                    showDetailsButton.Click += ShowDetailsButton_Click;

                    imagePanel.Children.Add(carImage);
                    imagePanel.Children.Add(showDetailsButton);
                    Grid.SetColumn(imagePanel, 0);
                    carGrid.Children.Add(imagePanel);

                    StackPanel carInfoPanel = new StackPanel
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    TextBlock carInfo = new TextBlock
                    {
                        Text = $"ID: {reader["CarID"]}\nМодель: {reader["Model"]}\nМарка: {reader["Make"]}\nГод: {reader["Year"]}\nСтатус: {reader["Status"]}\nЦена: {reader["Price"]} руб.",
                        Margin = new Thickness(5),
                        FontSize = 16,
                        Foreground = Brushes.White
                    };
                    carInfoPanel.Children.Add(carInfo);
                    Grid.SetColumn(carInfoPanel, 1);
                    carGrid.Children.Add(carInfoPanel);

                    CarUniformGrid.Children.Add(carGrid);
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
