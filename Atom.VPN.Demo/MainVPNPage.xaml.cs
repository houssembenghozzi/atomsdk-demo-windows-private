using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Navigation;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for MainVPNPage.xaml
    /// </summary>
    public partial class MainVPNPage : Page
    {
        private bool _isConnected = false;
        private bool _isConnecting = false;
        private DispatcherTimer _connectionTimer;
        private Dictionary<string, string> _appShortcuts = new Dictionary<string, string>();

        public MainVPNPage()
        {
            InitializeComponent();
            
            // Create connection timer
            _connectionTimer = new DispatcherTimer();
            _connectionTimer.Interval = TimeSpan.FromSeconds(3);
            _connectionTimer.Tick += ConnectionTimer_Tick;
            
            // Set initial visibility
            ConnectionInfoPanel.Visibility = Visibility.Collapsed;
            QuickLocationPanel.Visibility = Visibility.Visible;
            
            // Initialize app shortcuts
            InitializeAppShortcuts();
        }
        
        private void InitializeAppShortcuts()
        {
            // Default app shortcuts with download URLs as fallback when not installed
            _appShortcuts.Add("Chrome", GetProgramPath("chrome.exe", "https://www.google.com/chrome/"));
            _appShortcuts.Add("Firefox", GetProgramPath("firefox.exe", "https://www.mozilla.org/firefox/download/"));
            _appShortcuts.Add("Facebook", "https://www.facebook.com");
            _appShortcuts.Add("Instagram", "https://www.instagram.com");
            _appShortcuts.Add("Netflix", "https://www.netflix.com");
            _appShortcuts.Add("Prime", "https://www.primevideo.com");
            
            // Update the UI with shortcuts
            UpdateAppShortcutsUI();
        }
        
        private string GetProgramPath(string exeName, string downloadUrl = null)
        {
            // Common program locations - in a real app, you'd use a more robust method
            string[] commonPaths = {
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), exeName),
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), exeName),
                System.IO.Path.Combine(@"C:\Program Files", exeName),
                System.IO.Path.Combine(@"C:\Program Files (x86)", exeName)
            };
            
            foreach (string path in commonPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            
            // Return the download URL as fallback if app is not installed
            return downloadUrl ?? exeName;
        }
        
        private void LaunchApplication(string appName)
        {
            if (_appShortcuts.ContainsKey(appName))
            {
                try
                {
                    string path = _appShortcuts[appName];
                    
                    // Launch the application or URL
                    if (path.StartsWith("http"))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = path,
                            UseShellExecute = true
                        });
                    }
                    else if (File.Exists(path))
                    {
                        Process.Start(path);
                    }
                    else
                    {
                        // If file doesn't exist, try to open download page
                        string downloadUrl = null;
                        
                        switch (appName.ToLower())
                        {
                            case "chrome":
                                downloadUrl = "https://www.google.com/chrome/";
                                break;
                            case "firefox":
                                downloadUrl = "https://www.mozilla.org/firefox/download/";
                                break;
                            case "edge":
                                downloadUrl = "https://www.microsoft.com/edge";
                                break;
                            default:
                                downloadUrl = $"https://www.google.com/search?q=download+{appName}";
                                break;
                        }
                        
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = downloadUrl,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to launch {appName}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void UpdateAppShortcutsUI()
        {
            AppShortcutsPanel.Children.Clear();
            
            int count = 0;
            int row = 0;
            int column = 0;
            
            foreach (var shortcut in _appShortcuts)
            {
                if (count >= 6) break; // Max 6 icons (2 rows of 3)
                
                // Create border for the app icon
                Border shortcutBorder = new Border
                {
                    Width = 18.28,
                    Height = 18.28,
                    Background = new SolidColorBrush(GetColorFromAppName(shortcut.Key)),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(2),
                    Cursor = Cursors.Hand,
                    Tag = shortcut.Key
                };
                
                shortcutBorder.MouseDown += AppShortcut_Click;
                
                Image iconImage = new Image
                {
                    Width = 12,
                    Height = 12
                };
                
                try
                {
                    // Attempt to load the SVG icon
                    string iconPath = $"/Resources/icon/{shortcut.Key.ToLower()}.svg";
                    Uri iconUri = new Uri(iconPath, UriKind.Relative);
                    iconImage.Source = new System.Windows.Media.Imaging.BitmapImage(iconUri);
                    shortcutBorder.Child = iconImage;
                }
                catch
                {
                    // Fallback to showing first letter
                    TextBlock letterBlock = new TextBlock
                    {
                        Text = shortcut.Key.Substring(0, 1).ToUpper(),
                        FontSize = 8,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    
                    shortcutBorder.Child = letterBlock;
                }
                
                // Set Grid position
                Grid.SetRow(shortcutBorder, row);
                Grid.SetColumn(shortcutBorder, column);
                
                AppShortcutsPanel.Children.Add(shortcutBorder);
                
                // Update column and row for next icon
                column++;
                if (column >= 3)
                {
                    column = 0;
                    row++;
                }
                
                count++;
            }
        }
        
        private Color GetColorFromAppName(string appName)
        {
            switch (appName.ToLower())
            {
                case "facebook": return Color.FromRgb(66, 103, 178);  // Facebook blue
                case "instagram": return Color.FromRgb(225, 48, 108); // Instagram pink
                case "snapchat": return Color.FromRgb(255, 252, 0);   // Snapchat yellow
                case "chrome": return Color.FromRgb(66, 133, 244);    // Chrome blue
                case "firefox": return Color.FromRgb(228, 77, 38);    // Firefox orange
                case "netflix": return Color.FromRgb(229, 9, 20);     // Netflix red
                case "prime": return Color.FromRgb(0, 168, 225);      // Prime blue
                default: return Color.FromRgb(100, 100, 100);         // Default gray
            }
        }
        
        private void AppShortcut_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is string appName)
            {
                LaunchApplication(appName);
            }
        }
        
        private void AddShortcut_Click(object sender, RoutedEventArgs e)
        {
            OpenAppShortcutsManager();
        }
        
        private void OpenAppShortcutsManager()
        {
            // Create and show the app shortcuts manager window
            AppShortcutsWindow shortcutsWindow = new AppShortcutsWindow(_appShortcuts);
            shortcutsWindow.Owner = Window.GetWindow(this);
            
            if (shortcutsWindow.ShowDialog() == true)
            {
                // Update shortcuts if changes were made
                _appShortcuts = shortcutsWindow.AppShortcuts;
                UpdateAppShortcutsUI();
            }
        }
        
        private void ConnectionTimer_Tick(object sender, EventArgs e)
        {
            _connectionTimer.Stop();
            _isConnecting = false;
            _isConnected = true;
            
            // Change to connected state
            StatusText.Text = "Connected";
            StatusText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green color
            
            // Change description
            DescriptionText.Text = "Press on button to disconnect";
            
            // Change power icon to disconnect icon
            var powerIcon = GetTemplateChild(PowerButton, "PowerIcon") as System.Windows.Shapes.Path;
            if (powerIcon != null)
            {
                powerIcon.Data = Geometry.Parse("M13,3H11V13H13V3M13,17H11V21H13V17Z M12,2A10,10 0 0,1 22,12A10,10 0 0,1 12,22A10,10 0 0,1 2,12A10,10 0 0,1 12,2Z");
            }
            
            // Change button color to green
            var buttonBackground = GetTemplateChild(PowerButton, "ButtonBackground") as Shape;
            if (buttonBackground != null)
            {
                buttonBackground.Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green color
            }

            // Keep animation circles visible and showing the animations
            var animationCircles = new string[] { "AnimationCircle1", "AnimationCircle2", "AnimationCircle3" };
            foreach (var circleName in animationCircles)
            {
                var circle = GetTemplateChild(PowerButton, circleName) as Ellipse;
                if (circle != null)
                {
                    circle.Visibility = Visibility.Visible;
                    circle.Stroke = new SolidColorBrush(Colors.White);
                }
            }
            
            // Make sure wave animation is running
            var powerButtonGrid = GetTemplateChild(PowerButton, "PowerButtonGrid") as Grid;
            if (powerButtonGrid != null)
            {
                var waveAnimation = powerButtonGrid.Resources["WaveAnimation"] as Storyboard;
                if (waveAnimation != null)
                {
                    waveAnimation.Begin();
                }
            }
            
            // Show connection info panel and hide quick location panel
            ConnectionInfoPanel.Visibility = Visibility.Visible;
            QuickLocationPanel.Visibility = Visibility.Collapsed;
            
            // Optional: Add a subtle pulse effect for the connected state
            var pulseAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 1.05,
                Duration = TimeSpan.FromSeconds(1.5),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            
            PowerButton.RenderTransform = new ScaleTransform(1, 1);
            PowerButton.RenderTransformOrigin = new Point(0.5, 0.5);
            PowerButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, pulseAnimation);
            PowerButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, pulseAnimation);
        }

        private void PowerButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isConnecting)
                return; // Ignore clicks during connecting state
                
            if (_isConnected)
            {
                // Disconnect immediately
                _isConnected = false;
                
                // Change status to disconnected
                StatusText.Text = "Not Connected";
                StatusText.Foreground = new SolidColorBrush(Colors.White);
                
                // Change description
                DescriptionText.Text = "Press on button to connect";
                
                // Change power icon back to connect icon
                var powerIcon = GetTemplateChild(PowerButton, "PowerIcon") as System.Windows.Shapes.Path;
                if (powerIcon != null)
                {
                    powerIcon.Data = Geometry.Parse("M50.1054 35.1611V50.2134M59.6786 42.1454C61.5728 44.0402 62.8626 46.454 63.3849 49.0818C63.9073 51.7095 63.6388 54.4332 62.6133 56.9083C61.5878 59.3835 59.8515 61.499 57.6237 62.9873C55.396 64.4757 52.777 65.2701 50.0979 65.2701C47.4187 65.2701 44.7997 64.4757 42.572 62.9873C40.3443 61.499 38.6079 59.3835 37.5824 56.9083C36.5569 54.4332 36.2884 51.7095 36.8108 49.0818C37.3331 46.454 38.6229 44.0402 40.5171 42.1454");
                }
                
                // Change button color back to gray as in the design
                var buttonBackground = GetTemplateChild(PowerButton, "ButtonBackground") as Shape;
                if (buttonBackground != null)
                {
                    buttonBackground.Fill = new SolidColorBrush(Color.FromRgb(134, 139, 150)); // #868B96
                }
                
                // Keep our animation circles visible
                var animationCircles = new string[] { "AnimationCircle1", "AnimationCircle2", "AnimationCircle3" };
                foreach (var circleName in animationCircles)
                {
                    var circle = GetTemplateChild(PowerButton, circleName) as Ellipse;
                    if (circle != null)
                    {
                        circle.Visibility = Visibility.Visible;
                        circle.Stroke = new SolidColorBrush(Colors.White);
                    }
                }
                
                // Hide connection info panel and show quick location panel
                ConnectionInfoPanel.Visibility = Visibility.Collapsed;
                QuickLocationPanel.Visibility = Visibility.Visible;
                
                // Stop the pulsing animation but keep our wave animation
                PowerButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                PowerButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            }
            else
            {
                // Start connecting with delay
                _isConnecting = true;
                
                // Change status to connecting
                StatusText.Text = "Connecting...";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange color
                
                // Change description
                DescriptionText.Text = "Please wait...";
                
                // Change button color to yellow/orange
                var buttonBackground = GetTemplateChild(PowerButton, "ButtonBackground") as Shape;
                if (buttonBackground != null)
                {
                    buttonBackground.Fill = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange color
                }
                
                // Keep our animation circles visible during connecting state
                var animationCircles = new string[] { "AnimationCircle1", "AnimationCircle2", "AnimationCircle3" };
                foreach (var circleName in animationCircles)
                {
                    var circle = GetTemplateChild(PowerButton, circleName) as Ellipse;
                    if (circle != null)
                    {
                        circle.Visibility = Visibility.Visible;
                    }
                }
                
                // Start the connection timer
                _connectionTimer.Start();
            }
        }
        
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the MenuPage
            NavigationService.Navigate(new MenuPage());
        }
        
        // Helper method to get named elements from a control template
        private static UIElement GetTemplateChild(Button button, string childName)
        {
            if (button.Template != null)
            {
                return button.Template.FindName(childName, button) as UIElement;
            }
            return null;
        }
    }
} 