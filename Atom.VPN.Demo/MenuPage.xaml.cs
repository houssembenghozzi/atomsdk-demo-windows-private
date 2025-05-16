using Atom.VPN.Demo;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void AccountRow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true; // Mark as handled immediately
            try
            {
                MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                if (mainWindow != null)
                {
                    mainWindow.NavigateToAccountPage();
                }
                else if (NavigationService != null)
                {
                    NavigationService.Navigate(new AccountPage());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error in AccountRow_MouseDown: {ex.Message}");
                // Optionally, reconsider if e.Handled should be false here if an error occurs before navigation
            }
        }

        private void SettingsRow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new SettingsPage());
                    e.Handled = true;
                }
                else
                {
                    // Fallback if NavigationService is null
                    MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                    mainWindow?.NavigateToSettingsPage();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                // Prevent the exception from crashing the app
                e.Handled = true;
            }
        }

        private void HelpCenterRow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new HelpCenterPage());
                    e.Handled = true;
                }
                else
                {
                    // Fallback if NavigationService is null
                    MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                    mainWindow?.NavigateToHelpCenterPage();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error in HelpCenterRow_MouseDown: {ex.Message}");
                // Prevent the exception from crashing the app
                e.Handled = true;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the parent window
                var mainWindow = Window.GetWindow(this) as MainContainerWindow;
                
                if (mainWindow != null)
                {
                    // Call the logout method
                    mainWindow.LogOut();
                }
                else
                {
                    MessageBox.Show("Unable to find the main window to log out.", 
                                   "Logout Error", 
                                   MessageBoxButton.OK, 
                                   MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error logging out: {ex.Message}", 
                               "Logout Error", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Error);
            }
        }
    }
} 