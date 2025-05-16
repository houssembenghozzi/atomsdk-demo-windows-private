using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for ServerLocationPage.xaml
    /// </summary>
    public partial class ServerLocationPage : Page
    {
        private List<Border> _allCountryItems = new List<Border>();
        private string _defaultSearchText = "Search country";
        
        public ServerLocationPage()
        {
            InitializeComponent();
            
            // Store all country items for searching
            foreach (var child in CountriesPanel.Children)
            {
                if (child is Border border)
                {
                    _allCountryItems.Add(border);
                }
            }
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main VPN page
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
                FilterCountries(textBox.Text);
            }
            else
            {
                // Show all countries
                foreach (var country in _allCountryItems)
                {
                    country.Visibility = Visibility.Visible;
                }
            }
        }
        
        private void FilterCountries(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all countries if search is empty
                foreach (var country in _allCountryItems)
                {
                    country.Visibility = Visibility.Visible;
                }
                return;
            }
            
            // Filter countries based on search text
            foreach (var country in _allCountryItems)
            {
                string countryName = country.Tag?.ToString() ?? "";
                
                if (countryName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    country.Visibility = Visibility.Visible;
                }
                else
                {
                    country.Visibility = Visibility.Collapsed;
                }
            }
        }
        
        private void CountryItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border countryBorder && countryBorder.Tag is string countryName)
            {
                // Get the flag emoji from the country item
                string flagEmoji = "";
                var childBorder = countryBorder.Child as Grid;
                if (childBorder != null)
                {
                    var flagContainer = (childBorder.Children[0] as Border);
                    if (flagContainer != null && flagContainer.Child is TextBlock flagText)
                    {
                        flagEmoji = flagText.Text;
                    }
                }
                
                // Navigate to the CitySelectionPage instead of returning to main page
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new CitySelectionPage(countryName, flagEmoji));
                }
            }
        }
    }
} 