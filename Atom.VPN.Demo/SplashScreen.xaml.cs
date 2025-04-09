using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private DispatcherTimer timer;
        private MainWindow mainWindow;
        private bool isAtomServiceInstalled = false;

        public SplashScreen()
        {
            InitializeComponent();
            
            // Start the spinner animation
            DoubleAnimation spinnerAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                RepeatBehavior = RepeatBehavior.Forever
            };
            
            SpinnerRotation.BeginAnimation(RotateTransform.AngleProperty, spinnerAnimation);
            
            // Start timer to check dependencies and load the main window after 2 seconds
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            
            timer.Tick += Timer_Tick;
            timer.Start();
            
            // Try to detect if Atom service is installed
            try
            {
                // This is a placeholder for actual detection logic
                // You would use AtomManager or a similar class to check if the service is available
                isAtomServiceInstalled = CheckAtomServiceInstalled();
            }
            catch (Exception)
            {
                isAtomServiceInstalled = false;
            }
        }

        private bool CheckAtomServiceInstalled()
        {
            // Placeholder for actual detection logic
            // In a real implementation, you would use AtomManager or system API to check
            // if the Atom VPN Service is available on the system
            try
            {
                // You could try to create an instance of AtomConfiguration or use P/Invoke to check services
                return System.ServiceProcess.ServiceController.GetServices()
                    .Any(service => service.ServiceName.Contains("Atom") || service.DisplayName.Contains("Atom"));
            }
            catch
            {
                return false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            
            // Create and initialize the main window
            mainWindow = new MainWindow();
            
            // Set the main window's automatic initialization of SDK with the key
            // Pre-populate the key from our embedded value
            mainWindow.SecretKey = "17355649429f7d4adbe993a8d227bc580c8f369b";
            
            // Call the InitializeSDK method
            if (isAtomServiceInstalled)
            {
                // We'll check if the window has the InitializeSDK method using reflection
                var initMethod = mainWindow.GetType().GetMethod("InitializeSDK", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (initMethod != null)
                {
                    // Call the method with no parameters
                    initMethod.Invoke(mainWindow, null);
                }
            }
            
            // Show main window
            mainWindow.Show();
            
            // Close splash screen
            this.Close();
        }
    }
} 