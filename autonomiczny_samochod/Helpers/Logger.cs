using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace autonomiczny_samochod
{
    class Logger
    {
        private static string logFile = "Log.txt";
        private static string statsFile = "Stats.txt";
        private static bool isItFirstLog = true;
        private static DateTime firstLogTime;

        public static void Log(Object loggingObj, string msg)
        {
            if (isItFirstLog)
            {
                isItFirstLog = false;

                //remove old log
                if (File.Exists(logFile))
                {
                    File.Delete(logFile);
                }

                //save loging time of first log
                firstLogTime = DateTime.Now;
            }

            string msgWithDateAndObjectName = String.Format("[{0}]:<<'{1}'>>:   {2}",
                loggingObj.ToString(),
                String.Format(@"{0:mm\:ss\:ff}", GetTimeFromFirstLog()),
                msg
            );
            
            Console.WriteLine(msgWithDateAndObjectName);

            try
            {
                using (StreamWriter sw = new StreamWriter(logFile, true))
                {
                    sw.WriteLine(msgWithDateAndObjectName);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Logger couldn't write above msg to log");
            }
        }

        private static TimeSpan GetTimeFromFirstLog()
        {
            return DateTime.Now - firstLogTime;
        }
    }
}
