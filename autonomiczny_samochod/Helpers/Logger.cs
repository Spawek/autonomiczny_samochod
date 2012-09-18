using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace autonomiczny_samochod
{
    static class Logger
    {
        private static string logFile = "Log.txt";
        private static bool isItFirstLog = true;

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

            }

            string msgWithDateAndObjectName = String.Format("[{0}]:<<'{1}'>>:   {2}",
                loggingObj.ToString(),
                String.Format(@"{0:mm\:ss\:ff}", Time.GetTimeFromProgramBeginnig()),
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

    }
}
