

using System.Windows;
using System.Windows.Controls;

namespace ImageViewerWPF
{
    public static class ToolsMenuHandlers
    {
        public static void ColorPicker_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Color Picker toggled.");
        }

        public static void CropImage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Crop Image toggled.");
        }

        public static void FrameNavigation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Frame Navigation toggled.");
        }

        public static void ExifViewer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ExifGlass toggled.");
        }

        public static void GetMoreTools_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Redirecting to download more tools...");
        }
    }
}
