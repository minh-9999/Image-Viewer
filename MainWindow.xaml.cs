using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using ImageMagick;
using System.Collections.ObjectModel;

namespace ImageViewerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private double _currentZoom = 1.0;
        public static BitmapImage CurrentImage { get; set; } = new BitmapImage();
        public static string CurrentImagePath { get; set; } = string.Empty;
        //public Image ImgViewer => this.ImgViewer;
        public static MainWindow Instance { get; private set; } = null!;

        //private List<string> _imagePaths = new List<string>();
        private List<string> _imagePaths = [];
        private ObservableCollection<ImageItem> _images = [];
        private int _currentIndex;


        public MainWindow()
        {
            InitializeComponent();
            InitializeToolbarButtonMap();


            var theme = ThemeLoader.LoadTheme("Resources/SVG/theme.json");

            if (theme != null)
            {
                ApplyThemeIcons(theme);
            }
        }

        public class ImageItem(string imagePath)
        {
            public string ImagePath { get; set; } = imagePath;

            public BitmapImage Thumbnail { get; } = new(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }


        private void ApplyThemeIcons(ThemeConfig config)
        {
            string themeDir = Path.Combine(AppContext.BaseDirectory, @"..\..\..\", "Resources", "SVG");
            themeDir = Path.GetFullPath(themeDir);

            foreach (var pair in config.ToolbarIcons)
            {
                if (_toolbarButtons.TryGetValue(pair.Key, out var btn))
                {
                    string iconPath = Path.Combine(themeDir, pair.Value);
                    if (!File.Exists(iconPath))
                        continue;

                    var drawing = LoadSvgDrawing(iconPath, Brushes.White); // 👈 Thay bằng màu theme nếu có
                    if (drawing == null)
                        continue;

                    btn.Content = new System.Windows.Controls.Image
                    {
                        Source = new DrawingImage(drawing),
                        Width = 24,
                        Height = 24
                    };
                }
            }
        }

        private DrawingGroup? LoadSvgDrawing(string filePath, Brush overrideBrush)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = true
            };

            var reader = new FileSvgReader(settings);
            var drawing = reader.Read(filePath);

            if (drawing != null)
            {
                ApplyBrushToDrawing(drawing, overrideBrush);
                return drawing;
            }

            return null;
        }

        private void ApplyBrushToDrawing(Drawing drawing, Brush brush)
        {
            if (drawing is DrawingGroup group)
            {
                foreach (var child in group.Children)
                {
                    ApplyBrushToDrawing(child, brush);
                }
            }
            else if (drawing is GeometryDrawing geo)
            {
                if (geo.Brush != null)
                    geo.Brush = brush;

                if (geo.Pen != null)
                    geo.Pen = new Pen(brush, geo.Pen.Thickness);
            }
        }


        private Dictionary<string, Button> _toolbarButtons = [];
        private void InitializeToolbarButtonMap()
        {
            _toolbarButtons = new Dictionary<string, Button>
            {
                { "OpenFile", BtnOpen },
                { "Save", BtnSave },
                { "ZoomIn", BtnZoomIn },
                { "ZoomOut", BtnZoomOut },
                // Thêm các ánh xạ khác nếu có nhiều nút
            };
        }


        private void LoadImagesFromDirectory(string directoryPath, string? selectedImage = null)
        {
            string[] extensions = [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp"];

            var files = Directory.GetFiles(directoryPath)
                .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                .ToList();

            _imagePaths = files;
            _images.Clear();

            foreach (var path in files)
                _images.Add(new ImageItem(path));

            GalleryPanel.ItemsSource = _images;

            if (_images.Count > 0)
            {
                int selectedIndex = selectedImage != null
                    ? _images.ToList().FindIndex(img => img.ImagePath == selectedImage)
                    : 0;


                GalleryPanel.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
            }

            MessageBox.Show($"Đã load {_images.Count} ảnh.");
        }



        private void GalleryPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GalleryPanel.SelectedItem is ImageItem item)
            {
                _currentIndex = GalleryPanel.SelectedIndex;
                ImgViewer.Source = new BitmapImage(new Uri(item.ImagePath));
                ShowImageInfoWithMagick(item.ImagePath, _currentIndex, _imagePaths.Count, _currentZoom);
            }
        }

        private void ShowImageInfoWithMagick(string imagePath, int currentIndex, int totalCount, double zoomRatio)
        {
            MessageBox.Show("Đang gọi ShowImageInfoWithMagick:\n" + imagePath);

            try
            {
                var fileInfo = new FileInfo(imagePath);
                using var magickImage = new MagickImage(imagePath);

                FileNameText.Text = $"Name: {fileInfo.Name}";
                ImageIndexText.Text = $"Image: {currentIndex + 1} of {totalCount}";
                ZoomRatioText.Text = $"Zoom: {zoomRatio * 100:0}%";
                DimensionsText.Text = $"Dimensions: {magickImage.Width} x {magickImage.Height}";

                double sizeMb = fileInfo.Length / (1024.0 * 1024);
                FileSizeText.Text = $"Size: {sizeMb:0.00} MB";

                ColorSpaceText.Text = $"Color space: {magickImage.ColorSpace}";

                var modified = fileInfo.LastWriteTime;
                LastModifiedText.Text = $"Modified: {modified:yyyy-MM-dd HH:mm:ss}";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đọc ảnh bằng Magick.NET:\n" + ex.Message);
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainContextMenu.Items.Clear();

            // Load FileMenu từ ResourceDictionary
            var fileDict = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/FileMenu.xaml", UriKind.Relative)
            };

            if (fileDict["FileMenu"] is MenuItem fileMenu)
            {
                AttachFileHandlers(fileMenu); // 👈 Gán sự kiện click thủ công
                MainContextMenu.Items.Add(fileMenu);
            }

            // Load NavigationMenu từ ResourceDictionary
            var navDict = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/NavigationMenu.xaml", UriKind.Relative)
            };

            if (navDict["NavigationMenu"] is MenuItem navMenu)
            {
                AttachNavHandlers(navMenu);
                MainContextMenu.Items.Add(navMenu);
            }

            // Load ZoomMenu từ ResourceDictionary
            var zoomDict = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/ZoomMenu.xaml", UriKind.Relative)
            };

            if (zoomDict["ZoomMenu"] is MenuItem zoomMenu)
            {
                AttachZoomHandlers(zoomMenu);
                MainContextMenu.Items.Add(zoomMenu);
            }

            // Load ImageMenu từ ResourceDictionary
            var imageDict = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/ImageMenu.xaml", UriKind.Relative)
            };

            if (imageDict["ImageMenu"] is MenuItem imageMenu)
            {
                AttachImageHandlers(imageMenu);
                MainContextMenu.Items.Add(imageMenu);
            }

            // Load ViewMenu từ ResourceDictionary
            var viewDict = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/ViewModeMenu.xaml", UriKind.Relative)
            };

            string[] keys = ["WindowFitMenuItem", "FramelessMenuItem", "FullScreenMenuItem", "SlideshowMenuItem"];

            foreach (string key in keys)
            {
                if (viewDict[key] is not MenuItem item)
                    continue;

                AttachViewModeHandlers(item); // hoặc theo từng chức năng riêng
                MainContextMenu.Items.Add(item);
            }

            // Load LayoutMenu từ ResourceDictionary
            var layoutDict = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/LayoutMenu.xaml", UriKind.Relative)
            };

            if (layoutDict["LayoutMenu"] is MenuItem layoutMenu)
            {
                AttachLayoutMenuHandlers(layoutMenu);
                MainContextMenu.Items.Add(layoutMenu);
            }

            // Load ToolsMenu từ ResourceDictionary
            var toolsDict = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/ToolsMenu.xaml", UriKind.Relative)
            };

            if (toolsDict["ToolsMenu"] is MenuItem toolsMenu)
            {
                AttachToolsMenuHandlers(toolsMenu);
                MainContextMenu.Items.Add(toolsMenu);
            }

            // Load SettingsMenu từ ResourceDictionary
            var settingsMenu = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/SettingsMenu.xaml", UriKind.Relative)
            };

            if (settingsMenu["SettingsMenu"] is MenuItem settingsItem)
            {
                SettingsMenuHandlers.AttachSettingsMenuHandlers(settingsItem);
                MainContextMenu.Items.Add(settingsItem);
            }

            // Load HelpMenu từ ResourceDictionary
            var helpDict = new ResourceDictionary
            {
                Source = new Uri("/ImageViewerWPF;component/HelpMenu.xaml", UriKind.Relative)
            };

            if (helpDict["HelpMenu"] is MenuItem helpMenu)
            {
                AttachHelpHandlers(helpMenu);
                MainContextMenu.Items.Add(helpMenu);
            }

            MainContextMenu.IsOpen = true;
        }


        private void AttachFileHandlers(MenuItem fileMenu)
        {
            foreach (var item in fileMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    var header = menuItem.Header as string;
                    if (string.IsNullOrEmpty(header))
                        continue;

                    switch (header)
                    {
                        case "Open file...":
                            menuItem.Click += File_Open_Click;
                            break;
                        case "Open new window":
                            menuItem.Click += FileMenuHandlers.File_NewWindow_Click;
                            break;
                        case "Save":
                            menuItem.Click += FileMenuHandlers.File_Save_Click;
                            break;
                        case "Save as...":
                            menuItem.Click += FileMenuHandlers.File_SaveAs_Click;
                            break;
                        case "Open with...":
                            menuItem.Click += FileMenuHandlers.File_OpenWith_Click;
                            break;
                        case "Edit image ...":
                            menuItem.Click += FileMenuHandlers.File_EditImage_Click;
                            break;
                        case "Print...":
                            menuItem.Click += FileMenuHandlers.File_Print_Click;
                            break;
                        case "Share...":
                            menuItem.Click += FileMenuHandlers.File_Share_Click;
                            break;
                        case "Refresh":
                            menuItem.Click += FileMenuHandlers.File_Refresh_Click;
                            break;
                        case "Reload image":
                            menuItem.Click += FileMenuHandlers.File_Reload_Click;
                            break;
                        case "Reload image list":
                            menuItem.Click += FileMenuHandlers.File_ReloadList_Click;
                            break;
                        case "Unload image":
                            menuItem.Click += FileMenuHandlers.File_Unload_Click;
                            break;
                        case "Exit":
                            menuItem.Click += FileMenuHandlers.File_Exit_Click;
                            break;
                    }
                }
            }
        }

        public void LoadImageList()
        {
            // Lấy thư mục hiện tại
            string currentDir = Directory.GetCurrentDirectory();

            // Lọc tất cả các file ảnh từ thư mục đó
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };
            _imagePaths = Directory.GetFiles(currentDir)
                .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                .ToList();

            if (_imagePaths.Count == 0)
            {
                MessageBox.Show("Không tìm thấy ảnh nào trong thư mục hiện tại.");
                return;
            }

            _currentIndex = 0;
            LoadImageAtCurrentIndex();
        }

        private void LoadImageAtCurrentIndex()
        {
            if (_imagePaths.Count == 0 || _currentIndex < 0 || _currentIndex >= _imagePaths.Count)
                return;

            var imagePath = _imagePaths[_currentIndex];

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImgViewer.Source = bitmap;

                CurrentImage = bitmap;
                CurrentImagePath = imagePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message);
            }
        }

        public void ReloadCurrentImage()
        {
            if (!string.IsNullOrEmpty(CurrentImagePath) && File.Exists(CurrentImagePath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // tránh khóa file
                bitmap.UriSource = new Uri(CurrentImagePath);
                bitmap.EndInit();
                ImgViewer.Source = bitmap;
            }
            else
            {
                MessageBox.Show("No image to reload.", "Reload", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp",
                Title = "Chọn ảnh"
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFile = dialog.FileName;
                string folder = Path.GetDirectoryName(selectedFile)!;
                LoadImagesFromDirectory(folder, selectedFile);
            }
        }

        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

        private static void AttachNavHandlers(MenuItem navMenu)
        {
            foreach (var item in navMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    switch ((string)menuItem.Header)
                    {
                        case "View next image":
                            menuItem.Click += NavigationMenuHandlers.Nav_ViewNextImage_Click;
                            break;

                        case "View previous image":
                            menuItem.Click += NavigationMenuHandlers.Nav_ViewPreviousImage_Click;
                            break;

                        case "Go to...":
                            menuItem.Click += NavigationMenuHandlers.Nav_GoTo_Click;
                            break;

                        case "Go to first image":
                            menuItem.Click += NavigationMenuHandlers.Nav_GoToFirstImage_Click;
                            break;

                        case "Go to last image":
                            menuItem.Click += NavigationMenuHandlers.Nav_GoToLastImage_Click;
                            break;
                    }
                }
            }
        }

        private static void AttachZoomHandlers(MenuItem zoomMenu)
        {
            foreach (var item in zoomMenu.Items)
            {
                if (item is MenuItem { Header: string header } menuItem)
                {
                    switch (header)
                    {
                        case "Zoom in":
                            menuItem.Click += ZoomMenuHandlers.Zoom_ZoomIn_Click;
                            break;

                        case "Zoom out":
                            menuItem.Click += ZoomMenuHandlers.Zoom_ZoomOut_Click;
                            break;

                        case "Custom zoom...":
                            menuItem.Click += ZoomMenuHandlers.Zoom_CustomZoom_Click;
                            break;

                        case "Actual size":
                            menuItem.Click += ZoomMenuHandlers.Zoom_ActualSize_Click;
                            break;

                        case "Auto zoom":
                            menuItem.Click += ZoomMenuHandlers.Zoom_AutoZoom_Click;
                            break;

                        case "Lock zoom ratio":
                            menuItem.Click += ZoomMenuHandlers.Zoom_LockZoomRatio_Click;
                            break;

                        case "Scale to width":
                            menuItem.Click += ZoomMenuHandlers.Zoom_ScaleToWidth_Click;
                            break;

                        case "Scale to height":
                            menuItem.Click += ZoomMenuHandlers.Zoom_ScaleToHeight_Click;
                            break;

                        case "Scale to fit":
                            menuItem.Click += ZoomMenuHandlers.Zoom_ScaleToFit_Click;
                            break;

                        case "Scale to fill":
                            menuItem.Click += ZoomMenuHandlers.Zoom_ScaleToFill_Click;
                            break;
                    }
                }
            }
        }

        private static void AttachImageHandlers(MenuItem imageMenu)
        {
            foreach (var item in imageMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    switch ((string)menuItem.Header)
                    {
                        case "Rotate left":
                            menuItem.Click += ImageMenuHandlers.Image_RotateLeft_Click;
                            break;

                        case "Rotate right":
                            menuItem.Click += ImageMenuHandlers.Image_RotateRight_Click;
                            break;

                        case "Flip Horizontal":
                            menuItem.Click += ImageMenuHandlers.Image_FlipHorizontal_Click;
                            break;

                        case "Flip Vertical":
                            menuItem.Click += ImageMenuHandlers.Image_FlipVertical_Click;
                            break;
                        case "Rename image...":
                            menuItem.Click += ImageMenuHandlers.Image_Rename_Click;
                            break;

                        case "Move to the Recycle Bin":
                            menuItem.Click += ImageMenuHandlers.Image_MoveToRecycleBin_Click;
                            break;

                        case "Delete permanently":
                            menuItem.Click += ImageMenuHandlers.Image_DeletePermanently_Click;
                            break;
                        case "Export image frames...":
                            menuItem.Click += ImageMenuHandlers.Image_ExportFrames_Click;
                            break;

                        case "Set as Desktop background":
                            menuItem.Click += ImageMenuHandlers.Image_SetDesktopBackground_Click;
                            break;

                        case "Set as Lock screen image":
                            menuItem.Click += ImageMenuHandlers.Image_SetLockScreen_Click;
                            break;

                        case "Open image location":
                            menuItem.Click += ImageMenuHandlers.Image_OpenLocation_Click;
                            break;

                        case "Image properties":
                            menuItem.Click += ImageMenuHandlers.Image_ShowProperties_Click;
                            break;
                    }
                }
            }
        }

        private static void AttachViewModeHandlers(MenuItem parentMenu)
        {
            foreach (var item in parentMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    switch ((string)menuItem.Header)
                    {
                        case "Window Fit":
                            menuItem.Click += ViewModeMenuHandlers.View_WindowFit_Click;
                            break;

                        case "Frameless":
                            menuItem.Click += ViewModeMenuHandlers.View_Frameless_Click;
                            break;

                        case "Full Screen":
                            menuItem.Click += ViewModeMenuHandlers.View_FullScreen_Click;
                            break;

                        case "Slideshow":
                            menuItem.Click += ViewModeMenuHandlers.View_Slideshow_Click;
                            break;
                    }
                }
            }
        }


        private void AttachLayoutMenuHandlers(MenuItem layoutMenu)
        {
            foreach (var item in layoutMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    menuItem.Click += LayoutMenuItem_Click;
                }
            }
        }

        private void LayoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem item)
                return;

            bool isChecked = item.IsChecked;

            switch (item.Header.ToString())
            {
                case "Toolbar":
                    Toolbar.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
                    break;

                case "Gallery panel":
                    GalleryPanel.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
                    break;

                case "Checkerboard background":
                    // Giả sử bạn có một Control hiển thị ảnh (ví dụ: ImageViewerGrid)
                    ImageViewerGrid.Background = isChecked ? CreateCheckerboardBrush : Brushes.Transparent;
                    break;

                case "Keep window always on top":
                    this.Topmost = isChecked;
                    break;
            }
        }

        private static Brush CreateCheckerboardBrush
        {
            get
            {
                var brush = new DrawingBrush
                {
                    TileMode = TileMode.Tile,
                    Viewport = new Rect(0, 0, 10, 10),
                    ViewportUnits = BrushMappingMode.Absolute,
                    Drawing = new DrawingGroup
                    {
                        Children =
                        [
                            new GeometryDrawing(Brushes.LightGray, null, new RectangleGeometry(new Rect(0, 0, 10, 10))),
                        new GeometryDrawing(Brushes.White, null, new RectangleGeometry(new Rect(0, 0, 5, 5))),
                        new GeometryDrawing(Brushes.White, null, new RectangleGeometry(new Rect(5, 5, 5, 5)))
                        ]
                    }
                };
                return brush;
            }
        }

        private static void AttachToolsMenuHandlers(MenuItem toolsMenu)
        {
            foreach (var item in toolsMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    switch (menuItem.Header.ToString())
                    {
                        case "Color picker":
                            menuItem.Click += ToolsMenuHandlers.ColorPicker_Click;
                            break;

                        case "Crop image":
                            menuItem.Click += ToolsMenuHandlers.CropImage_Click;
                            break;

                        case "Frame navigation":
                            menuItem.Click += ToolsMenuHandlers.FrameNavigation_Click;
                            break;

                        case "ExifGlass - EXIF metadata viewer":
                            menuItem.Click += ToolsMenuHandlers.ExifViewer_Click;
                            break;

                        case "Get more tools...":
                            menuItem.Click += ToolsMenuHandlers.GetMoreTools_Click;
                            break;

                        case "Set default photo viewer":
                            menuItem.Click += (_, _) => MessageBox.Show("Set as default viewer");
                            break;

                        case "Remove default photo viewer":
                            menuItem.Click += (_, _) => MessageBox.Show("Remove as default viewer");
                            break;
                    }
                }
            }
        }


        private static void AttachHelpHandlers(MenuItem helpMenu)
        {
            foreach (var item in helpMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    switch (menuItem.Header.ToString())
                    {
                        case "About":
                            menuItem.Click += HelpMenuHandlers.HelpMenu_About_Click;
                            break;

                        case "Check for Updates":
                            menuItem.Click += HelpMenuHandlers.HelpMenu_Update_Click;
                            break;
                    }
                }
            }
        }


        // #############################################################################################

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape || (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control))
            {
                Application.Current.Shutdown();
            }

            // Ctrl + ,
            if (e.Key == Key.OemComma && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var settingsWindow = new SettingsWindow
                {
                    Owner = this
                };
                settingsWindow.ShowDialog();
                e.Handled = true;
            }

            // F1 → Mở About
            if (e.Key == Key.F1)
            {
                var aboutWindow = new AboutWindow
                {
                    Owner = this
                };
                aboutWindow.ShowDialog();
                e.Handled = true;
                return;
            }

            // F6 → Kiểm tra cập nhật
            if (e.Key != Key.F6)
                return;

            MessageBox.Show(
                "You're using the latest version.",
                "Check for Updates",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            e.Handled = true;
        }

        // #############################################################################################

        private void File_Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (dlg.ShowDialog() != true)
                return;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dlg.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImgViewer.Source = bitmap;
                //ResetZoom(); // Về mặc định 100%
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot load image: " + ex.Message);
            }
        }


        // ------------------------------------------------------------------------------------------


        private void ToggleToolbar_Click(object sender, RoutedEventArgs e)
        {
            Toolbar.Visibility = Toolbar.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ToggleGalleryPanel_Click(object sender, RoutedEventArgs e)
        {
            GalleryPanel.Visibility = GalleryPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }



    }
}