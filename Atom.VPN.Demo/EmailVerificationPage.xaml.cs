using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for EmailVerificationPage.xaml
    /// </summary>
    public partial class EmailVerificationPage : Window
    {
        private string userEmail;
        private DispatcherTimer _timer;
        private int _timeRemaining = 30; // 30 seconds countdown
        
        public EmailVerificationPage(string email)
        {
            InitializeComponent();
            userEmail = email;
            
            // Set focus to first OTP digit box
            this.Loaded += (s, e) => 
            {
                OTPDigit1.Focus();
                // Start the countdown timer
                InitializeTimer();
            };
            
            // Set email info text
            InstructionsText.Text = $"Please enter the OTP that was sent to your email address \"{email}\"";
        }

        private void OTPDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OTPDigit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text.Length > 0)
            {
                // Move focus to next box
                if (textBox == OTPDigit1)
                    OTPDigit2.Focus();
                else if (textBox == OTPDigit2)
                    OTPDigit3.Focus();
                else if (textBox == OTPDigit3)
                    OTPDigit4.Focus();
            }
        }

        private void OTPDigit_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.Back && string.IsNullOrEmpty(textBox.Text))
                {
                    // Move focus to previous box when backspace is pressed on empty box
                    if (textBox == OTPDigit4)
                        OTPDigit3.Focus();
                    else if (textBox == OTPDigit3)
                        OTPDigit2.Focus();
                    else if (textBox == OTPDigit2)
                        OTPDigit1.Focus();
                }
                else if (e.Key == Key.Left)
                {
                    // Navigate left
                    if (textBox == OTPDigit4)
                        OTPDigit3.Focus();
                    else if (textBox == OTPDigit3)
                        OTPDigit2.Focus();
                    else if (textBox == OTPDigit2)
                        OTPDigit1.Focus();
                    
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    // Navigate right
                    if (textBox == OTPDigit1)
                        OTPDigit2.Focus();
                    else if (textBox == OTPDigit2)
                        OTPDigit3.Focus();
                    else if (textBox == OTPDigit3)
                        OTPDigit4.Focus();
                    
                    e.Handled = true;
                }
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            // Validate OTP (all fields must be filled)
            if (string.IsNullOrEmpty(OTPDigit1.Text) || 
                string.IsNullOrEmpty(OTPDigit2.Text) || 
                string.IsNullOrEmpty(OTPDigit3.Text) || 
                string.IsNullOrEmpty(OTPDigit4.Text))
            {
                MessageBox.Show("Please enter the complete 4-digit OTP.", "Incomplete OTP", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Combine OTP digits
            string otp = OTPDigit1.Text + OTPDigit2.Text + OTPDigit3.Text + OTPDigit4.Text;
            
            // For demo purposes, any OTP is valid
            MessageBox.Show("Email verified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Navigate to main application
            MainWindow mainWindow = new MainWindow();
            mainWindow.SecretKey = "17355649429f7d4adbe993a8d227bc580c8f369b";
            
            // Set event handler to shut down app when main window is closed
            mainWindow.Closed += (s, args) => Application.Current.Shutdown();
            
            // Show the main window
            mainWindow.Show();
            
            // Close verification window
            this.Close();
        }

        private void ChangeEmail_Click(object sender, RoutedEventArgs e)
        {
            // Go back to sign up page
            SignUpPage signUpPage = new SignUpPage();
            signUpPage.Show();
            this.Close();
        }

        private void ResendOTP_Click(object sender, RoutedEventArgs e)
        {
            // Clear the OTP fields
            OTPDigit1.Text = "";
            OTPDigit2.Text = "";
            OTPDigit3.Text = "";
            OTPDigit4.Text = "";
            
            // Set focus to first digit box
            OTPDigit1.Focus();
            
            // Show message
            MessageBox.Show("A new OTP has been sent to your email address.", "OTP Sent", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Reset the timer
            _timeRemaining = 30;
            _timer.Start();
            UpdateResendButtonText();
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            
            UpdateResendButtonText();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timeRemaining--;
            UpdateResendButtonText();
            
            if (_timeRemaining <= 0)
            {
                _timer.Stop();
            }
        }

        private void UpdateResendButtonText()
        {
            // Make sure UI is loaded before updating
            if (ResendLink != null)
            {
                foreach (var inline in ResendLink.Inlines)
                {
                    if (inline is Run run)
                    {
                        run.Text = _timeRemaining > 0
                            ? $"Resend OTP in {_timeRemaining}s"
                            : "Resend OTP";
                        
                        ResendLink.IsEnabled = _timeRemaining == 0;
                        break;
                    }
                }
            }
        }

        private bool IsNumeric(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
} 