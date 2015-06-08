using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public IEnumerable<InsiderInfo> ToAvanzaInsiderRecord(string html, out int pages)
        {
            pages = 0;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectNodes("//div[@data-component_type='insiderevents']").FirstOrDefault();
            if (node != null)
            {
                pages = node.SelectNodes(".//div//ul/li").Count;
            }

            var rows = doc.GetElementbyId("surface").SelectNodes(".//table/tbody/tr");
            var insiderInfo = new List<InsiderInfo>();
            foreach (var row in rows)
            {
                var fields = row.SelectNodes(".//td");
                var counter = 0;
                var info = new InsiderInfo();
                foreach (var field in fields)
                {
                    var text = Regex.Replace(field.InnerText, "\\s", " ").Trim();
                    if (counter == 0)
                    {
                        info.CompanyName = text;
                    }
                    else if (counter == 1)
                    {
                        var insiderInfoArr = text.Replace(", ", ",").Replace(" ", ",")
                            .Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                        if (insiderInfoArr.Length > 4)
                        {
                            info.Person = insiderInfoArr[1] + " " + insiderInfoArr[0];
                            info.Position = insiderInfoArr[2];
                            //befattning?? = insiderInfoArr[4]
                        }
                    }
                    else if (counter == 2)
                    {
                        info.Transaction = text.Contains("Köp") ? "Köp" : text.Contains("Sälj") ? "Försäljning" : "Annat";
                        info.Number = Regex.Match(text, "[1-9]*\\s{0,1}[0-9]+", RegexOptions.IgnoreCase).Value.Trim();
                        info.Type = text.Contains("Köp") ||
                                    text.Contains("Sälj") && text.Contains("aktier") && !text.Contains("teckn")
                            ? "Aktie"
                            : "Annat";
                    }
                    else if (counter == 3)
                    {
                        info.Date = DateTime.Parse(text);
                    }
                    counter++;
                }
                insiderInfo.Add(info);
            }
            return insiderInfo.Where(i => IsIntresting(i.Position, i.Transaction, i.Type));
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