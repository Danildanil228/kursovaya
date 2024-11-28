using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CarRentalApp
{
    public partial class ServiceCostWindow : Window
    {
        string DatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "identifier.sqlite");


        public ServiceCostWindow()
        {
            InitializeComponent();
            LoadCars();
        }

        private void LoadCars()
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={DatabasePath}"))
                {
                    connection.Open();
                    string query = "SELECT CarID, Make, Model FROM Car";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int carId = Convert.ToInt32(reader["CarID"]);
                        string make = reader["Make"].ToString();
                        string model = reader["Model"].ToString();

                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = $"{make} {model}",
                            Tag = carId
                        };

                        CarComboBox.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private void CalculateCostButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out int carId, out DateTime startDate, out DateTime endDate))
            {
                return;
            }

            CarImage.Source = new BitmapImage(new Uri($"/Images/{carId}.jpg", UriKind.Relative));

            if (IsCarOccupied(carId, startDate, endDate, out DateTime? availableDate))
            {
                ShowErrorMessage($"Автомобиль занят в выбранный период.\nДоступен с: {availableDate?.ToShortDateString()}");
                return;
            }

            TimeSpan rentalDuration = endDate - startDate;
            int totalDays = (int)rentalDuration.TotalDays + 1;

            try
            {
                using (var connection = new SQLiteConnection($"Data Source={DatabasePath}"))
                {
                    connection.Open();

                    string query = @"SELECT Model, Make, Price FROM Car WHERE CarID = @CarID";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@CarID", carId);
                    SQLiteDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string model = reader["Model"].ToString();
                        string make = reader["Make"].ToString();
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        decimal totalCost = totalDays * price;

                        ResultTextBlock.Text = $"Автомобиль: {make} {model}\n" +
                                               $"Дата начала: {startDate.ToShortDateString()}\n" +
                                               $"Дата окончания: {endDate.ToShortDateString()}\n" +
                                               $"Итоговая стоимость: {totalCost} руб.";

                        ResultPanel.Visibility = Visibility.Visible;
                        ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private bool ValidateInputs(out int carId, out DateTime startDate, out DateTime endDate)
        {
            carId = 0;
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            if (CarComboBox.SelectedItem is not ComboBoxItem selectedItem || selectedItem.Tag is not int selectedCarId)
            {
                ShowErrorMessage("Пожалуйста, выберите автомобиль.");
                return false;
            }

            if (!StartDatePicker.SelectedDate.HasValue)
            {
                ShowErrorMessage("Пожалуйста, выберите начальную дату.");
                return false;
            }

            if (!EndDatePicker.SelectedDate.HasValue)
            {
                ShowErrorMessage("Пожалуйста, выберите конечную дату.");
                return false;
            }

            startDate = StartDatePicker.SelectedDate.Value;
            endDate = EndDatePicker.SelectedDate.Value;

            if (endDate < startDate)
            {
                ShowErrorMessage("Конечная дата не может быть раньше начальной даты.");
                return false;
            }

            carId = selectedCarId;
            return true;
        }

        private bool IsCarOccupied(int carId, DateTime startDate, DateTime endDate, out DateTime? availableDate)
        {
            availableDate = null;

            try
            {
                using (var connection = new SQLiteConnection($"Data Source={DatabasePath}"))
                {
                    connection.Open();

                    string query = @"
                        SELECT EndDate FROM Rental 
                        WHERE CarID = @CarID AND ((@StartDate BETWEEN StartDate AND EndDate) OR (@EndDate BETWEEN StartDate AND EndDate) OR (StartDate BETWEEN @StartDate AND @EndDate))";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@CarID", carId);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    SQLiteDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        availableDate = Convert.ToDateTime(reader["EndDate"]).AddDays(1);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка при проверке занятости автомобиля: " + ex.Message);
            }

            return false;
        }

        private void ShowErrorMessage(string message)
        {
            ErrorMessageTextBlock.Text = message;
            ErrorMessageTextBlock.Visibility = Visibility.Visible;
        }

        private void CarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            ResultPanel.Visibility = Visibility.Collapsed;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            ResultPanel.Visibility = Visibility.Collapsed;
        }
    }
}
