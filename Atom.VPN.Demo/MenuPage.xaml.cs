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
            try
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new AccountPage());
                    e.Handled = true;
                }
                else
                {
                    // Fallback if NavigationService is null
                    MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                    mainWindow?.NavigateToAccountPage();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error in AccountRow_MouseDown: {ex.Message}");
                // Prevent the exception from crashing the app
                e.Handled = true;
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
    }
} 