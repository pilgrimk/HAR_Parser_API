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
    [Route("api/[controller]")]
    public class LogFileController : Controller
    {
        private ns_HAR_parser.Utils.Logger myLogger = new ns_HAR_parser.Utils.Logger();

        [Route("GetLogFile")]
        [HttpPost]
        public JsonResult GetLogFile()
        {
            try
            {
                WriteToLogFile("GetLogFile, getting log file data", ns_HAR_parser.Utils.Logger.logMessageType.PROCESS);
                string[] result = myLogger.GetLogFile();
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                WriteToLogFile("GetLogFile, " + ex.Message, ns_HAR_parser.Utils.Logger.logMessageType.ERROR);
                return new JsonResult("GetLogFile Error: " + ex.Message);
            }
        }

        [Route("DeleteLogFile")]
        [HttpPost]
        public JsonResult DeleteLogFile()
        {
            try
            {
                myLogger.DeleteLogFile();
                return new JsonResult("Log file deleted");
            }
            catch (Exception ex)
            {
                WriteToLogFile("DeleteLogFile, " + ex.Message, ns_HAR_parser.Utils.Logger.logMessageType.ERROR);
                return new JsonResult("DeleteLogFile Error: " + ex.Message);
            }
        }

        private void WriteToLogFile(string msg, ns_HAR_parser.Utils.Logger.logMessageType msgType)
        {
            myLogger.WriteToLog(msg, msgType);
        }
    }
}
