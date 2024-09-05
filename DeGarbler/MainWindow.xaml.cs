using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeGarbler
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private int LastUsed = 0;
        private int index = 0;
        private bool IsReady = false;
        string[] args = null; 
        string[] folders = null;
        string[] files = null;
        string[] joined = null;

        Dictionary<string, System.Text.Encoding> Codecs = new Dictionary<string, Encoding>()
        {
            {"Default", System.Text.Encoding.Default },
            {"Unicode", System.Text.Encoding.Unicode },
            {"UTF-7", System.Text.Encoding.UTF7 },
            {"UTF-8", System.Text.Encoding.UTF8 },
            {"UTF-32", System.Text.Encoding.UTF32 },
            {"ASCII", System.Text.Encoding.ASCII },
            {"Latin1", System.Text.Encoding.Latin1 },
            {"BigEndianUnicode", System.Text.Encoding.BigEndianUnicode }
        };

        public MainWindow()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this); // m_window in App.cs
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            OverlappedPresenter presenter = appWindow.Presenter as OverlappedPresenter;

            Windows.Graphics.SizeInt32 size = new Windows.Graphics.SizeInt32();
            size.Width = 600;
            size.Height = 400;

            appWindow.Resize(size);
            presenter.IsResizable = false;

            Codecs.Add("CP437", System.Text.Encoding.GetEncoding(437));
            Codecs.Add("CP866", System.Text.Encoding.GetEncoding(866));
            Codecs.Add("Windows-1251", System.Text.Encoding.GetEncoding(1251));
            Codecs.Add("Windows-1252", System.Text.Encoding.GetEncoding(1252));

            Encoding.SelectedIndex = 0;
            Decoding.SelectedIndex = 0;

            Encoding.Items.Clear();
            Decoding.Items.Clear();

            foreach (KeyValuePair<string, Encoding> item in Codecs)
            {
                Encoding.Items.Add(item.Key);
                Decoding.Items.Add(item.Key);
            }

            Encoding.SelectedIndex = 0;
            Decoding.SelectedIndex = 0;

            IsReady = true;

            args = Environment.GetCommandLineArgs();
            string path = "C:\\Users\\retrd\\Downloads\\СиТ ИВТ\\СиТ ИВТ";
            try
            {
                folders = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);
                joined = folders.Concat(files).ToArray();
                if (joined.Length < 2)
                    FileIndexUp.IsEnabled = false;
                if (joined.Length > 0)
                    Example.Text = Path.GetFileName(joined[0]);
                else
                    Example.Text = "Empty folder";
            }
            catch
            {
                FileIndexDown.IsEnabled = false;
                FileIndexUp.IsEnabled = false;
                ConfirmButton.IsEnabled = false;
                AppTitle.Foreground = (SolidColorBrush)Application.Current.Resources["AccentTextFillColorPrimaryBrush"];
            }
        }

        private void OnSelectionChangedEncoder(object sender, SelectionChangedEventArgs e)
        {
            if (IsReady)
            {
                LastUsed = 1;
                Example.Text = Recode(Path.GetFileName(joined[index]), Codecs[(string)Encoding.SelectedItem], Codecs[(string)Decoding.SelectedItem]);
            }
        }

        private void OnSelectionChangedDencoder(object sender, SelectionChangedEventArgs e)
        {
            if (IsReady)
            {
                LastUsed = 1;
                Example.Text = Recode(Path.GetFileName(joined[index]), Codecs[(string)Encoding.SelectedItem], Codecs[(string)Decoding.SelectedItem]);
            }
        }

        private void OnSelectionChangedVariants(object sender, SelectionChangedEventArgs e)
        {
            LastUsed = 2;
        }

        private void OnClickAccept(object sender, RoutedEventArgs e)
        {
            if (LastUsed == 1)
            {
                for (int i = 0; i < files.Length; i++)
                    File.Move(files[i], Path.GetDirectoryName(files[i]) + "\\" + Recode(Path.GetFileName(files[i]), Codecs[(string)Encoding.SelectedItem], Codecs[(string)Decoding.SelectedItem]));
                for (int i = 0; i < folders.Length; i++)
                    Directory.Move(folders[i], Path.GetDirectoryName(folders[i]) + "\\" + Recode(Path.GetFileName(folders[i]), Codecs[(string)Encoding.SelectedItem], Codecs[(string)Decoding.SelectedItem]));
            }
            else
            {

            }
            this.Close();
        }

        private void OnClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnClickIndexDown(object sender, RoutedEventArgs e)
        {
            FileIndexUp.IsEnabled = true;
            if (index == 0)
                FileIndexDown.IsEnabled = false;
            Example.Text = Recode(Path.GetFileName(joined[--index]), Codecs[(string)Encoding.SelectedItem], Codecs[(string)Decoding.SelectedItem]);
        }

        private void OnClickIndexUp(object sender, RoutedEventArgs e)
        {
            FileIndexDown.IsEnabled = true;
            if (index == joined.Length - 1)
                FileIndexUp.IsEnabled= false;
            Example.Text = Recode(Path.GetFileName(joined[++index]), Codecs[(string)Encoding.SelectedItem], Codecs[(string)Decoding.SelectedItem]);
        }

        private string Recode(string text, Encoding encoding, Encoding decoding)
        {
            byte[] encoded = encoding.GetBytes(text);
            string result = decoding.GetString(encoded);
            return result;
        }
    }
}
