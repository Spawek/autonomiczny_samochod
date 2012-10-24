﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Helpers
{
    public static class Logger
    {
        private static string logFile = "Log.txt";
        private static bool isItFirstLog = true;
        private static int MAX_PRIORITY = 10;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingObj"></param>
        /// <param name="msg"></param>
        /// <param name="priority">0 = lowest, more -> bigger priority</param>
        public static void Log(Object loggingObj, string msg, int priority = 0)
        {
            if (priority > MAX_PRIORITY)
                throw new ArgumentException("priority is too big", "priority");

            if (isItFirstLog)
            {
                isItFirstLog = false;

                //removing old log files
                for (int i = 0; i <= MAX_PRIORITY; i++) //for priorities > 0
                {
                    if (File.Exists(logFile + Convert.ToString(i)))
                    {
                        File.Delete(logFile + Convert.ToString(i));
                    }    
                }

            }

            string priorityMsg = String.Empty;
            if (priority > 0)
            {
                priorityMsg = String.Format("<<<PRIORITY:{0}>>>", priority);
            }

            string loggingObjName = String.Empty;
            if(loggingObj != null)
            {
                loggingObjName = loggingObj.ToString();
            }

            string msgWithDateAndObjectName = String.Format("{0}[{1}]:<<'{2}'>>:   {3}",
                priorityMsg,
                loggingObjName,
                String.Format(@"{0:mm\:ss\:ff}", Time.GetTimeFromProgramBeginnig()),
                msg
            );
            
            Console.WriteLine(msgWithDateAndObjectName);

            for (int i = 0; i <= priority; i++)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(logFile + Convert.ToString(i), true)) //TODO: add some buffors or something, files are being oppened and closed all the time now
                    {
                        sw.WriteLine(msgWithDateAndObjectName);
                    } 
                }
                catch (Exception e)
                {
                    Console.WriteLine("Logger couldn't write above msg to log");
                }
            }

        }

    }
}