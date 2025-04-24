using System;
using System.Windows;
using System.Windows.Controls;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            
            // Setup password visibility toggle
            SetupPasswordVisibilityToggle();
            
            // Focus the email field when the page loads
            this.Loaded += (s, e) => EmailTextBox.Focus();
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
            try
            {
                // Get the parent window
                var parentWindow = Window.GetWindow(this);
                
                // Check what type of window we're in
                if (parentWindow is MainContainerWindow mainContainerWindow)
                {
                    // Navigate to the main VPN page using MainContainerWindow
                    mainContainerWindow.NavigateToMainVPNPage();
                }
                else if (parentWindow is LoginWindow loginWindow)
                {
                    // For LoginWindow, we need to create a new MainContainerWindow to show the main VPN page
                    var newMainWindow = new MainContainerWindow();
                    newMainWindow.Show();
                    
                    // Close the login window
                    loginWindow.Close();
                }
                else
                {
                    throw new InvalidOperationException($"Parent window is not a navigation window: {parentWindow?.GetType().Name ?? "null"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error signing in: {ex.Message}",
                                "Sign In Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the parent window
                var parentWindow = Window.GetWindow(this);
                
                // Check what type of window we're in
                if (parentWindow is MainContainerWindow mainContainerWindow)
                {
                    // Navigate to forgot password page using MainContainerWindow
                    mainContainerWindow.NavigateToForgotPasswordPage();
                }
                else if (parentWindow is LoginWindow loginWindow)
                {
                    // Navigate to forgot password page using LoginWindow
                    loginWindow.NavigateToForgotPasswordPage();
                }
                else
                {
                    throw new InvalidOperationException($"Parent window is not a navigation window: {parentWindow?.GetType().Name ?? "null"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Forgot Password screen: {ex.Message}", 
                                "Navigation Error", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the parent window
                var parentWindow = Window.GetWindow(this);
                
                // Check what type of window we're in
                if (parentWindow is MainContainerWindow mainContainerWindow)
                {
                    // Navigate to sign up page using MainContainerWindow
                    mainContainerWindow.NavigateToSignUpPage();
                }
                else if (parentWindow is LoginWindow loginWindow)
                {
                    // Navigate to sign up page using LoginWindow
                    loginWindow.NavigateToSignUpPage();
                }
                else
                {
                    throw new InvalidOperationException($"Parent window is not a navigation window: {parentWindow?.GetType().Name ?? "null"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Sign Up screen: {ex.Message}",
                                "Navigation Error",
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }
        }
    }
} 