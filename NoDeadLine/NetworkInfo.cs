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
        private bool _isReport = false;
        public void SaveFromCmd(bool isReport)
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                _isReport = isReport;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                startInfo.FileName = @"C:\Windows\System32\cmd.exe";
                if (isReport)
                {
                    fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "Network" + ".json";

                   
                }
                else
                {
                    fileName = GetNetworkInfo() + "networkpool.json";

                    
                }

                startInfo.Arguments = @"/C " + "cd " + FarmSettings.NetworkInfoCommandLine + " & " +
                                      "speedtest.exe " +
                                      "> " + fileName;
                process.Exited += Process_Exited;
                process.StartInfo = startInfo;

                process.EnableRaisingEvents = true;
               

                process.Start();
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            }
        }

        

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            string[] nameFiles = Directory.GetFiles(FarmSettings.NetworkInfoCommandLine + "\\", "*.json");
            if (nameFiles.Length > 0)
            {
                for (int i = 0; i < nameFiles.Length; i++)
                {
                    File.Delete(nameFiles[i]);
                }
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            string pathStart = FarmSettings.NetworkInfoCommandLine + "\\" + fileName;
            string pathEnd = "";

            if (_isReport)
            {
                pathEnd= FarmSettings.DeadLineReportFolderWin + "\\" + fileName;
            }
            else
            {
                pathEnd=FarmSettings.NetworkPool + "\\" + fileName;
            }

            try
            {
                if (File.Exists(pathStart))
                {
                    if (_isReport)
                    {
                        File.Move(pathStart, pathEnd);
                    }
                    else
                    {
                        if (File.Exists(pathEnd))
                        {
                            File.Delete(pathEnd);
                        }
                        File.Move(pathStart, pathEnd);
                    }

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
