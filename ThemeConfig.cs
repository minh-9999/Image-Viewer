namespace ImageViewerWPF
{
    public class ThemeConfig
    {
        public Info Info { get; set; } = new Info();
        public Settings Settings { get; set; } = new Settings();
        public Dictionary<string, string> ToolbarIcons { get; set; } = [];
        public Colors Colors { get; set; } = new Colors();
    }

    public class Colors
    {
        public string BgColor { get; set; } = "";
        public string TextColor { get; set; } = "";

        public string ToolbarBgColor { get; set; } = "";
        public string ToolbarTextColor { get; set; } = "";
        public string ToolbarItemHoverColor { get; set; } = "";
        public string ToolbarItemActiveColor { get; set; } = "";
        public string ToolbarItemSelectedColor { get; set; } = "";

        public string GalleryBgColor { get; set; } = "";
        public string GalleryTextColor { get; set; } = "";
        public string GalleryItemHoverColor { get; set; } = "";
        public string GalleryItemActiveColor { get; set; } = "";
        public string GalleryItemSelectedColor { get; set; } = "";

        public string MenuBgColor { get; set; } = "";
        public string MenuBgHoverColor { get; set; } = "";
        public string MenuTextColor { get; set; } = "";
        public string MenuTextHoverColor { get; set; } = "";

        public string NavigationButtonColor { get; set; } = "";
    }


    public class Info
    {
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        public string Email { get; set; } = "";
        public string Website { get; set; } = "";
        public string Description { get; set; } = "";
        public string Version { get; set; } = "";
    }


    public class Settings
    {
        public bool IsDarkMode { get; set; }
        public bool IsShowTitlebarLogo { get; set; }

        public string NavButtonLeft { get; set; } = "";
        public string NavButtonRight { get; set; } = "";
        public string PreviewImage { get; set; } = "";
        public string AppLogo { get; set; } = "";
    }

}