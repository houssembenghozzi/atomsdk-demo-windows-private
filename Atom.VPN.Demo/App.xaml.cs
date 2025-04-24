using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Atom.VPN.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Windows API for finding and closing message boxes
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        // Constants for window messages
        private const uint WM_CLOSE = 0x0010;
        
        public App()
        {
            //var procList = Process.GetProcesses().Where(x => x.ProcessName == Process.GetCurrentProcess().ProcessName).ToList();
            //if (procList.Count > 1)
            //    Environment.Exit(-1);
            
            // Add global exception handler
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            // Set up a timer to look for and close "Email Sent" message boxes
            DispatcherTimer messageBoxTimer = new DispatcherTimer();
            messageBoxTimer.Interval = TimeSpan.FromMilliseconds(100);
            messageBoxTimer.Tick += MessageBoxTimer_Tick;
            messageBoxTimer.Start();
        }
        
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log the error instead of showing MessageBox
            Console.WriteLine("An unhandled exception occurred: " + e.Exception.Message);
            
            // Mark as handled to prevent application shutdown
            e.Handled = true;
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                // Log the error instead of showing MessageBox
                Console.WriteLine("A fatal error occurred: " + ex.Message);
            }
        }
        
        private void MessageBoxTimer_Tick(object sender, EventArgs e)
        {
            // Look for message boxes with the title "Email Sent"
            IntPtr hwnd = FindWindow("#32770", "Email Sent");
            if (hwnd != IntPtr.Zero)
            {
                // Close the message box by sending a WM_CLOSE message
                SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                
                // Log that we closed a message box (for debugging)
                Console.WriteLine("Closed an 'Email Sent' message box");
            }
        }
    }
}
