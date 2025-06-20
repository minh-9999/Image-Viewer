using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Navigation;

namespace ImageViewerWPF
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            DisplayEnvironmentInfo();
        }

        private void DisplayEnvironmentInfo()
        {
            string runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            string os = RuntimeInformation.OSDescription;
            //EnvironmentInfoTextBlock.Text = $"Runtime: {runtime}\nOS: {os}";
        }

        private void WebsiteLink_Click(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
