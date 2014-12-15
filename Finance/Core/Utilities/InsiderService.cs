using System;
using System.Collections.Generic;
using Finance.Core.Configuration;
using Finance.Models.EF;

namespace Finance.Core.Utilities
{
    public class InsiderService
    {
        public static IEnumerable<InsiderInfo> Get(DateTime date)
        {
            try {
                var url = string.Format(Constants.FiUrl, date.ToShortDateString(), DateTime.Now.AddDays(-1).ToShortDateString());
                var retriever = new WebRequestRetriever();
                var resolver = new HtmlResolver();
                var result = retriever.Get(new Uri(url));
                return resolver.ToInsiderRecord(result.Trim());
            }
            catch (Exception)
            {
                return new List<InsiderInfo>();
            }
        }
 
       
    }
}