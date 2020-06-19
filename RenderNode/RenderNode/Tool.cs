using System;
using System.IO;

public class Tools {
    static public void OverrideLogToFile()
    {
        FileStream ostrm;
        StreamWriter writer;
        TextWriter oldOut = Console.Out;
        try
        {
            ostrm = new FileStream(Path.Combine(RenderNodeVariables.LogDirectory,"Log.txt"), FileMode.OpenOrCreate, FileAccess.Write);
            writer = new StreamWriter(ostrm);
        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot open Redirect.txt for writing");
            Console.WriteLine(e.Message);
            return;
        }
        Console.SetOut(writer); 
        Console.SetOut(oldOut);
        writer.Close();
        ostrm.Close();
        Console.WriteLine("Done");
    }
}