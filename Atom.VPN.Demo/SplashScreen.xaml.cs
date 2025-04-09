using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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
            
            // Load images directly
            LoadImages();
            
            // Start the spinner animation
            DoubleAnimation spinnerAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(0.8)),
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
        
        private void LoadImages()
        {
            try
            {
                // Get the executing assembly's location
                string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                
                // Try loading directly from file paths with BuildAction=Resource
                WorldMapImage.Source = LoadImage("world_map.png", exeDir);
                
                // If world map failed to load, create a placeholder
                if (WorldMapImage.Source == null)
                {
                    WorldMapImage.Source = CreatePlaceholderWorldMap();
                }
                
                LogoImage.Source = LoadImage("fortis_logo.png", exeDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading images: " + ex.Message);
            }
        }
        
        private BitmapImage LoadImage(string filename, string exeDir)
        {
            // Try multiple methods to load the image
            
            try
            {
                // Method 1: Try loading from resource
                var uri = new Uri($"pack://application:,,,/Atom.VPN.Demo;component/Resources/Images/{filename}");
                return new BitmapImage(uri);
            }
            catch
            {
                try
                {
                    // Method 2: Try loading from the Images folder relative to the exe
                    string path = System.IO.Path.Combine(exeDir, "Resources", "Images", filename);
                    
                    // Make sure the file exists
                    if (File.Exists(path))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(path);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        return bitmap;
                    }
                }
                catch
                {
                    // Method 3: Try with a simple relative path
                    return new BitmapImage(new Uri("Resources/Images/" + filename, UriKind.Relative));
                }
            }
            
            // If all else fails, return null
            return null;
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

        private ImageSource CreatePlaceholderWorldMap()
        {
            // Create a simple drawing to use in place of the world map
            int width = 460;
            int height = 350;
            
            DrawingVisual drawingVisual = new DrawingVisual();
            
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    Brushes.Transparent,
                    null,
                    new Rect(0, 0, width, height));
                    
                // Draw some simple shapes to simulate a world map
                for (int i = 0; i < 20; i++)
                {
                    // Create random ovals as "continents"
                    Random rand = new Random(i);
                    double x = rand.Next(width);
                    double y = rand.Next(height);
                    double w = rand.Next(30, 100);
                    double h = rand.Next(20, 60);
                    
                    drawingContext.DrawEllipse(
                        new SolidColorBrush(Color.FromArgb(40, 200, 200, 200)),
                        null,
                        new Point(x, y),
                        w, h);
                }
            }
            
            // Render the drawing to a bitmap
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            
            renderTargetBitmap.Render(drawingVisual);
            
            return renderTargetBitmap;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            
            try
            {
                // Create and initialize the main window
                mainWindow = new MainWindow();
                
                // Set the main window's automatic initialization of SDK with the key
                // Pre-populate the key from our embedded value
                mainWindow.SecretKey = "17355649429f7d4adbe993a8d227bc580c8f369b";
                
                // Make sure the main window is created successfully before closing the splash screen
                mainWindow.Closed += (s, args) => 
                {
                    // When main window is closed, exit the application
                    Application.Current.Shutdown();
                };
                
                // Try to initialize the SDK directly - but don't show any error if it fails
                // as the main window will handle SDK initialization anyway
                if (isAtomServiceInstalled)
                {
                    try
                    {
                        // Initialize SDK directly through MainWindow's methods
                        var initMethod = mainWindow.GetType().GetMethod("InitializeSDK", 
                           System.Reflection.BindingFlags.NonPublic | 
                           System.Reflection.BindingFlags.Public |
                           System.Reflection.BindingFlags.Instance, 
                           null, 
                           new Type[0], 
                           null);
                           
                        if (initMethod != null)
                        {
                            initMethod.Invoke(mainWindow, null);
                        }
                    }
                    catch
                    {
                        // Silently ignore initialization errors here
                        // The main window will handle initialization
                    }
                }
                
                // Create a second timer to wait a bit more before showing the main window
                DispatcherTimer finalizeTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(2) // Additional delay before showing main window
                };
                
                finalizeTimer.Tick += (s, args) =>
                {
                    finalizeTimer.Stop();
                    
                    try
                    {
                        // Show the main window
                        mainWindow.Show();
                        
                        // Close splash screen
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error finalizing main window: " + ex.Message);
                    }
                };
                
                finalizeTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening main window: " + ex.Message);
                
                // If we can't open the main window, don't close the splash screen
                // Restart the timer to try again
                timer.Start();
            }
        }
    }
} 