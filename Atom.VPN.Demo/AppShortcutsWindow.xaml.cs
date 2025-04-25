using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for AppShortcutsWindow.xaml
    /// </summary>
    public partial class AppShortcutsWindow : Window
    {
        public Dictionary<string, string> AppShortcuts { get; private set; }
        private ObservableCollection<ShortcutItem> _shortcutItems;
        
        // List of all available shortcuts
        private readonly List<ShortcutInfo> _availableShortcuts = new List<ShortcutInfo>
        {
            new ShortcutInfo { Name = "Chrome", Path = "https://www.google.com/chrome/" },
            new ShortcutInfo { Name = "Firefox", Path = "https://www.mozilla.org/firefox/download/" },
            new ShortcutInfo { Name = "Facebook", Path = "https://www.facebook.com" },
            new ShortcutInfo { Name = "Instagram", Path = "https://www.instagram.com" },
            new ShortcutInfo { Name = "Snapchat", Path = "https://www.snapchat.com" },
            new ShortcutInfo { Name = "Netflix", Path = "https://www.netflix.com" },
            new ShortcutInfo { Name = "Prime", Path = "https://www.primevideo.com" }
        };
        
        public AppShortcutsWindow(Dictionary<string, string> existingShortcuts)
        {
            InitializeComponent();
            
            // Initialize with existing shortcuts
            AppShortcuts = new Dictionary<string, string>(existingShortcuts);
            
            // Populate the list
            InitializeShortcutItems();
        }
        
        private void InitializeShortcutItems()
        {
            _shortcutItems = new ObservableCollection<ShortcutItem>();
            
            // Add all available shortcuts
            foreach (var shortcut in _availableShortcuts)
            {
                // Check if the shortcut is currently selected
                bool isSelected = AppShortcuts.ContainsKey(shortcut.Name);
                
                _shortcutItems.Add(new ShortcutItem
                {
                    Key = shortcut.Name,
                    Value = shortcut.Path,
                    IsSelected = isSelected
                });
            }
            
            ShortcutsItemsControl.ItemsSource = _shortcutItems;
        }
        
        private void ToggleShortcut_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is string shortcutName)
            {
                var shortcutItem = _shortcutItems.FirstOrDefault(item => item.Key == shortcutName);
                if (shortcutItem != null)
                {
                    // Toggle selection
                    shortcutItem.IsSelected = !shortcutItem.IsSelected;
                    
                    // Refresh the UI
                    int index = _shortcutItems.IndexOf(shortcutItem);
                    _shortcutItems.Remove(shortcutItem);
                    _shortcutItems.Insert(index, shortcutItem);
                }
            }
        }
        
        private void SaveButton_Click(object sender, MouseButtonEventArgs e)
        {
            // Update the shortcuts based on selection
            AppShortcuts.Clear();
            
            foreach (var item in _shortcutItems.Where(i => i.IsSelected).Take(6))
            {
                if (!AppShortcuts.ContainsKey(item.Key))
                {
                    var shortcutInfo = _availableShortcuts.FirstOrDefault(s => s.Name == item.Key);
                    if (shortcutInfo != null)
                    {
                        AppShortcuts.Add(item.Key, shortcutInfo.Path);
                    }
                }
            }
            
            DialogResult = true;
            Close();
        }
    }
    
    // Data model for shortcut items in the list
    public class ShortcutItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        
        public string Key { get; set; }
        public string Value { get; set; }
        
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    // Data model for available shortcuts
    public class ShortcutInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
    
    // Converter to get app icon from name
    public class AppIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string appName)
            {
                try
                {
                    string iconPath = $"/Resources/icon/{appName.ToLower()}.svg";
                    return new BitmapImage(new Uri(iconPath, UriKind.Relative));
                }
                catch
                {
                    // Return null if icon not found
                    return null;
                }
            }
            return null;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    // Converter to get background color based on selection state
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return new SolidColorBrush(Color.FromRgb(220, 0, 78)); // Selected: Red color
            }
            return new SolidColorBrush(Color.FromRgb(231, 231, 231)); // Not selected: Light gray
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    // Converter to get visibility based on selection state
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 