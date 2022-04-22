﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ns_HAR_parser;
using System.Data;
using System.IO;
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

        ns_HAR_parser.Utils.Logger myLogger = new ns_HAR_parser.Utils.Logger();

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
                var physicalPath = _env.ContentRootPath + UPLOADS_DIRECTORY + fileName;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
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
                    var physicalPath = _env.ContentRootPath + UPLOADS_DIRECTORY + fileName;

                    homes = parser.ParseFile(physicalPath);
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