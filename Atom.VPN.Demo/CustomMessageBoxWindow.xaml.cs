using System;
using System.Windows;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for CustomMessageBoxWindow.xaml
    /// </summary>
    public partial class CustomMessageBoxWindow : Window
    {
        public CustomMessageBoxWindow(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        
        public static void Show(string message)
        {
            CustomMessageBoxWindow window = new CustomMessageBoxWindow(message);
            window.ShowDialog();
        }
        
        public static void Show(string message, string title)
        {
            CustomMessageBoxWindow window = new CustomMessageBoxWindow(message);
            window.Title = title;
            window.ShowDialog();
        }
    }
} 