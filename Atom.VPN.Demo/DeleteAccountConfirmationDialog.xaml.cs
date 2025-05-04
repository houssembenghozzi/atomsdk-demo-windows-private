using System.Windows;

namespace Atom.VPN.Demo
{
    public partial class DeleteAccountConfirmationDialog : Window
    {
        public bool UserConfirmedDeletion { get; private set; } = false;

        public DeleteAccountConfirmationDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmedDeletion = false;
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmedDeletion = true;
            this.Close();
        }
    }
} 