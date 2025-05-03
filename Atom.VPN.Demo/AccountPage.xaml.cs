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
    }
} 