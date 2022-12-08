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
    public class File_Record
    {
        public string name;
        public string fullName;
        public DateTime created;
        public long length;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UploadedFiles : ControllerBase
    {
        private const string UPLOADSFILE_DIRECTORY = "\\Uploads\\";

        // GET: api/<UploadedFiles>
        [HttpGet]
        public List<File_Record> Get()
        {
            List<File_Record> response = new List<File_Record>();
            string dir = MyUtils.GetWorkingDirectory() + UPLOADSFILE_DIRECTORY;
            DirectoryInfo d = new DirectoryInfo(dir);

            FileInfo[] Files = d.GetFiles("*.har"); // getting HAR files only
            foreach (FileInfo file in Files)
            {
                File_Record elem = new File_Record();
                elem.name = file.Name;
                elem.fullName = file.FullName;
                elem.length = file.Length;
                elem.created = file.CreationTime;

                response.Add(elem);
            }

            return response;
        }

        // GET: api/<GetUploadedFileNames>
        [Route("GetUploadedFileNames")]
        [HttpGet]
        public List<string> GetUploadedFileNames()
        {
            List<string> response = new List<string>();
            string dir = MyUtils.GetWorkingDirectory() + UPLOADSFILE_DIRECTORY;
            DirectoryInfo d = new DirectoryInfo(dir);

            FileInfo[] Files = d.GetFiles("*.har"); // getting HAR files only
            foreach (FileInfo file in Files)
            {
                response.Add(file.Name);
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
