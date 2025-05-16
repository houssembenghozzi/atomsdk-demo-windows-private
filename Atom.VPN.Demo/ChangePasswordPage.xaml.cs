using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;

namespace Atom.VPN.Demo
{
    public partial class ChangePasswordPage : Page
    {
        public ChangePasswordPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
        
        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string passwordBoxName)
            {
                // Get the corresponding PasswordBox
                var passwordBox = FindName(passwordBoxName) as PasswordBox;
                if (passwordBox == null) return;
                
                // Find the eye icons based on the button that was clicked
                string prefix = passwordBoxName.Replace("PasswordBox", "");
                var eyeIcon = FindName($"{prefix}PasswordEyeIcon") as UIElement;
                var eyeSlashIcon = FindName($"{prefix}PasswordEyeSlashIcon") as UIElement;
                
                if (eyeIcon == null || eyeSlashIcon == null) return;
                
                // Check if password is visible or hidden
                var isPasswordVisible = eyeSlashIcon.Visibility == Visibility.Visible;
                
                // Toggle password visibility
                if (isPasswordVisible)
                {
                    // Switch to hidden password mode
                    passwordBox.PasswordChar = '●';
                    eyeIcon.Visibility = Visibility.Visible;
                    eyeSlashIcon.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Switch to visible password mode
                    passwordBox.PasswordChar = '\0'; // Null character makes text visible
                    eyeIcon.Visibility = Visibility.Collapsed;
                    eyeSlashIcon.Visibility = Visibility.Visible;
                }
            }
        }
    }
} 