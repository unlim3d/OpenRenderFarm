using System;
using System.Net;
using System.IO;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;

public abstract class HttpRequestHandler
{
    
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
        // Construct a response.
        string responseString = "<HTML><BODY> Hello world!" + request.HttpMethod + "</BODY></HTML>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.Headers.Add("Access-Control-Allow-Origin", "*");//TODO: set multiple

        string body = GetRequestData(request);
       
        // Get a response stream and write the response to it.
        response.ContentLength64 = buffer.Length;
        System.IO.Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        Job.CheckJobName(body);
        Tools.OpenFolder();
        // You must close the output stream.
        output.Close();
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
