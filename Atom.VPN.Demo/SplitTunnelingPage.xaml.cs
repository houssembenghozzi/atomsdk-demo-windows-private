using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for SplitTunnelingPage.xaml
    /// </summary>
    public partial class SplitTunnelingPage : Page, INotifyPropertyChanged
    {
        private const string MODE_ALL_APPS = "AllApps";
        private const string MODE_DO_NOT_ALLOW = "DoNotAllow";
        private const string MODE_ONLY_ALLOW = "OnlyAllow";
        
        private string _currentMode = MODE_ONLY_ALLOW;
        private HashSet<string> _selectedApps = new HashSet<string>();
        private bool _isAppListVisible = true;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public bool IsAppListVisible
        {
            get { return _isAppListVisible; }
            set 
            { 
                _isAppListVisible = value;
                OnPropertyChanged("IsAppListVisible");
            }
        }
        
        public SplitTunnelingPage()
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                LoadSavedSettings();
                UpdateUI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SplitTunnelingPage: {ex.Message}");
            }
        }
        
        private void LoadSavedSettings()
        {
            try
            {
                // Load split tunneling mode
                if (App.Current.Properties.Contains("SplitTunnelingMode"))
                {
                    string savedMode = App.Current.Properties["SplitTunnelingMode"] as string;
                    if (!string.IsNullOrEmpty(savedMode))
                    {
                        _currentMode = savedMode;
                    }
                }
                
                // Load selected apps
                if (App.Current.Properties.Contains("SplitTunnelingApps"))
                {
                    string[] savedApps = App.Current.Properties["SplitTunnelingApps"] as string[];
                    if (savedApps != null && savedApps.Length > 0)
                    {
                        _selectedApps = new HashSet<string>(savedApps);
                    }
                }
                else
                {
                    // Default selection for demo
                    _selectedApps.Add("Chrome");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading split tunneling settings: {ex.Message}");
            }
        }
        
        private void UpdateUI()
        {
            try
            {
                // Set radio button state
                AllAppsRadio.IsChecked = (_currentMode == MODE_ALL_APPS);
                DoNotAllowRadio.IsChecked = (_currentMode == MODE_DO_NOT_ALLOW);
                OnlyAllowRadio.IsChecked = (_currentMode == MODE_ONLY_ALLOW);
                
                // Show/hide app list based on mode
                IsAppListVisible = (_currentMode != MODE_ALL_APPS);
                
                // Set app buttons
                UpdateAppButtons();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating UI: {ex.Message}");
            }
        }
        
        private void UpdateAppButtons()
        {
            try
            {
                // Update all app buttons based on selection state
                foreach (var button in FindVisualChildren<Button>(this))
                {
                    if (button.Tag != null)
                    {
                        string appName = button.Tag.ToString();
                        if (_selectedApps.Contains(appName))
                        {
                            button.Style = FindResource("AppSelectedButtonStyle") as Style;
                        }
                        else
                        {
                            button.Style = FindResource("AppAddButtonStyle") as Style;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating app buttons: {ex.Message}");
            }
        }
        
        private void SplitTunnelingOption_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton radioButton = sender as RadioButton;
                if (radioButton != null && radioButton.Tag != null)
                {
                    _currentMode = radioButton.Tag.ToString();
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting split tunneling option: {ex.Message}");
            }
        }
        
        private void AppToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null && button.Tag != null)
                {
                    string appName = button.Tag.ToString();
                    
                    // Toggle app selection
                    if (_selectedApps.Contains(appName))
                    {
                        _selectedApps.Remove(appName);
                        button.Style = FindResource("AppAddButtonStyle") as Style;
                    }
                    else
                    {
                        _selectedApps.Add(appName);
                        button.Style = FindResource("AppSelectedButtonStyle") as Style;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling app: {ex.Message}");
            }
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveSettings();
                
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
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
        
        private void SaveSettings()
        {
            try
            {
                // Save split tunneling mode
                App.Current.Properties["SplitTunnelingMode"] = _currentMode;
                
                // Save selected apps
                App.Current.Properties["SplitTunnelingApps"] = _selectedApps.ToArray();
                
                Console.WriteLine($"Split tunneling settings saved. Mode: {_currentMode}, Apps: {string.Join(", ", _selectedApps)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving split tunneling settings: {ex.Message}");
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
        
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 