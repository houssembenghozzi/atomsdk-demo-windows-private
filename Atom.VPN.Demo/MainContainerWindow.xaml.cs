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
            
            // Set window properties
            this.Title = "Atom VPN";
            this.Height = 700;
            this.Width = 460;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;
            
            // Handle navigation events
            MainFrame.Navigated += MainFrame_Navigated;
            
            // Handle closing event
            this.Closed += MainContainerWindow_Closed;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update the window title based on the current page
            if (e.Content is Page page)
            {
                this.Title = $"Atom VPN - {page.Title}";
            }
        }

        private void MainContainerWindow_Closed(object sender, EventArgs e)
        {
            // Close the application when main window is closed
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Opens the application menu
        /// </summary>
        public void OpenMenu()
        {
            // Navigate to the menu page
            MainFrame.Navigate(new MenuPage());
        }
        
        /// <summary>
        /// Logs the user out and returns to the login screen
        /// </summary>
        public void LogOut()
        {
            // Navigate to the login page
            NavigateToLoginPage();
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

        /// <summary>
        /// Navigates to the settings page
        /// </summary>
        public void NavigateToSettingsPage()
        {
            MainFrame.Navigate(new SettingsPage());
        }

        /// <summary>
        /// Navigates to the account page
        /// </summary>
        public void NavigateToAccountPage()
        {
            MainFrame.Navigate(new AccountPage());
        }
        
        /// <summary>
        /// Navigates to the protocol page
        /// </summary>
        public void NavigateToProtocolPage()
        {
            MainFrame.Navigate(new ProtocolPage());
        }
        
        /// <summary>
        /// Navigates to the language page
        /// </summary>
        public void NavigateToLanguagePage()
        {
            MainFrame.Navigate(new LanguagePage());
        }
        
        /// <summary>
        /// Navigates to the split tunneling page
        /// </summary>
        public void NavigateToSplitTunnelingPage()
        {
            MainFrame.Navigate(new SplitTunnelingPage());
        }
        
        /// <summary>
        /// Navigates to the help center page
        /// </summary>
        public void NavigateToHelpCenterPage()
        {
            MainFrame.Navigate(new HelpCenterPage());
        }
    }
} 