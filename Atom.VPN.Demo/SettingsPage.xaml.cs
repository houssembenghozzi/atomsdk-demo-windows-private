using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SettingsPage: {ex.Message}");
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
    }
} 