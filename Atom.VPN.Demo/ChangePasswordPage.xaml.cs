using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Atom.VPN.Demo
{
    public partial class ChangePasswordPage : Page
    {
        private static readonly HttpClient client = new HttpClient();
        // The API base URL provided by the user.
        private const string ApiBaseUrl = "http://203.161.50.155:3001";

        private bool isLengthValid = false;
        private bool hasNumber = false;
        private bool hasSpecialChar = false;

        public ChangePasswordPage()
        {
            InitializeComponent();
            // Initialize HttpClient base address if it's always the same
            // client.BaseAddress = new Uri(ApiBaseUrl); 
            // Or set it per request if it can vary
            
            // Initialize password validation UI on load for the new password
            UpdatePasswordValidationUI(NewPasswordBox.Password);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
        
        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string prefix)
            {
                var passwordBox = FindName($"{prefix}PasswordBox") as PasswordBox;
                var textBox = FindName($"{prefix}PasswordTextBox") as TextBox;
                var eyeIcon = FindName($"{prefix}PasswordEyeIcon") as UIElement;
                var eyeSlashIcon = FindName($"{prefix}PasswordEyeSlashIcon") as UIElement;

                if (passwordBox == null || textBox == null || eyeIcon == null || eyeSlashIcon == null) return;

                if (passwordBox.Visibility == Visibility.Visible) // Currently hidden, switch to visible
                {
                    textBox.Text = passwordBox.Password;
                    passwordBox.Visibility = Visibility.Collapsed;
                    textBox.Visibility = Visibility.Visible;
                    eyeIcon.Visibility = Visibility.Collapsed;
                    eyeSlashIcon.Visibility = Visibility.Visible;
                    textBox.Focus();
                }
                else // Currently visible, switch to hidden
                {
                    passwordBox.Password = textBox.Text;
                    textBox.Visibility = Visibility.Collapsed;
                    passwordBox.Visibility = Visibility.Visible;
                    eyeIcon.Visibility = Visibility.Visible;
                    eyeSlashIcon.Visibility = Visibility.Collapsed;
                    passwordBox.Focus();
                }
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = NewPasswordBox.Password;
            if (NewPasswordTextBox.Visibility == Visibility.Visible)
            {
                if (NewPasswordTextBox.Text != password) NewPasswordTextBox.Text = password; 
            }
            UpdatePasswordValidationUI(password);
        }

        private void NewPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string password = NewPasswordTextBox.Text;
            if (NewPasswordBox.Password != password) NewPasswordBox.Password = password;
            UpdatePasswordValidationUI(password);
        }

        private void UpdatePasswordValidationUI(string password)
        {
            isLengthValid = password.Length >= 12 && password.Length <= 32;
            LengthValidationText.Text = (isLengthValid ? "✔" : "●") + " Must be 12-32 characters long";
            LengthValidationText.Foreground = isLengthValid ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Color.FromRgb(0x75, 0x75, 0x75));

            hasNumber = Regex.IsMatch(password, @"\d");
            NumberValidationText.Text = (hasNumber ? "✔" : "●") + " Must contain at least one number";
            NumberValidationText.Foreground = hasNumber ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Color.FromRgb(0x75, 0x75, 0x75));

            hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*,.:{}/]");
            SpecialCharValidationText.Text = (hasSpecialChar ? "✔" : "●") + " Must contain at least one special character (!@#$%^&amp;*,.:{}/)";
            SpecialCharValidationText.Foreground = hasSpecialChar ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Color.FromRgb(0x75, 0x75, 0x75));
            
            UpdateSaveButtonState();
        }

        private void OldPassword_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb) {
                 if (OldPasswordTextBox.Text != pb.Password) OldPasswordTextBox.Text = pb.Password;
            } else if (sender is TextBox tb) {
                 if (OldPasswordBox.Password != tb.Text) OldPasswordBox.Password = tb.Text;
            }
            UpdateSaveButtonState();
        }

        private void ConfirmPassword_Changed(object sender, RoutedEventArgs e)
        {
             if (sender is PasswordBox pb) {
                 if (ConfirmPasswordTextBox.Text != pb.Password) ConfirmPasswordTextBox.Text = pb.Password;
            } else if (sender is TextBox tb) {
                 if (ConfirmPasswordBox.Password != tb.Text) ConfirmPasswordBox.Password = tb.Text;
            }
            UpdateSaveButtonState();
        }

        private void UpdateSaveButtonState()
        {
            string oldPassword = OldPasswordBox.IsVisible ? OldPasswordBox.Password : OldPasswordTextBox.Text;
            string newPassword = NewPasswordBox.IsVisible ? NewPasswordBox.Password : NewPasswordTextBox.Text;
            string confirmPassword = ConfirmPasswordBox.IsVisible ? ConfirmPasswordBox.Password : ConfirmPasswordTextBox.Text;

            bool passwordsMatch = newPassword == confirmPassword;
            // Could add visual feedback for password match here if desired

            bool canEnableSave = isLengthValid && hasNumber && hasSpecialChar && 
                                 !string.IsNullOrEmpty(oldPassword) && 
                                 passwordsMatch;
            SaveButton.IsEnabled = canEnableSave;
        }
        
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false; // Disable button to prevent multiple clicks

            string oldPassword = OldPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // --- Client-side Validation ---
            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                MessageBox.Show("Old password is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                OldPasswordBox.Focus();
                SaveButton.IsEnabled = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("New password is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                NewPasswordBox.Focus();
                SaveButton.IsEnabled = true;
                return;
            }
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New password and confirmation password do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfirmPasswordBox.Focus();
                SaveButton.IsEnabled = true;
                return;
            }

            var passwordErrors = ValidatePasswordLocal(newPassword);
            if (passwordErrors.Count > 0)
            {
                MessageBox.Show("New password does not meet requirements:\n- " + string.Join("\n- ", passwordErrors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                NewPasswordBox.Focus();
                SaveButton.IsEnabled = true;
                return;
            }

            string authToken = GetAuthToken();
            if (string.IsNullOrEmpty(authToken))
            {
                MessageBox.Show("Authentication token not found. Please log in again.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Optionally, navigate to login page or handle appropriately
                SaveButton.IsEnabled = true;
                return;
            }

            var payload = new
            {
                currentPassword = oldPassword,
                newPassword = newPassword,
                confirmPassword = confirmPassword // Backend requires this for its own validation
            };

            try
            {
                string jsonPayload = JsonSerializer.Serialize(payload);
                var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Ensure client is not re-adding headers if it's static and reused
                client.DefaultRequestHeaders.Authorization = null; 
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                HttpResponseMessage response = await client.PutAsync($"{ApiBaseUrl}/api/auth/password", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    new SuccessDialog("Password Changed", "Your password has been changed successfully!").ShowDialog();
                    
                    // Clear fields after successful change
                    OldPasswordBox.Password = string.Empty;
                    NewPasswordBox.Password = string.Empty;
                    ConfirmPasswordBox.Password = string.Empty;
                    UpdatePasswordValidationUI(string.Empty); // Reset validation UI
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    string messageFromServer = $"Failed to change password. Status: {response.StatusCode}";
                    try
                    {
                        // Attempt to parse a more structured error message from the backend
                        var errorResponseDoc = JsonDocument.Parse(errorContent);
                        if (errorResponseDoc.RootElement.TryGetProperty("message", out JsonElement msgProp))
                        {
                            messageFromServer = msgProp.GetString();
                        }
                        else if (errorResponseDoc.RootElement.TryGetProperty("errors", out JsonElement errorsProp) && errorsProp.ValueKind == JsonValueKind.Array && errorsProp.GetArrayLength() > 0)
                        {
                            // Concatenate multiple error messages if present
                            var errorMessages = new List<string>();
                            foreach (var err in errorsProp.EnumerateArray())
                            {
                                errorMessages.Add(err.GetString());
                            }
                            messageFromServer = string.Join("\n", errorMessages);
                        }
                    }
                    catch (JsonException) { /* Ignore JSON parsing errors, use the generic errorContent or status code */ }
                     MessageBox.Show($"Error: {messageFromServer}\nDetails: {errorContent.Substring(0, Math.Min(errorContent.Length, 500))}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"A network error occurred: {httpEx.Message}", "Network Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Error processing data: {jsonEx.Message}. Check if the API is returning valid JSON.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true; // Re-enable button
            }
        }

        // Local password validation logic (mirrors backend rules from auth.ts)
        private List<string> ValidatePasswordLocal(string password)
        {
            var errors = new List<string>();
            if (!Regex.IsMatch(password, @"^.{12,32}$")) errors.Add("Password must be 12-32 characters long.");
            if (!Regex.IsMatch(password, @"[A-Z]")) errors.Add("Password must contain at least one uppercase letter.");
            if (!Regex.IsMatch(password, @"[a-z]")) errors.Add("Password must contain at least one lowercase letter.");
            if (!Regex.IsMatch(password, @"[0-9]")) errors.Add("Password must contain at least one number.");
            // Special characters from backend: /[!@#$%^&*,.:{}/]/
            if (!Regex.IsMatch(password, @"[!@#$%^&*,.:{}/]")) errors.Add("Password must contain at least one special character (!@#$%^&amp;*,.:{}/).");
            if (!Regex.IsMatch(password, @"^[ -]*$")) errors.Add("Password must not contain non-ASCII characters.");
            return errors;
        }

        // Placeholder for getting the auth token - THIS NEEDS TO BE IMPLEMENTED BY YOU
        private string GetAuthToken()
        {
            // Use the existing AuthTokenManager from the login page
            return AuthTokenManager.CurrentToken;
        }
    }
} 