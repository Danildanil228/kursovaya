using System.Windows;
using System.Windows.Media;

namespace CarRentalApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            ResetStyles();

            if ((username == "admin") && password == "12345")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorMessage.Visibility = Visibility.Visible;
                PasswordBox.BorderBrush = Brushes.Red;
                PasswordBox.BorderThickness = new Thickness(2);
                PasswordBox.Foreground = Brushes.Red;
            }
        }

        private void ResetStyles()
        {
            PasswordBox.BorderBrush = Brushes.Gray;
            PasswordBox.BorderThickness = new Thickness(1);
            PasswordBox.Foreground = Brushes.Black;
            ErrorMessage.Visibility = Visibility.Collapsed;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!Application.Current.Windows.OfType<MainWindow>().Any())
            {
                Application.Current.Shutdown();
            }
            base.OnClosed(e);
        }
    }
}
