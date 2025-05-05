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

        private void ManageSessionsRow_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new ManageSessionsPage());
        }

        private void DeleteAccountRow_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DeleteAccountConfirmationDialog dialog = new DeleteAccountConfirmationDialog();
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();

            if (dialog.UserConfirmedDeletion)
            {
                // In a real application, you would delete the account here
                MessageBox.Show("Your account has been deleted.", "Account Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Navigate back to login page
                // This is a simplified example - in a real app you would properly sign out and clear credentials
                if (NavigationService != null)
                {
                    while (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
            }
        }
    }
} 