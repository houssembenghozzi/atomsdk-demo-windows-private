using System;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Window
    {
        private TextBox passwordTextBox;
        
        public SignUpPage()
        {
            InitializeComponent();
            
            // Set focus to the first input field when the window loads
            this.Loaded += (s, e) => EmailTextBox.Focus();
            
            // Setup password visibility toggle
            SetupPasswordVisibilityToggle();
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
        
        private void SetupPasswordVisibilityToggle()
        {
            this.Loaded += (s, e) =>
            {
                // Initialize the text box field from the XAML element
                passwordTextBox = PasswordTextBox;
                
                // Get the show password button from password box template
                if (PasswordBox != null && PasswordBox.Template != null && 
                    PasswordBox.Template.FindName("ShowPasswordButton", PasswordBox) is Button showPasswordButton)
                {
                    showPasswordButton.Click += (sender, args) => 
                    {
                        // Show the text version and hide the password version
                        passwordTextBox.Text = PasswordBox.Password;
                        PasswordBox.Visibility = Visibility.Collapsed;
                        passwordTextBox.Visibility = Visibility.Visible;
                        passwordTextBox.Focus();
                        args.Handled = true;
                    };
                }

                // Get the hide password button from text box template
                if (passwordTextBox != null && passwordTextBox.Template != null && 
                    passwordTextBox.Template.FindName("HidePasswordButton", passwordTextBox) is Button hidePasswordButton)
                {
                    hidePasswordButton.Click += (sender, args) => 
                    {
                        // Show the password version and hide the text version
                        PasswordBox.Password = passwordTextBox.Text;
                        passwordTextBox.Visibility = Visibility.Collapsed;
                        PasswordBox.Visibility = Visibility.Visible;
                        PasswordBox.Focus();
                        args.Handled = true;
                    };
                }

                // Sync password changes
                PasswordBox.PasswordChanged += (sender, args) => 
                {
                    if (passwordTextBox.Visibility == Visibility.Visible)
                    {
                        passwordTextBox.Text = PasswordBox.Password;
                    }
                };

                passwordTextBox.TextChanged += (sender, args) => 
                {
                    if (PasswordBox.Visibility == Visibility.Visible)
                    {
                        PasswordBox.Password = passwordTextBox.Text;
                    }
                };
            };
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Get and validate input values
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            
            // If password is being shown in textbox, get it from there
            if (PasswordTextBox.Visibility == Visibility.Visible)
            {
                password = PasswordTextBox.Text;
            }
            
            bool acceptTerms = TermsCheckbox.IsChecked ?? false;

            // Validate email
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter your email address.", "Email Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a password.", "Password Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                FocusPassword();
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Password Too Short", MessageBoxButton.OK, MessageBoxImage.Warning);
                FocusPassword();
                return;
            }

            // Validate terms acceptance
            if (!acceptTerms)
            {
                MessageBox.Show("You must accept the terms and conditions to sign up.", "Terms Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                TermsCheckbox.Focus();
                return;
            }

            // In a real app, you would register the user here
            // For this demo, navigate to email verification
            try
            {
                EmailVerificationPage verificationPage = new EmailVerificationPage(email);
                verificationPage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void FocusPassword()
        {
            if (passwordTextBox.Visibility == Visibility.Visible)
                passwordTextBox.Focus();
            else
                PasswordBox.Focus();
        }

        private void TermsAndConditions_Click(object sender, RoutedEventArgs e)
        {
            // Display terms and conditions in a dialog
            MessageBox.Show("Terms and Conditions\n\n" +
                "By using Atom VPN, you agree to our terms of service and privacy policy. " +
                "Your data will be processed in accordance with our privacy policy, and you " +
                "consent to the collection and use of your information as described therein.",
                "Terms and Conditions", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PrivacyPolicy_Click(object sender, RoutedEventArgs e)
        {
            // Display privacy policy in a dialog
            MessageBox.Show("Privacy Policy\n\n" +
                "Atom VPN is committed to protecting your privacy. We collect minimal information " +
                "necessary to provide our services. We do not sell or share your personal data with " +
                "third parties except as required by law.",
                "Privacy Policy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the login page
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