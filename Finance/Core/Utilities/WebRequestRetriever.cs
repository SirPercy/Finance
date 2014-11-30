using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Finance.Core.Utilities
{
    public class WebRequestRetriever
    {
        /// <returns>A response stream from external source</returns>
        public virtual string Get(Uri uri) {
            using (var client = new WebClient()) {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add("user-agent",
                                   "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                var data = client.OpenRead(uri);
                using (var reader = new StreamReader(data)) {
                    var s = reader.ReadToEnd();
                    data.Close();
                    reader.Close();
                    return s;
                }
            }

        }
    }
}