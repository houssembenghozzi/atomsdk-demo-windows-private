using System;
using System.Windows;
using System.Windows.Controls;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// A Window that hosts the LoginPage in a frame
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Frame _mainFrame;
        
        public LoginWindow()
        {
            InitializeComponent();
            
            // Set this window as the application's main window
            Application.Current.MainWindow = this;
            
            // Create and set up the frame
            _mainFrame = new Frame
            {
                NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden
            };
            
            // Add the frame to the window content
            this.Content = _mainFrame;
            
            // Navigate to the login page
            _mainFrame.Navigate(new LoginPage());
            
            // Set window properties
            this.Title = "Atom VPN - Login";
            this.Height = 700;
            this.Width = 460;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;
            this.Background = System.Windows.Media.Brushes.White;
            
            // Make sure we don't shut down the app when this window is closed
            this.Closed += LoginWindow_Closed;
        }

        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            // Only shut down the app if we're the main window and no other window is open
            if (Application.Current.MainWindow == this)
            {
                // Check for other windows
                bool otherWindowsOpen = false;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this && window.IsVisible)
                    {
                        otherWindowsOpen = true;
                        break;
                    }
                }
                
                // Only shutdown if no other windows are open
                if (!otherWindowsOpen)
                {
                    Application.Current.Shutdown();
                }
            }
        }
        
        // Helper method to navigate to the ForgotPasswordPage
        public void NavigateToForgotPasswordPage()
        {
            _mainFrame.Navigate(new ForgotPasswordPage());
        }
        
        // Helper method to show the ForgotPasswordWindow
        public void ShowForgotPasswordWindow()
        {
            // Create and show the forgot password window
            var forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            forgotPasswordWindow.Show();
            
            // Close this window to prevent multiple windows
            this.Close();
        }
        
        // Helper method to navigate to the SignUpPage
        public void NavigateToSignUpPage()
        {
            _mainFrame.Navigate(new SignUpPage());
        }
        
        // Helper method to navigate back to the LoginPage
        public void NavigateToLoginPage()
        {
            _mainFrame.Navigate(new LoginPage());
        }
    }
} 