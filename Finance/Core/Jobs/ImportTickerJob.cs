using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Finance.Core.Utilities;
using Finance.Models.EF;
using Finance.Repository;

namespace Finance.Core.Jobs
{
    public class ImportTickerJob
    {
        public void Execute()
        {
            var discManager = new DiscMananger();
            var tickers = discManager.GetYahooTickers();
            var quotes = discManager.GetFiQuoteNames();

            var repository = new Repository.Repository();
            repository.FindAndStoreTicker(quotes, tickers);

        }
    }
}