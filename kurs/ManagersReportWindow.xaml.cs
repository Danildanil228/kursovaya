using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarRentalApp
{
    public partial class ManagersReportWindow : Window
    {
        private SQLiteConnection _connection;

        public ManagersReportWindow()
        {
            InitializeComponent();
            LoadManagersData();
        }

        private void LoadManagersData()
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
                        m.FirstName, m.LastName, m.ContactInfo, 
                        c.Model, c.Make, c.Year, c.Price
                    FROM Manager m
                    LEFT JOIN Car c ON m.CarID = c.CarID";

                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    StackPanel managerPanel = new StackPanel
                    {
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48)),
                        Orientation = Orientation.Vertical
                    };

                    Grid infoGrid = new Grid();
                    infoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    infoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    infoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    TextBlock managerInfo = new TextBlock
                    {
                        Text = $"Имя: {reader["FirstName"]}\nEmail: {reader["LastName"]}\n" +
                               $"Телефон: {reader["ContactInfo"]}",
                        FontSize = 16,
                        Foreground = Brushes.White,
                        Margin = new Thickness(5)
                    };
                    Grid.SetRow(managerInfo, 0);
                    infoGrid.Children.Add(managerInfo);

                    TextBlock carInfo = new TextBlock
                    {
                        Text = $"Автомобиль: {reader["Make"]} {reader["Model"]} {reader["Year"]}\n",
                        FontSize = 16,
                        Foreground = Brushes.White,
                        Margin = new Thickness(5)
                    };
                    Grid.SetRow(carInfo, 1);
                    infoGrid.Children.Add(carInfo);

                    managerPanel.Children.Add(infoGrid);
                    ManagersStackPanel.Children.Add(managerPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }
}
