using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ns_HAR_parser.Services;

namespace ns_HAR_parser
{
    public class HAR_parser
    {
        private string baseURL_zillow = "https://www.zillow.com";
        private string baseURL_redfin = "https://www.redfin.com";
        private Utils.Logger myLogger = new Utils.Logger();

        public DataTable ParseFile(string filepath)
        {
            DataTable homes = new DataTable();

            try
            {
                string data_file = ImportDataFile(filepath);
                if (!String.IsNullOrEmpty(data_file))
                {
                    homes = ProcessDataFile(data_file);
                }
                return homes;
            }
            catch (Exception ex)
            {
                WriteToLogFile("ParseFile, " + ex.Message, Utils.Logger.logMessageType.ERROR);
                return homes;
            }
        }

        private string ImportDataFile(string filepath)
        {
            try
            {
                if (String.IsNullOrEmpty(filepath))
                {
                    throw new Exception("ImportDataFile: data source file is empty ");
                }

                //read entire file into a single string.
                string result = File.ReadAllText(filepath);

                //verify JSON formatted file 
                if (Utils.MyUtils.IsValidJson(result))
                {
                    WriteToLogFile(string.Format("Source file '{0}' successfully imported; length: {1} ", filepath, result.Length), Utils.Logger.logMessageType.PROCESS);
                }
                else
                {
                    throw new Exception(string.Format("Source file '{0}' is NOT a valid JSON file, filepath: ", filepath));
                }
                return result;
            }
            catch (Exception ex)
            {
                WriteToLogFile("ImportDataFile, " + ex.Message, Utils.Logger.logMessageType.ERROR);
                return string.Empty;
            }
        }

        private DataTable ProcessDataFile(string data_file)
        {
            DataTable homes = new DataTable();

            try
            {
                IProvider provider = GetProvider(data_file);
                provider.ProcessContentData();
                return provider.Homes;
            }
            catch (Exception ex)
            {
                WriteToLogFile("ProcessDataFile, " + ex.Message, Utils.Logger.logMessageType.ERROR);
                return homes;
            }
        }

        private IProvider GetProvider(string data_file)
        {
            IProvider provider = null;

            if (data_file.ToLower().IndexOf(baseURL_zillow) > 0)
            {
                provider = new ZillowProvider(data_file);
            }
            else if (data_file.IndexOf(baseURL_redfin) > 0)
            {
                provider = new RedfinProvider(data_file);
            }

            // return appropriate provider
            if (provider != null)
            {
                return provider;
            }
            else
            {
                throw new Exception("Unable to identify provider based upon specified data file: ");
            }
        }

        private void WriteToLogFile(string msg, Utils.Logger.logMessageType msgType)
        {
            myLogger.WriteToLog(msg, msgType);
        }
    }

}