
# 🖼️ WPF Image Viewer

A modern, lightweight image viewer application built using WPF (.NET), inspired by [ImageGlass](https://github.com/d2phap/ImageGlass). This project provides a smooth, customizable interface for viewing images with support for theming and toolbar icons.

## 🚀 Features

- 🖼️ View common image formats (PNG, JPG, BMP, GIF, etc.)
- 🔍 Zoom In / Out, Auto Zoom, Scale to Fit/Width/Height
- 🧭 Navigate images in folder (Next / Previous / First / Last)
- 🎨 Customizable themes with external SVG icon support
- 🧰 Basic tools: Rotate, Flip, Crop, Color Picker
- 🗂️ Gallery panel for thumbnail navigation
- ⚙️ Configurable via external `theme.json`

## 🧱 Getting Started

### Prerequisites

- .NET 6 or later
- Visual Studio 2022 or Visual Studio Code
- NuGet packages:
  - `Magick.NET-Q16-AnyCPU`
  - `SharpVectors.Wpf`
  - `SixLabors.ImageSharp`

### Build

1. Clone the repository:

```bash
git clone https://github.com/minh-9999/Image-Viewer.git
cd WpfImageViewer
```

2. Open the solution in Visual Studio and build the project.

### Run

Press `F5` in Visual Studio or run from terminal:

```bash
dotnet run --project ImageViewerWPF
```

## 📂 Project Structure

```
ImageViewerWPF/
│
├── Resources/
│   └── SVG/              # Theme icons (SVG files)
│        └── theme.json   # Theme config file
│        └── *svg files
│
│
├── App.xaml              # Application entry styles
├── MainWindow.xaml       # Main UI layout
├── MainWindow.xaml.cs    # Main window code-behind
├── ThemeConfig.cs        # Data models for theme.json
├── ThemeLoader.cs        # JSON loader logic
└── ...                   # Other menus and settings
```

## 🖼️ Screenshot

![App UI](https://raw.githubusercontent.com/yourusername/yourrepo/main/assets/screenshot.png)

## 📄 License

This project is licensed under the MIT License.

## 📅 Credits

Inspired by the amazing [ImageGlass](https://github.com/d2phap/ImageGlass) project.
