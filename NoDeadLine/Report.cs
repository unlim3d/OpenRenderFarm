using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using ICSharpCode.SharpZipLib.BZip2;
using System.Text;
using System.Diagnostics;

public class Report
{
    public string path;
     
   public bool check;
   public Report(string path_, bool check_)
    {
        path = path_;
        check = check_;
    }
    
   public static void CreaterReports()
    {
        Stopwatch clock = Stopwatch.StartNew();
      
        List<string> strs = new List<string>();
        strs.AddRange(Program.SearchFile(FarmSettings.DeadLineReportFolderSlaves, "*.bz2").ToList());
        int c = 0;
        Console.WriteLine("Собираем репорты:" +strs.Count);

        foreach (var item in strs)
        {
            Tools.ClearCurrentConsoleLine();
            if (Program.Reports.Find(x => x.path == item)==null)
            {
                Console.Write(Program.Reports.Count);
                Report R = new Report(item,false);
                R.path = item;
                R.check = false;
                Program.Reports.Add(R);
                c++;
                DecompressAndLoadDeadlineSlaveReport(R.path);
                
            //    Thread newThreadReports = new Thread(Report.CheckOneReportInThread);
            //    newThreadReports.Start(R.path);

            }
           
        }
        clock.Stop();
        Tools.ClearCurrentConsoleLine();
        Console.WriteLine("\n"+clock.Elapsed+ " Новых Репортов обнаружено: " + c);
        Console.WriteLine("______________________________________________________");
    }
    public static void CheckOneReportInThread(object data)
    { 
            DecompressAndLoadDeadlineSlaveReport(data.ToString());
    }
    static void DecompressAndLoadDeadlineSlaveReport(string path)
    {/*
        string zipFileName = @path;
        if (File.Exists(zipFileName))
            using (FileStream fileToDecompressAsStream = File.OpenRead(zipFileName))
            {
                MemoryStream MS = new MemoryStream();

                BZip2.Decompress(fileToDecompressAsStream, MS, false);

                _ = MS.Seek(0, SeekOrigin.Begin);
                 
                    string result = Encoding.ASCII.GetString(MS.ToArray());
                    MS.Close();
                    fileToDecompressAsStream.Close();



                   int SeekPoint = result.IndexOf("Saved image to ");
                    if (SeekPoint != -1)
                    {
                        RenderTask temptask = new RenderTask(Tools.FindToStrEnd(result.Substring(SeekPoint + 15, 400), -1));
                        
                    }
                    else
                    {
                        SeekPoint = result.IndexOf("copying to ");
                        if (SeekPoint != -1)
                        {
                            RenderTask temptask = new RenderTask(Tools.FindToStrEnd(result.Substring(SeekPoint + 11, 400), -1));
                            temptask.OriginalFileName = Tools.FindToStrEnd(result.Substring(SeekPoint + 11, 400), -1);
                            temptask.ReportPath = path;














                            SeekPoint = result.IndexOf("Render frame ");
                            if (SeekPoint != -1)
                            {

                                string temp = result.Substring(SeekPoint + 13, 6);
                                _ = int.TryParse(Tools.FindToStrEnd(temp, 0), out temptask.Frame);

                            }
                            else return;
                          
                            
                            SeekPoint = result.IndexOf("Slave Name: ");

                            if (SeekPoint != -1)
                            {
                                temptask.Slave = Tools.FindToStrEnd(result.Substring(SeekPoint + 12, 100), -1);
                            }

                            if ((result.IndexOf("V-Ray")) != -1) temptask.Renderer = "V-Ray";
                            else temptask.Renderer = "RedShift";


                            Job.CheckJobName(temptask.OriginalFileName);
                            return;

                        }
                }
                 
            }

         */
    }

    
}