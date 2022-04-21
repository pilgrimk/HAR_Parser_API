using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ns_HAR_parser.Services
{
    class ZillowProvider : IProvider
    {
        private string _xml_text;
        private DataTable _homes_tbl = new DataTable();

        public ZillowProvider(string data_file)
        {
            _xml_text = data_file;
        }

        public string DataFile
        {
            get { return _xml_text; }
        }

        public DataTable Homes
        {
            get { return _homes_tbl; }
        }

        public void ProcessContentData()
        {
            throw new NotImplementedException();
        }
    }
}
