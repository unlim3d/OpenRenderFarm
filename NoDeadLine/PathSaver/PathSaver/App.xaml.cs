using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public  enum StartupArgument
        {
            RenderPath,
            CollectPath
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            if (e.Args.Length == 1)
            {
                if (e.Args[0] == "RenderPath")
                {
                    startupArgument = StartupArgument.RenderPath;
                    MessageBox.Show("Choice: \n\n" + e.Args[0]);
                    MainWindow wnd = new MainWindow();
                }

                if (e.Args[0] == "CollectPath")
                {
                    startupArgument = StartupArgument.CollectPath;
                    MessageBox.Show("Choice: \n\n" + e.Args[0]);
                    MainWindow wnd = new MainWindow();
                }
            }
        }
    }
}
