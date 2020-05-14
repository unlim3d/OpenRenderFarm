using System;
using System.IO;

namespace OpenRenderFarmAutoUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Paths.TestDownload();

            Paths.FindNewVersion();

            System.Threading.Thread.Sleep(2000);
        
             
            Paths.DownloadNewVersionOfBuild();


        }
    }
}
