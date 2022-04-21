using System.Data;

namespace ns_HAR_parser.Services
{
    interface IProvider
    {

        string DataFile
        {
            get;
        }

        DataTable Homes
        {
            get;
        }

        void ProcessContentData();
    }
}
