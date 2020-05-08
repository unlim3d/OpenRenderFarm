using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using Newtonsoft.Json;

namespace PathSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            Button_Click();
        }
        private void Button_Click()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;
            dlg.IsFolderPicker = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                // Do something with selected folder string
                UserPath path = new UserPath();
                path.SelectedPath = folder;
                string json = JsonConvert.SerializeObject(path);
                System.IO.File.WriteAllText(GetPath() + "\\path.json", json);
                //MessageBoxResult result = MessageBox.Show(folder, "Your choice");
                //if (result == MessageBoxResult.OK || result == MessageBoxResult.Cancel)
                //{
                //    System.Windows.Application.Current.Shutdown();
                //}
                System.Windows.Application.Current.Shutdown();
            }
        }

        private string GetPath()
        {
            RootPath();
            string tempPath = Path.Combine(Resources, "FarmSettings");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }
        public static void RootPath()
        {
            if (Directory.GetDirectories(Resources, "PathSaver").Length > 0)
            {
                Resources = Resources;

            }
            else
            {
                Resources = Resources.Substring(0, Resources.LastIndexOf("\\"));
                RootPath();
            }

        }
        private static string _resources = Directory.GetCurrentDirectory();
        public static string Resources
        {
            get { return _resources; }
            set { _resources = value; }
        }

    }

    public class UserPath
    {
        public string SelectedPath { get; set; }
    }
}

