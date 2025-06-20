using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace ImageViewerWPF
{
    public static class ThemeLoader
    {
        public static ThemeConfig? LoadTheme(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                return null;

            string json = File.ReadAllText(jsonPath);
            var config = JsonSerializer.Deserialize<ThemeConfig>(json);

            if (config != null)
                ApplyColors(config);

            return config;
        }

        private static void ApplyColors(ThemeConfig config)
        {
            void SetBrush(string key, string? colorValue)
            {
                if (!string.IsNullOrEmpty(colorValue) && TryParseColor(colorValue, out var color))
                {
                    Application.Current.Resources[key] = new SolidColorBrush(color);
                }
            }

            // Đăng ký brush dựa trên JSON key -> Resource key
            var mapping = new Dictionary<string, string>
            {
                { config.Colors.BgColor, "BgBrush" },
                { config.Colors.TextColor, "TextBrush" },

                { config.Colors.ToolbarBgColor, "ToolbarBgBrush" },
                { config.Colors.ToolbarTextColor, "ToolbarTextBrush" },

                { config.Colors.GalleryBgColor, "GalleryBgBrush" },
                { config.Colors.GalleryTextColor, "GalleryTextBrush" },

                { config.Colors.MenuBgColor, "MenuBgBrush" },
                { config.Colors.MenuTextColor, "MenuTextBrush" }
            };

            foreach (var pair in mapping)
            {
                SetBrush(pair.Value, pair.Key);
            }
        }

        private static bool TryParseColor(string value, out System.Windows.Media.Color color)
        {
            color = System.Windows.Media.Colors.Transparent;

            if (value.StartsWith("#"))
            {
                try
                {
                    color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(value)!;
                    return true;
                }
                catch { return false; }
            }

            // Hỗ trợ kiểu accent:70 hoặc accent:120 nếu cần
            if (value.StartsWith("accent", StringComparison.OrdinalIgnoreCase))
            {
                color = System.Windows.Media.Color.FromArgb(255, 0, 120, 215); // mặc định màu accent
                return true;
            }

            return false;
        }
    }
}
