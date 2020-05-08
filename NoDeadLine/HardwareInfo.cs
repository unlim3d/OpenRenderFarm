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
            }
            catch (Exception exception)
            {
                Console.WriteLine("The process failed: {0}", exception.ToString());
            }
        }
    }
}
