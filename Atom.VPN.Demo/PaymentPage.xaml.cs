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
        private const double DEDICATED_IP_PRICE = 4.99;
        private bool includeDedicatedIp = false;
        private double totalPrice = 0.0;
        
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
            
            // Hide the add-on panel initially
            AddOnPanel.Visibility = Visibility.Collapsed;
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
                totalPrice = selectedPlanPrice; // Initialize total price
                
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
            if (selectedPlanPrice == 0.0 || selectedPlanCard == null)
            {
                // No plan selected, don't proceed
                MessageBox.Show("Please select a subscription plan to continue.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            // Update the add-on panel with selected plan details
            UpdateAddOnPanel();
            
            // Show the add-on panel with animation
            ShowAddOnPanel();
        }
        
        private void UpdateAddOnPanel()
        {
            try
            {
                // Update selected plan information
                SelectedPlanNameText.Text = selectedPlanName;
                SelectedPlanPriceText.Text = $"${selectedPlanPrice:F2}";
                
                // Reset dedicated IP toggle
                DedicatedIpToggle.IsChecked = includeDedicatedIp = false;
                DedicatedIPRow.Visibility = Visibility.Collapsed;
                
                // Update total price (initially just the plan price)
                totalPrice = selectedPlanPrice;
                TotalPriceText.Text = $"${totalPrice:F2}";
                
                Debug.WriteLine($"{TAG}: Add-on panel updated with {selectedPlanName} at ${selectedPlanPrice:F2}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error updating add-on panel - {ex.Message}");
            }
        }
        
        private void ShowAddOnPanel()
        {
            try
            {
                // Make the panel visible
                AddOnPanel.Visibility = Visibility.Visible;
                
                // Find the animation
                Storyboard fadeIn = FindResource("FadeInStoryboard") as Storyboard;
                
                if (fadeIn != null)
                {
                    // Set the target
                    Storyboard.SetTarget(fadeIn, AddOnPanel);
                    
                    // Start the animation
                    fadeIn.Begin();
                }
                
                Debug.WriteLine($"{TAG}: Showing add-on panel with animation");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error showing add-on panel - {ex.Message}");
                
                // Fallback if animation fails
                AddOnPanel.Opacity = 1;
                AddOnPanel.Visibility = Visibility.Visible;
            }
        }
        
        private void HideAddOnPanel()
        {
            try
            {
                // Find the animation
                Storyboard fadeOut = FindResource("FadeOutStoryboard") as Storyboard;
                
                if (fadeOut != null)
                {
                    // Set the target
                    Storyboard.SetTarget(fadeOut, AddOnPanel);
                    
                    // Add a completed event handler to hide the panel after animation completes
                    EventHandler onCompleted = null;
                    onCompleted = (s, e) => 
                    {
                        fadeOut.Completed -= onCompleted;
                        AddOnPanel.Visibility = Visibility.Collapsed;
                    };
                    
                    fadeOut.Completed += onCompleted;
                    
                    // Start the animation
                    fadeOut.Begin();
                    
                    Debug.WriteLine($"{TAG}: Hiding add-on panel with animation");
                }
                else
                {
                    // Fallback if animation not found
                    AddOnPanel.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error hiding add-on panel - {ex.Message}");
                
                // Fallback if animation fails
                AddOnPanel.Visibility = Visibility.Collapsed;
            }
        }
        
        private void AddOnBackButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the add-on panel
            HideAddOnPanel();
        }
        
        private void DedicatedIpToggle_CheckedChanged(object sender, RoutedEventArgs e)
        {
            // Update the dedicated IP flag
            includeDedicatedIp = DedicatedIpToggle.IsChecked ?? false;
            
            // Show or hide the dedicated IP row in the summary section
            DedicatedIPRow.Visibility = includeDedicatedIp ? Visibility.Visible : Visibility.Collapsed;
            
            // Update total price
            UpdateTotalPrice();
            
            Debug.WriteLine($"{TAG}: Dedicated IP toggle changed to {includeDedicatedIp}");
        }
        
        private void UpdateTotalPrice()
        {
            // Calculate the total price
            totalPrice = selectedPlanPrice;
            
            if (includeDedicatedIp)
            {
                totalPrice += DEDICATED_IP_PRICE;
            }
            
            // Update the UI
            TotalPriceText.Text = $"${totalPrice:F2}";
            
            Debug.WriteLine($"{TAG}: Total price updated to ${totalPrice:F2}");
        }
        
        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dedicatedIpText = includeDedicatedIp ? "with Dedicated IP" : "without add-ons";
                MessageBox.Show(
                    $"Processing payment of ${totalPrice:F2} for {selectedPlanName} {dedicatedIpText}",
                    "Payment Processing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                // Here you would implement the actual payment processing
                
                // Navigate back to main VPN page
                if (NavigationService != null && NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    // Fallback if NavigationService is null or can't go back
                    var containerWindow = Application.Current.MainWindow as MainContainerWindow;
                    if (containerWindow != null)
                    {
                        containerWindow.NavigateToMainVPNPage();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error processing payment - {ex.Message}");
                MessageBox.Show($"Error processing payment: {ex.Message}", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 