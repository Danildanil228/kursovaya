using System.Windows;

namespace CarRentalApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Cars_Click(object sender, RoutedEventArgs e)
        {
            CarsWindow carsWindow = new CarsWindow();
            carsWindow.Show();
        }

        private void Renters_Click(object sender, RoutedEventArgs e)
        {
            RentersWindow rentersWindow = new RentersWindow();
            rentersWindow.Show();
        }

        private void Managers_Click(object sender, RoutedEventArgs e)
        {
            ManagersWindow managersWindow = new ManagersWindow();
            managersWindow.Show();
        }

        private void Rentals_Click(object sender, RoutedEventArgs e)
        {
            RentalsWindow rentalsWindow = new RentalsWindow();
            rentalsWindow.Show();
        }
        private void OpenQueriesWindow_Click(object sender, RoutedEventArgs e)
        {
            QueriesWindow queriesWindow = new QueriesWindow();
            queriesWindow.Show();
        }
        private void CarReport_Click(object sender, RoutedEventArgs e)
        {
            CarReportWindow carReportWindow = new CarReportWindow();
            carReportWindow.Show();
        }
        private void ServiceReport_Click(object sender, RoutedEventArgs e)
        {
            ServiceReportWindow serviceReportWindow = new ServiceReportWindow();
            serviceReportWindow.Show();
        }
        private void RentersReport_Click(object sender, RoutedEventArgs e)
        {
            RentersReportWindow rentersReportWindow = new RentersReportWindow();
            rentersReportWindow.Show();
        }
        private void ManagersReport_Click(object sender, RoutedEventArgs e)
        {
            ManagersReportWindow managersReportWindow = new ManagersReportWindow();
            managersReportWindow.Show();
        }
        private void FreeCarsReport_Click(object sender, RoutedEventArgs e)
        {
            FreeCarsReportWindow freeCarsReportWindow = new FreeCarsReportWindow();
            freeCarsReportWindow.Show();
        }
        private void ServiceCostReport_Click(object sender, RoutedEventArgs e)
        {
            ServiceCostWindow serviceCostWindow = new ServiceCostWindow();
            serviceCostWindow.Show();
        }
        private void OccupiedCarsReport_Click(object sender, RoutedEventArgs e)
        {
            OccupiedCarsReportWindow occupiedCarsReportWindow = new OccupiedCarsReportWindow();
            occupiedCarsReportWindow.Show();
        }
        private void AvailableCarsReport_Click(object sender, RoutedEventArgs e)
        {
            AvailableCarsWindow availableCarsWindow = new AvailableCarsWindow();
            availableCarsWindow.Show();
        }
        private void RenterReport_Click(object sender, RoutedEventArgs e)
        {
            RenterReportWindow renterReportWindow = new RenterReportWindow();
            renterReportWindow.Show();
        }
        private void DebtorsReport_Click(object sender, RoutedEventArgs e)
        {
            DebtorsReportWindow debtorsReportWindow = new DebtorsReportWindow();
            debtorsReportWindow.Show();
        }
        private void DeleteRenterById_Click(object sender, RoutedEventArgs e)
        {
            DeleteRenterByIdWindow deleteRenterByIdWindow = new DeleteRenterByIdWindow();
            deleteRenterByIdWindow.Show();
        }
        private void DeleteCarMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteCarByIdWindow deleteCarByIdWindow = new DeleteCarByIdWindow();
            deleteCarByIdWindow.Show();
        }
        private void UpdateRenterContactInfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateRenterContactInfoWindow updateRenterContactInfoWindow = new UpdateRenterContactInfoWindow();
            updateRenterContactInfoWindow.Show();
        }
        private void UpdateCarStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateCarStatusWindow updateCarStatusWindow = new UpdateCarStatusWindow();
            updateCarStatusWindow.Show();
        }
    }
}