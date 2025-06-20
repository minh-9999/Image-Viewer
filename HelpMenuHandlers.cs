
using System.Windows;

namespace ImageViewerWPF
{
    public static class HelpMenuHandlers
    {
        public static void HelpMenu_About_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow
            {
                Owner = Application.Current.MainWindow
            };
            aboutWindow.ShowDialog();
        }


        public static void HelpMenu_Update_Click(object sender, RoutedEventArgs e)
        {
            // Ví dụ đơn giản: Thông báo chưa có cập nhật
            MessageBox.Show("You're using the latest version.", "Check for Updates", MessageBoxButton.OK, MessageBoxImage.Information);

            // TODO: Sau này có thể kết nối API để kiểm tra cập nhật thực tế
        }
    }
}
