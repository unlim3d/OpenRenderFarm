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
			string curdir = Directory.GetCurrentDirectory();
			return Directory.GetDirectoryRoot(curdir);
#endif
			return InstalledPath;
		}
	}
	public static List<string> Pools = new List<string>();
	public static List<string> Builds = new List<string>();
	public static void Install()
	{
		 
		Environment.SetEnvironmentVariable("OpenRenderFarm", Root, EnvironmentVariableTarget.User);

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
				if (!Dirs[i].Contains("Site")) Directory.Delete(Dirs[i]);
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
	public static void DownloadNewVersionOfBuild()
	{
		List<int> versions = new List<int>();
		for (int i = 0; i < Builds.Count; i++)
		{
			
			int parse = 0;
			string str =  Builds[i].Substring(Builds[i].Length-8, 4);
			int.TryParse( str , out parse);
			if (parse != null) versions.Add(parse);

		}
		versions.Sort();

		using (var client = new WebClient())
		{
			Console.WriteLine("Download new Build: " + Builds[versions[versions.Count - 1] - 1]);
			string input = Builds[versions[versions.Count - 1]-1];
			input = input.Replace("dir?path", "files?filename");
			string output = Path.Combine(Root, Builds[versions[versions.Count - 1] - 1].Substring(Builds[versions[versions.Count - 1] - 1].Length-23,23));
			client.DownloadFile(input,output);
		}

	}


}
 
