using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Input;

namespace Atom.VPN.Demo
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            try
            {
                InitializeComponent();
                EnsureRequiredResourcesExist();
                UpdateProtocolText();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SettingsPage: {ex.Message}");
            }
        }
        
        private void UpdateProtocolText()
        {
            try
            {
                // Get the selected protocol from App properties
                if (App.Current.Properties.Contains("SelectedProtocol"))
                {
                    string protocol = App.Current.Properties["SelectedProtocol"] as string;
                    
                    // Find all TextBlocks in the Protocol border and update the one showing the protocol
                    foreach (var border in FindVisualChildren<Border>(this))
                    {
                        if (border.Child is Grid grid)
                        {
                            // Find the protocol text block (column 1)
                            foreach (var child in FindVisualChildren<TextBlock>(grid))
                            {
                                if (Grid.GetColumn(child) == 1)
                                {
                                    // Find the first TextBlock in the row that contains "Protocol"
                                    TextBlock rowTitle = null;
                                    foreach (var title in FindVisualChildren<TextBlock>(grid))
                                    {
                                        if (Grid.GetColumn(title) == 0 && title.Text == "Protocol")
                                        {
                                            rowTitle = title;
                                            break;
                                        }
                                    }
                                    
                                    if (rowTitle != null)
                                    {
                                        child.Text = protocol ?? "Automatic";
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
                Console.WriteLine($"Error updating protocol text: {ex.Message}");
            }
        }
        
        private void UpdateLanguageText()
        {
            try
            {
                // Get the selected language from App properties
                if (App.Current.Properties.Contains("SelectedLanguage"))
                {
                    string language = App.Current.Properties["SelectedLanguage"] as string;
                    
                    // Find all TextBlocks in the Language border and update the one showing the language
                    foreach (var border in FindVisualChildren<Border>(this))
                    {
                        if (border.Child is Grid grid)
                        {
                            // Find the language text block (column 1)
                            TextBlock rowTitle = null;
                            foreach (var title in FindVisualChildren<TextBlock>(grid))
                            {
                                if (Grid.GetColumn(title) == 0 && title.Text == "Language")
                                {
                                    rowTitle = title;
                                    break;
                                }
                            }
                            
                            if (rowTitle != null)
                            {
                                foreach (var child in FindVisualChildren<TextBlock>(grid))
                                {
                                    if (Grid.GetColumn(child) == 1)
                                    {
                                        child.Text = language ?? "English (US)";
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
                Console.WriteLine($"Error updating language text: {ex.Message}");
            }
        }
        
        private void UpdateSplitTunnelingText()
        {
            try
            {
                // Get the split tunneling mode from App properties
                if (App.Current.Properties.Contains("SplitTunnelingMode"))
                {
                    string mode = App.Current.Properties["SplitTunnelingMode"] as string;
                    string description = "";
                    
                    // Determine description based on mode
                    switch (mode)
                    {
                        case "AllApps":
                            description = "All apps use VPN";
                            break;
                        case "DoNotAllow":
                            description = "Some apps excluded";
                            break;
                        case "OnlyAllow":
                            description = "Selected apps only";
                            break;
                        default:
                            description = "Selected apps only";
                            break;
                    }
                    
                    // Update the ToggleButton in the Split tunnelling row
                    foreach (var border in FindVisualChildren<Border>(this))
                    {
                        if (border.Child is Grid grid)
                        {
                            bool isSplitTunnelingRow = false;
                            
                            // Find the split tunneling row
                            foreach (var stackPanel in FindVisualChildren<StackPanel>(grid))
                            {
                                foreach (var textBlock in FindVisualChildren<TextBlock>(stackPanel))
                                {
                                    if (textBlock.Text == "Split tunnelling")
                                    {
                                        isSplitTunnelingRow = true;
                                        break;
                                    }
                                }
                                
                                if (isSplitTunnelingRow)
                                {
                                    break;
                                }
                            }
                            
                            if (isSplitTunnelingRow)
                            {
                                // Update the toggle button state
                                foreach (var toggleButton in FindVisualChildren<System.Windows.Controls.Primitives.ToggleButton>(grid))
                                {
                                    toggleButton.IsChecked = (mode != "AllApps");
                                    break;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating split tunneling text: {ex.Message}");
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
        
        private void EnsureRequiredResourcesExist()
        {
            // Check if ToggleSwitchStyle exists in application resources
            if (Application.Current.Resources["ToggleSwitchStyle"] == null)
            {
                try
                {
                    // Create the style programmatically
                    var style = new Style(typeof(System.Windows.Controls.Primitives.ToggleButton));
                    
                    var template = new ControlTemplate(typeof(System.Windows.Controls.Primitives.ToggleButton));
                    
                    var factoryGrid = new FrameworkElementFactory(typeof(Grid));
                    factoryGrid.SetValue(FrameworkElement.WidthProperty, 46.0);
                    factoryGrid.SetValue(FrameworkElement.HeightProperty, 24.0);
                    
                    var factoryBgBorder = new FrameworkElementFactory(typeof(Border));
                    factoryBgBorder.Name = "BackgroundElement";
                    factoryBgBorder.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(230, 230, 230)));
                    factoryBgBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(12));
                    factoryBgBorder.SetValue(FrameworkElement.WidthProperty, 46.0);
                    factoryBgBorder.SetValue(FrameworkElement.HeightProperty, 24.0);
                    
                    var factoryThumbBorder = new FrameworkElementFactory(typeof(Border));
                    factoryThumbBorder.Name = "ThumbElement";
                    factoryThumbBorder.SetValue(Border.BackgroundProperty, Brushes.White);
                    factoryThumbBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(12));
                    factoryThumbBorder.SetValue(FrameworkElement.WidthProperty, 20.0);
                    factoryThumbBorder.SetValue(FrameworkElement.HeightProperty, 20.0);
                    factoryThumbBorder.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    factoryThumbBorder.SetValue(FrameworkElement.MarginProperty, new Thickness(2, 2, 0, 0));
                    
                    var effect = new DropShadowEffect
                    {
                        ShadowDepth = 1,
                        BlurRadius = 2,
                        Opacity = 0.3
                    };
                    factoryThumbBorder.SetValue(Border.EffectProperty, effect);
                    
                    factoryGrid.AppendChild(factoryBgBorder);
                    factoryGrid.AppendChild(factoryThumbBorder);
                    
                    template.VisualTree = factoryGrid;
                    
                    // Add triggers
                    var checkedTrigger = new Trigger
                    {
                        Property = System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
                        Value = true
                    };
                    
                    var setter1 = new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(233, 64, 87)));
                    setter1.TargetName = "BackgroundElement";
                    checkedTrigger.Setters.Add(setter1);
                    
                    var setter2 = new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right);
                    setter2.TargetName = "ThumbElement";
                    checkedTrigger.Setters.Add(setter2);
                    
                    var setter3 = new Setter(FrameworkElement.MarginProperty, new Thickness(0, 2, 2, 0));
                    setter3.TargetName = "ThumbElement";
                    checkedTrigger.Setters.Add(setter3);
                    
                    template.Triggers.Add(checkedTrigger);
                    
                    style.Setters.Add(new Setter(System.Windows.Controls.Primitives.ToggleButton.TemplateProperty, template));
                    
                    // Add it to the application resources
                    Application.Current.Resources.Add("ToggleSwitchStyle", style);
                    
                    Console.WriteLine("ToggleSwitchStyle was created programmatically");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating ToggleSwitchStyle: {ex.Message}");
                    
                    // Create a simpler fallback style as a last resort
                    try
                    {
                        var fallbackStyle = new Style(typeof(System.Windows.Controls.Primitives.ToggleButton));
                        fallbackStyle.Setters.Add(new Setter(System.Windows.Controls.Control.BackgroundProperty, new SolidColorBrush(Color.FromRgb(230, 230, 230))));
                        fallbackStyle.Setters.Add(new Setter(System.Windows.Controls.Control.ForegroundProperty, Brushes.White));
                        fallbackStyle.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 46.0));
                        fallbackStyle.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 24.0));
                        
                        var checkedTrigger = new Trigger
                        {
                            Property = System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
                            Value = true
                        };
                        checkedTrigger.Setters.Add(new Setter(System.Windows.Controls.Control.BackgroundProperty, new SolidColorBrush(Color.FromRgb(233, 64, 87))));
                        fallbackStyle.Triggers.Add(checkedTrigger);
                        
                        Application.Current.Resources.Add("ToggleSwitchStyle", fallbackStyle);
                        Console.WriteLine("Fallback ToggleSwitchStyle was created");
                    }
                    catch (Exception fallbackEx)
                    {
                        Console.WriteLine($"Error creating fallback style: {fallbackEx.Message}");
                    }
                }
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
                        mainWindow.NavigateToMainVPNPage();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
            }
        }
        
        private void ProtocolRow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new ProtocolPage());
                    e.Handled = true;
                }
                else
                {
                    // Fallback if NavigationService is null
                    MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                    if (mainWindow != null && mainWindow.MainFrame != null)
                    {
                        mainWindow.MainFrame.Navigate(new ProtocolPage());
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                e.Handled = true;
            }
        }
        
        private void LanguageRow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new LanguagePage());
                    e.Handled = true;
                }
                else
                {
                    // Fallback if NavigationService is null
                    MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                    if (mainWindow != null && mainWindow.MainFrame != null)
                    {
                        mainWindow.MainFrame.Navigate(new LanguagePage());
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                e.Handled = true;
            }
        }
        
        private void SplitTunnelingRow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new SplitTunnelingPage());
                    e.Handled = true;
                }
                else
                {
                    // Fallback if NavigationService is null
                    MainContainerWindow mainWindow = Application.Current.MainWindow as MainContainerWindow;
                    if (mainWindow != null && mainWindow.MainFrame != null)
                    {
                        mainWindow.MainFrame.Navigate(new SplitTunnelingPage());
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                e.Handled = true;
            }
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // When the page is loaded, update the protocol and language texts
            UpdateProtocolText();
            UpdateLanguageText();
            UpdateSplitTunnelingText();
        }
    }
} 