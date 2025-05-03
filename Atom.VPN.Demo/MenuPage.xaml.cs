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
            NavigationService?.Navigate(new AccountPage());
            e.Handled = true;
        }
    }
} 