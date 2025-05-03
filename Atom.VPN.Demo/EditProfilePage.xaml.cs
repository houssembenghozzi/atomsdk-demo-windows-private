using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;

namespace Atom.VPN.Demo
{
    public partial class EditProfilePage : Page
    {
        public EditProfilePage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
} 