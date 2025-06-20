using System.Windows;
using System.Windows.Controls;

namespace ImageViewerWPF
{
    public static class SettingsMenuHandlers
    {
        public static void AttachSettingsMenuHandlers(MenuItem settingsMenuItem)
        {
            settingsMenuItem.Click += OpenSettings_Click;
        }

        private static void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ SettingsWindow
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = Application.Current.MainWindow;
            settingsWindow.ShowDialog();
        }
    }
}