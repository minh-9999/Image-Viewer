using System.Windows;

namespace ImageViewerWPF
{
    public static class NavigationMenuHandlers
    {
        public static void Nav_ViewNextImage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Next image");
        }

        public static void Nav_ViewPreviousImage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Previous image");
        }

        public static void Nav_GoTo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Go to...");
        }

        public static void Nav_GoToFirstImage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("First image");
        }

        public static void Nav_GoToLastImage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Last image");
        }
    }
}