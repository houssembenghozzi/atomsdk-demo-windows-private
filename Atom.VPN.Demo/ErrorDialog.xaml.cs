using System.Windows;

namespace Atom.VPN.Demo
{
    public partial class ErrorDialog : Window
    {
        public ErrorDialog(string title, string message)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 