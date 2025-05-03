using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;

namespace Atom.VPN.Demo
{
    public partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void EditProfileRow_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new EditProfilePage());
        }

        private void ChangePasswordRow_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new ChangePasswordPage());
        }
    }
} 