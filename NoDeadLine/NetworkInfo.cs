using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace NoDeadLine
{
    class NetworkInfo
    {
        public string addressIP = "";
        private string fileName = "";
        public void SaveFromCmd()
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
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "Network" + ".json";
                startInfo.Arguments = @"/C " + "cd " + FarmSettings.NetworkInfoCommandLine + " & " + "speedtest.exe " +
                                      "> " + fileName;
                process.StartInfo = startInfo;

                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;

                process.Start();
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            string pathStart = FarmSettings.NetworkInfoCommandLine + "\\" + fileName;
            string pathEnd = FarmSettings.DeadLineReportFolderWin + "\\" + fileName ;
            try
            {
                if (File.Exists(pathStart))
                {
                    File.Move(pathStart, pathEnd);
                    File.AppendAllText(pathEnd,"IPAddress : "+GetNetworkInfo());
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("The process failed: {0}", exception.ToString());
            }
        }
        public string GetNetworkInfo()
        {
            addressIP = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
            return addressIP;

        }
        
    }
}
