using System;
using System.Diagnostics;
using System.Windows;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for AddOnDialog.xaml
    /// </summary>
    public partial class AddOnDialog : Window
    {
        private const string TAG = "AddOnDialog";
        private readonly double basePlanPrice;
        private readonly string planName;
        private const double DEDICATED_IP_PRICE = 5.00;
        
        // Public properties for accessing dialog results
        public bool IncludeDedicatedIp { get; private set; }
        public double TotalPrice { get; private set; }
        
        public AddOnDialog(double planPrice, string planName)
        {
            InitializeComponent();
            
            // Store the base price and plan name
            this.basePlanPrice = planPrice;
            this.planName = planName;
            
            // Initialize UI elements
            PlanNameText.Text = planName;
            PlanPriceText.Text = $"${planPrice:F2}";
            
            // Initialize total price (no add-ons selected by default)
            UpdateTotalPrice();
            
            // Set window dragging behavior
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
            
            Debug.WriteLine($"{TAG}: Dialog created for {planName} at ${planPrice:F2}");
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        
        private void DedicatedIpToggle_CheckedChanged(object sender, RoutedEventArgs e)
        {
            IncludeDedicatedIp = DedicatedIpToggle.IsChecked ?? false;
            Debug.WriteLine($"{TAG}: Dedicated IP toggle changed to {IncludeDedicatedIp}");
            UpdateTotalPrice();
        }
        
        private void UpdateTotalPrice()
        {
            // Calculate the total price based on selections
            TotalPrice = basePlanPrice;
            
            if (IncludeDedicatedIp)
            {
                TotalPrice += DEDICATED_IP_PRICE;
            }
            
            // Update the UI
            TotalPriceText.Text = $"${TotalPrice:F2}";
            Debug.WriteLine($"{TAG}: Total price updated to ${TotalPrice:F2}");
        }
        
        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"{TAG}: Pay button clicked. Processing payment for {planName} at ${TotalPrice:F2}");
            
            // Set dialog result to true to indicate payment confirmation
            this.DialogResult = true;
            this.Close();
        }
    }
} 