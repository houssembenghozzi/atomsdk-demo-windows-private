using System;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Atom.VPN.Demo
{
    public static class AuthTokenManager
    {
        public static string CurrentToken { get; set; }
    }

    public class LoginApiResponse
    {
        [JsonProperty("token")] // Assuming the token field is named "token" in the API response
        public string Token { get; set; }
        // Add other properties if your API returns more user details, e.g.,
        // [JsonProperty("userId")]
        // public string UserId { get; set; }
    }

    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private static readonly HttpClient client = new HttpClient();

        public LoginPage()
        {
            InitializeComponent();
            
            // Setup password visibility toggle
            SetupPasswordVisibilityToggle();
            
            // Focus the email field when the page loads
            this.Loaded += (s, e) => EmailTextBox.Focus();
        }

        private void SetupPasswordVisibilityToggle()
        {
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

            // Get the hide password button from text box template
            if (PasswordTextBox.Template.FindName("HidePasswordButton", PasswordTextBox) is Button hidePasswordButton)
            {
                hidePasswordButton.Click += (s, e) => 
                {
                    // Show the password version and hide the text version
                    PasswordBox.Password = PasswordTextBox.Text;
                    PasswordTextBox.Visibility = Visibility.Collapsed;
                    PasswordBox.Visibility = Visibility.Visible;
                    PasswordBox.Focus();
                    e.Handled = true;
                };
            }

            // Sync password changes
            PasswordBox.PasswordChanged += (s, e) => 
            {
                if (PasswordTextBox.Visibility == Visibility.Visible)
                {
                    PasswordTextBox.Text = PasswordBox.Password;
                }

                // Manage PasswordBox's own placeholder visibility
                var pbPlaceholder = PasswordBox.Template.FindName("Placeholder", PasswordBox) as TextBlock;
                if (pbPlaceholder != null)
                {
                    pbPlaceholder.Visibility = PasswordBox.Password.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            };

            PasswordTextBox.TextChanged += (s, e) => 
            {
                if (PasswordBox.Visibility == Visibility.Visible)
                {
                    PasswordBox.Password = PasswordTextBox.Text;
                }
            };
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
                
                // Placeholder visibility is primarily handled by XAML triggers based on focus and text content.
                // The foreground color of the placeholder can be adjusted here if needed when focused.
                // var placeholder = textBox.Template.FindName("Placeholder", textBox) as TextBlock;
                // if (placeholder != null) {
                //     placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 52, 56));
                // }
            }
            else if (sender is PasswordBox passwordBox)
            {
                var border = passwordBox.Template.FindName("border", passwordBox) as Border;
                if (border != null)
                {
                    border.BorderBrush = System.Windows.Media.Brushes.Black;
                }
                
                // Placeholder visibility for PasswordBox is primarily handled by XAML triggers on focus 
                // and its PasswordChanged event for content.
            }
        }

        private void InputField_LostFocus(object sender, RoutedEventArgs e)
        {
            // Change border color and placeholder text to gray when the input field loses focus
            if (sender is TextBox textBox)
            {
                var border = textBox.Template.FindName("border", textBox) as Border;
                var placeholder = textBox.Template.FindName("Placeholder", textBox) as TextBlock;

                if (border != null)
                {
                    border.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                }

                if (placeholder != null) 
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                        placeholder.Visibility = Visibility.Visible; 
                    }
                    else
                    {
                        placeholder.Visibility = Visibility.Collapsed; 
                    }
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                var border = passwordBox.Template.FindName("border", passwordBox) as Border;
                var placeholder = passwordBox.Template.FindName("Placeholder", passwordBox) as TextBlock;

                if (border != null)
                {
                     border.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                }

                if (placeholder != null)
                {
                    if (passwordBox.Password.Length == 0)
                    {
                        placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                        placeholder.Visibility = Visibility.Visible; // Show placeholder if password is empty
                    }
                    else
                    {
                        placeholder.Visibility = Visibility.Collapsed; // Hide placeholder if password is present
                    }
                }
            }
        }

        private void AutoLoginButton_Click(object sender, RoutedEventArgs e)
        {
            EmailTextBox.Text = "fortisac";
            PasswordBox.Password = "Houssem1995!";
            // If PasswordTextBox is visible (show password mode), update it too.
            if (PasswordTextBox.Visibility == Visibility.Visible)
            {
                PasswordTextBox.Text = "Houssem1995!";
            }
            
            // Get the sign-in button
            var signInButton = FindName("SignInButton") as Button;
            if (signInButton != null)
            {
                // Update button state before triggering
                signInButton.Content = "Connecting...";
                signInButton.IsEnabled = false;
            }
            
            // Trigger the sign-in process
            SignIn_Click(signInButton ?? sender, e);
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password; // PasswordBox.Password should be up-to-date due to syncing

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                // Use new ErrorDialog
                new ErrorDialog("Input Error", "Please enter both email/username and password.").ShowDialog();
                return;
            }

            // Show loading state
            Button loginButton = sender as Button;
            string originalContent = loginButton.Content.ToString();
            loginButton.Content = "Connecting...";
            loginButton.IsEnabled = false;

            try
            {
                var loginCredentials = new { email = email, password = password }; // Backend expects 'email' field for identifier
                string jsonData = JsonConvert.SerializeObject(loginCredentials);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Ensure the URL includes the port if you are directly accessing the backend
                HttpResponseMessage response = await client.PostAsync("http://203.161.50.155:3001/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    LoginApiResponse apiResponse = JsonConvert.DeserializeObject<LoginApiResponse>(responseBody);

                    if (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.Token))
                    {
                        AuthTokenManager.CurrentToken = apiResponse.Token;
                        // Successfully logged in, proceed with existing navigation logic

                        var parentWindow = Window.GetWindow(this);
                        if (parentWindow is MainContainerWindow mainContainerWindow)
                        {
                            mainContainerWindow.NavigateToMainVPNPage();
                        }
                        else if (parentWindow is LoginWindow loginWindow)
                        {
                            var containerWindow = new MainContainerWindow();
                            containerWindow.Height = loginWindow.Height;
                            containerWindow.Width = loginWindow.Width;
                            containerWindow.Left = loginWindow.Left;
                            containerWindow.Top = loginWindow.Top;
                            containerWindow.WindowStartupLocation = loginWindow.WindowStartupLocation;
                            containerWindow.NavigateToMainVPNPage();
                            Application.Current.MainWindow = containerWindow;
                            containerWindow.Show();
                            loginWindow.Close();
                        }
                        else
                        {
                            // Restore button state
                            loginButton.Content = originalContent;
                            loginButton.IsEnabled = true;
                            
                            // Use new ErrorDialog for navigation error
                            new ErrorDialog("Navigation Error", $"Login successful but navigation failed. Parent window type: {parentWindow?.GetType().Name ?? "null"}").ShowDialog();
                        }
                    }
                    else
                    {
                        // Restore button state
                        loginButton.Content = originalContent;
                        loginButton.IsEnabled = true;
                        
                        // Token missing in response
                        new ErrorDialog("Authentication Error", "Authentication successful but token is missing.").ShowDialog();
                    }
                }
                else
                {
                    // Restore button state
                    loginButton.Content = originalContent;
                    loginButton.IsEnabled = true;
                    
                    string errorContent = await response.Content.ReadAsStringAsync();
                    string errorMessage = "Authentication failed.";
                    
                    try
                    {
                        // Try to parse the error message from the response
                        var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                        if (errorResponse != null && errorResponse.message != null)
                        {
                            errorMessage = errorResponse.message.ToString();
                        }
                    }
                    catch { /* Use default error message if parsing fails */ }
                    
                    new ErrorDialog("Authentication Error", errorMessage).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                // Restore button state
                loginButton.Content = originalContent;
                loginButton.IsEnabled = true;
                
                new ErrorDialog("Connection Error", $"Failed to connect to server: {ex.Message}").ShowDialog();
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the parent window
                var parentWindow = Window.GetWindow(this);
                
                // Check what type of window we're in
                if (parentWindow is MainContainerWindow mainContainerWindow)
                {
                    // Navigate to forgot password page using MainContainerWindow
                    mainContainerWindow.NavigateToForgotPasswordPage();
                }
                else if (parentWindow is LoginWindow loginWindow)
                {
                    // Create and show ForgotPasswordWindow directly
                    var forgotPasswordWindow = new ForgotPasswordWindow();
                    forgotPasswordWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    forgotPasswordWindow.Show();
                    
                    // Close the login window to prevent multiple windows
                    loginWindow.Close();
                }
                else
                {
                    throw new InvalidOperationException($"Parent window is not a navigation window: {parentWindow?.GetType().Name ?? "null"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Forgot Password screen: {ex.Message}", 
                                "Navigation Error", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the parent window
                var parentWindow = Window.GetWindow(this);
                
                // Check what type of window we're in
                if (parentWindow is MainContainerWindow mainContainerWindow)
                {
                    // Navigate to sign up page using MainContainerWindow
                    mainContainerWindow.NavigateToSignUpPage();
                }
                else if (parentWindow is LoginWindow loginWindow)
                {
                    // Convert LoginWindow to MainContainerWindow to maintain page-based flow
                    var containerWindow = new MainContainerWindow();
                    
                    // Copy relevant properties
                    containerWindow.Height = loginWindow.Height;
                    containerWindow.Width = loginWindow.Width;
                    containerWindow.Left = loginWindow.Left;
                    containerWindow.Top = loginWindow.Top;
                    containerWindow.WindowStartupLocation = loginWindow.WindowStartupLocation;
                    
                    // Navigate to sign up page
                    containerWindow.NavigateToSignUpPage();
                    
                    // Set as main application window
                    Application.Current.MainWindow = containerWindow;
                    
                    // Show the container window
                    containerWindow.Show();
                    
                    // Close the login window
                    loginWindow.Close();
                }
                else
                {
                    throw new InvalidOperationException($"Parent window is not a navigation window: {parentWindow?.GetType().Name ?? "null"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Sign Up screen: {ex.Message}",
                                "Navigation Error",
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }
        }
    }
} 