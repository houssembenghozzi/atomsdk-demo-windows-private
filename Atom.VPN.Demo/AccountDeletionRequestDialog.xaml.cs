using System.Windows;

namespace Atom.VPN.Demo
{
    public partial class AccountDeletionRequestDialog : Window
    {
        public AccountDeletionRequestDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 