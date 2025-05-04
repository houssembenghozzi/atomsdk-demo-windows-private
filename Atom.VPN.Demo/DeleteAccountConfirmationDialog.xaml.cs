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
            
            // Show account deletion request confirmation dialog
            AccountDeletionRequestDialog requestDialog = new AccountDeletionRequestDialog();
            requestDialog.Owner = this.Owner; // Set the same owner
            this.Close();
            requestDialog.ShowDialog();
        }
    }
} 