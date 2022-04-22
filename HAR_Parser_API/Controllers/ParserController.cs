using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ns_HAR_parser;
using ns_HAR_parser.Utils;
using System.Data;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace HAR_Parser_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private const string UPLOADS_DIRECTORY = "\\Uploads\\";
        private const string FILE_NAME_KEY = "fileName";

        private ns_HAR_parser.Utils.Logger myLogger = new ns_HAR_parser.Utils.Logger();

        public ParserController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [Route("UploadFile")]
        [HttpPost]
        public JsonResult UploadFile()
        {
            try 
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                var fileName = postedFile.FileName;
                var uploadPath = ns_HAR_parser.Utils.MyUtils.GetWorkingDirectory() + UPLOADS_DIRECTORY + fileName;
                var uploadDir = Path.GetDirectoryName(uploadPath);

                if (!Directory.Exists(uploadDir))
                {
                    WriteToLogFile(string.Format("UploadFile, creating directory: {0}", uploadDir), ns_HAR_parser.Utils.Logger.logMessageType.PROCESS);
                    Directory.CreateDirectory(Path.GetDirectoryName(uploadDir));
                }

                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(fileName);
            }
            catch (Exception ex)
            {
                WriteToLogFile("UploadFile, " + ex.Message, ns_HAR_parser.Utils.Logger.logMessageType.ERROR);
                return new JsonResult("UploadFile Error: " + ex.Message);
            }
        }


        [Route("ParseFile")]
        [HttpPost]
        public JsonResult ParseFile()
        {
            try 
            {
                DataTable homes = new DataTable();
                var httpRequest = Request.Form;
                Microsoft.Extensions.Primitives.StringValues fileName;

                if (httpRequest.TryGetValue(FILE_NAME_KEY, out fileName))
                {
                    HAR_parser parser = new HAR_parser();
                    var physicalPath = ns_HAR_parser.Utils.MyUtils.GetWorkingDirectory() + UPLOADS_DIRECTORY + fileName;

                    if (System.IO.File.Exists(physicalPath))
                    {
                        homes = parser.ParseFile(physicalPath);
                    }
                    else
                    {
                        WriteToLogFile(string.Format("ParseFile, specified file does not exist: {0}", physicalPath), ns_HAR_parser.Utils.Logger.logMessageType.PROCESS);
                    }
                }
                else
                {
                    throw new Exception(string.Format("ParseFile: {0} key not found in passed request ", FILE_NAME_KEY));
                }

                return new JsonResult(homes);
            }
            catch (Exception ex)
            {
                WriteToLogFile("ParseFile, " + ex.Message, ns_HAR_parser.Utils.Logger.logMessageType.ERROR);
                return new JsonResult("ParseFile Error: " + ex.Message);
            }
        }

        private void WriteToLogFile(string msg, ns_HAR_parser.Utils.Logger.logMessageType msgType)
        {
            myLogger.WriteToLog(msg, msgType);
        }
    }
}
