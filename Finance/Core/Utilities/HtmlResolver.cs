using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Finance.Models.EF;
using HtmlAgilityPack;

namespace Finance.Core.Utilities
{
    public class HtmlResolver
    {


        public IEnumerable<InsiderInfo> ToInsiderRecord(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var insiderInfo = new List<InsiderInfo>();
            var rows = doc.DocumentNode.SelectNodes("//table[@class='GridClass']/tr");
            if (rows == null)
                return insiderInfo;
            foreach (var row in rows)
            {
                var nodesToGet = row.ChildNodes.Where(n => n.Name.Equals("td"));
                if (!nodesToGet.Any())
                    continue;

                var position = HttpUtility.HtmlDecode(nodesToGet.ElementAt(3).InnerText);
                var transaction = HttpUtility.HtmlDecode(nodesToGet.ElementAt(4).InnerText);
                var type = HttpUtility.HtmlDecode(nodesToGet.ElementAt(5).InnerText);
                if (!IsIntresting(position, transaction, type))
                    continue;

                insiderInfo.Add(new InsiderInfo
                                    {
                                        Date = DateTime.Parse(nodesToGet.ElementAt(0).InnerText),
                                        CompanyName = HttpUtility.HtmlDecode(nodesToGet.ElementAt(1).InnerText),
                                        Person = HttpUtility.HtmlDecode(nodesToGet.ElementAt(2).InnerText),
                                        Position = position,
                                        Transaction = transaction,
                                        Type = type,
                                        Number = nodesToGet.ElementAt(6).InnerText

                                    });
            }
            return insiderInfo;
        }

        private static bool IsIntresting(string position, string transaction, string type)
        {
            if ((position.Equals("Eget") || position.Equals("Maka/Make") || position.Equals("Närstående") || position.Equals("Sambo") || position.Equals("Barn"))
                && (transaction.Equals("Köp") || transaction.Equals("Försäljning")) && type.StartsWith("Aktie"))
            {
                return true;
            }

            return false;

        }
    }
}