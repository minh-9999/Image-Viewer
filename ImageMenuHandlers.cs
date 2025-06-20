using System.Windows;

namespace ImageViewerWPF
{
    public static class ImageMenuHandlers
    {
        public static void Image_RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rotate left clicked");
        }

        public static void Image_RotateRight_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rotate right clicked");
        }

        public static void Image_FlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Flip horizontal clicked");
        }

        public static void Image_FlipVertical_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Flip vertical clicked");
        }

        public static void Image_Rename_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rename image clicked");
        }

        public static void Image_MoveToRecycleBin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Move to Recycle Bin clicked");
        }

        public static void Image_DeletePermanently_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete permanently clicked");
        }

        public static void Image_ExportFrames_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export image frames clicked");
        }

        public static void Image_SetDesktopBackground_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Set as Desktop background clicked");
        }

        public static void Image_SetLockScreen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Set as Lock screen image clicked");
        }

        public static void Image_OpenLocation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open image location clicked");
        }

        public static void Image_ShowProperties_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Image properties clicked");
        }
    }
}
