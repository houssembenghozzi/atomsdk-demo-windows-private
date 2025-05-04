using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for ProtocolPage.xaml
    /// </summary>
    public partial class ProtocolPage : Page
    {
        // Track the currently selected protocol
        private string _selectedProtocol = "Automatic";
        
        public ProtocolPage()
        {
            try
            {
                InitializeComponent();
                
                // Add event handlers for all radio buttons
                foreach (var border in FindVisualChildren<Border>(this))
                {
                    if (border.Child is Grid grid)
                    {
                        foreach (var child in FindVisualChildren<RadioButton>(grid))
                        {
                            child.Checked += RadioButton_Checked;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing ProtocolPage: {ex.Message}");
            }
        }
        
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton radioButton = sender as RadioButton;
                if (radioButton != null)
                {
                    // Find the parent grid and get the protocol name from the TextBlock
                    Grid parentGrid = FindParent<Grid>(radioButton);
                    if (parentGrid != null)
                    {
                        foreach (var child in FindVisualChildren<TextBlock>(parentGrid))
                        {
                            if (child.FontSize == 16) // The protocol name TextBlock has FontSize 16
                            {
                                _selectedProtocol = child.Text;
                                Console.WriteLine($"Selected protocol: {_selectedProtocol}");
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting protocol: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save the selected protocol before navigating back
                SaveSelectedProtocol();
                
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
        
        private void SaveSelectedProtocol()
        {
            try
            {
                // In a real app, this would save the protocol to settings
                // For this demo, we'll just print to console
                Console.WriteLine($"Saving protocol selection: {_selectedProtocol}");
                
                // Update the protocol text in SettingsPage
                // This is just a simple demo implementation
                App.Current.Properties["SelectedProtocol"] = _selectedProtocol;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving protocol: {ex.Message}");
            }
        }
        
        // Helper method to find a parent of a specific type
        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            
            if (parentObject == null)
                return null;
                
            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
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