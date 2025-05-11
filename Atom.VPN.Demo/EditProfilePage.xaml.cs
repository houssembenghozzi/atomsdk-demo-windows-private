using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace Atom.VPN.Demo
{
    // Model for the user profile data expected from /api/auth/me
    public class UserProfileDataResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public UserProfileData Data { get; set; }
        
        [JsonProperty("message")] // For error cases
        public string Message { get; set; }
    }

    public class UserProfileData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        // Removed Language and Timezone
        // [JsonProperty("language")]
        // public string Language { get; set; }

        // [JsonProperty("timezone")]
        // public string Timezone { get; set; }
    }
    
    // Model for PUT request to /api/auth/profile
    public class UpdateProfileRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        // Removed Language and Timezone
        // [JsonProperty("language")]
        // public string Language { get; set; }

        // [JsonProperty("timezone")]
        // public string Timezone { get; set; }
    }

    public partial class EditProfilePage : Page
    {
        private static readonly HttpClient client = new HttpClient();
        private bool _isCountryComboBoxLoaded = false; // Flag to prevent premature SelectionChanged event

        public EditProfilePage()
        {
            InitializeComponent();
            Loaded += EditProfilePage_Loaded;
            SaveButton.Click += SaveButton_Click;
            PopulateCountryComboBox();
        }

        private void PopulateCountryComboBox()
        {
            // Test with the test helper
            string test = Atom.VPN.Demo.TestHelper.GetTestString();
            
            // Try explicit assembly qualification
            var countries = typeof(Atom.VPN.Demo.CountryData).Assembly.GetType("Atom.VPN.Demo.CountryData")
                ?.GetProperty("AllCountries")?.GetValue(null) as System.Collections.Generic.List<Atom.VPN.Demo.CountryInfo>;
                
            if (countries != null)
            {
                CountryComboBox.ItemsSource = countries.Select(c => c.Name).ToList();
                
                // Setup ComboBox filtering when typing
                CountryComboBox.IsEditable = true;
                CountryComboBox.StaysOpenOnEdit = true;
                CountryComboBox.IsTextSearchEnabled = true;
                
                // Add event handler for text filtering
                CountryComboBox.KeyUp += CountryComboBox_KeyUp;
            }
            else
            {
                // Fallback - hardcode a few countries for testing
                CountryComboBox.ItemsSource = new List<string> { "United States", "Canada", "United Kingdom", "Australia" };
            }
        }
        
        private void CountryComboBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            
            if (comboBox.IsDropDownOpen)
            {
                // Get the search text from the editable part
                string searchText = comboBox.Text.ToLower();
                
                // Get the original list
                var originalList = GetOriginalCountryList();
                
                // Filter the countries based on the search text
                if (!string.IsNullOrEmpty(searchText))
                {
                    var filteredItems = originalList.Where(c => c.ToLower().Contains(searchText)).ToList();
                    comboBox.ItemsSource = filteredItems;
                    
                    // Keep the dropdown open during filtering
                    comboBox.IsDropDownOpen = true;
                }
                else
                {
                    // If empty, show all
                    comboBox.ItemsSource = originalList;
                }
            }
        }
        
        private List<string> GetOriginalCountryList()
        {
            // Try to get the original countries list
            var countries = typeof(Atom.VPN.Demo.CountryData).Assembly.GetType("Atom.VPN.Demo.CountryData")
                ?.GetProperty("AllCountries")?.GetValue(null) as System.Collections.Generic.List<Atom.VPN.Demo.CountryInfo>;
                
            if (countries != null)
            {
                return countries.Select(c => c.Name).ToList();
            }
            else
            {
                // Fallback
                return new List<string> { "United States", "Canada", "United Kingdom", "Australia" };
            }
        }

        private async void EditProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            _isCountryComboBoxLoaded = false; // Reset flag
            await LoadUserProfileAsync();
            _isCountryComboBoxLoaded = true; // Set flag after loading profile data
        }

        private async Task LoadUserProfileAsync()
        {
            string token = AuthTokenManager.CurrentToken;
            if (string.IsNullOrEmpty(token))
            {
                new ErrorDialog("Authentication Error", "You are not logged in. Please log in again.").ShowDialog();
                // Optionally navigate to login page
                // var mainWin = Application.Current.MainWindow as MainContainerWindow;
                // mainWin?.NavigateToLoginPage();
                return;
            }

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://203.161.50.155:3001/api/auth/me");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    UserProfileDataResponse profileResponse = JsonConvert.DeserializeObject<UserProfileDataResponse>(responseBody);

                    if (profileResponse != null && profileResponse.Success && profileResponse.Data != null)
                    {
                        var userData = profileResponse.Data;
                        NameTextBox.Text = userData.Name;
                        EmailTextBox.Text = userData.Email;
                        PhoneTextBox.Text = userData.Phone ?? "";
                        
                        // Convert country code to name and select in ComboBox
                        string countryName = GetCountryNameByCode(userData.Country ?? "");
                        
                        // Force the text to show regardless of whether it's in the list
                        if (CountryComboBox.IsEditable)
                        {
                            CountryComboBox.Text = countryName;
                        }
                        
                        // Also try to select in the list if it exists
                        CountryComboBox.SelectedItem = countryName;
                        
                        // If countryName was the code itself (not found), ComboBox won't select. Handle as needed.

                    }
                    else
                    {
                        string errorMessage = profileResponse?.Message ?? "Failed to load profile data.";
                        new ErrorDialog("Profile Load Error", errorMessage).ShowDialog();
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    new ErrorDialog("API Error", $"Failed to load profile. Status: {response.StatusCode}. Details: {errorContent}").ShowDialog();
                }
            }
            catch (HttpRequestException httpEx)
            {
                new ErrorDialog("Network Error", $"Could not connect to server: {httpEx.Message}").ShowDialog();
            }
            catch (JsonException jsonEx)
            {
                new ErrorDialog("Data Error", $"Error parsing profile data: {jsonEx.Message}").ShowDialog();
            }
            catch (Exception ex)
            {
                new ErrorDialog("Error", $"An unexpected error occurred: {ex.Message}").ShowDialog();
            }
        }

        private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isCountryComboBoxLoaded || CountryComboBox.SelectedItem == null) // Check flag and if an item is selected
            {
                return;
            }

            string selectedCountryName = CountryComboBox.SelectedItem.ToString();
            var selectedCountryInfo = GetCountryInfoByName(selectedCountryName);

            if (selectedCountryInfo != null)
            {
                string phoneCode = GetCountryInfoProperty(selectedCountryInfo, "PhoneCode");
                
                if (!string.IsNullOrEmpty(phoneCode))
                {
                    // Get current phone number
                    string currentPhone = PhoneTextBox.Text;
                    
                    // Check if the current phone number starts with any known country code
                    bool hasKnownPrefix = false;
                    string existingPrefix = "";
                    
                    try
                    {
                        var countriesField = typeof(Atom.VPN.Demo.CountryData).Assembly
                            .GetType("Atom.VPN.Demo.CountryData")?
                            .GetField("AllCountries");
                            
                        if (countriesField != null)
                        {
                            var countries = countriesField.GetValue(null) as System.Collections.IEnumerable;
                            if (countries != null)
                            {
                                foreach (var country in countries)
                                {
                                    var countryPhoneCode = country.GetType().GetProperty("PhoneCode")?.GetValue(country) as string;
                                    if (!string.IsNullOrEmpty(countryPhoneCode) && currentPhone.StartsWith(countryPhoneCode))
                                    {
                                        hasKnownPrefix = true;
                                        existingPrefix = countryPhoneCode;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch 
                    {
                        // Fallback to simple check for common prefixes if reflection fails
                        // Add common prefixes here for checking
                        string[] commonPrefixes = new[] { "+1", "+44", "+61", "+33", "+49", "+86", "+91" };
                        foreach (string prefix in commonPrefixes)
                        {
                            if (currentPhone.StartsWith(prefix))
                            {
                                hasKnownPrefix = true;
                                existingPrefix = prefix;
                                break;
                            }
                        }
                    }
                    
                    // Update the phone number
                    if (string.IsNullOrEmpty(currentPhone))
                    {
                        // If empty, just set the new code
                        PhoneTextBox.Text = phoneCode;
                    }
                    else if (hasKnownPrefix)
                    {
                        // Replace only the prefix part
                        PhoneTextBox.Text = phoneCode + currentPhone.Substring(existingPrefix.Length);
                    }
                    else
                    {
                        // If no known prefix, add the new country code in front of existing number
                        PhoneTextBox.Text = phoneCode + " " + currentPhone;
                    }
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string token = AuthTokenManager.CurrentToken;
            if (string.IsNullOrEmpty(token))
            {
                new ErrorDialog("Authentication Error", "You are not logged in. Please log in again.").ShowDialog();
                return;
            }

            // Provide immediate feedback
            SaveButton.IsEnabled = false;
            SaveButton.Content = "Saving...";

            // Get selected country code from ComboBox
            string selectedCountryName = CountryComboBox.SelectedItem?.ToString();
            string countryCodeToSave = null;
            if (!string.IsNullOrEmpty(selectedCountryName))
            {
                var selectedCountryInfo = GetCountryInfoByName(selectedCountryName);
                if (selectedCountryInfo != null)
                {
                    countryCodeToSave = GetCountryInfoProperty(selectedCountryInfo, "Code");
                }
            }

            var updateRequest = new UpdateProfileRequest
            {
                Name = NameTextBox.Text,
                Email = EmailTextBox.Text,
                Phone = PhoneTextBox.Text,
                Country = countryCodeToSave 
            };

            try
            {
                string jsonData = JsonConvert.SerializeObject(updateRequest);
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "http://203.161.50.155:3001/api/auth/profile");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming the response might contain the updated user or a success message
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // You could deserialize if the backend sends back the updated user or a specific success object
                    // For now, a generic success message.
                    new ErrorDialog("Success", "Profile updated successfully!").ShowDialog(); 
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    // Try to parse a more specific error message if the API returns one in a known format
                    try 
                    {
                        var errorResponse = JsonConvert.DeserializeObject<UserProfileDataResponse>(errorContent); // Reusing for {success, message} structure
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message)) 
                        {
                            new ErrorDialog("Update Error", errorResponse.Message).ShowDialog();
                            return;
                        }
                    }
                    catch { /* Ignore if parsing fails, just show raw error */ }
                    new ErrorDialog("API Error", $"Failed to update profile. Status: {response.StatusCode}. Details: {errorContent}").ShowDialog();
                }
            }
            catch (HttpRequestException httpEx)
            {
                new ErrorDialog("Network Error", $"Could not connect to server: {httpEx.Message}").ShowDialog();
            }
            catch (JsonException jsonEx)
            {
                new ErrorDialog("Data Error", $"Error processing update data: {jsonEx.Message}").ShowDialog();
            }
            catch (Exception ex)
            {
                new ErrorDialog("Error", $"An unexpected error occurred: {ex.Message}").ShowDialog();
            }
            finally
            {
                // Restore button state
                SaveButton.IsEnabled = true;
                SaveButton.Content = "Save";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        // Helper methods to replace direct CountryData calls
        private string GetCountryNameByCode(string code)
        {
            try
            {
                var methodInfo = typeof(Atom.VPN.Demo.CountryData).Assembly
                    .GetType("Atom.VPN.Demo.CountryData")?
                    .GetMethod("GetCountryNameByCode");

                if (methodInfo != null)
                {
                    return methodInfo.Invoke(null, new object[] { code }) as string ?? code;
                }
            }
            catch
            {
                // Fallback if reflection fails
            }
            
            // Simple fallback
            if (code == "US") return "United States";
            if (code == "CA") return "Canada";
            if (code == "GB") return "United Kingdom";
            if (code == "AU") return "Australia";
            return code;
        }

        private object GetCountryInfoByName(string name)
        {
            try
            {
                var methodInfo = typeof(Atom.VPN.Demo.CountryData).Assembly
                    .GetType("Atom.VPN.Demo.CountryData")?
                    .GetMethod("GetCountryInfoByName");

                if (methodInfo != null)
                {
                    return methodInfo.Invoke(null, new object[] { name });
                }
            }
            catch
            {
                // Fallback if reflection fails
            }
            
            return null;
        }

        private string GetCountryInfoProperty(object countryInfo, string propertyName)
        {
            if (countryInfo == null) return null;
            
            try
            {
                return countryInfo.GetType().GetProperty(propertyName)?.GetValue(countryInfo) as string;
            }
            catch
            {
                return null;
            }
        }
    }
} 