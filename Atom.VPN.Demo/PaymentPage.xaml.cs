using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

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
            
            // Set initial selection (1 Month plan)
            PlanCard_MouseDown(Plan1Month, null);
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
                    if (card == clickedCard)
                    {
                        // Apply selected style
                        card.Style = FindResource("PlanCardSelectedStyle") as Style;
                        selectedPlanCard = card;
                    }
                    else
                    {
                        // Reset to default style
                        card.Style = FindResource("PlanCardStyle") as Style;
                    }
                }
            }
        }
        
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAddOnDialog();
        }
        
        private void ShowAddOnDialog()
        {
            if (selectedPlanPrice == 0.0 || selectedPlanCard == null)
            {
                // Fallback to 1-month plan if no plan selected
                selectedPlanPrice = 4.99;
                selectedPlanName = "1 Month";
            }
            
            // Create and show the AddOn dialog
            var addOnDialog = new AddOnDialog(selectedPlanPrice, selectedPlanName);
            addOnDialog.Owner = Window.GetWindow(this);
            addOnDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            bool? result = addOnDialog.ShowDialog();
            
            if (result.HasValue && result.Value)
            {
                // Handle payment confirmation
                bool includeDedicatedIp = addOnDialog.IncludeDedicatedIp;
                double totalPrice = addOnDialog.TotalPrice;
                
                string dedicatedIpText = includeDedicatedIp ? "with Dedicated IP" : "without add-ons";
                MessageBox.Show(
                    $"Processing payment of ${totalPrice:F2} for {selectedPlanName} {dedicatedIpText}",
                    "Payment Processing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                // Here you would implement the payment processing and navigate to the confirmation screen
                // Navigate back to the VPN page
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }
    }
} 