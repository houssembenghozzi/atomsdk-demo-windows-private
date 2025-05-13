using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for AddOnPage.xaml
    /// </summary>
    public partial class AddOnPage : Page
    {
        private const string TAG = "AddOnPage";
        private readonly double basePlanPrice;
        private readonly string planName;
        private const double DEDICATED_IP_PRICE = 4.99;
        
        // Properties
        public bool IncludeDedicatedIp { get; private set; }
        public double TotalPrice { get; private set; }
        
        // Event to notify when the add-on process is complete
        public event EventHandler OnComplete;
        
        public AddOnPage(double planPrice, string planName)
        {
            try
            {
                InitializeComponent();
                
                // Store the base price and plan name
                this.basePlanPrice = planPrice;
                this.planName = planName ?? "Unknown Plan";
                
                // Initialize UI elements
                PlanNameText.Text = this.planName;
                PlanPriceText.Text = $"${planPrice:F2}";
                
                // Initialize total price (no add-ons selected by default)
                TotalPrice = planPrice;
                TotalPriceText.Text = $"${TotalPrice:F2}";
                
                // Set initial state
                IncludeDedicatedIp = false;
                DedicatedIPRow.Visibility = Visibility.Collapsed;
                
                Debug.WriteLine($"{TAG}: Page created for {this.planName} at ${planPrice:F2}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error initializing page - {ex.Message}");
                MessageBox.Show($"Error setting up payment options: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Notify that the add-on process is complete/canceled
                OnComplete?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error in BackButton_Click - {ex.Message}");
            }
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Notify that the add-on process is complete/canceled
                OnComplete?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error in CloseButton_Click - {ex.Message}");
            }
        }
        
        private void DedicatedIpCheck_CheckedChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                IncludeDedicatedIp = DedicatedIpCheck.IsChecked ?? false;
                Debug.WriteLine($"{TAG}: Dedicated IP toggle changed to {IncludeDedicatedIp}");
                
                // Update UI based on selection
                DedicatedIPRow.Visibility = IncludeDedicatedIp ? Visibility.Visible : Visibility.Collapsed;
                
                // Update total price
                UpdateTotalPrice();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error in DedicatedIpCheck_CheckedChanged - {ex.Message}");
                // Try to reset to a safe state
                IncludeDedicatedIp = false;
                if (DedicatedIPRow != null)
                {
                    DedicatedIPRow.Visibility = Visibility.Collapsed;
                }
                UpdateTotalPrice();
            }
        }
        
        private void UpdateTotalPrice()
        {
            try
            {
                // Calculate the total price based on selections
                TotalPrice = basePlanPrice;
                
                if (IncludeDedicatedIp)
                {
                    TotalPrice += DEDICATED_IP_PRICE;
                }
                
                // Update the UI
                if (TotalPriceText != null)
                {
                    TotalPriceText.Text = $"${TotalPrice:F2}";
                }
                Debug.WriteLine($"{TAG}: Total price updated to ${TotalPrice:F2}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error updating total price - {ex.Message}");
                // In case of error, try to set a fallback value
                if (TotalPriceText != null)
                {
                    TotalPriceText.Text = $"${basePlanPrice:F2}";
                }
            }
        }
        
        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine($"{TAG}: Pay button clicked. Processing payment for {planName} at ${TotalPrice:F2}");
                
                string dedicatedIpText = IncludeDedicatedIp ? "with Dedicated IP" : "without add-ons";
                MessageBox.Show(
                    $"Processing payment of ${TotalPrice:F2} for {planName} {dedicatedIpText}",
                    "Payment Processing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                // Here you would implement the actual payment processing
                
                // Notify that the add-on process is complete
                OnComplete?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{TAG}: Error processing payment - {ex.Message}");
                MessageBox.Show($"Error processing payment: {ex.Message}", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 