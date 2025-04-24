using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for MainContainerWindow.xaml
    /// </summary>
    public partial class MainContainerWindow : Window
    {
        public MainContainerWindow()
        {
            InitializeComponent();
            
            // Set this window as the application's main window
            Application.Current.MainWindow = this;
            
            // Navigate to the login page initially
            NavigateToLoginPage();
            
            // Handle navigation events
            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update the window title based on the current page
            if (e.Content is Page page)
            {
                this.Title = $"Atom VPN - {page.Title}";
            }
        }

        /// <summary>
        /// Navigates to the login page
        /// </summary>
        public void NavigateToLoginPage()
        {
            MainFrame.Navigate(new LoginPage());
        }

        /// <summary>
        /// Navigates to the forgot password page
        /// </summary>
        public void NavigateToForgotPasswordPage()
        {
            MainFrame.Navigate(new ForgotPasswordPage());
        }

        /// <summary>
        /// Navigates to the sign up page
        /// </summary>
        public void NavigateToSignUpPage()
        {
            MainFrame.Navigate(new SignUpPage());
        }

        /// <summary>
        /// Navigates to the main VPN page
        /// </summary>
        public void NavigateToMainVPNPage()
        {
            MainFrame.Navigate(new MainVPNPage());
        }
    }
} 