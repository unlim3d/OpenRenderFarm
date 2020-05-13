using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NoDeadLine
{
    class HardwareInfo
    {
        private string fileName = "";
        public void StartCmd()
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                startInfo.FileName = @"C:\Windows\System32\cmd.exe";
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                startInfo.Arguments = @"/C " + "cd " + FarmSettings.HardwareInfoCommandLine + " & " +
                                      "OpenHardwareMonitorReport.exe " + "> " + fileName;
                process.StartInfo = startInfo;

                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;

                process.Start();

                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            }
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            string[] nameFiles = Directory.GetFiles(FarmSettings.HardwareInfoCommandLine + "\\", "*.json");
            if ( nameFiles.Length> 0)
            {
                for (int i = 0; i < nameFiles.Length; i++)
                {
                    File.Delete(nameFiles[i]);
                }
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            string pathStart = FarmSettings.HardwareInfoCommandLine + "\\" + fileName;
            string pathEnd = FarmSettings.DeadLineReportFolderWin + "\\" + fileName;
            try
            {
                if (File.Exists(pathStart))
                {
                    File.Move(pathStart, pathEnd);
                }
                string[] files = Directory.GetFiles(FarmSettings.DeadLineReportFolderWin);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddDays(-1))
                        fi.Delete();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("The process failed: {0}", exception.ToString());
            }
        }
    }
}
