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
            
            // Setup password visibility toggle
            SetupPasswordVisibilityToggle();
        }

        private void SetupPasswordVisibilityToggle()
        {
            // Get the show password button from password box template
            if (PasswordBox.Template.FindName("ShowPasswordButton", PasswordBox) is Button showPasswordButton)
            {
                showPasswordButton.Click += (s, e) => 
                {
                    // Show the text version and hide the password version
                    PasswordTextBox.Text = PasswordBox.Password;
                    PasswordBox.Visibility = Visibility.Collapsed;
                    PasswordTextBox.Visibility = Visibility.Visible;
                    PasswordTextBox.Focus();
                    e.Handled = true;
                };
            }

            // Get the hide password button from text box template
            if (PasswordTextBox.Template.FindName("HidePasswordButton", PasswordTextBox) is Button hidePasswordButton)
            {
                hidePasswordButton.Click += (s, e) => 
                {
                    // Show the password version and hide the text version
                    PasswordBox.Password = PasswordTextBox.Text;
                    PasswordTextBox.Visibility = Visibility.Collapsed;
                    PasswordBox.Visibility = Visibility.Visible;
                    PasswordBox.Focus();
                    e.Handled = true;
                };
            }

            // Sync password changes
            PasswordBox.PasswordChanged += (s, e) => 
            {
                if (PasswordTextBox.Visibility == Visibility.Visible)
                {
                    PasswordTextBox.Text = PasswordBox.Password;
                }
            };

            PasswordTextBox.TextChanged += (s, e) => 
            {
                if (PasswordBox.Visibility == Visibility.Visible)
                {
                    PasswordBox.Password = PasswordTextBox.Text;
                }
            };
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