using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CarRentalApp
{
    public partial class CarDetailsWindow : Window
    {
        private int _carId;

        public CarDetailsWindow(int carId)
        {
            InitializeComponent();
            _carId = carId;
            LoadCarDetails();
        }

        private void LoadCarDetails() 
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
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT Model, Make, Year, Status, Price FROM Car WHERE CarID = @CarID";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@CarID", _carId);
                    SQLiteDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        var carDetails = new Dictionary<int, (int Mileage, int Accidents, string FuelType, string Transmission, int Horsepower, int Torque, int NumberOfDoors, int NumberOfSeats, string Color)>
                        {
                            { 3, (50000, 2, "Бензин", "Автоматическая", 220, 300, 4, 5, "Белый") },
                            { 4, (60000, 1, "Бензин", "Автоматическая", 250, 320, 4, 5, "Черный") },
                            { 5, (70000, 0, "Бензин", "Механическая", 200, 280, 4, 5, "Серый") },
                            { 6, (55000, 1, "Дизель", "Автоматическая", 190, 270, 4, 5, "Красный") },
                            { 7, (80000, 3, "Бензин", "Автоматическая", 240, 310, 4, 5, "Синий") },
                            { 8, (45000, 0, "Бензин", "Механическая", 210, 290, 4, 5, "Зеленый") },
                            { 9, (90000, 2, "Бензин", "Автоматическая", 230, 310, 4, 5, "Черный") },
                            { 10, (30000, 1, "Бензин", "Автоматическая", 180, 260, 4, 5, "Белый") },
                            { 11, (60000, 2, "Дизель", "Механическая", 200, 280, 4, 5, "Серый") },
                            { 12, (50000, 1, "Бензин", "Автоматическая", 220, 300, 4, 5, "Красный") }
                        };

                        var details = carDetails[_carId];

                        string carInfo = $"Модель: {reader["Model"]}\n" +
                                         $"Марка: {reader["Make"]}\n" +
                                         $"Год: {reader["Year"]}\n" +
                                         $"Статус: {reader["Status"]}\n" +
                                         $"Цена: {reader["Price"]} руб.\n" +
                                         $"Пробег: {details.Mileage} км\n" +
                                         $"Количество аварий: {details.Accidents}\n" +
                                         $"Тип топлива: {details.FuelType}\n" +
                                         $"Коробка передач: {details.Transmission}\n" +
                                         $"Лошадиные силы: {details.Horsepower}\n" +
                                         $"Крутящий момент: {details.Torque} Нм\n" +
                                         $"Количество дверей: {details.NumberOfDoors}\n" +
                                         $"Количество мест: {details.NumberOfSeats}\n" +
                                         $"Цвет: {details.Color}";

                        if (reader["Status"].ToString() == "Занята")
                        {
                            string rentalQuery = @"SELECT EndDate FROM Rental WHERE CarID = @CarID ORDER BY EndDate DESC LIMIT 1";
                            SQLiteCommand rentalCommand = new SQLiteCommand(rentalQuery, connection);
                            rentalCommand.Parameters.AddWithValue("@CarID", _carId);
                            var endDate = rentalCommand.ExecuteScalar();
                            carInfo += $"\nДата освобождения: {endDate}";
                        }

                        CarDetailsTextBlock.Text = carInfo;

                        string imageName = $"{_carId}.jpg";
                        CarImage.Source = new BitmapImage(new Uri($"/Images/{imageName}", UriKind.Relative));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
