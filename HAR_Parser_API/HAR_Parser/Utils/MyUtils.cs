using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System;
using System.Reflection;

namespace ns_HAR_parser.Utils
{
    public static class MyUtils
    {
        public class Home_Record
        {
            public string mlsId;
            public string mlsStatus;
            public long price;
            public string hoa;
            public long sqFt;
            public long pricePerSqFt;
            public long lotSize;
            public int beds;
            public decimal baths;
            public string location;
            public decimal latitude;
            public decimal longitude;
            public string address1;
            public string address2;
            public string city;
            public string state;
            public string zip;
            public string postalCode;
            public string countryCode;
            public long soldDate;
            public int propertyType;
            public int listingType;
            public long propertyId;
            public long listingId;
            public int yearBuilt;
            public long timeOnRedfin;
            public string url;
            public string listingRemarks;
        }

        public static string SerializeObject_XML<T>(this T toSerialize)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, toSerialize);
                    return textWriter.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string SerializeObject_JSON<T>(this T toSerialize)
        {
            try
            {

                DataContractJsonSerializer js = new DataContractJsonSerializer(toSerialize.GetType());
                MemoryStream msObj = new MemoryStream();
                js.WriteObject(msObj, toSerialize);
                msObj.Position = 0;
                StreamReader sr = new StreamReader(msObj);

                // "{\"Description\":\"Share Knowledge\",\"Name\":\"C-sharpcorner\"}"  
                string json = sr.ReadToEnd();

                sr.Close();
                msObj.Close();

                return json;
            }
            catch
            {
                return string.Empty;
            }

        }

        public static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static string GetWorkingDirectory()
        {
            string workingDirectory = "";

            if (Assembly.GetEntryAssembly().Location.IndexOf("bin\\") > 0)
            {
                workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\")));
            }
            else
            {
                workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            return workingDirectory;
        }
    }
}
