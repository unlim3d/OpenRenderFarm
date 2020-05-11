using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
 

public class Job
{
	public Job (string Rpath, int ID)
	{
		RenderPath = Rpath;
		Id = ID;
		 
		
	}
	public int Id;
	public string RenderPath;
	public string[] ExistingFiles;
	public string[] RenderChannels;
	public int[] FramesPreviewsOnServer;	 
	public List<string> FramesMissed;	 
	
	public string Renderer;
	public  int LastMovFramesCounter=0;
	public  int MinimumFrameRendered=999999999;
	public  int MaximumFrameRendered=-1111111;
	public string PreviewPath;
	public string JsonPath;
	 
	public string CollectPath = "";
	public string ServerPreviewMovFilePath;
	private string _RenderNameMask;
	public string FileFormat;
	public double FullRendersSize;
	public double OneFrameSize;
	public bool MovRendered =false;
	public string RenderNameMask
	{
		set
		{
			PreviewPath =Path.Combine( FarmSettings.SitePath);
			ServerPreviewMovFilePath = Path.Combine( FarmSettings.SitePath, Id.ToString()+"_"+ value + ".mov");
			_RenderNameMask =value;
			JsonPath = Path.Combine(FarmSettings.SitePath, Id.ToString() + "_" + value + ".json");
			
			//	Console.WriteLine("\nВсего Джоб Обнаружено: " + Program.Jobs.Count);
			Console.WriteLine("Новая джоба :" + _RenderNameMask);
			
		}
		get
		{
			return _RenderNameMask;
		}

	}
	public static void CheckJobName(string RenderFile)
	{



		if (File.Exists(RenderFile))
		{
			string SearchDirectory = RenderTask.GetDirectoryPathFromFile(RenderFile);
			Tools.ClearCurrentConsoleLine();
			Console.Write("Попытка найти новую джобу: " + SearchDirectory);

			if (Program.Jobs.Find(x => x.RenderPath == SearchDirectory) == null)
			{


				Job Joba = new Job(RenderTask.GetDirectoryPathFromFile(RenderFile), Program.Jobs.Count);

				string str = Path.GetFileNameWithoutExtension(RenderFile);
				str = str.Substring(0, str.Length - 4);
				Joba.RenderNameMask = str;

				Program.Jobs.Add(Joba);


				Joba.ExistingFiles = Program.SearchFile(Joba.RenderPath, "*" + Joba.RenderNameMask + "*");


			
				TryParseOtherFrames(Joba);
				CheckSequence(Joba);
				GetChannels(Joba);
				GetDirectorySize(Joba);
				GetFileFormat(Joba);
				SaveJobJson(Joba);
			}
		}

	}


	
	static void GetChannels(Job joba)
	{
	



		List<string> Channels = new List<string>();
		string findPattern = "*"+joba.RenderNameMask.Substring(0, joba.RenderNameMask.Length - 11)+"*";
		Channels.AddRange(Directory.GetFiles(joba.RenderPath, findPattern)); ;
		for (int i = 0; i < Channels.Count; i++)
		{
			if (Channels[i].Contains("опия")) { Channels.Remove(Channels[i]);i--; }

		}
		for (int i = 0; i < Channels.Count; i++)
		{

		 
			Channels[i] = Channels[i].Substring(0, Channels[i].Length - 8);

		}
		List<string> UniqueChannels = new List<string>();
		for (int i = 0; i <Channels.Count; i++)
		{
			if (!UniqueChannels.Contains(Channels[i])) UniqueChannels.Add(Channels[i]);
		}
		joba.RenderChannels = UniqueChannels.ToArray();
	}
	static void GetDirectorySize(Job joba)
	{
		// Part 1: get info.
		DirectoryInfo directoryInfo = new DirectoryInfo(joba.RenderPath);
		FileSystemInfo[] array = directoryInfo.GetFileSystemInfos();

		// Part 2: sum all FileInfo Lengths.
		double sum = 0;
		for (int i = 0; i < array.Length; i++)
		{
			FileInfo fileInfo = array[i] as FileInfo;
			if (fileInfo != null)
			{
				sum += (int)fileInfo.Length;
			}
		}
		joba.FullRendersSize = sum;
		Console.WriteLine("All Renders Size: " + (joba.FullRendersSize / (1024d*1024d*1024d)).ToString());
		
		joba.OneFrameSize = sum/joba.RenderChannels.Length;
		Console.WriteLine("One Frame:  " + (joba.OneFrameSize / (1024d * 1024d * 1024d) ).ToString());


	}




	static void TryParseOtherFrames(Job joba)
	{
			int z = 0;
			Console.WriteLine("\n Проверяем джобу номер: " + joba.Id.ToString() + " файлов в папке:  " + joba.ExistingFiles.Length+ ":   ");
		
		for (int j = 0; j < joba.ExistingFiles.Length; j++)
		{
			//if (File.Exists(RenderTask.GetServerPreviewFileNameByOriginalFileName(joba.ExistingFiles[j],joba)))
			if (Tools.FilesEqual(joba.ExistingFiles[j], RenderTask.GetServerPreviewFileNameByOriginalFileName(joba.ExistingFiles[j], joba)))
				Console.Write("*" );
			else
			{
				
				Thread newThread = new Thread(Program.RunOperation);
					Console.Write("Запускаем поток: "+z++);

				Program.GenerateFromToFFmpegJpg(joba.ExistingFiles[j], joba);
				 
				 
				newThread.Start(Program.GenerateFromToFFmpegJpg(joba.ExistingFiles[j], joba));

				Tools.ClearCurrentConsoleLine();
				
			}
		}
			 
				 
		  
	}

 
	 
	/*  static string GenerateMov(Job job)
	{
		string offset = " -start_number " + job.MinimumFrameRendered;


		// -vf scale = 320:-1 "- vf scale = 320:-1, "+ + GammaCorretion + job.Id.ToString()+"_"
		string tmp = offset + " -i  " +job.RenderNameMask+"%04d" + "-y " + job.ServerPreviewMovFilePath;
		Console.WriteLine("\nFFMPEG:  " + tmp + "\n");
		_ = Program.RunFFMpeg(tmp);
		return tmp;
	}*/
	 
	  static void CheckSequence(Job job)
	{
		if (job.Id == 2)
		{

		}
		int SequenceCounter = 0;
		foreach (var item in job.ExistingFiles)
		{
			int framenumber = RenderTask.GetFrameNumberFromFileName(item);
			if (framenumber <100000)
			{
				if (job.MaximumFrameRendered < framenumber) job.MaximumFrameRendered = framenumber;
				if (job.MinimumFrameRendered > framenumber) job.MinimumFrameRendered = framenumber;
			}
		}
		job.FramesMissed = new List<string>();
		job.MovRendered = false;
		string Frame = "";


		if (job.Id == 3)
		{

		}
		for (int j = job.MinimumFrameRendered; j <=job.MaximumFrameRendered; j++)
		{
			Frame = j.ToString();
			if (Frame.Length == 1) Frame = "000" + Frame;
			if (Frame.Length == 2) Frame = "00" + Frame;
			if (Frame.Length == 3) Frame = "0" + Frame;
			string tempFrameId = Frame;
			Frame = RenderTask.GetServerPreviewFileNameByOriginalFileName( job.RenderNameMask + Frame + ".jpg",job);
			
			
			// проверка  на новизну ебаную
			if (File.Exists(Frame)) SequenceCounter++;



			else
			{
				job.FramesMissed.Add(Frame.Substring(Frame.Length-8,4));
				Program.GenerateFromMissedToServerPreviewFrame(job, Frame);
					 SequenceCounter++;
				/*
				if (  job.MovRendered==false)
				{
					if (job.LastMovFramesCounter != j)
					{
						Console.WriteLine("\nGenerating MOV: "+ job.Id+" " + job.MinimumFrameRendered.ToString() + "-" + job.MaximumFrameRendered.ToString());

						 GenerateMovFile(job, Frame, job.MinimumFrameRendered, j);
						job.MovRendered = true;
						job.LastMovFramesCounter = j;
					}
					else
					{
						Console.WriteLine("\nПропускаем MOV, так как число файлов не изменилось\n");
					}
				}

				*/
			}
			if (SequenceCounter >= (job.MaximumFrameRendered-job.MinimumFrameRendered))
			{
				job.MovRendered = true;
				GenerateMovFile(job, Frame, job.MinimumFrameRendered, j);
			}

		}
		Console.WriteLine("Пропущенных кадров: " + job.FramesMissed.Count);

	}
	
	static void SyncJob(Job joba)
	{
		for (int i = 0; i < joba.ExistingFiles.Length; i++)
		{

		}
	}
	 
	 static void SaveJobJson(Job job)
	{
		string output = JsonConvert.SerializeObject(job);
		File.WriteAllText(Path.Combine(FarmSettings.JobsDirectory,  (job.Id.ToString()+"_"+ ".json")), output);
		Console.ForegroundColor = ConsoleColor.DarkGreen;
		Console.WriteLine("\nЗаписываем JsonJob: " + job.RenderNameMask);
		Console.ForegroundColor = ConsoleColor.White;
	}
	 
	public static void ClearJsonsDirectory()
	{
		if (Directory.Exists(FarmSettings.JobsDirectory))
			Directory.Delete(FarmSettings.JobsDirectory,true);
	}
	  static string GenerateMovFile(Job job,string path,int startFrame,int CountOfFrames)
	{
		if (job.Id == 10)
		{

		}
		string filemask = Path.GetFileNameWithoutExtension(path);
		filemask = filemask.Substring(0, filemask.Length - 4);
		string OutputMov = RenderTask.GetServerPreviewFileNameByOriginalFileName( filemask);
		
		path = path.Substring(0, path.Length -8) + "%04d.jpg ";//-vframes "+(job.MaximumFrameRendered-job.MinimumFrameRendered+1)

		string offset = " -start_number " + job.MinimumFrameRendered;

		OutputMov = (OutputMov.Substring(0, OutputMov.Length - 3)) + ".mov ";
		path = " -i " + path + " " + job.ServerPreviewMovFilePath;
		string tmp = offset + path   + " -y ";
		Program.RunFFMpeg(tmp);
		Console.WriteLine("Mov: " + job.RenderPath);
		


		return null;
	}

	public static void GetFileFormat(Job joba)
	{
		if (joba.ExistingFiles.Length>0)
		joba.FileFormat = joba.ExistingFiles[0].Substring(joba.ExistingFiles[0].Length-3,3);
	}

	public static string FindVrayRGBColorRenderMask(string path)
	{
		if(Directory.Exists(path))
		{ 
		string[] strs = Directory.GetDirectories(path);
		List<string> checkedstr= new List<string>();
		for (int i = 0; i < strs.Length; i++)
		{
			if (!strs[i].Contains("System Volume Information") && (!strs[i].Contains("RECYCLE.BIN")) && (!strs[i].Contains("web_server"))) 
				checkedstr.Add(strs[i]);
			 
			 
			 
		 
		}

		List<string> UniquePaths = new List<string>();
		for (int i = 0; i < checkedstr.Count; i++)
		{
			string[] str = Directory.GetFiles(checkedstr[i], "*RGB_color.*", SearchOption.AllDirectories);

			if (str.Length != 0)
				for (int j = 0; j < str.Length; j++)
				{
					string temp = str[j].Substring(0, str[j].Length - 8);
					if (!UniquePaths.Contains(temp))
					{
						UniquePaths.Add(temp);
						CheckJobName(str[j]);
					}
				}
		}
		}
	
		
		 


		
		
		 
	 

		return null;
	}
	  static string DeleteAllSybmols(string str)
	{
		string temps="";
		for (int i = 0; i < str.Length; i++)
		{
			if ((str.Substring(i, 1) == "0")) temps += str[i];
			if ((str.Substring(i, 1) == "1")) temps += str[i];
			if ((str.Substring(i, 1) == "2")) temps += str[i];
			if ((str.Substring(i, 1) == "3")) temps += str[i];
			if ((str.Substring(i, 1) == "4")) temps += str[i];
			if ((str.Substring(i, 1) == "5")) temps += str[i];
			if ((str.Substring(i, 1) == "6")) temps += str[i];
			if ((str.Substring(i, 1) == "7")) temps += str[i];
			if ((str.Substring(i, 1) == "8")) temps += str[i];
			if ((str.Substring(i, 1) == "9")) temps += str[i];
		}
 
		
		return temps;
	}
	
	  static int GetJobIdByFileName(string rendername)
	{
		if(Program.Jobs!=null)
		for (int i = 0; i < Program.Jobs.Count; i++)
		{
			if (Program.Jobs[i].RenderPath == rendername) return i;
		}
		return -1;
	}
}
