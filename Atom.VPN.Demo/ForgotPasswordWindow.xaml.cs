using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using Atom.VPN.Demo.Helpers;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        // Flag to track if window was manually closed
        private bool isClosingManually = false;

        public ForgotPasswordWindow()
        {
            try
            {
                InitializeComponent();
                
                // Handle window closing event
                this.Closed += ForgotPasswordWindow_Closed;
                
                // Set this window as the application's main window temporarily
                // This prevents the application from closing when no main window is active
                Application.Current.MainWindow = this;
                
                // Focus the email field when the window loads
                this.Loaded += (s, e) => 
                {
                    // Ensure window is positioned at the center of the screen
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    EmailTextBox.Focus();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Forgot Password window: {ex.Message}", 
                                "Initialization Error", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }
        }

        private void ForgotPasswordWindow_Closed(object sender, EventArgs e)
        {
            // Only execute this if the window wasn't closed by our own code
            if (!isClosingManually)
            {
                try
                {
                    // If the window is closed unexpectedly, make sure we go back to the login page
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    loginWindow.Show();
                    
                    // Update the application's main window
                    Application.Current.MainWindow = loginWindow;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error returning to login page: {ex.Message}", 
                                    "Navigation Error", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Error);
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear any previous error message
            EmailErrorMessage.Visibility = Visibility.Collapsed;
            
            // Validate email address
            string email = EmailTextBox.Text.Trim();
            
            // Check if valid email 
            bool isValid = !string.IsNullOrWhiteSpace(email) && 
                           email.Contains("@") && 
                           email.Contains(".");
                           
            if (!isValid)
            {
                // Show error message instead of MessageBox
                EmailErrorMessage.Text = "Please enter a valid email address.";
                EmailErrorMessage.Visibility = Visibility.Visible;
                EmailTextBox.Focus();
                return;
            }

            // IMPORTANT: Completely bypass any message display
            
            // We don't want any more UI interaction, so immediately
            // return to login without showing any messages
            isClosingManually = true;
            BackToLogin();
        }

        private void CloseMessageBox_Click(object sender, RoutedEventArgs e)
        {
            // Hide the custom message box
            CustomMessageBoxUI.Visibility = Visibility.Collapsed;
            
            // Return to login page
            BackToLogin();
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Just call the BackToLogin method directly
            // It already has error handling
            BackToLogin();
        }

        private void BackToLogin()
        {
            try
            {
                // Mark that we're handling the closing ourselves
                isClosingManually = true;
                
                // Create a new login window first
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                
                // Show it immediately
                loginWindow.Show();
                
                // Make it the application's main window
                Application.Current.MainWindow = loginWindow;
                
                // Close this window immediately
                this.Close();
            }
            catch (Exception)
            {
                // Reset flag if there's an error
                isClosingManually = false;
                
                // Show error in UI
                EmailErrorMessage.Text = "Unable to return to login screen. Please try again.";
                EmailErrorMessage.Visibility = Visibility.Visible;
            }
        }
    }

    // Helper extension method to find child elements of a specific type
    public static class VisualTreeHelperExtensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
} 