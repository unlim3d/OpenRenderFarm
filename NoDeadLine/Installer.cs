using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;

public class Installer
{
    public static void CheckNodeInstalled()
    {

        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = " -v",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        try { proc.Start(); }
        catch
        {
            Console.WriteLine("No Node.js Detected");
            return;
        } 
        while (!proc.StandardOutput.EndOfStream)
        {
            string line = proc.StandardOutput.ReadLine();
            // do something with line
        }
    }
}
