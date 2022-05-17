using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns_HAR_parser.Utils;
using System.Data;

namespace ns_HAR_parser.Services
{
    class ZillowProvider : IProvider
    {
        Utils.Logger myLogger = new Utils.Logger();
        private string _xml_text;
        private DataTable _homes_tbl = new DataTable();

        private JObject obj_JSON_file;
        private List<JToken> homes_data = new List<JToken>();

        // Constructor
        public ZillowProvider(string data_file)
        {
            _xml_text = data_file;
            obj_JSON_file = JObject.Parse(_xml_text);
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
            Dictionary<string, string> mlsListings = new Dictionary<string, string>();
            _homes_tbl.Clear();
            long homes_counter_total = 0;

            // we're looking for "Content" elements in JSON data file
            homes_data.Clear();
            IEnumerable<JToken> content_json = obj_JSON_file.SelectTokens("$..content");

            foreach (JToken content in content_json)
            {
                int size = (int)content["size"];
                if (((int)content["size"] > 0) &&
                    ((string)content["mimeType"] == "application/json"))
                {
                    string data = (string)content["text"];
                    JObject json = JObject.Parse(data);

                    JToken cat1_value;
                    if (json.TryGetValue("cat1", out cat1_value))
                    {
                        JToken searchResults_value;
                        JObject obj_cat1_value = cat1_value.ToObject<JObject>();
                        if (obj_cat1_value.TryGetValue("searchResults", out searchResults_value))
                        {
                            JToken listResults_value;
                            JObject obj_searchResults_value = searchResults_value.ToObject<JObject>();
                            if (obj_searchResults_value.TryGetValue("listResults", out listResults_value))
                            {
                                //add to the homes_data list
                                homes_data.Add(listResults_value);

                                foreach (JToken home in listResults_value.Children())
                                {
                                    try
                                    {
                                        homes_counter_total += 1;
                                        MyUtils.Home_Record home_rec = new MyUtils.Home_Record();

                                        // check to see if the MLS number already exists in the dictionary
                                        home_rec.mlsId = (string)home["zpid"];
                                        home_rec.mlsStatus = (string)home["statusType"];
                                        if ((!mlsListings.ContainsKey(home_rec.mlsId)) && (home_rec.mlsStatus == "FOR_SALE"))
                                        {
                                            // get the rest of the JSON data
                                            home_rec.price = (long)home["unformattedPrice"];
                                            home_rec.sqFt = (long)home["area"];
                                            decimal price_per_sqft = (home_rec.price / home_rec.sqFt);
                                            home_rec.pricePerSqFt = (long)Math.Floor(price_per_sqft);
                                            decimal lotAreaValue = (long)home["hdpData"]["homeInfo"]["lotAreaValue"];
                                            home_rec.lotSize = (long)Math.Floor(lotAreaValue);
                                            home_rec.beds = (int)home["beds"];
                                            home_rec.baths = (decimal)home["baths"];
                                            home_rec.latitude = (decimal)home["latLong"]["latitude"];
                                            home_rec.longitude = (decimal)home["latLong"]["longitude"];
                                            home_rec.address1 = (string)home["addressStreet"];
                                            home_rec.city = (string)home["addressCity"];
                                            home_rec.state = (string)home["addressState"];
                                            home_rec.zip = (string)home["addressZipcode"];
                                            home_rec.countryCode = (string)home["hdpData"]["homeInfo"]["country"];
                                            home_rec.listingId = (long)home["id"];
                                            home_rec.url = (string)home["detailUrl"];

                                            home_rec.daysListed = "";
                                            if ((string)home["variableData"]["type"] == "DAYS_ON")
                                            {
                                                home_rec.daysListed = (string)home["variableData"]["text"];
                                                home_rec.daysListed = home_rec.daysListed.Replace(" on Zillow", "");
                                            };

                                            //home_rec.hoa = (string)home["hoa"]["value"];
                                            //home_rec.location = (string)home["location"]["value"];
                                            //home_rec.address2 = (string)home["unitNumber"]["value"];
                                            //home_rec.postalCode = (string)home["postalCode"]["value"];
                                            //home_rec.soldDate = (long)home["soldDate"];
                                            //home_rec.propertyType = (int)home["propertyType"];
                                            //home_rec.listingType = (int)home["listingType"];
                                            //home_rec.propertyId = (long)home["propertyId"];
                                            //home_rec.yearBuilt = (int)home["yearBuilt"]["value"];
                                            //home_rec.listingRemarks = (string)home["listingRemarks"];

                                            // add to the HOME data table
                                            if (_homes_tbl.Columns.Count == 0)
                                            {
                                                AddTableColumns();
                                            }
                                            AddTableRow(home_rec);

                                            // add the MLS to the dictionary
                                            mlsListings.Add(home_rec.mlsId, home_rec.address1);
                                        }
                                    }
                                    catch
                                    {
                                        // just continue for now
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddTableColumns()
        {
            _homes_tbl.Columns.Add("price");
            _homes_tbl.Columns.Add("hoa");
            _homes_tbl.Columns.Add("sqFt");
            _homes_tbl.Columns.Add("pricePerSqFt");
            _homes_tbl.Columns.Add("lotSize");
            _homes_tbl.Columns.Add("beds");
            _homes_tbl.Columns.Add("baths");
            _homes_tbl.Columns.Add("location");
            _homes_tbl.Columns.Add("latitude");
            _homes_tbl.Columns.Add("longitude");
            _homes_tbl.Columns.Add("streetLine");
            _homes_tbl.Columns.Add("unitNumber");
            _homes_tbl.Columns.Add("city");
            _homes_tbl.Columns.Add("state");
            _homes_tbl.Columns.Add("zip");
            _homes_tbl.Columns.Add("postalCode");
            _homes_tbl.Columns.Add("countryCode");
            _homes_tbl.Columns.Add("soldDate");
            _homes_tbl.Columns.Add("propertyType");
            _homes_tbl.Columns.Add("listingType");
            _homes_tbl.Columns.Add("propertyId");
            _homes_tbl.Columns.Add("listingId");
            _homes_tbl.Columns.Add("yearBuilt");
            _homes_tbl.Columns.Add("daysListed");
            _homes_tbl.Columns.Add("url");
            _homes_tbl.Columns.Add("listingRemarks");
        }

        private void AddTableRow(MyUtils.Home_Record home_rec)
        {
            DataRow myRow = _homes_tbl.NewRow();

            myRow["price"] = home_rec.price;
            myRow["hoa"] = home_rec.hoa;
            myRow["sqFt"] = home_rec.sqFt;
            myRow["pricePerSqFt"] = home_rec.pricePerSqFt;
            myRow["lotSize"] = home_rec.lotSize;
            myRow["beds"] = home_rec.beds;
            myRow["baths"] = home_rec.baths;
            myRow["location"] = home_rec.location;
            myRow["latitude"] = home_rec.latitude;
            myRow["longitude"] = home_rec.longitude;
            myRow["streetLine"] = home_rec.address1;
            myRow["unitNumber"] = home_rec.address2;
            myRow["city"] = home_rec.city;
            myRow["state"] = home_rec.state;
            myRow["zip"] = home_rec.zip;
            myRow["postalCode"] = home_rec.postalCode;
            myRow["countryCode"] = home_rec.countryCode;
            myRow["soldDate"] = home_rec.soldDate;
            myRow["propertyType"] = home_rec.propertyType;
            myRow["listingType"] = home_rec.listingType;
            myRow["propertyId"] = home_rec.propertyId;
            myRow["listingId"] = home_rec.listingId;
            myRow["yearBuilt"] = home_rec.yearBuilt;
            myRow["daysListed"] = home_rec.daysListed;
            myRow["url"] = home_rec.url;
            myRow["listingRemarks"] = home_rec.listingRemarks;

            _homes_tbl.Rows.Add(myRow);
        }

        private void WriteToLogFile(string msg, Utils.Logger.logMessageType msgType)
        {
            myLogger.WriteToLog(msg, msgType);
        }
    }
}
