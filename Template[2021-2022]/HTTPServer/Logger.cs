using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
           //create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            
                sr.WriteLine("Datetime : {0}", DateTime.Now.ToString());
                sr.WriteLine("message : {0}",ex);
            
          //  string readText = File.ReadAllText("log.txt");
           // Console.WriteLine(readText);
        }
        }
    }

