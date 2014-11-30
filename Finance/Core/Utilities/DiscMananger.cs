using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Finance.Core.Utilities
{
    public class DiscMananger
    {

        public static void WriteMissingTickerToFiler(IEnumerable<string> names)
        {
            using (var writer = new StreamWriter(HttpContext.Current.Server.MapPath("~/Logfiles/ticker.txt")))
            {
                foreach (var name in names)
                {
                    writer.WriteLine(name);
  
                }
            }
        }

        public Dictionary<string, string> GetYahooTickers()
        {
            using(var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/ImportFiles/tickerlist.csv")))
            {
                var dict = new Dictionary<string, string>();
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var arr = line.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries);
                    if(!dict.ContainsKey(arr[1]))
                        dict.Add(arr[1], arr[0]);
                }
                return dict;
            }
        }
        public IEnumerable<string> GetFiQuoteNames() {
            using (var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/ImportFiles/fi.csv"))) {
                var list = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null) {
                      list.Add(line.Trim());
                }
                return list;
            }
        }
    }
}