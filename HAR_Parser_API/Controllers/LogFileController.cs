using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace HAR_Parser_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogFileController : Controller
    {
        private ns_HAR_parser.Utils.Logger myLogger = new ns_HAR_parser.Utils.Logger();

        [Route("Test")]
        [HttpGet]
        public JsonResult Test()
        {
            return new JsonResult("Test");
        }

        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                string[] result = myLogger.GetLogFile();
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                WriteToLogFile("LogFile Get, Error: " + ex.Message, ns_HAR_parser.Utils.Logger.logMessageType.ERROR);
                return new JsonResult("LogFile Get, Error: " + ex.Message);
            }
        }

        [HttpDelete]
        public JsonResult Delete()
        {
            try
            {
                myLogger.DeleteLogFile();
                return new JsonResult("Log file deleted");
            }
            catch (Exception ex)
            {
                WriteToLogFile("LogFile Delete, Error:" + ex.Message, ns_HAR_parser.Utils.Logger.logMessageType.ERROR);
                return new JsonResult("LogFile Delete, Error:" + ex.Message);
            }
        }

        private void WriteToLogFile(string msg, ns_HAR_parser.Utils.Logger.logMessageType msgType)
        {
            myLogger.WriteToLog(msg, msgType);
        }
    }
}