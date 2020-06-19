using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using Newtonsoft.Json;

#region Reports 
public class RenderNodeReport
{
    public string NameManual;// заданное пользователем имя для рендер ноды
    public string NamePC;// имя компьютера в ОС
    public string AdressIPGlobal;// глобальный адресс в интернете
    public string AdressIPLocal;// локальный адресс внутри сети
    public string AdressGEO;//адрес на карте мира

   

    public string [] Disks;// локальные диски
    public double[] SpaceTotal;// максимальный объем дисков
    public double []SpaceFree;// свободный сейчас объем дисков
    public float RAMsize;// общий объем оперативной памяти
    public float RAMFrequency;// частота памяти
    public int RAMChannels;// каналы памяти
    public string CPUName;// имя процессора
    public int CPUCount;// количество ядер/потоков
    public string[] GPUs;// названия видеокарт    
}
#endregion
#region Settings
public class RenderNodeSettings
{
    public RenderNodeMode CurrentMode= RenderNodeSettings.RenderNodeMode.Default;
    public string AdressIPFarmCurrent= "127.0.0.1"; // IP адрес рендер фермы для этой ноды
    public string RenderPath="RenderDirectory"; // Локальный путь сохранения рендеров
    public enum RenderNodeMode
    {   
        Default =-1,
        Disabled = 0,// отключено вручную
        Ready = 1,// ожидает задач
        Rendering = 2,// выполняет задачу
        Waiting = 3, // ожидает начала рендеринга
        Stalled = 4// зависла 
    }
    public RenderNodeSettings (RenderNodeMode mode, string adress, string rPath)// ручная смена настроек пользователем
    {
        CurrentMode = mode;
        AdressIPFarmCurrent = adress;
        RenderPath = rPath;
        File.WriteAllText(Path.Join(RenderNodeVariables.SettingsDirectory, RenderNodeVariables.SettingsFileName), JsonConvert.SerializeObject(this));
    }
    public RenderNodeSettings() // стартовый запуск ноды каждый раз
    {
      
      if(!File.Exists(Path.Join(RenderNodeVariables.SettingsDirectory, RenderNodeVariables.SettingsFileName))) 
        {            
            File.WriteAllText(Path.Join(RenderNodeVariables.SettingsDirectory, RenderNodeVariables.SettingsFileName), JsonConvert.SerializeObject(this));
        }
    }
}
#endregion
#region Variables
public class RenderNodeVariables
{
    public static string SettingsDirectory
    {
        get
        {
            return Directory.GetCurrentDirectory();
        }
    }
    public static string SettingsFileName = "Settings.json";
    public static string ReportsDirectory
    {
        get
        {
            if (!Directory.Exists(Path.Combine(SettingsDirectory, "Reports")))
                Directory.CreateDirectory(Path.Combine(SettingsDirectory, "Reports"));
            return Path.Combine(SettingsDirectory, "Reports");
        }
    }
    public static string RenderDirectory;
    public static string LogDirectory
    {
        get
        {
            return SettingsDirectory;
        }
    }
}

    public class Node
    {

    }
    #endregion
#region LocalServer
    public class LocalServer
    {
        public static void StartListen()
        {
            Thread th1 = new Thread(ClientListener);
            th1.Start();
        }
        public static void ClientListener()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            string[] prefixes = { "http://127.0.0.1:8091/" };

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request. 
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                Handle(context);
            }
            listener.Stop();
        }
    public static void Handle(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        // Obtain a response object.
        HttpListenerResponse response = context.Response;

        if (request.HttpMethod == "OPTIONS")
        {
            response.Headers.Add("Access-Control-Allow-Headers", "*");
            response.Headers.Add("Access-Control-Allow-Origin", "*");//TODO: set multiple
            System.IO.Stream opts_output = response.OutputStream;
            opts_output.Close();
            return;
        }
        if (request.RawUrl.IndexOf("Settings") != -1)
        { 
        string responseString = "<HTML><BODY> Hello Сучка!" + request.HttpMethod + "</BODY></HTML>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.Headers.Add("Access-Control-Allow-Origin", "*");//TODO: set multiple

        string body = GetRequestData(request);

        // Get a response stream and write the response to it.
        response.ContentLength64 = buffer.Length;
        System.IO.Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);

        // You must close the output stream.
        output.Close();
        }
        }

        private static string GetRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return "";
            }
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            // Convert the data to a string and display it on the console.
            string s = reader.ReadToEnd();
            Console.WriteLine(s);
            Console.WriteLine("End of client data:");
            body.Close();
            reader.Close();
            // If you are finished with the request, it should be closed also.
            return s;
        }
    }
    #endregion
