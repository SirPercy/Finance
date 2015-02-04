using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Finance.Core.Configuration;
using Finance.Models.Entities;

namespace Finance.Core.Utilities
{
    public class QuoteService
    {
        public static QuoteInfo GetHistoricalPrice(string symbol, DateTime forDate)
        {
            try {
                var startdatetogethistoricaldata = DateTime.Now.AddDays(-30);
                var webreq = (HttpWebRequest)WebRequest.Create(string.Format(Constants.YahooUrl, symbol, (startdatetogethistoricaldata.Month - 1), startdatetogethistoricaldata.Day, startdatetogethistoricaldata.Year, (forDate.Month - 1), forDate.Day, forDate.Year));
                var webresp = (HttpWebResponse)webreq.GetResponse();

                using (var reader = new StreamReader(webresp.GetResponseStream(), Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("Date"))
                            continue;
                        else
                        {
                            var content = line.Replace("\"", "");
                            var contents = content.Split(',');
                            return new QuoteInfo
                            {

                                Symbol = symbol,
                                Last = contents.Length > 1 ? contents[4] : "",
                                Date = contents.Length > 2 ? contents[0] : "",
                            };
                            break;
                        }
               
                    }


                }

            }
            catch (Exception)
            {
                return new QuoteInfo();
            }
            return new QuoteInfo();
        }

        public static IEnumerable<QuoteInfo> GetHistoricalPrices(string symbol, DateTime fromDate, DateTime toDate)
        {
            var quoteInfoList = new List<QuoteInfo>();
            try
            {
                var webreq = (HttpWebRequest)WebRequest.Create(string.Format(Constants.YahooMonthUrl, symbol, (fromDate.Month - 1), fromDate.Day, fromDate.Year, (toDate.Month - 1), toDate.Day, toDate.Year));
                var webresp = (HttpWebResponse)webreq.GetResponse();

                using (var reader = new StreamReader(webresp.GetResponseStream(), Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("Date"))
                            continue;
                        else
                        {
                            var content = line.Replace("\"", "");
                            var contents = content.Split(',');
                            quoteInfoList.Add(new QuoteInfo
                            {

                                Symbol = symbol,
                                Last = contents.Length > 1 ? contents[4] : "",
                                Date = contents.Length > 2 ? contents[0] : "",
                            });
                            
                        }

                    }


                }

            }
            catch (Exception)
            {
                return quoteInfoList;
            }
            return quoteInfoList;
        }
        public static QuoteInfo GetTodaysPrice(string symbol)
        {
            try
            {
                var webreq = (HttpWebRequest)WebRequest.Create(string.Format("http://finance.yahoo.com/d/quotes.csv?s={0}&f=k1", symbol));
                var webresp = (HttpWebResponse)webreq.GetResponse();

                using (var reader = new StreamReader(webresp.GetResponseStream(), Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var result =  GetStrBetweenTags(line, "<b>", "</b>");
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            return new QuoteInfo
                            {

                                Symbol = symbol,
                                Last = result,
                                Date = DateTime.Now.ToString()
                            };
                        }
                    }


                }

            }
            catch (Exception)
            {
                return new QuoteInfo();
            }
            return new QuoteInfo();
        }

        public static string GetStrBetweenTags(string value,
                                       string startTag,
                                       string endTag)
        {
            if (value.Contains(startTag) && value.Contains(endTag))
            {
                int index = value.IndexOf(startTag) + startTag.Length;
                return value.Substring(index, value.IndexOf(endTag) - index);
            }
            else
                return null;
        }

    }
}