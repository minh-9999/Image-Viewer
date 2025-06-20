using System.Windows;

namespace ImageViewerWPF
{
    public static class ViewModeMenuHandlers
    {
        public static void View_WindowFit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Window Fit clicked");
        }

        public static void View_Frameless_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Frameless clicked");
        }

        public static void View_FullScreen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Full Screen clicked");
        }

        public static void View_Slideshow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Slideshow clicked");
        }
    }
}