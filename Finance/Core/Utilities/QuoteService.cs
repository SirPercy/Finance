using System;
using System.IO;
using System.Net;
using System.Text;
using Finance.Core.Configuration;
using Finance.Models.Entities;
using System.Linq;

namespace Finance.Core.Utilities
{
    public class QuoteService
    {
        public static QuoteInfo GetPrice(string symbol, DateTime forDate)
        {
            // Set the return string to null.
            string result = null;
            try {
                var webreq = (HttpWebRequest)WebRequest.Create(string.Format(Constants.YahooUrl, symbol, (forDate.Month - 1), forDate.Day, forDate.Year, (forDate.Month - 1), forDate.Day, forDate.Year));
                var webresp = (HttpWebResponse)webreq.GetResponse();

                using (var reader = new StreamReader(webresp.GetResponseStream(), Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if(line.StartsWith("Date"))
                            continue;
                        var content = line.Replace("\"", "");
                        var contents = content.Split(',');
                        return new QuoteInfo
                        {

                            Symbol = symbol,
                            Last = contents.Length > 1 ? contents[4] : "",
                            Date = contents.Length > 2 ? contents[0] : "",
                        };
               
                    }


                }

            }
            catch (Exception)
            {
                return new QuoteInfo();
            }
            return new QuoteInfo();
        }   

    }
}