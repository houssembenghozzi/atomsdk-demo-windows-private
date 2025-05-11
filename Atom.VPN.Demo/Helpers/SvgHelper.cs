using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Atom.VPN.Demo.Helpers
{
    public static class SvgHelper
    {
        public static ImageSource ConvertSvgToImageSource(string svgFilePath)
        {
            try
            {
                // For a simple approach, we'll use a PNG image as fallback
                // In a real scenario, you'd use a proper SVG rendering library
                if (File.Exists(svgFilePath) && Path.GetExtension(svgFilePath).Equals(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    // If SVG libraries were properly installed, here we would convert SVG to bitmap
                    // But for now, we'll create a simple placeholder image
                    DrawingVisual drawingVisual = new DrawingVisual();
                    
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        // Draw a blue circle with "SVG" text as placeholder
                        drawingContext.DrawEllipse(
                            Brushes.Blue, 
                            new Pen(Brushes.Black, 2), 
                            new Point(50, 50), 
                            40, 40);
                            
                        FormattedText text = new FormattedText(
                            "SVG",
                            System.Globalization.CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            14,
                            Brushes.White,
#if NET45_OR_GREATER                            
                            1.0);
#else
                            1.0);
#endif
                            
                        drawingContext.DrawText(text, new Point(38, 43));
                    }
                    
                    RenderTargetBitmap bmp = new RenderTargetBitmap(100, 100, 96, 96, PixelFormats.Pbgra32);
                    bmp.Render(drawingVisual);
                    return bmp;
                }
                
                // Return default image if SVG file doesn't exist
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading SVG: {ex.Message}");
                return null;
            }
        }
    }
} 