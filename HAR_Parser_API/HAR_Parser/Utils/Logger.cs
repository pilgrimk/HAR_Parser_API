﻿using System;
using System.IO;
using System.Reflection;

namespace ns_HAR_parser.Utils
{
    class Logger
    {
        private string logFilePath = string.Empty;
        private const string logFileName = "Log.txt";
        private const string LOGFILES_DIRECTORY = "\\LogFiles\\";
        private const string timestampFormat = "yyyy-MM-ddTHH:mm:ss";
        private const string ERROR_MSG_TEMPLATE = "[Error]- {0}";
        private const string PROCESS_MSG_TEMPLATE = "[Process]- {0}";

        public enum logMessageType
        {
            ERROR,
            PROCESS
        }

        // instantiate the class
        public Logger()
        {
            logFilePath = ns_HAR_parser.Utils.MyUtils.GetWorkingDirectory() + LOGFILES_DIRECTORY + logFileName;
        }

        public void WriteToLog(string logMessage, logMessageType msgType = logMessageType.ERROR)
        {
            string msg = string.Empty;

            switch (msgType)
            {
                case logMessageType.ERROR:
                    {
                        msg = string.Format(ERROR_MSG_TEMPLATE, logMessage);
                        break;
                    }

                case logMessageType.PROCESS:
                    {
                        msg = string.Format(PROCESS_MSG_TEMPLATE, logMessage);
                        break;
                    }

                default:
                    {
                        msg = logMessage;
                        break;
                    }
            }

            LogWrite(msg);
        }

        public string[] GetLogFile()
        {
            if (!File.Exists(logFilePath))
            {
                CreateEmptyFile(logFilePath);
                return new string[] { string.Format("GetLogFile, new log file created at file location: {0}", logFilePath) };

            }
            return File.ReadAllLines(logFilePath);
        }

        public void DeleteLogFile()
        {
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
        }

        private void LogWrite(string logMessage)
        {
            try
            {
                if (!File.Exists(logFilePath))
                {
                    // create an empty Log file if it does not exist
                    CreateEmptyFile(logFilePath);
                }
                using (StreamWriter w = File.AppendText(logFilePath))
                {
                    w.WriteLine("[{0}] {1}", GetTimestamp(DateTime.Now), logMessage);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        private string GetTimestamp(DateTime value)
        {
            return value.ToString(timestampFormat);
        }

        private void CreateEmptyFile(string filename)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
            {
                // create the directory
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            }

            // create the file
            File.Create(filename).Dispose();
        }
    }
}