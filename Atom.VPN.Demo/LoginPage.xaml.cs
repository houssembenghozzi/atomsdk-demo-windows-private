using System;
using System.Windows;
using System.Windows.Controls;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private MainWindow mainWindow;
        
        public LoginPage()
        {
            InitializeComponent();
            
            // Handle the eye button in password box for showing/hiding password
            var passwordTemplate = PasswordBox.Template;
            var showPasswordButton = passwordTemplate.FindName("ShowPasswordButton", PasswordBox) as Button;
            if (showPasswordButton != null)
            {
                showPasswordButton.Click += ShowPasswordButton_Click;
            }
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // This would normally toggle password visibility, but WPF PasswordBox doesn't support this directly
            // In a real application, you'd need to implement a custom control or use a workaround
            MessageBox.Show("Password visibility toggle is not implemented in this demo.", "Feature Not Implemented", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void InputField_GotFocus(object sender, RoutedEventArgs e)
        {
            // Change border color and placeholder text to black when the input field gets focus
            if (sender is TextBox textBox)
            {
                var border = textBox.Template.FindName("border", textBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = System.Windows.Media.Brushes.Black;
                }
                
                var placeholder = textBox.Template.FindName("Placeholder", textBox) as TextBlock;
                if (placeholder != null)
                {
                    placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 52, 56));
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                var border = passwordBox.Template.FindName("border", passwordBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = System.Windows.Media.Brushes.Black;
                }
                
                var placeholder = passwordBox.Template.FindName("Placeholder", passwordBox) as TextBlock;
                if (placeholder != null)
                {
                    placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 52, 56));
                }
            }
        }

        private void InputField_LostFocus(object sender, RoutedEventArgs e)
        {
            // Change border color and placeholder text to gray when the input field loses focus
            if (sender is TextBox textBox)
            {
                var border = textBox.Template.FindName("border", textBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                }
                
                var placeholder = textBox.Template.FindName("Placeholder", textBox) as TextBlock;
                if (placeholder != null && string.IsNullOrEmpty(textBox.Text))
                {
                    placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                var border = passwordBox.Template.FindName("border", passwordBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                }
                
                var placeholder = passwordBox.Template.FindName("Placeholder", passwordBox) as TextBlock;
                if (placeholder != null && passwordBox.Password.Length == 0)
                {
                    placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                }
            }
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // For demo purposes, simply open the main window
            mainWindow = new MainWindow();
            mainWindow.SecretKey = "17355649429f7d4adbe993a8d227bc580c8f369b";
            
            // Set event handler to shut down app when main window is closed
            mainWindow.Closed += (s, args) => Application.Current.Shutdown();
            
            // Show the main window
            mainWindow.Show();
            
            // Close this login window
            this.Close();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Password recovery functionality is not implemented in this demo.", "Feature Not Implemented", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sign up functionality is not implemented in this demo.", "Feature Not Implemented", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
} 