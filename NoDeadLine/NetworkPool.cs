using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Net;
public class NetworkPool
{
	public  NetworkPool()
	{
	 
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
			Console.WriteLine(hostName);
		// Get the IP  
			IPAdress = Dns.GetHostByName(hostName).AddressList[0].ToString();
			Console.WriteLine("this machine IP Address is :" + IPAdress);
			SaveJSON(this);
		 
	}
	public string DNS;
	public string IPAdress;
	public string Port;
	 



public static void SaveJSON(NetworkPool pool)
{
	string SavePath = System.IO.Directory.GetCurrentDirectory().ToString() + "//" + "NetworkPool//White//";
	string output = JsonConvert.SerializeObject(pool);
	File.WriteAllText(SavePath+".json", output);
	Console.ForegroundColor = ConsoleColor.DarkGreen;
}
}