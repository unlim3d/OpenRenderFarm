using System;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;

public class Paths
{
	const string FirstFarmNode = "http://nodeadline.mykeenetic.com:8089/";
	public static  string Root;
	 public static void Install()
	{
		if(Root==null)		Root = Directory.GetCurrentDirectory();
		Environment.SetEnvironmentVariable("OpenRenderFarm", Root, EnvironmentVariableTarget.User);

	}

	public static void UninstallAll()
	{
		string InstalledPath = Environment.GetEnvironmentVariable("OpenRenderFarm", EnvironmentVariableTarget.User);

		if (InstalledPath == null)
		{
			Debug.WriteLine("Cant find previously installed version. Install new one to: " + Directory.GetCurrentDirectory());
			Install();
		}
		else
		{
			Debug.WriteLine("Find Previous Version on: " + InstalledPath);
			Root = InstalledPath;
			string[] Dirs = Directory.GetDirectories("web_server");
			for (int i = 0; i < Dirs.Length; i++)
			{
				if (!Dirs[i].Contains("Site")) Directory.Delete(Dirs[i]);
			}
		}
	}
}
