using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats;
//using SixLabors.ImageSharp.PixelFormats;
//using SixLabors.ImageSharp.Processing;
//using SixLabors.ImageSharp.Formats.Png;
//using SixLabors.ImageSharp.Formats.Jpeg;
//using SixLabors.ImageSharp.Formats.Bmp;
//using SixLabors.ImageSharp.Formats.Webp;
//using SixLabors.ImageSharp.Formats.Gif;
//using SixLabors.ImageSharp.Formats.Ico;
using ImageMagick;

namespace ImageViewerWPF
{
    public static class FileMenuHandlers
    {

        public static void File_NewWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new MainWindow();
            newWindow.Show();
        }

        public static void File_Save_Click(object sender, RoutedEventArgs e)
        {
            var encoder = new PngBitmapEncoder(); // hoặc chọn encoder phù hợp theo phần mở rộng
            encoder.Frames.Add(BitmapFrame.Create(MainWindow.CurrentImage));

            try
            {
                using (var stream = new FileStream(MainWindow.CurrentImagePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
                MessageBox.Show("Đã lưu ảnh.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu ảnh: " + ex.Message);
            }

        }

        public static void File_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG|*.png|JPEG|*.jpg;*.jpeg|BMP|*.bmp|WEBP|*.webp|ICO|*.ico|GIF|*.gif|TIFF|*.tiff|Base64|*.b64|Text|*.txt|All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.FileName;
                string ext = Path.GetExtension(path).ToLower();

                try
                {
                    // Chuyển đổi từ BitmapImage sang MagickImage
                    MagickImage magickImage = ConvertBitmapImageToMagick(MainWindow.CurrentImage);

                    if (ext is ".b64" or ".txt")
                    {
                        using var ms = new MemoryStream();
                        magickImage.Write(ms, GetMagickFormat(ext));
                        string base64 = Convert.ToBase64String(ms.ToArray());
                        File.WriteAllText(path, base64);
                    }
                    else
                    {
                        magickImage.Format = GetMagickFormat(ext);
                        magickImage.Write(path);
                    }

                    MainWindow.CurrentImagePath = path;
                    MessageBox.Show("Ảnh đã được lưu.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu ảnh: {ex.Message}");
                }
            }
        }

        private static MagickImage ConvertBitmapImageToMagick(BitmapImage bmpImg)
        {
            using MemoryStream ms = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpImg));
            encoder.Save(ms);
            ms.Position = 0;
            return new MagickImage(ms);
        }

        private static MagickFormat GetMagickFormat(string ext) => ext switch
        {
            ".jpg" or ".jpeg" => MagickFormat.Jpeg,
            ".png" => MagickFormat.Png,
            ".bmp" => MagickFormat.Bmp,
            ".gif" => MagickFormat.Gif,
            ".webp" => MagickFormat.WebP,
            ".ico" => MagickFormat.Icon,
            ".tiff" => MagickFormat.Tiff,
            ".b64" or ".txt" => MagickFormat.Png, // lưu base64 theo định dạng PNG
            _ => MagickFormat.Png
        };


        public static void File_OpenWith_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MainWindow.CurrentImagePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = MainWindow.CurrentImagePath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Chưa có ảnh nào được mở.");
            }
        }

        public static void File_EditImage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MainWindow.CurrentImagePath))
            {
                Process.Start("mspaint", $"\"{MainWindow.CurrentImagePath}\"");
            }
            else
            {
                MessageBox.Show("Chưa có ảnh nào để chỉnh sửa.");
            }
        }

        public static void File_Print_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
            {
                dlg.PrintVisual(MainWindow.Instance.ImgViewer, "In ảnh");
            }
        }

        public static void File_Share_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MainWindow.CurrentImagePath))
            {
                var folder = Path.GetDirectoryName(MainWindow.CurrentImagePath);
                if (folder != null)
                    Process.Start("explorer.exe", folder);
            }
            else
            {
                MessageBox.Show("Chưa có ảnh nào để chia sẻ.");
            }
        }

        public static void File_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MainWindow.CurrentImagePath))
            {
                var newImage = new BitmapImage();
                newImage.BeginInit();
                newImage.CacheOption = BitmapCacheOption.OnLoad;
                newImage.UriSource = new Uri(MainWindow.CurrentImagePath);
                newImage.EndInit();

                MainWindow.Instance.ImgViewer.Source = newImage;
                MainWindow.CurrentImage = newImage;

                MessageBox.Show("Đã làm mới ảnh.");
            }
            else
            {
                MessageBox.Show("Chưa có ảnh nào để refresh.");
            }
        }

        public static void File_Reload_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ReloadCurrentImage();
            }
        }


        public static void File_ReloadList_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                mainWin.LoadImageList();
                MessageBox.Show("Đã tải lại danh sách ảnh.");
            }

            MessageBox.Show("Đã tải lại danh sách ảnh.");
        }

        public static void File_Unload_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement menuItem)
                return;

            var window = Window.GetWindow(menuItem);

            if (window is MainWindow mainWin)
            {
                mainWin.ImgViewer.Source = null;
            }
        }

        public static void File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }


}
