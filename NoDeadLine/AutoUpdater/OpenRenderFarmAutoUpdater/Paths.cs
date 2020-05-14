using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft;
using Newtonsoft.Json.Linq;

public class Paths
{
	const string FirstFarmNode = "http://nodeadline.mykeenetic.com:8089/";
	public static string Root
	{
		get
		{

			string InstalledPath = Environment.GetEnvironmentVariable("OpenRenderFarm", EnvironmentVariableTarget.User);


#if DEBUG
			//string curdir = Directory.GetCurrentDirectory();
			//return Directory.GetDirectoryRoot(curdir);
#endif
			return InstalledPath;
		}
	}
	public static string ResourcesFolder
	{
		get
		{
			string res = Directory.GetDirectories(Root, "*web_server*")[0];
			res = Directory.GetDirectories(res, "*Site*")[0];
			res = Directory.GetDirectories(res, "*Resources*")[0];
			return res;
		}
	}
	public static string NewVersionZip { get; private set; }

	public static List<string> Pools = new List<string>();
	public static List<string> Builds = new List<string>();
	public static void Install()
	{
		UninstallAll();
		Environment.SetEnvironmentVariable("OpenRenderFarm", Root, EnvironmentVariableTarget.User);
		System.IO.Compression.ZipFile.ExtractToDirectory(NewVersionZip, Root, true);
		CheckNodeJSInstalled();
	}
	public static void InstallClean()
	{

		Environment.SetEnvironmentVariable("OpenRenderFarm", Root, EnvironmentVariableTarget.User);

		if (CheckNodeJSInstalled())
		{

		}
		else
		{
			InstallNodeJS();
		}
	}

	public static void InstallNodeJS()
	{
		string nodeEXE = Directory.GetFiles(ResourcesFolder, "*node-v*")[0];
		ProcessStartInfo oInfo = new ProcessStartInfo("msiexec.exe", @"/quiet /passive /i " +  nodeEXE );//msiexec.exe /i node-v0.10.23-x64.msi INSTALLDIR="C:\Tools\NodeJS" /quiet
		oInfo.UseShellExecute = false;
		oInfo.CreateNoWindow = false;
		

		Process process = Process.Start(oInfo);
		process.WaitForExit();
	}

	public static bool CheckNodeJSInstalled()
	{
		ProcessStartInfo oInfo = new ProcessStartInfo("node", "-v");
		oInfo.UseShellExecute = false;
		oInfo.CreateNoWindow = true;

		//so we are going to redirect the output and error so that we can parse the return
		oInfo.RedirectStandardOutput = true;
		oInfo.RedirectStandardError = true;

		oInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;


		//Create the output and streamreader to get the output
		string output = null; StreamReader srOutput = null;

		//try the process
		try
		{
			//run the process
			Process proc = System.Diagnostics.Process.Start(oInfo);

			proc.WaitForExit();

			//get the output
			srOutput = proc.StandardOutput;

			//now put it in a string
			output = srOutput.ReadToEnd();

			proc.Close();
			return true;
		}
		catch (Exception e)
		{
			output = string.Empty;
			Console.WriteLine("NodeJS not installed, install new one from Resources Folder  ");
			return false;
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
	public static void UninstallAll()
	{
		


		if (Root == null)
		{
			Debug.WriteLine("Cant find previously installed version. Install new one to: " + Directory.GetCurrentDirectory());
			Install();
		}
		else
		{
			Debug.WriteLine("Find Previous Version on: " + Root);
		 
			string[] Dirs = Directory.GetDirectories(Root,"*web_server*");
			if(Dirs.Length>0)
			for (int i = 0; i < Dirs.Length; i++)
			{
				if (!Dirs[i].Contains("Site")) Directory.Delete(Dirs[i], true);
			}
		}
	}
	public static void FindNewVersion()
	{
		Pools.Add(FirstFarmNode);

		string[] localPool = Directory.GetFiles(Root, "*networkpool*");
		for (int i = 0; i < localPool.Length; i++)
		{
			JObject o1 = JObject.Parse(File.ReadAllText(localPool[i]));
			string adress = o1.GetValue("adress").ToString();
			Pools.Add(adress);
		}
		for (int i = 0; i < Pools.Count; i++)
		{
			FindNewVersionAsync(Pools[i] + "dir?path=Resources/Build");
		}
	}
	static async System.Threading.Tasks.Task<bool> FindNewVersionAsync(string path)
	{
		HttpClient client = new HttpClient();
		var response = await client.GetAsync(path);
		var pageContents = await response.Content.ReadAsStringAsync();

		Console.WriteLine(pageContents);
		if (JArray.Parse(pageContents)[0].ToObject<int>() == 0)
		{
			string[] tempBuilds = JArray.Parse(pageContents)[1].ToObject<string[]>();
			foreach (var item in tempBuilds)
			{
				if (!Builds.Contains(item))
				{
					Builds.Add(Path.Combine(path, item));
					Console.WriteLine("New Build found: " + Path.Combine(path, item));
				}
			}
		
		}
		
	 
		return true;
	}

    public static void TestDownload()
    {
        const string tempfile = "tempfile.tmp";
        System.Net.WebClient webClient = new System.Net.WebClient();

        Console.WriteLine("Downloading file....");

        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
        webClient.DownloadFile("http://dl.google.com/googletalk/googletalk-setup.exe", tempfile);
        sw.Stop();

        FileInfo fileInfo = new FileInfo(tempfile);  
        float speed = fileInfo.Length / sw.Elapsed.Seconds/1024f;

        Console.WriteLine("Download duration: {0}", sw.Elapsed);
        Console.WriteLine("File size: {0}", fileInfo.Length.ToString("N0"));
        Console.WriteLine("Speed: {0} Mbps ", speed.ToString("N0"));

        Console.WriteLine("Press any key to continue...");
        Console.ReadLine();
	}
	public static void DownloadNewVersionOfBuild()
	{
		List<int> versions = new List<int>();
		if (Builds.Count > 0)
		{
			for (int i = 0; i < Builds.Count; i++)
			{

				int parse = 0;
				string str = Builds[i].Substring(Builds[i].Length - 8, 4);
				int.TryParse(str, out parse);
				if (parse != null) versions.Add(parse);

			}
			versions.Sort();

			using (var client = new WebClient())
			{
				Console.WriteLine("Download new Build: " + Builds[versions[versions.Count - 1] - 1]);
				string input = Builds[versions[versions.Count - 1] - 1];
				input = input.Replace("dir?path", "files?filename");
				NewVersionZip = Path.Combine(Root, Builds[versions[versions.Count - 1] - 1].Substring(Builds[versions[versions.Count - 1] - 1].Length - 23, 23));
				client.DownloadFile(input, NewVersionZip);
				Install();

			}

		}
		else
		{
			CheckUnzippedFolderAndInstall();

		}
	}
	

	public static void CheckUnzippedFolderAndInstall()
	{
		string webserv = Directory.GetDirectories(Root, "*web_server*")[0];
		if (webserv == null)
		{
			Console.WriteLine("new Builds are not available, install from .Zip file please.");
		}
		else
		{
			InstallClean();
		}
	}
}
 
