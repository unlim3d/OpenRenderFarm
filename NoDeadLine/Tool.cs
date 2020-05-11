using System;
using System.Diagnostics;
using System.IO;

public class Tools
{
	public static int coreCount = 0;


	public static string FindToStrEnd(string str, int shift)
	{

		int t = str.IndexOf('\n');
		if (t != -1) return str.Substring(0, t + shift);

		else
			return str;
	}
	public static void ClearCurrentConsoleLine()
	{
		int currentLineCursor = Console.CursorTop;
		Console.SetCursorPosition(0, Console.CursorTop);
		Console.Write(new string(' ', Console.WindowWidth));
		Console.SetCursorPosition(0, currentLineCursor);
	}

	public static void OpenFolder()
	{
		 ProcessStartInfo oInfo = new ProcessStartInfo(FarmSettings.OpenDirectoryExe, "");
		oInfo.UseShellExecute = false;
		oInfo.CreateNoWindow = true;

		//so we are going to redirect the output and error so that we can parse the return
		oInfo.RedirectStandardOutput = true;
		oInfo.RedirectStandardError = true;

		//Create the output and streamreader to get the output
		string output = null; StreamReader srOutput = null;

		//try the process
		try
		{
			//run the process
			Process proc = System.Diagnostics.Process.Start(oInfo);

			//proc.WaitForExit();

			//get the output
			srOutput = proc.StandardError;

			//now put it in a string
			output = srOutput.ReadToEnd();

			proc.Close();
		}
		catch (Exception)
		{
			output = string.Empty;
			Console.WriteLine("Косяк бляяя:  ");
		}
		finally
		{
			//now, if we succeded, close out the streamreader
			if (srOutput != null)
			{
				srOutput.Close();
				srOutput.Dispose();
			}
		}
		 
	}
	public static bool FilesEqual(string SourceFile,string OutputFile)
	{
		if (!File.Exists(OutputFile)) return false;
		long length = new System.IO.FileInfo(OutputFile).Length;
		if (length < 20000) return false;

		return true;
	}

}
	
