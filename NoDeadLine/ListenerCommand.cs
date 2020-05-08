using System;
using System.Net;
using System.Threading;

public class ListenerCommand
{
    public static void StartThread()
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

         string[] prefixes = { "http://127.0.0.1:8090/", "http://nodeadline.mykeenetic.com:8090/"};

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
            HttpRequestHandler.Handle(context);
        }
        listener.Stop();
    }
}