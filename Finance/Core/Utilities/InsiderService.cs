using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Finance.Core.Configuration;
using Finance.Models.EF;
using Finance.Models.Entities;

namespace Finance.Core.Utilities
{
    public class InsiderService
    {
        public static IEnumerable<InsiderInfo> Get(DateTime date)
        {
            try {
                var url = string.Format(Constants.FiUrl, date.ToShortDateString(), DateTime.Now.ToShortDateString());
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