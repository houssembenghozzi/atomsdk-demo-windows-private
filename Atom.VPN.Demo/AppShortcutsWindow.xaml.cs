using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Data;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for AppShortcutsWindow.xaml
    /// </summary>
    public partial class AppShortcutsWindow : Window
    {
        public Dictionary<string, string> AppShortcuts { get; private set; }
        
        public AppShortcutsWindow(Dictionary<string, string> shortcuts)
        {
            InitializeComponent();
            AppShortcuts = shortcuts;
            
            // Set the dark overlay background when shown
            this.Loaded += AppShortcutsWindow_Loaded;
            this.Closed += AppShortcutsWindow_Closed;
        }
        
        private void AppShortcutsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // The dark background overlay is handled by the Rectangle in XAML
        }
        
        private void AppShortcutsWindow_Closed(object sender, EventArgs e)
        {
            // Nothing to clean up anymore since we're not applying effects to the parent window
        }
        
        private void ShortcutIcon_Click(object sender, MouseButtonEventArgs e)
        {
            // Close the dialog when an icon is clicked
            this.Close();
        }
        
        private void AddNewButton_Click(object sender, MouseButtonEventArgs e)
        {
            // Close the dialog when Add New is clicked
            this.Close();
        }
        
        private void CloseOnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            // Close the dialog when clicking on the dark background
            this.Close();
        }
    }
    
    // Converter to get app icon from name
    public class AppIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string appName)
            {
                // Get the right icon path based on the app name
                string pathData = GetPathDataForApp(appName);
                
                if (!string.IsNullOrEmpty(pathData))
                {
                    // Create a drawing visual to host the path
                    DrawingVisual drawingVisual = new DrawingVisual();
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        drawingContext.DrawRectangle(
                            GetBackgroundColorForApp(appName),
                            null,
                            new Rect(0, 0, 48, 48));
                        
                        // Create a layout transform to center the path in the visual
                        double size = 30; // Size of the path
                        double offset = (48 - size) / 2; // Center offset
                        
                        drawingContext.PushTransform(new TranslateTransform(offset, offset));
                        
                        // Special handling for Firefox icon
                        if (appName.ToLower() == "firefox")
                        {
                            // Draw with stroke and transparent fill
                            drawingContext.DrawGeometry(
                                Brushes.Transparent,
                                new Pen(Brushes.White, 1.5),
                                Geometry.Parse(pathData));
                        }
                        else
                        {
                            // Draw with white fill for other icons
                            drawingContext.DrawGeometry(
                                new SolidColorBrush(Colors.White),
                                null,
                                Geometry.Parse(pathData));
                        }
                        
                        drawingContext.Pop();
                    }
                    
                    // Create a bitmap from the drawing visual
                    RenderTargetBitmap bmp = new RenderTargetBitmap(48, 48, 96, 96, PixelFormats.Pbgra32);
                    bmp.Render(drawingVisual);
                    bmp.Freeze();
                    return bmp;
                }
            }
            
            return null;
        }
        
        private Brush GetBackgroundColorForApp(string appName)
        {
            switch (appName.ToLower())
            {
                case "chrome":
                    return new SolidColorBrush(Color.FromRgb(66, 133, 244)); // Google Blue
                case "firefox":
                    return new SolidColorBrush(Color.FromRgb(228, 77, 38)); // Firefox Orange
                case "facebook":
                    return new SolidColorBrush(Color.FromRgb(59, 89, 152)); // Facebook Blue
                case "instagram":
                    return new LinearGradientBrush(
                        new GradientStopCollection {
                            new GradientStop(Color.FromRgb(193, 53, 132), 0), // Instagram Purple
                            new GradientStop(Color.FromRgb(225, 48, 108), 0.5), // Instagram Pink
                            new GradientStop(Color.FromRgb(253, 29, 29), 1) // Instagram Orange
                        }, 
                        new Point(0, 0), 
                        new Point(1, 1));
                case "snapchat":
                    return new SolidColorBrush(Color.FromRgb(255, 252, 0)); // Snapchat Yellow
                case "netflix":
                    return new SolidColorBrush(Color.FromRgb(229, 9, 20)); // Netflix Red
                case "prime":
                    return new SolidColorBrush(Color.FromRgb(0, 168, 225)); // Prime Video Blue
                default:
                    return new SolidColorBrush(Color.FromRgb(128, 128, 128)); // Default gray
            }
        }
        
        private string GetPathDataForApp(string appName)
        {
            switch (appName.ToLower())
            {
                case "chrome":
                    return "M12,20L12.76,13.3C11.5,12.87 10.73,11.69 10.73,10.36C10.73,8.63 12.13,7.23 13.86,7.23C15.59,7.23 16.99,8.63 16.99,10.36C16.99,11.69 16.22,12.87 14.96,13.3L15.72,20M12,4C7.03,4 3,8.03 3,13C3,17.97 7.03,22 12,22C16.97,22 21,17.97 21,13C21,8.03 16.97,4 12,4Z";
                
                case "firefox":
                    return "M19.5795 7.76001C19.1869 6.81273 18.391 5.79006 17.7662 5.46701C18.2748 6.46631 18.5689 7.46863 18.6814 8.2165C18.6814 8.2165 18.6814 8.22178 18.6832 8.23158C17.6613 5.6781 15.9281 4.64827 14.5127 2.40653C14.4409 2.29345 14.3694 2.18036 14.2995 2.05936C14.26 1.99113 14.228 1.92931 14.2002 1.87089C14.1415 1.75754 14.0962 1.63774 14.0651 1.51391C14.0654 1.50803 14.0635 1.50227 14.0597 1.49774C14.0559 1.49322 14.0507 1.49025 14.0448 1.48941C14.0393 1.4879 14.0335 1.4879 14.0279 1.48941C14.0264 1.49009 14.025 1.49098 14.0238 1.49205C14.0215 1.49205 14.0193 1.49469 14.017 1.49544L14.0208 1.49054C11.7501 2.82344 10.9798 5.28909 10.9091 6.52285C10.0017 6.58504 9.1341 6.91989 8.41949 7.4837C8.34479 7.4204 8.26669 7.36126 8.18553 7.30654C7.97959 6.58402 7.97089 5.81936 8.16033 5.09232C7.23168 5.51639 6.50951 6.18548 5.98444 6.77767H5.9803C5.62185 6.32269 5.64706 4.82204 5.66737 4.5088C5.66323 4.4892 5.40032 4.64563 5.36646 4.66938C5.05017 4.89552 4.7545 5.14932 4.48294 5.42781C4.17392 5.7418 3.8916 6.0811 3.63891 6.44218C3.05757 7.26786 2.64529 8.20082 2.42591 9.18715C2.42177 9.20675 2.41801 9.22711 2.41387 9.24709C2.39694 9.32662 2.33564 9.72581 2.32473 9.81251V9.83249C2.2448 10.245 2.19502 10.6627 2.17578 11.0825V11.1288C2.17578 16.1317 6.22291 20.1877 11.2149 20.1877C15.6859 20.1877 19.3979 16.9346 20.1249 12.6615C20.14 12.5458 20.1524 12.4289 20.1659 12.3121C20.3457 10.7583 20.146 9.12533 19.5795 7.76001Z";
                
                case "facebook":
                    return "M12 2.04C6.5 2.04 2 6.53 2 12.06C2 17.06 5.66 21.21 10.44 21.96V14.96H7.9V12.06H10.44V9.85C10.44 7.34 11.93 5.96 14.22 5.96C15.31 5.96 16.45 6.15 16.45 6.15V8.62H15.19C13.95 8.62 13.56 9.39 13.56 10.18V12.06H16.34L15.89 14.96H13.56V21.96A10 10 0 0 0 22 12.06C22 6.53 17.5 2.04 12 2.04Z";
                
                case "instagram":
                    return "M7.8,2H16.2C19.4,2 22,4.6 22,7.8V16.2A5.8,5.8 0 0,1 16.2,22H7.8C4.6,22 2,19.4 2,16.2V7.8A5.8,5.8 0 0,1 7.8,2M7.6,4A3.6,3.6 0 0,0 4,7.6V16.4C4,18.39 5.61,20 7.6,20H16.4A3.6,3.6 0 0,0 20,16.4V7.6C20,5.61 18.39,4 16.4,4H7.6M17.25,5.5A1.25,1.25 0 0,1 18.5,6.75A1.25,1.25 0 0,1 17.25,8A1.25,1.25 0 0,1 16,6.75A1.25,1.25 0 0,1 17.25,5.5M12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9Z";
                
                case "snapchat":
                    return "M12,20.07L11.56,20A6.63,6.63 0 0,1 4.93,13.37C5,11.3 5.31,9.91 5.54,8.83C5.58,8.64 5.61,8.45 5.63,8.27C5.38,8.05 5.19,7.78 5.04,7.5C4.87,7.21 4.5,6.45 5.26,5.92C5.89,5.47 7.01,5.5 7.7,5.5H7.92L7.95,5.33C8,4.9 8.07,4.25 8.11,3.96C8.26,2.93 8.73,2.31 9.24,1.93C9.89,1.43 10.59,1.25 11.44,1.31C12.34,1.37 13.12,1.56 13.83,2.12C14.5,2.65 14.83,3.42 15,4.08C15.06,4.67 15.1,5.25 15.17,5.33H15.5C16.3,5.33 17.23,5.25 17.89,5.73L17.95,5.78C18.38,6.15 18.32,6.73 18.18,7.14C18.09,7.38 17.97,7.62 17.85,7.85C17.64,8.25 17.43,8.72 17.17,9.27L17.15,9.3C17.2,9.71 17.29,10.16 17.39,10.67L17.44,10.95C18,13.88 18.19,15.34 17.5,15.92C17.46,15.95 17.42,16 17.38,16C16.62,16.68 15.65,16.67 14.82,16.66C14.5,16.66 14.16,16.67 13.86,16.69C13.56,16.72 13.28,16.84 13,16.96C12.33,17.25 11.89,17.5 10.93,17.5C10.59,17.5 10.25,17.43 9.93,17.29C9.6,17.16 9.28,17.06 8.92,17.03C8.55,17 8.25,17 7.95,17C7.08,17 6.35,17.12 5.58,17.68C5.11,18 4.37,17.69 4.09,17.03C3.78,16.32 4,15.27 4.25,14.33C4.34,14 4.42,13.66 4.53,13.19C4.21,12.3 4.5,11.37 4.78,10.44C4.93,9.94 5.09,9.42 5.15,8.93C5.14,8.75 5.1,8.57 5.05,8.38C4.94,7.96 4.77,7.37 4.66,6.71L4.64,6.53C4.55,5.96 4.57,5.37 5.27,5.07C5.59,4.93 6,4.88 6.5,4.88V4.88Z";
                
                case "netflix":
                    return "M6.5,21.5H18.5V3.5H14.5V13.5L10.5,10.5L6.5,13.5V3.5H2.5V21.5H6.5Z";
                
                case "prime":
                    return "M8.17,2.81C10.6,2.08 13.22,2.25 15.53,3.28C17.85,4.32 19.7,6.15 20.7,8.48C21.7,10.81 21.9,13.43 21.2,15.86C20.5,18.28 19.03,20.36 17,21.8C14.98,23.24 12.53,23.96 10.05,23.84C7.58,23.72 5.22,22.76 3.36,21.11C1.5,19.45 0.27,17.2 -0.14,14.76C-0.55,12.33 -0.11,9.83 1.11,7.67H4.68C3.5,9.77 3.56,12.4 4.84,14.43C6.12,16.46 8.45,17.63 10.87,17.52C13.29,17.4 15.51,16.04 16.6,13.88C17.7,11.72 17.55,9.12 16.19,7.1C14.84,5.08 12.48,3.91 10.05,4.01V4.01L10.06,1C9.39,1.05 8.76,1.14 8.17,1.3V2.81M14.79,8.45H17.29V10.12H13.68V3.6H14.79V8.45M11.64,8.45V10.12H8V3.6H11.64V5.28H9.11V6.08H11.3V7.67H9.11V8.45H11.64Z";
                
                default:
                    return null;
            }
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 