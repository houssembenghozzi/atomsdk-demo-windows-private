using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.IO;
using System.Xml;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for PaymentPage.xaml
    /// </summary>
    public partial class PaymentPage : Page
    {
        // Define a tag for logging
        private const string TAG = "PaymentPage";
        private double selectedPlanPrice = 0.0;
        private string selectedPlanName = "";
        private Border selectedPlanCard = null;
        
        // Dictionary to store plan pricing
        private readonly Dictionary<string, double> planPrices = new Dictionary<string, double>
        {
            { "plan1Month", 4.99 },
            { "plan6Months", 25.99 },
            { "plan12Months", 49.99 },
            { "plan15Months", 69.99 }
        };
        
        // Dictionary to store plan names
        private readonly Dictionary<string, string> planNames = new Dictionary<string, string>
        {
            { "plan1Month", "1 Month" },
            { "plan6Months", "6 Months" },
            { "plan12Months", "12 Months" },
            { "plan15Months", "15 Months" }
        };
        
        public PaymentPage()
        {
            InitializeComponent();
            Debug.WriteLine($"{TAG}: Page Created");
            
            // Initially, no plan is selected and the Next button should be hidden
            NextButton.Visibility = Visibility.Collapsed;
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to previous page
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        
        private void PlanCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border clickedCard)
            {
                string planId = clickedCard.Tag?.ToString();
                
                if (string.IsNullOrEmpty(planId) || !planPrices.ContainsKey(planId))
                {
                    return;
                }
                
                // Store selected plan details
                selectedPlanPrice = planPrices[planId];
                selectedPlanName = planNames[planId];
                
                Debug.WriteLine($"{TAG}: Selected plan: {selectedPlanName}, Price: ${selectedPlanPrice}");
                
                // Get all plan cards
                var allCards = new List<Border> { Plan1Month, Plan6Months, Plan12Months, Plan15Months };
                
                // Update visual state for all cards
                foreach (var card in allCards)
                {
                    string cardPlanId = card.Tag?.ToString();
                    
                    if (card == clickedCard)
                    {
                        // Apply selected style
                        card.Style = FindResource("PlanCardSelectedStyle") as Style;
                        selectedPlanCard = card;
                        
                        // Apply white text styles based on the selected plan
                        ApplySelectedTextStyles(cardPlanId, true);
                    }
                    else
                    {
                        // Reset to default style
                        card.Style = FindResource("PlanCardStyle") as Style;
                        
                        // Reset text styles
                        ApplySelectedTextStyles(cardPlanId, false);
                    }
                }
                
                // Show the Next button when a plan is selected
                NextButton.Visibility = Visibility.Visible;
            }
        }
        
        private void ApplySelectedTextStyles(string planId, bool isSelected)
        {
            switch (planId)
            {
                case "plan1Month":
                    Plan1MonthTitle.Style = isSelected ? FindResource("PlanTextSelectedStyle") as Style : null;
                    Plan1MonthSubtitle.Style = isSelected ? FindResource("PlanSubtextSelectedStyle") as Style : null;
                    Plan1MonthPrice.Style = isSelected ? FindResource("PlanPriceSelectedStyle") as Style : null;
                    break;
                    
                case "plan6Months":
                    Plan6MonthsTitle.Style = isSelected ? FindResource("PlanTextSelectedStyle") as Style : null;
                    Plan6MonthsSubtitle.Style = isSelected ? FindResource("PlanSubtextSelectedStyle") as Style : null;
                    Plan6MonthsPrice.Style = isSelected ? FindResource("PlanPriceSelectedStyle") as Style : null;
                    break;
                    
                case "plan12Months":
                    Plan12MonthsTitle.Style = isSelected ? FindResource("PlanTextSelectedStyle") as Style : null;
                    Plan12MonthsSubtitle.Style = isSelected ? FindResource("PlanSubtextSelectedStyle") as Style : null;
                    Plan12MonthsPrice.Style = isSelected ? FindResource("PlanPriceSelectedStyle") as Style : null;
                    break;
                    
                case "plan15Months":
                    Plan15MonthsTitle.Style = isSelected ? FindResource("PlanTextSelectedStyle") as Style : null;
                    Plan15MonthsSubtitle.Style = isSelected ? FindResource("PlanSubtextSelectedStyle") as Style : null;
                    Plan15MonthsPrice.Style = isSelected ? FindResource("PlanPriceSelectedStyle") as Style : null;
                    break;
            }
        }
        
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAddOnOverlay();
        }
        
        private void ShowAddOnOverlay()
        {
            try
            {
                if (selectedPlanPrice == 0.0 || selectedPlanCard == null)
                {
                    // No plan selected, don't proceed
                    MessageBox.Show("Please select a subscription plan to continue.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Create a completely new overlay page
                // Create a simple page for the addon
                Frame overlayFrame = new Frame();
                AddOnPage addOnPage = new AddOnPage(selectedPlanPrice, selectedPlanName);
                
                // Navigate to the add-on page
                overlayFrame.Navigate(addOnPage);
                
                // Create a new window to host the overlayFrame
                Window overlayWindow = new Window
                {
                    Content = overlayFrame,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStyle = WindowStyle.None,
                    ResizeMode = ResizeMode.NoResize,
                    ShowInTaskbar = false,
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    AllowsTransparency = true,
                    Background = Brushes.Transparent,
                    Topmost = true
                };
                
                // Set up the Page's size
                addOnPage.Width = 420;
                addOnPage.Height = 650;
                
                // Add drop shadow effect
                addOnPage.Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 270,
                    ShadowDepth = 5,
                    BlurRadius = 10,
                    Opacity = 0.3
                };
                
                // Listen for when the user completes or cancels the add-on selection
                addOnPage.OnComplete += (sender, args) => 
                {
                    // Close the overlay window
                    overlayWindow.Close();
                };
                
                // Show the window as a dialog
                overlayWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error showing add-on overlay - {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 