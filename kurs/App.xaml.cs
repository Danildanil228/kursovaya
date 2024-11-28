using System.Configuration;
using System.Data;
using System.Windows;

namespace CarRentalApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoginWindow loginwindow = new LoginWindow();
            loginwindow.Show();
            //base.OnStartup(e);
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
        }
    }
}
