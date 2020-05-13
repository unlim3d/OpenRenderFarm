using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PathSaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static StartupArgument startupArgument;
        public static int idCollectPath;
        public  enum StartupArgument
        {
            RenderPath,
            CollectPath
        }
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
           
            System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
            nIcon.Visible = true;
            nIcon.Icon = SystemIcons.Application;
            nIcon.ShowBalloonTip(5000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);
            //nIcon.Click += NIcon_Click;
            nIcon.ContextMenuStrip= new System.Windows.Forms.ContextMenuStrip();
            nIcon.ContextMenuStrip.Items.Add("Exit", null, this.MenuExit_Click);

            if (e.Args.Length == 1)
            {
                if (e.Args[0] == "RenderPath")
                {
                    startupArgument = StartupArgument.RenderPath;
                   // MessageBox.Show("Choice: \n\n" + e.Args[0]);
                    MainWindow wnd = new MainWindow();
                }

                
            }

            if (e.Args.Length == 2)
            {
                if (e.Args[0] == "CollectPath")
                {
                    startupArgument = StartupArgument.CollectPath;
                    idCollectPath = int.Parse(e.Args[1]);
                   // MessageBox.Show("Choice: \n\n" + e.Args[0]);
                    MainWindow wnd = new MainWindow();
                }
            }

            
        }
        void MenuExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void NIcon_Click(object sender, EventArgs e)
        {
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.WindowState = WindowState.Normal;
        }
    }
}
