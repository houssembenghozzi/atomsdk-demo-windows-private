using System;
using System.Windows;
using Microsoft.Win32;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for AddShortcutDialog.xaml
    /// </summary>
    public partial class AddShortcutDialog : Window
    {
        public string ShortcutName { get; private set; }
        public string ShortcutPath { get; private set; }
        
        public AddShortcutDialog()
        {
            InitializeComponent();
            
            // Set focus to name field
            Loaded += (s, e) => NameTextBox.Focus();
        }
        
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Application (*.exe)|*.exe|All files (*.*)|*.*",
                Title = "Select Application"
            };
            
            if (dialog.ShowDialog() == true)
            {
                PathTextBox.Text = dialog.FileName;
            }
        }
        
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ShortcutName = NameTextBox.Text?.Trim();
            ShortcutPath = PathTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(ShortcutName))
            {
                MessageBox.Show("Please enter a name for the shortcut.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }
            
            if (string.IsNullOrEmpty(ShortcutPath))
            {
                MessageBox.Show("Please enter a path or URL for the shortcut.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                PathTextBox.Focus();
                return;
            }
            
            // URL validation is simplified here
            if (ShortcutPath.StartsWith("http") && !ShortcutPath.Contains("://"))
            {
                MessageBox.Show("Please enter a valid URL starting with http:// or https://", "Invalid URL", MessageBoxButton.OK, MessageBoxImage.Warning);
                PathTextBox.Focus();
                return;
            }
            
            DialogResult = true;
            Close();
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 