using System;
using System.Collections.Generic;
using System.Diagnostics;
using Finance.Core.Configuration;
using Finance.Models.EF;

namespace Finance.Core.Utilities
{
    public class InsiderService
    {
        private readonly WebRequestRetriever _retriever;

        public InsiderService()
        {
            _retriever = new WebRequestRetriever();
        }

        public IEnumerable<InsiderInfo> Get(DateTime date, DateTime to)
        {
            try {
                var url = string.Format(Constants.FiUrl, date.ToShortDateString(), to.ToShortDateString());
                var resolver = new HtmlResolver();
                var result = _retriever.Get(new Uri(url));
                return resolver.ToInsiderRecord(result.Trim());
            }
            catch (Exception)
            {
                return new List<InsiderInfo>();
            }
        }

        public IEnumerable<InsiderInfo> AvanzaGet(DateTime date)
        {
            var list = new List<InsiderInfo>();
            var url = string.Format(Constants.AvanzaUrl, date.ToShortDateString(), DateTime.Now.AddDays(-1).ToShortDateString(), "1");
            var result = _retriever.Get(new Uri(url));
            var resolver = new HtmlResolver();
            int pages;
            list.AddRange(resolver.ToAvanzaInsiderRecord(result, out pages));
            if (pages <= 1) return list;
            for (var i = 2; i <= pages; i++)
            {
                var pageUrl = string.Format(Constants.AvanzaUrl, date.ToShortDateString(), DateTime.Now.AddDays(-1).ToShortDateString(), i);
                var pageResult = _retriever.Get(new Uri(pageUrl));
                list.AddRange(resolver.ToAvanzaInsiderRecord(pageResult, out pages));
            }
            return list;
        } 
    }
}