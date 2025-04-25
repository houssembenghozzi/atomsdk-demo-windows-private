using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for MainVPNPage.xaml
    /// </summary>
    public partial class MainVPNPage : Page
    {
        private bool _isConnected = false;

        public MainVPNPage()
        {
            InitializeComponent();
        }

        private void PowerButton_Click(object sender, RoutedEventArgs e)
        {
            _isConnected = !_isConnected;
            
            if (_isConnected)
            {
                // Change status to connected
                var statusText = this.FindName("StatusText") as TextBlock;
                if (statusText != null)
                {
                    statusText.Text = "Connected";
                    statusText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green color
                }
                
                // Change description
                var descriptionText = this.FindName("DescriptionText") as TextBlock;
                if (descriptionText != null)
                {
                    descriptionText.Text = "Your connection is secure";
                }
                
                // Change button color to green
                var buttonBackground = GetTemplateChild(PowerButton, "ButtonBackground") as Shape;
                if (buttonBackground != null)
                {
                    buttonBackground.Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green color
                }
                
                // Change glow effect to green
                var outerCircles = new string[] { "OuterCircle1", "OuterCircle2", "OuterCircle3", "OuterCircle4" };
                foreach (var circleName in outerCircles)
                {
                    var circle = GetTemplateChild(PowerButton, circleName) as Shape;
                    if (circle != null)
                    {
                        circle.Fill = new SolidColorBrush(Color.FromArgb(40, 76, 175, 80)); // Green with alpha
                    }
                }
                
                // Animate the power button (pulse effect)
                var pulseAnimation = new DoubleAnimation
                {
                    From = 1.0,
                    To = 1.1,
                    Duration = TimeSpan.FromSeconds(1),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                
                PowerButton.RenderTransform = new ScaleTransform(1, 1);
                PowerButton.RenderTransformOrigin = new Point(0.5, 0.5);
                PowerButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, pulseAnimation);
                PowerButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, pulseAnimation);
            }
            else
            {
                // Change status to disconnected
                var statusText = this.FindName("StatusText") as TextBlock;
                if (statusText != null)
                {
                    statusText.Text = "Not Connected";
                    statusText.Foreground = new SolidColorBrush(Colors.White);
                }
                
                // Change description
                var descriptionText = this.FindName("DescriptionText") as TextBlock;
                if (descriptionText != null)
                {
                    descriptionText.Text = "Press on button to connect";
                }
                
                // Change button color back to dark
                var buttonBackground = GetTemplateChild(PowerButton, "ButtonBackground") as Shape;
                if (buttonBackground != null)
                {
                    buttonBackground.Fill = new SolidColorBrush(Color.FromRgb(34, 34, 34)); // Dark color #222222
                }
                
                // Change glow effect to red
                var outerCircles = new string[] { "OuterCircle1", "OuterCircle2", "OuterCircle3", "OuterCircle4" };
                foreach (var circleName in outerCircles)
                {
                    var circle = GetTemplateChild(PowerButton, circleName) as Shape;
                    if (circle != null)
                    {
                        circle.Fill = new SolidColorBrush(Color.FromArgb(40, 255, 26, 26)); // Red with alpha
                    }
                }
                
                // Stop animation
                PowerButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                PowerButton.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            }
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