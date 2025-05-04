using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for LanguagePage.xaml
    /// </summary>
    public partial class LanguagePage : Page
    {
        // Track the currently selected language
        private string _selectedLanguage = "English (US)";
        
        public LanguagePage()
        {
            try
            {
                InitializeComponent();
                LoadCurrentLanguage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing LanguagePage: {ex.Message}");
            }
        }
        
        private void LoadCurrentLanguage()
        {
            try
            {
                // Get the selected language from App properties if available
                if (App.Current.Properties.Contains("SelectedLanguage"))
                {
                    string savedLanguage = App.Current.Properties["SelectedLanguage"] as string;
                    if (!string.IsNullOrEmpty(savedLanguage))
                    {
                        _selectedLanguage = savedLanguage;
                        
                        // Set the corresponding radio button to checked
                        foreach (var border in FindVisualChildren<Border>(this))
                        {
                            if (border.Child is Grid grid)
                            {
                                foreach (var radioButton in FindVisualChildren<RadioButton>(grid))
                                {
                                    string tag = radioButton.Tag as string;
                                    if (tag == _selectedLanguage)
                                    {
                                        radioButton.IsChecked = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading current language: {ex.Message}");
            }
        }
        
        private void Language_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton radioButton = sender as RadioButton;
                if (radioButton != null && radioButton.Tag != null)
                {
                    _selectedLanguage = radioButton.Tag.ToString();
                    Console.WriteLine($"Selected language: {_selectedLanguage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting language: {ex.Message}");
            }
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save the selected language
                SaveSelectedLanguage();
                
                // Navigate back
                if (NavigationService != null && NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    // Fallback if NavigationService is null or can't go back
                    MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.NavigateToSettingsPage();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving language: {ex.Message}");
            }
        }
        
        private void SaveSelectedLanguage()
        {
            try
            {
                // In a real app, this would save the language to settings
                // For this demo, we'll just store it in app properties
                App.Current.Properties["SelectedLanguage"] = _selectedLanguage;
                Console.WriteLine($"Language saved: {_selectedLanguage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving language: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NavigationService != null && NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    // Fallback if NavigationService is null or can't go back
                    MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.NavigateToSettingsPage();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
            }
        }
        
        // Helper method to find all visual children of a specific type
        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
} 