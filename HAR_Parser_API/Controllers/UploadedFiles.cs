using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using ns_HAR_parser.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HAR_Parser_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadedFiles : ControllerBase
    {
        private const string UPLOADSFILE_DIRECTORY = "\\Uploads\\";

        // GET: api/<UploadedFiles>
        [HttpGet]
        public string Get()
        {
            string response = "";
            string dir = MyUtils.GetWorkingDirectory() + UPLOADSFILE_DIRECTORY;
            DirectoryInfo d = new DirectoryInfo(dir);

            FileInfo[] Files = d.GetFiles("*.har"); // getting HAR files only
            foreach (FileInfo file in Files)
            {
                if (string.IsNullOrEmpty(response))
                {
                    response = file.Name;
                }
                else 
                {
                    response = response + "," + file.Name;
                }
            }

            return response;
        }

        // DELETE api/<UploadedFiles>/5
        [HttpDelete("{filename}")]
        public string Delete(string filename)
        {
            string response = "File not found";
            string dir = MyUtils.GetWorkingDirectory() + UPLOADSFILE_DIRECTORY;
            DirectoryInfo d = new DirectoryInfo(dir);

            FileInfo[] Files = d.GetFiles("*.har"); // getting HAR files only
            foreach (FileInfo file in Files)
            {
                if (file.Name == filename)
                {
                    file.Delete();
                    response = "File deleted";
                }
            }

            return response;
        }
    }
}
