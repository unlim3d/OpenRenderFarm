using System;
using System.Collections.Generic;
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
            this.Closed += MainWindow_Closed;
            this.Hide();
            Button_Click();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
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
                    ModifyJsonOnId(folder);
                    System.Windows.Application.Current.Shutdown();
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

        private void ModifyJsonOnId(string pathToSave)
        {
            int tempIdJson = 0;
            string path = "";
            string[] pathFiles = Directory.GetFiles(GetPahJobs(), "*.json");

            for (int i = 0; i < pathFiles.Length; i++)
            {
                string tempString = "";

                tempString = pathFiles[i].Substring(pathFiles[i].LastIndexOf('\\') + 1,
                    pathFiles[i].LastIndexOf(".json") - pathFiles[i].LastIndexOf('\\'));
                tempString = tempString.Substring(0, tempString.IndexOf("_"));
                tempIdJson = int.Parse(tempString);
                if (App.idCollectPath == tempIdJson)
                {
                    path = pathFiles[i];
                }
            }

            string textPath = "";

            string text = File.ReadAllText(path);
            int startOldPath = 0;
            int endOldPath = 0;
            startOldPath = text.LastIndexOf("\"CollectPath\":\"") + 15;
            endOldPath = text.LastIndexOf("\",\"ServerPreviewMovFilePath\"");
            textPath = text.Substring(startOldPath, endOldPath - startOldPath);
            if (textPath.Length > 0)
            {
                text = text.Replace(textPath, "");
            }
            endOldPath = text.LastIndexOf("\",\"ServerPreviewMovFilePath\"");
            List<char> test=new List<char>();
            for (int i = 0; i < pathToSave.Length; i++)
            {
                test.Add(pathToSave[i]);
                if (pathToSave[i] == '\\')
                {
                    test.Add('\\');
                }
            }

            string testString = "";
            for (int i = 0; i < test.Count; i++)
            {
                testString += test[i];
            }
            text = text.Insert(endOldPath, testString);
            File.WriteAllText(path, text);
        }

        private string GetPahJobs()
        {
            RootPathSite();
            string tempPath = Path.Combine(Root, "Jobs");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }
        private string GetPathRenderPath()
        {
            RootPathResources();
            string tempPath = Path.Combine(Root, "RenderPath");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }
        public static void RootPathResources()
        {
            if (Directory.GetDirectories(Root, "PathsSaver").Length > 0)
            {
                Root = Root;

            }
            else
            {
                Root = Root.Substring(0, Root.LastIndexOf("\\"));
                RootPathResources();
            }

        }

        public static void RootPathSite()
        {
            if (Directory.GetDirectories(Root, "Resources").Length > 0)
            {
                Root = Root;

            }
            else
            {
                Root = Root.Substring(0, Root.LastIndexOf("\\"));
                RootPathSite();
            }
        }

        private static string _root = Directory.GetCurrentDirectory();
        public static string Root
        {
            get { return _root; }
            set { _root = value; }
        }

    }

    public class UserPath
    {
        public string SelectedPath { get; set; }
    }
}

