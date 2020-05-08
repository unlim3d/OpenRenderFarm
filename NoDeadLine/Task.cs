using System;
using System.Collections.Generic;
using System.IO;

public class RenderTask
{
    public RenderTask(string OriginalFile,string _ServerPreviewFileName)
    {
        OriginalFileName = OriginalFile;
        ServerPreviewFileName = _ServerPreviewFileName;
    }
    
    public string SceneName;
    public long Weight;
    public float RenderTime;
    public string Slave;
    public int Frame;
    public DateTime CreationDate;
    public string Renderer;
    public Job Jobinstance;
    public int jobid;
    public string ReportPath;
    public Status CurrentStatus = Status.Skipped;
    private string _OriginalFileName;
    public string ServerPreviewFileName;
    public string OriginalFileName
    {
        set
        { 

                _OriginalFileName =value;
              
                Frame = GetFrameNumberFromFileName(value);
             //   jobid = Jobinstance.Id;
                
            if(File.Exists(value))  
            Weight = new System.IO.FileInfo(value).Length;

             
        }
        get
        {
            return _OriginalFileName;
        }
    }

    private string GetPathWithoutFileName(string value)
    {
        int lastPathSymbol = value.LastIndexOf(@"\");
        return value.Substring(0, lastPathSymbol + 1);
    }

   
    public static string GetServerPreviewFileNameByOriginalFileName(  string value, Job joba)
    {
             string str = Path.Combine( FarmSettings.SitePath ,joba.Id+"_"+ Path.GetFileNameWithoutExtension(value));
             
            str = str.Replace(@"\\", @"\");
        return   str+".jpg";
    }
    public string ServerPreviewJson
    {
        get
        {
            return ServerPreviewFileName.Substring(0, ServerPreviewFileName.Length - 4)+".json";
        }
    }

    public enum Status
    {
        Skipped = -1,
        Completed =0,
        Damaged=1,
        Copyed=2,

        

    }

   
    public static string GetDirectoryPathFromFile(string filepath)
    {
        int seek = filepath.LastIndexOf("\\");
        return filepath.Substring(0, seek);
    }
    public static int GetFrameNumberFromFileName(string filename)
    {

        int frame = 0;
        int.TryParse(filename.Substring(filename.Length - 8, 4), out frame);
        return frame;
    }
}
