using System;
using System.IO;
using System.Reflection;

namespace ns_HAR_parser.Utils
{
    class Logger
    {
        private string assemblyPath = string.Empty;
        private string logFilePath = string.Empty;

        private const string timestampFormat = "yyyy-MM-ddTHH:mm:ss";
        private const string logFileName = "Log.txt";
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
            assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            logFilePath = assemblyPath + "\\" + logFileName;
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

        private void LogWrite(string logMessage)
        {
            try
            {
                using (StreamWriter w = File.AppendText(logFilePath))
                {
                    w.WriteLine("[{0}] {1}", GetTimestamp(DateTime.Now), logMessage);
                }
            }
            catch
            {
            }
        }

        private string GetTimestamp(DateTime value)
        {
            return value.ToString(timestampFormat);
        }
    }
}
