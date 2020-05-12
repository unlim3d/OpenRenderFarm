using System;
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
                if (App.startupArgument == App.StartupArgument.RenderPath)
                {
                    if (IsUnique(folder))
                    {
                        string json = JsonConvert.SerializeObject(path);
                        System.IO.File.WriteAllText(GetPathRenderPath() + "\\"+ DateTime.Now.ToString("yyyyMMddHHmmss")+".json", json);
                        //MessageBoxResult result = MessageBox.Show(folder, "Your choice");
                        //if (result == MessageBoxResult.OK || result == MessageBoxResult.Cancel)
                        //{
                        //    System.Windows.Application.Current.Shutdown();
                        //}
                        System.Windows.Application.Current.Shutdown();
                    }
                }

                if (App.startupArgument == App.StartupArgument.CollectPath)
                {

                }

            }
        }

        private bool IsUnique(string path)
        {
            bool isUnique = true;
            foreach (var VARIABLE in Directory.GetFiles(GetPathRenderPath(), "*.json"))
            {

                UserPath userPath = JsonConvert.DeserializeObject<UserPath>(File.ReadAllText(VARIABLE));
                string pathFromJson = userPath.SelectedPath;


                if (pathFromJson == path)
                {
                    isUnique = false;
                }

            }

            return isUnique;



        }
        private string GetPathRenderPath()
        {
            RootPath();
            string tempPath = Path.Combine(Resources, "RenderPath");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }
        public static void RootPath()
        {
            if (Directory.GetDirectories(Resources, "PathsSaver").Length > 0)
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

