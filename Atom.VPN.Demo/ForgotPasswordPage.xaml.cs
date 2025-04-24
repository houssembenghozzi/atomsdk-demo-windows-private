using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for ForgotPasswordPage.xaml
    /// </summary>
    public partial class ForgotPasswordPage : Window
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
            
            // Set focus to email textbox when the window loads
            this.Loaded += (s, e) => EmailTextBox.Focus();
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
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            // Basic validation for email input
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Please enter your email address.", "Email Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

            // Simple email validation
            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

            // Navigate to email verification page
            try
            {
                var verificationPage = new EmailVerificationPage(EmailTextBox.Text);
                verificationPage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to sign up page
            try
            {
                SignUpPage signUpPage = new SignUpPage();
                signUpPage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the login page
            try
            {
                LoginPage loginPage = new LoginPage();
                loginPage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private bool IsValidEmail(string email)
        {
            // Simple regex for email validation
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }
} 