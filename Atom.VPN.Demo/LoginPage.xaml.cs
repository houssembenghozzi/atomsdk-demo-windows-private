using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using shapes = System.Windows.Shapes;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private MainWindow mainWindow;
        private TextBox passwordVisibleTextBox;
        private bool passwordIsVisible = false;
        
        public LoginPage()
        {
            InitializeComponent();
        }

        private void PasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Handle the eye button in password box for showing/hiding password
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                var toggleButton = passwordBox.Template.FindName("PART_TogglePasswordButton", passwordBox) as Button;
                if (toggleButton != null)
                {
                    toggleButton.Click += TogglePasswordButton_Click;
                }
            }
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle password visibility
            if (passwordIsVisible)
            {
                // Hide password - transfer text from TextBox back to PasswordBox
                if (passwordVisibleTextBox != null)
                {
                    PasswordBox.Password = passwordVisibleTextBox.Text;
                    
                    // Remove the temporary TextBox
                    var parentGrid = passwordVisibleTextBox.Parent as Grid;
                    if (parentGrid != null)
                    {
                        parentGrid.Children.Remove(passwordVisibleTextBox);
                        passwordVisibleTextBox = null;
                    }
                    
                    // Show the PasswordBox again
                    PasswordBox.Visibility = Visibility.Visible;
                    
                    // Update the icon to the closed eye
                    var eyeIcon = ((sender as Button).Content as shapes.Path);
                    if (eyeIcon != null)
                    {
                        eyeIcon.Stroke = new SolidColorBrush(Color.FromRgb(190, 190, 190));
                    }
                }
            }
            else
            {
                // Show password - create a TextBox with the same style and content
                var passwordBoxParent = PasswordBox.Parent as Grid;
                if (passwordBoxParent != null)
                {
                    // Create a TextBox to show the password
                    passwordVisibleTextBox = new TextBox
                    {
                        Text = PasswordBox.Password,
                        FontFamily = PasswordBox.FontFamily,
                        FontSize = PasswordBox.FontSize,
                        Foreground = PasswordBox.Foreground,
                        VerticalContentAlignment = PasswordBox.VerticalContentAlignment,
                        Margin = new Thickness(0),
                        Padding = new Thickness(0),
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        VerticalAlignment = PasswordBox.VerticalAlignment,
                        HorizontalAlignment = PasswordBox.HorizontalAlignment
                    };
                    
                    // Add event handlers
                    passwordVisibleTextBox.GotFocus += InputField_GotFocus;
                    passwordVisibleTextBox.LostFocus += InputField_LostFocus;
                    
                    // Hide the PasswordBox and add the TextBox in the same position
                    PasswordBox.Visibility = Visibility.Hidden;
                    Grid.SetColumn(passwordVisibleTextBox, 2);
                    passwordBoxParent.Children.Add(passwordVisibleTextBox);
                    passwordVisibleTextBox.Focus();
                    
                    // Update the icon to the open eye
                    var eyeIcon = ((sender as Button).Content as shapes.Path);
                    if (eyeIcon != null)
                    {
                        eyeIcon.Stroke = new SolidColorBrush(Color.FromRgb(33, 33, 33));
                    }
                }
            }
            
            // Toggle the visibility state
            passwordIsVisible = !passwordIsVisible;
        }

        private void InputField_GotFocus(object sender, RoutedEventArgs e)
        {
            // Change border color and hide placeholder text when the input field gets focus
            if (sender is TextBox textBox)
            {
                var border = textBox.Template?.FindName("border", textBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(33, 33, 33));
                }
                
                var placeholder = textBox.Template?.FindName("PlaceholderText", textBox) as TextBlock;
                if (placeholder != null)
                {
                    placeholder.Visibility = Visibility.Collapsed;
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                var border = passwordBox.Template?.FindName("border", passwordBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(33, 33, 33));
                }
                
                var placeholder = passwordBox.Template?.FindName("PlaceholderText", passwordBox) as TextBlock;
                if (placeholder != null)
                {
                    placeholder.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void InputField_LostFocus(object sender, RoutedEventArgs e)
        {
            // Change border color and show placeholder text when the input field loses focus (if empty)
            if (sender is TextBox textBox)
            {
                var border = textBox.Template?.FindName("border", textBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(190, 190, 190));
                }
                
                var placeholder = textBox.Template?.FindName("PlaceholderText", textBox) as TextBlock;
                if (placeholder != null && string.IsNullOrEmpty(textBox.Text))
                {
                    placeholder.Visibility = Visibility.Visible;
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                var border = passwordBox.Template?.FindName("border", passwordBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(190, 190, 190));
                }
                
                var placeholder = passwordBox.Template?.FindName("PlaceholderText", passwordBox) as TextBlock;
                if (placeholder != null && passwordBox.Password.Length == 0)
                {
                    placeholder.Visibility = Visibility.Visible;
                }
            }
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // For demo purposes, simply open the main window
            mainWindow = new MainWindow();
            mainWindow.SecretKey = "17355649429f7d4adbe993a8d227bc580c8f369b";
            
            // Set event handler to shut down app when main window is closed
            mainWindow.Closed += (s, args) => Application.Current.Shutdown();
            
            // Show the main window
            mainWindow.Show();
            
            // Close this login window
            this.Close();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Password recovery functionality is not implemented in this demo.", "Feature Not Implemented", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sign up functionality is not implemented in this demo.", "Feature Not Implemented", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
} 