using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarRentalApp
{
    public partial class DebtorsReportWindow : Window
    {
        private SQLiteConnection _connection;

        public DebtorsReportWindow()
        {
            InitializeComponent();
            LoadDebtorsData();
        }

        private void LoadDebtorsData()
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

                string query = @"
                    SELECT rt.FirstName, rt.LastName, rt.ContactInfo, c.Model, c.Make, c.Year, c.Price, r.StartDate, r.EndDate, r.Cost,
                           ((julianday(r.EndDate) - julianday(r.StartDate)) * c.Price) AS FullCost
                    FROM Rental r
                    JOIN Renter rt ON r.RenterID = rt.RenterID
                    JOIN Car c ON r.CarID = c.CarID
                    WHERE r.Cost < (julianday(r.EndDate) - julianday(r.StartDate)) * c.Price";

                SQLiteCommand command = new SQLiteCommand(query, _connection);
                SQLiteDataReader reader = command.ExecuteReader();

                StackPanel mainPanel = new StackPanel { Margin = new Thickness(10) };

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        StackPanel debtorPanel = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Margin = new Thickness(5),
                            Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48))
                        };

                        double fullCost = Convert.ToDouble(reader["FullCost"]);
                        double paidCost = Convert.ToDouble(reader["Cost"]);
                        double remainingCost = fullCost - paidCost;

                        TextBlock debtorInfo = new TextBlock
                        {
                            Text = $"Имя Фамилия: {reader["FirstName"]} {reader["LastName"]}\n" +
                                   $"Контактная информация: {reader["ContactInfo"]}\n" +
                                   $"Модель: {reader["Model"]}\n" +
                                   $"Марка: {reader["Make"]}\n" +
                                   $"Год: {reader["Year"]}\n" +
                                   $"Цена за день: {reader["Price"]} руб.\n" +
                                   $"Дата начала аренды: {reader["StartDate"]}\n" +
                                   $"Дата окончания аренды: {reader["EndDate"]}\n" +
                                   $"Стоимость: {reader["Cost"]} руб.\n" +
                                   $"Должен: {remainingCost} руб.",
                            Margin = new Thickness(5),
                            FontSize = 14,
                            Foreground = Brushes.White
                        };

                        debtorPanel.Children.Add(debtorInfo);
                        mainPanel.Children.Add(debtorPanel);
                    }
                }
                else
                {
                    TextBlock noDebtorsInfo = new TextBlock
                    {
                        Text = "Нет должников.",
                        Margin = new Thickness(5),
                        FontSize = 14,
                        Foreground = Brushes.White
                    };

                    mainPanel.Children.Add(noDebtorsInfo);
                }

                DebtorsScrollViewer.Content = mainPanel;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }
}
