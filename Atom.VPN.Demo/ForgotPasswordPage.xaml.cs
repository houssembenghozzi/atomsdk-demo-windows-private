using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for ForgotPasswordPage.xaml
    /// </summary>
    public partial class ForgotPasswordPage : Page
    {
        private readonly Regex _emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public ForgotPasswordPage()
        {
            InitializeComponent();
            Loaded += ForgotPasswordPage_Loaded;
        }

        private void ForgotPasswordPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus the email field when the page loads
            EmailTextBox.Focus();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate email format
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    MessageBox.Show("Please enter your email address.", "Email Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EmailTextBox.Focus();
                    return;
                }

                if (!_emailRegex.IsMatch(EmailTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EmailTextBox.Focus();
                    return;
                }

                // Disable inputs to prevent additional clicks
                EmailTextBox.IsEnabled = false;
                NextButton.IsEnabled = false;
                
                // Show success message directly in UI
                if (this.FindName("SuccessMessage") is TextBlock successMessage)
                {
                    // If we found the SuccessMessage TextBlock, show it
                    successMessage.Text = $"A password reset link has been sent to {EmailTextBox.Text}. Redirecting...";
                    successMessage.Visibility = Visibility.Visible;
                }
                
                // Use a timer to delay navigation so the user can see the success message
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1.5);
                timer.Tick += (s, args) => {
                    timer.Stop();
                    
                    // Get the parent window
                    var parentWindow = Window.GetWindow(this);
                    
                    // Check what type of window we're in
                    if (parentWindow is MainContainerWindow mainContainerWindow)
                    {
                        // Navigate to login page using MainContainerWindow
                        mainContainerWindow.NavigateToLoginPage();
                    }
                    else if (parentWindow is LoginWindow loginWindow)
                    {
                        // Navigate to login page using LoginWindow
                        loginWindow.NavigateToLoginPage();
                    }
                    else
                    {
                        MessageBox.Show("Could not navigate back to login page: Unknown parent window type.", 
                                       "Navigation Error", 
                                       MessageBoxButton.OK, 
                                       MessageBoxImage.Error);
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the parent window
                var parentWindow = Window.GetWindow(this);
                
                // Check what type of window we're in
                if (parentWindow is MainContainerWindow mainContainerWindow)
                {
                    // Navigate to login page using MainContainerWindow
                    mainContainerWindow.NavigateToLoginPage();
                }
                else if (parentWindow is LoginWindow loginWindow)
                {
                    // Navigate to login page using LoginWindow
                    loginWindow.NavigateToLoginPage();
                }
                else
                {
                    MessageBox.Show("Could not navigate back to login page: Unknown parent window type.", 
                                   "Navigation Error", 
                                   MessageBoxButton.OK, 
                                   MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 