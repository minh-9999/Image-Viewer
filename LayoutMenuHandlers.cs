using System.Windows;
using System.Windows.Controls;

namespace ImageViewerWPF
{
    public static class LayoutMenuHandlers
    {
        public static void AttachHandlers(ContextMenu contextMenu, MainWindow window)
        {
            var layoutMenu = contextMenu.Items
                .OfType<MenuItem>()
                .FirstOrDefault(m => m.Header?.ToString() == "Layout");

            if (layoutMenu == null)
                return;

            foreach (var item in layoutMenu.Items.OfType<MenuItem>())
            {
                switch (item.Header?.ToString())
                {
                    case "Toolbar":
                        item.Click += (_, _) => ToggleVisibility(window.Toolbar);
                        break;

                    case "Gallery panel":
                        item.Click += (_, _) => ToggleVisibility(window.GalleryPanel);
                        break;
                }
            }
        }

        private static void ToggleVisibility(UIElement element)
        {
            element.Visibility = element.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}