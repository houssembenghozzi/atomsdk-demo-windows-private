using System;
using System.Windows;
using System.Windows.Threading;
using Atom.VPN.Demo;

namespace Atom.VPN.Demo.Helpers
{
    /// <summary>
    /// A custom MessageBox that overrides the System.Windows.MessageBox functionality
    /// </summary>
    public static class CustomMessageBox
    {
        // Flag to temporarily disable the override
        public static bool IsOverrideEnabled { get; set; } = true;
        
        // Event that will be raised when a MessageBox would be shown
        public static event Action<string, string, MessageBoxButton, MessageBoxImage> OnMessageBoxRequested;

        // Static constructor
        static CustomMessageBox()
        {
            // Replace the standard MessageBox implementation
            MessageBox.Show = Show;
        }

        // Our implementation
        public static MessageBoxResult Show(string messageBoxText)
        {
            return Show(messageBoxText, "", MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(messageBoxText, caption, button, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            // Special handling for password reset confirmations
            if (caption == "Email Sent" && messageBoxText.Contains("password reset link has been sent"))
            {
                // Get the current window (likely ForgotPasswordWindow)
                if (Application.Current.Windows.Count > 0)
                {
                    // Get the current ForgotPasswordWindow
                    var currentWindow = Application.Current.Windows[0];
                    
                    // Create a timer to return to login after a brief delay
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += (s, args) => {
                        timer.Stop();
                        
                        // Create and show the login window
                        var loginWindow = new LoginWindow();
                        loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        loginWindow.Show();
                        
                        // Close the current window
                        currentWindow.Close();
                    };
                    timer.Start();
                }
                
                // Return OK immediately without showing a message box
                return MessageBoxResult.OK;
            }
            
            // If override is disabled, don't intercept
            if (!IsOverrideEnabled)
            {
                // Use reflection to call the original MessageBox.Show method
                return System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);
            }
            
            // Raise the event if there are subscribers
            OnMessageBoxRequested?.Invoke(messageBoxText, caption, button, icon);
            
            // For all other cases, show the standard message box
            return System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText)
        {
            return Show(messageBoxText);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
        {
            return Show(messageBoxText, caption);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(messageBoxText, caption, button);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return Show(messageBoxText, caption, button, icon);
        }
    }
} 