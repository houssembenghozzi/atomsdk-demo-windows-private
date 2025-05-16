using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for CitySelectionPage.xaml
    /// </summary>
    public partial class CitySelectionPage : Page
    {
        private List<Border> _allCityItems = new List<Border>();
        private List<Border> _recommendedCityItems = new List<Border>();
        private string _defaultSearchText = "Search city";
        private string _countryName;
        private string _countryFlag;
        
        public CitySelectionPage(string countryName, string countryFlag)
        {
            InitializeComponent();
            
            _countryName = countryName;
            _countryFlag = countryFlag;
            
            // Set the country name in the header
            CountryNameText.Text = countryName;
            
            // Store all city items for searching
            foreach (var child in CitiesPanel.Children)
            {
                if (child is Border border)
                {
                    _allCityItems.Add(border);
                }
            }
            
            // Store recommended city items
            foreach (var child in RecommendedCitiesPanel.Children)
            {
                if (child is Border border)
                {
                    _recommendedCityItems.Add(border);
                }
            }
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the server location page
            if (NavigationService != null)
            {
                NavigationService.GoBack();
            }
        }
        
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == _defaultSearchText)
            {
                textBox.Text = string.Empty;
            }
        }
        
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = _defaultSearchText;
            }
        }
        
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text != _defaultSearchText)
            {
                FilterCities(textBox.Text);
            }
            else
            {
                // Show all cities
                foreach (var city in _allCityItems)
                {
                    city.Visibility = Visibility.Visible;
                }
            }
        }
        
        private void FilterCities(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all cities if search is empty
                foreach (var city in _allCityItems)
                {
                    city.Visibility = Visibility.Visible;
                }
                return;
            }
            
            // Filter cities based on search text
            foreach (var city in _allCityItems)
            {
                string cityName = city.Tag?.ToString() ?? "";
                
                if (cityName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    city.Visibility = Visibility.Visible;
                }
                else
                {
                    city.Visibility = Visibility.Collapsed;
                }
            }
        }
        
        private void CityItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border cityBorder && cityBorder.Tag is string cityName)
            {
                // Return to the main page with the selected city
                if (NavigationService != null && NavigationService.CanGoBack)
                {
                    // If we can navigate back twice (to get to main page), get the main page and update its location
                    NavigationService.GoBack(); // First go back to country selection
                    
                    var mainPage = NavigationService.Content as MainVPNPage;
                    if (mainPage != null)
                    {
                        // Format the location as "Country - City"
                        string location = $"{_countryName} - {cityName}";
                        mainPage.UpdateSelectedCountry(_countryName, _countryFlag, location);
                    }
                    
                    NavigationService.GoBack(); // Then go back to main page
                }
            }
        }
    }
} 