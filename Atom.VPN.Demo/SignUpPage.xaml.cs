using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private static readonly HttpClient client = new HttpClient();
        private const string ApiBaseUrl = "http://203.161.50.155:3001";
        
        private bool isLengthValid = false;
        private bool hasNumber = false;
        private bool hasSpecialChar = false;
        private bool hasUppercase = false;
        private bool hasLowercase = false;
        private bool isAsciiOnly = false;
        
        public SignUpPage()
        {
            InitializeComponent();
            
            // Set focus to the first input field when the window loads
            this.Loaded += (s, e) => EmailTextBox.Focus();
            
            // Setup password visibility toggle
            SetupPasswordVisibilityToggle();
            
            // Initialize password validation UI
            UpdatePasswordValidationUI(PasswordBox.Password);
            
            // Update button state when terms checkbox is clicked
            TermsCheckbox.Click += (s, e) => UpdateSignUpButtonState();
            
            // Add handler for username text box to properly handle placeholder
            UsernameTextBox.TextChanged += (s, e) => 
            {
                UsernameTextBoxPlaceholder.Visibility = string.IsNullOrEmpty(UsernameTextBox.Text) ? 
                    Visibility.Visible : Visibility.Collapsed;
            };
        }
        
        private void InputField_GotFocus(object sender, RoutedEventArgs e)
        {
            // Change border color and placeholder text to black when the input field gets focus
            if (sender is TextBox textBox)
            {
                if (textBox == UsernameTextBox)
                {
                    // Handle username text box separately
                    UsernameBorder.BorderBrush = System.Windows.Media.Brushes.Black;
                    if (UsernameTextBoxPlaceholder != null)
                    {
                        UsernameTextBoxPlaceholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 52, 56));
                    }
                    // Change username icon color
                    if (UsernameIcon != null)
                    {
                        UsernameIcon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(17, 24, 39));
                    }
                }
                else
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
                if (textBox == UsernameTextBox)
                {
                    // Handle username text box separately
                    UsernameBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                    if (UsernameTextBoxPlaceholder != null)
                    {
                        // Only show placeholder if there's no text
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            UsernameTextBoxPlaceholder.Visibility = Visibility.Visible;
                            UsernameTextBoxPlaceholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                        }
                        else
                        {
                            UsernameTextBoxPlaceholder.Visibility = Visibility.Collapsed;
                        }
                    }
                    // Reset username icon color
                    if (UsernameIcon != null)
                    {
                        UsernameIcon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                    }
                }
                else
                {
                    var border = textBox.Template.FindName("border", textBox) as Border;
                    if (border != null)
                    {
                        border.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                    }
                    
                    var placeholder = textBox.Template.FindName("Placeholder", textBox) as TextBlock;
                    if (placeholder != null)
                    {
                        // Only show placeholder if there's no text
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            placeholder.Visibility = Visibility.Visible;
                            placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                        }
                        else
                        {
                            placeholder.Visibility = Visibility.Collapsed;
                        }
                    }
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
                if (placeholder != null)
                {
                    // Only show placeholder if there's no password
                    if (passwordBox.Password.Length == 0)
                    {
                        placeholder.Visibility = Visibility.Visible;
                        placeholder.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 190, 190));
                    }
                    else
                    {
                        placeholder.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
        
        private void SetupPasswordVisibilityToggle()
        {
            this.Loaded += (s, e) =>
            {
                // Get the show password button from PasswordBox template
                if (PasswordBox != null && PasswordBox.Template != null && 
                    PasswordBox.Template.FindName("ShowPasswordButton", PasswordBox) is Button showPasswordButton)
                {
                    showPasswordButton.Click += (sender, args) => 
                    {
                        if (PasswordBox != null && PasswordTextBox != null)
                        {
                            // Show the text version and hide the password version
                            PasswordTextBox.Text = PasswordBox.Password;
                            PasswordBox.Visibility = Visibility.Collapsed;
                            PasswordTextBox.Visibility = Visibility.Visible;
                            PasswordTextBox.Focus(); // Keep focus on the field
                            args.Handled = true;
                        }
                    };
                }

                // Get the hide password button from TextBox template
                if (PasswordTextBox != null && PasswordTextBox.Template != null && 
                    PasswordTextBox.Template.FindName("HidePasswordButton", PasswordTextBox) is Button hidePasswordButton)
                {
                    hidePasswordButton.Click += (sender, args) => 
                    {
                        if (PasswordBox != null && PasswordTextBox != null)
                        {
                            // Show the password version and hide the text version
                            PasswordBox.Password = PasswordTextBox.Text;
                            PasswordTextBox.Visibility = Visibility.Collapsed;
                            PasswordBox.Visibility = Visibility.Visible;
                            PasswordBox.Focus(); // Keep focus on the field
                            args.Handled = true;
                        }
                    };
                }

                // Sync password changes between PasswordBox and TextBox
                if (PasswordBox != null && PasswordTextBox != null)
                {
                    PasswordBox.PasswordChanged += (sender, args) => 
                    {
                        if (PasswordTextBox.Visibility == Visibility.Visible)
                        {
                            PasswordTextBox.Text = PasswordBox.Password;
                        }
                        UpdatePasswordValidationUI(PasswordBox.Password);
                    };

                    PasswordTextBox.TextChanged += (sender, args) => 
                    {
                        if (PasswordBox.Visibility == Visibility.Visible)
                        {
                            PasswordBox.Password = PasswordTextBox.Text;
                        }
                        UpdatePasswordValidationUI(PasswordTextBox.Text);
                    };
                }
                
                // Do the same for confirm password toggle
                if (ConfirmPasswordBox != null && ConfirmPasswordBox.Template != null && 
                    ConfirmPasswordBox.Template.FindName("ShowPasswordButton", ConfirmPasswordBox) is Button confirmShowPasswordButton)
                {
                    confirmShowPasswordButton.Click += (sender, args) => 
                    {
                        if (ConfirmPasswordBox != null && ConfirmPasswordTextBox != null)
                        {
                            // Show the text version and hide the password version
                            ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
                            ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                            ConfirmPasswordTextBox.Visibility = Visibility.Visible;
                            ConfirmPasswordTextBox.Focus(); // Keep focus on the field
                            args.Handled = true;
                        }
                    };
                }

                // Get the hide password button from TextBox template
                if (ConfirmPasswordTextBox != null && ConfirmPasswordTextBox.Template != null && 
                    ConfirmPasswordTextBox.Template.FindName("HidePasswordButton", ConfirmPasswordTextBox) is Button confirmHidePasswordButton)
                {
                    confirmHidePasswordButton.Click += (sender, args) => 
                    {
                        if (ConfirmPasswordBox != null && ConfirmPasswordTextBox != null)
                        {
                            // Show the password version and hide the text version
                            ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;
                            ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
                            ConfirmPasswordBox.Visibility = Visibility.Visible;
                            ConfirmPasswordBox.Focus(); // Keep focus on the field
                            args.Handled = true;
                        }
                    };
                }

                // Sync confirm password changes 
                if (ConfirmPasswordBox != null && ConfirmPasswordTextBox != null)
                {
                    ConfirmPasswordBox.PasswordChanged += (sender, args) => 
                    {
                        if (ConfirmPasswordTextBox.Visibility == Visibility.Visible)
                        {
                            ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
                        }
                        UpdateSignUpButtonState();
                    };

                    ConfirmPasswordTextBox.TextChanged += (sender, args) => 
                    {
                        if (ConfirmPasswordBox.Visibility == Visibility.Visible)
                        {
                            ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;
                        }
                        UpdateSignUpButtonState();
                    };
                }
            };
        }

        private void UpdatePasswordValidationUI(string password)
        {
            // Update length validation
            isLengthValid = Regex.IsMatch(password, @"^.{12,32}$");
            LengthValidationText.Text = (isLengthValid ? "✔" : "●") + " Must be 12-32 characters long";
            LengthValidationText.Foreground = isLengthValid 
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green) 
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x75, 0x75, 0x75));

            // Update number validation
            hasNumber = Regex.IsMatch(password, @"[0-9]");
            NumberValidationText.Text = (hasNumber ? "✔" : "●") + " Must contain at least one number";
            NumberValidationText.Foreground = hasNumber 
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green) 
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x75, 0x75, 0x75));

            // Update special character validation
            hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*,.:{}/]");
            SpecialCharValidationText.Text = (hasSpecialChar ? "✔" : "●") + " Must contain at least one special character (!@#$%^&amp;*,.:{}/)";
            SpecialCharValidationText.Foreground = hasSpecialChar 
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green) 
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x75, 0x75, 0x75));
            
            // Update uppercase validation
            hasUppercase = Regex.IsMatch(password, @"[A-Z]");
            UppercaseValidationText.Text = (hasUppercase ? "✔" : "●") + " Must contain at least one uppercase letter";
            UppercaseValidationText.Foreground = hasUppercase 
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green) 
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x75, 0x75, 0x75));
            
            // Update lowercase validation
            hasLowercase = Regex.IsMatch(password, @"[a-z]");
            LowercaseValidationText.Text = (hasLowercase ? "✔" : "●") + " Must contain at least one lowercase letter";
            LowercaseValidationText.Foreground = hasLowercase 
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green) 
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x75, 0x75, 0x75));
            
            // Update ASCII only validation
            isAsciiOnly = !Regex.IsMatch(password, @"[^\x00-\x7F]");
            AsciiValidationText.Text = (isAsciiOnly ? "✔" : "●") + " Must contain only ASCII characters";
            AsciiValidationText.Foreground = isAsciiOnly 
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green) 
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x75, 0x75, 0x75));
            
            UpdateSignUpButtonState();
        }
        
        private void UpdateSignUpButtonState()
        {
            // Get password and confirm password
            string password = PasswordBox.IsVisible ? PasswordBox.Password : PasswordTextBox.Text;
            string confirmPassword = ConfirmPasswordBox.IsVisible ? ConfirmPasswordBox.Password : ConfirmPasswordTextBox.Text;
            string email = EmailTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            
            // Check if passwords match
            bool passwordsMatch = password == confirmPassword && !string.IsNullOrEmpty(password);
            
            // Visually indicate password match status only if confirm password is not empty
            if (!string.IsNullOrEmpty(confirmPassword))
            {
                var border = ConfirmPasswordBox.IsVisible 
                    ? ConfirmPasswordBox.Template.FindName("border", ConfirmPasswordBox) as Border
                    : ConfirmPasswordTextBox.Template.FindName("border", ConfirmPasswordTextBox) as Border;
                
                if (border != null)
                {
                    // Change border color based on password match
                    border.BorderBrush = passwordsMatch 
                        ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green)
                        : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                }
                
                // Update password match status text
                if (passwordsMatch)
                {
                    PasswordMatchStatus.Text = "Passwords match";
                    PasswordMatchStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                    PasswordMatchStatus.Visibility = Visibility.Visible;
                }
                else
                {
                    PasswordMatchStatus.Text = "Passwords don't match";
                    PasswordMatchStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                    PasswordMatchStatus.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Hide password match status when confirm password is empty
                PasswordMatchStatus.Visibility = Visibility.Collapsed;
            }
            
            // Check email validity
            bool isEmailValid = IsValidEmail(email);
            
            // Check username validity (max 8 characters)
            bool isUsernameValid = !string.IsNullOrEmpty(username) && username.Length <= 8;

            // Check terms checkbox is checked
            bool termsAccepted = TermsCheckbox.IsChecked ?? false;
            
            // Check all password requirements and terms
            bool allRequirementsMet = isLengthValid && hasNumber && hasSpecialChar && 
                                      hasUppercase && hasLowercase && isAsciiOnly && 
                                      passwordsMatch && isEmailValid && isUsernameValid && termsAccepted;
            
            // Update sign up button state
            SignUpButton.IsEnabled = allRequirementsMet;
        }

        private async void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Disable sign up button to prevent multiple submissions
            SignUpButton.IsEnabled = false;

            try
            {
                // Get and validate input values
                string email = EmailTextBox.Text.Trim();
                string username = UsernameTextBox.Text.Trim();
                string password = PasswordBox.IsVisible ? PasswordBox.Password : PasswordTextBox.Text;
                string confirmPassword = ConfirmPasswordBox.IsVisible ? ConfirmPasswordBox.Password : ConfirmPasswordTextBox.Text;
                
                // Final validation check before API call
                if (string.IsNullOrWhiteSpace(email))
                {
                    new ErrorDialog("Validation Error", "Please enter your email address.").ShowDialog();
                    EmailTextBox.Focus();
                    SignUpButton.IsEnabled = true;
                    return;
                }

                if (!IsValidEmail(email))
                {
                    new ErrorDialog("Validation Error", "Please enter a valid email address.").ShowDialog();
                    EmailTextBox.Focus();
                    SignUpButton.IsEnabled = true;
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(username))
                {
                    new ErrorDialog("Validation Error", "Please enter a username.").ShowDialog();
                    UsernameTextBox.Focus();
                    SignUpButton.IsEnabled = true;
                    return;
                }
                
                if (username.Length > 8)
                {
                    new ErrorDialog("Validation Error", "Username must be maximum 8 characters.").ShowDialog();
                    UsernameTextBox.Focus();
                    SignUpButton.IsEnabled = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    new ErrorDialog("Validation Error", "Please enter a password.").ShowDialog();
                    FocusPassword();
                    SignUpButton.IsEnabled = true;
                    return;
                }

                if (password != confirmPassword)
                {
                    new ErrorDialog("Validation Error", "Passwords do not match.").ShowDialog();
                    ConfirmPasswordBox.Focus();
                    SignUpButton.IsEnabled = true;
                    return;
                }
                
                // Validate password using the detailed validation logic
                var passwordErrors = ValidatePasswordLocal(password);
                if (passwordErrors.Count > 0)
                {
                    new ErrorDialog("Password Validation Error", $"Password does not meet requirements:\n• {string.Join("\n• ", passwordErrors)}").ShowDialog();
                    FocusPassword();
                    SignUpButton.IsEnabled = true;
                    return;
                }

                // Create the payload according to the register API expectations
                var payload = new
                {
                    username = username,
                    email = email,
                    password = password,
                    name = username, // Using username as name for simplicity
                    subscriptionType = "trial" // Default subscription type
                };

                // Serialize and send the request
                string jsonPayload = JsonSerializer.Serialize(payload);
                var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                
                // Make the API call
                HttpResponseMessage response = await client.PostAsync($"{ApiBaseUrl}/api/auth/register", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    // Show success dialog
                    new SuccessDialog("Registration Successful", 
                        "Your account has been created successfully! You can now log in.").ShowDialog();
                    
                    // Navigate to login page
                    var parentWindow = Window.GetWindow(this);
                    if (parentWindow is MainContainerWindow mcWindow)
                    {
                        mcWindow.NavigateToLoginPage();
                    }
                    else
                    {
                        // If we can't navigate, just clear the form
                        EmailTextBox.Text = string.Empty;
                        UsernameTextBox.Text = string.Empty;
                        PasswordBox.Password = string.Empty;
                        ConfirmPasswordBox.Password = string.Empty;
                        PasswordTextBox.Text = string.Empty;
                        ConfirmPasswordTextBox.Text = string.Empty;
                        UpdatePasswordValidationUI(string.Empty);
                        
                        // Re-enable the sign up button
                        SignUpButton.IsEnabled = true;
                    }
                }
                else
                {
                    // Handle API error response
                    string errorContent = await response.Content.ReadAsStringAsync();
                    string errorMessage = $"Failed to register. Status: {response.StatusCode}";
                    
                    try
                    {
                        // Try to parse a more detailed error message from the response
                        var errorResponseDoc = JsonDocument.Parse(errorContent);
                        if (errorResponseDoc.RootElement.TryGetProperty("message", out JsonElement msgProp))
                        {
                            errorMessage = msgProp.GetString();
                        }
                        else if (errorResponseDoc.RootElement.TryGetProperty("errors", out JsonElement errorsProp) && 
                                errorsProp.ValueKind == JsonValueKind.Array && 
                                errorsProp.GetArrayLength() > 0)
                        {
                            // Concatenate multiple error messages if present
                            var errorMessages = new List<string>();
                            foreach (var err in errorsProp.EnumerateArray())
                            {
                                errorMessages.Add(err.GetString());
                            }
                            errorMessage = string.Join("\n", errorMessages);
                        }
                    }
                    catch (JsonException)
                    {
                        // If we can't parse the JSON, just use the raw response
                        errorMessage = errorContent;
                    }
                    
                    new ErrorDialog("Registration Error", errorMessage).ShowDialog();
                    SignUpButton.IsEnabled = true;
                }
            }
            catch (HttpRequestException httpEx)
            {
                new ErrorDialog("Network Error", $"A network error occurred: {httpEx.Message}").ShowDialog();
                SignUpButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                new ErrorDialog("Error", $"An unexpected error occurred: {ex.Message}").ShowDialog();
                SignUpButton.IsEnabled = true;
            }
        }

        private void FocusPassword()
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordBox.Focus();
            }
            else
            {
                PasswordTextBox.Focus();
            }
        }

        private void TermsAndConditions_Click(object sender, RoutedEventArgs e)
        {
            // Open terms and conditions
            MessageBox.Show("Terms and Conditions page would open here.", "Terms and Conditions", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PrivacyPolicy_Click(object sender, RoutedEventArgs e)
        {
            // Open privacy policy
            MessageBox.Show("Privacy Policy page would open here.", "Privacy Policy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the parent window
                var parentWindow = Window.GetWindow(this);
                
                // Navigate to login page
                if (parentWindow is MainContainerWindow mcWindow)
                {
                    mcWindow.NavigateToLoginPage();
                }
                else
                {
                    new ErrorDialog("Navigation Error", "Could not navigate to login page. Try restarting the application.").ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new ErrorDialog("Navigation Error", $"Error navigating to login screen: {ex.Message}").ShowDialog();
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                // Simple regex for email validation
                var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
        
        // Comprehensive password validation from ChangePasswordPage
        private List<string> ValidatePasswordLocal(string password)
        {
            var errors = new List<string>();
            if (!Regex.IsMatch(password, @"^.{12,32}$")) errors.Add("Password must be 12-32 characters long.");
            if (!Regex.IsMatch(password, @"[A-Z]")) errors.Add("Password must contain at least one uppercase letter.");
            if (!Regex.IsMatch(password, @"[a-z]")) errors.Add("Password must contain at least one lowercase letter.");
            if (!Regex.IsMatch(password, @"[0-9]")) errors.Add("Password must contain at least one number.");
            if (!Regex.IsMatch(password, @"[!@#$%^&*,.:{}/]")) errors.Add("Password must contain at least one special character (!@#$%^&amp;*,.:{}/).");
            if (Regex.IsMatch(password, @"[^\x00-\x7F]")) errors.Add("Password must contain only ASCII characters.");
            return errors;
        }
    }
} 