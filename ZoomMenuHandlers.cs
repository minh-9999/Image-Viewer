using System.Windows;

namespace ImageViewerWPF
{
    public static class ZoomMenuHandlers
    {
        public static void Zoom_ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Zoom in");
        }

        public static void Zoom_ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Zoom out");
        }

        public static void Zoom_CustomZoom_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Custom zoom...");
        }

        public static void Zoom_ActualSize_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Actual size");
        }

        public static void Zoom_AutoZoom_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Auto zoom");
        }

        public static void Zoom_LockZoomRatio_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Lock zoom ratio");
        }

        public static void Zoom_ScaleToWidth_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Scale to width");
        }

        public static void Zoom_ScaleToHeight_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Scale to height");
        }

        public static void Zoom_ScaleToFit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Scale to fit");
        }

        public static void Zoom_ScaleToFill_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Scale to fill");
        }
    }
}