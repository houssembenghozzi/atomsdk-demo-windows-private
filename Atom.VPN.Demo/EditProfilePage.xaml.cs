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
        private static UserProfileData _cachedUserProfileData = null; // Added for caching

        public EditProfilePage()
        {
            InitializeComponent();
            
            // Populate the country dropdown first
            PopulateCountryComboBox();
            
            // Then set up event handlers
            Loaded += EditProfilePage_Loaded;
            SaveButton.Click += SaveButton_Click;
        }

        private void PopulateCountryComboBox()
        {
            // Clear any existing items first
            CountryComboBox.Items.Clear();
            
            try
            {
                // Get the countries list
                var countries = CountryData.AllCountries;
                
                if (countries != null && countries.Count > 0)
                {
                    // Add countries as ComboBoxItems for better styling and search
                    foreach (var country in countries)
                    {
                        if (!string.IsNullOrEmpty(country.Name))
                        {
                            ComboBoxItem item = new ComboBoxItem();
                            item.Content = country.Name;
                            item.Tag = country; // Store the country object for later reference
                            CountryComboBox.Items.Add(item);
                        }
                    }
                }
                else
                {
                    // Fallback - hardcode a few countries for testing
                    string[] fallbackCountries = { "United States", "Canada", "United Kingdom", "Australia" };
                    foreach (string country in fallbackCountries)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = country;
                        CountryComboBox.Items.Add(item);
                    }
                }
            }
            catch
            {
                // In case of any error, use the fallback countries
                string[] fallbackCountries = { "United States", "Canada", "United Kingdom", "Australia" };
                foreach (string country in fallbackCountries)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = country;
                    CountryComboBox.Items.Add(item);
                }
            }
        }
        
        private async void EditProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_cachedUserProfileData != null)
            {
                PopulateUIFromData(_cachedUserProfileData);
            }
            else
            {
                LoadingOverlay.Visibility = Visibility.Visible; // Show loading indicator
                await LoadUserProfileAsync();
            }
        }

        // New method to populate UI from data object
        private void PopulateUIFromData(UserProfileData userData)
        {
            NameTextBox.Text = userData.Name;
            EmailTextBox.Text = userData.Email;
            // PhoneTextBox.Text = userData.Phone ?? ""; // Initial assignment

            string countryName = GetCountryNameByCode(userData.Country ?? "");
            SelectCountryByName(countryName); // This might trigger CountryComboBox_SelectionChanged
            UpdateCountryCode(countryName); // Sets CountryCodeTextBlock

            // Set phone number without country code, after country code is determined
            if (!string.IsNullOrEmpty(userData.Phone))
            {
                string phoneCodeWithPlus = CountryCodeTextBlock.Text; // e.g., "+1"
                string phoneCode = phoneCodeWithPlus.StartsWith("+") ? phoneCodeWithPlus.Substring(1) : phoneCodeWithPlus;

                // Check if the userData.Phone starts with the numeric phone code (e.g. "1" for "+1")
                // Or if it starts with the phone code including '+'
                if (!string.IsNullOrEmpty(phoneCode) && userData.Phone.StartsWith(phoneCode) && !userData.Phone.StartsWith("+")) 
                {
                     // If userData.Phone is like "1XXXXXXXXXX" and phoneCode is "1"
                    if (userData.Phone.Length > phoneCode.Length && char.IsDigit(userData.Phone[phoneCode.Length]))
                    {
                        PhoneTextBox.Text = userData.Phone.Substring(phoneCode.Length).Trim();
                    }
                    else if (userData.Phone.Equals(phoneCode)) // Handles case where phone is just the code
                    {
                         PhoneTextBox.Text = "";
                    }
                    else 
                    {
                        PhoneTextBox.Text = userData.Phone; // Fallback if substring logic is complex
                    }
                }
                else if (!string.IsNullOrEmpty(phoneCodeWithPlus) && userData.Phone.StartsWith(phoneCodeWithPlus))
                {
                     // If userData.Phone is like "+1XXXXXXXXXX" and phoneCodeWithPlus is "+1"
                     PhoneTextBox.Text = userData.Phone.Substring(phoneCodeWithPlus.Length).Trim();
                }
                else
                {
                    PhoneTextBox.Text = userData.Phone ?? ""; // Default if no specific prefix matches
                }
            }
            else
            {
                PhoneTextBox.Text = ""; // Clear if userData.Phone is null or empty
            }
        }

        private async Task LoadUserProfileAsync()
        {
            // Ensure overlay is visible if called directly and cache is empty, 
            // though EditProfilePage_Loaded should handle the initial call.
            if (_cachedUserProfileData == null) 
            {
                LoadingOverlay.Visibility = Visibility.Visible;
            }

            string token = AuthTokenManager.CurrentToken;
            if (string.IsNullOrEmpty(token))
            {
                new ErrorDialog("Authentication Error", "You are not logged in. Please log in again.").ShowDialog();
                LoadingOverlay.Visibility = Visibility.Collapsed; // Hide on early exit
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
                        _cachedUserProfileData = userData; // Cache the data
                        PopulateUIFromData(userData); // Populate UI using the new method
                        
                        // Original population logic removed from here
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
            finally
            {
                LoadingOverlay.Visibility = Visibility.Collapsed; // Hide loading indicator in all cases
            }
        }

        private void UpdateCountryCode(string countryName)
        {
            // Get phone code for selected country
            string phoneCode = GetCountryCodeFromCountryName(countryName);
            
            // Update the country code display
            CountryCodeTextBlock.Text = !string.IsNullOrEmpty(phoneCode) ? phoneCode : "+00";
        }
        
        private string GetCountryCodeFromCountryName(string countryName)
        {
            if (string.IsNullOrEmpty(countryName))
                return "";
                
            var countryInfo = GetCountryInfoByName(countryName);
            return GetCountryInfoProperty(countryInfo, "PhoneCode") ?? "";
        }

        private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Remove the flag check that was preventing initial clicks from working
            // Only validate that an item is actually selected
            if (CountryComboBox.SelectedItem == null)
            {
                return;
            }

            if (CountryComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedCountryName = selectedItem.Content.ToString();
                // Update country code for phone
                UpdateCountryCode(selectedCountryName);
            }
        }

        private void SelectCountryByName(string countryName)
        {
            if (string.IsNullOrEmpty(countryName))
                return;
                
            foreach (ComboBoxItem item in CountryComboBox.Items)
            {
                if (item.Content.ToString() == countryName)
                {
                    CountryComboBox.SelectedItem = item;
                    break;
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
            string countryCodeToSave = null;
            
            if (CountryComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedCountryName = selectedItem.Content.ToString();
                
                // Try to get from Tag first (which would contain the actual CountryInfo object)
                if (selectedItem.Tag != null)
                {
                    countryCodeToSave = GetCountryInfoProperty(selectedItem.Tag, "Code");
                }
                
                // If Tag wasn't available, look up by name
                if (string.IsNullOrEmpty(countryCodeToSave))
                {
                    var selectedCountryInfo = GetCountryInfoByName(selectedCountryName);
                    if (selectedCountryInfo != null)
                    {
                        countryCodeToSave = GetCountryInfoProperty(selectedCountryInfo, "Code");
                    }
                }
            }

            // Get the full phone number with country code
            string fullPhoneNumber = CountryCodeTextBlock.Text;
            if (!string.IsNullOrEmpty(PhoneTextBox.Text))
            {
                fullPhoneNumber += " " + PhoneTextBox.Text.Trim();
            }

            var updateRequest = new UpdateProfileRequest
            {
                Name = NameTextBox.Text,
                Email = EmailTextBox.Text,
                Country = countryCodeToSave,
                Phone = fullPhoneNumber.Trim()
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