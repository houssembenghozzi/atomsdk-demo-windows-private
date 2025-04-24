using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for ResetPasswordPage.xaml
    /// </summary>
    public partial class ResetPasswordPage : Window
    {
        private string userEmail;
        private TextBox PasswordTextBox;
        private TextBox ConfirmPasswordTextBox;
        
        public ResetPasswordPage(string email)
        {
            InitializeComponent();
            userEmail = email;
            
            // Set focus to password field when the window loads
            this.Loaded += (s, e) => PasswordBox.Focus();
            
            // Display the email for which we're resetting the password
            EmailInfoTextBlock.Text = $"Reset password for {email}";
            
            // Setup password visibility toggle
            SetupPasswordVisibilityToggle();
        }
        
        private void SetupPasswordVisibilityToggle()
        {
            // Create TextBoxes for password display
            PasswordTextBox = new TextBox();
            PasswordTextBox.Visibility = Visibility.Collapsed;
            
            ConfirmPasswordTextBox = new TextBox();
            ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
            
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
            
            // Handle the same for confirm password box if needed
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            
            // If the password is being shown in a textbox, get it from there
            if (PasswordTextBox != null && PasswordTextBox.Visibility == Visibility.Visible)
            {
                password = PasswordTextBox.Text;
            }
            
            // Validate password
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a new password.", "Password Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                FocusPassword();
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Password Too Short", MessageBoxButton.OK, MessageBoxImage.Warning);
                FocusPassword();
                return;
            }

            // Validate password confirmation
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Password Mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
                ConfirmPasswordBox.Focus();
                return;
            }

            // In a real app, you would save the new password here
            MessageBox.Show("Your password has been reset successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Navigate to login page
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        
        private void FocusPassword()
        {
            if (PasswordTextBox != null && PasswordTextBox.Visibility == Visibility.Visible)
                PasswordTextBox.Focus();
            else
                PasswordBox.Focus();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Go back to forgot password page
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.Show();
            this.Close();
        }
    }
} 