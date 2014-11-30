using System;
using System.Collections.Generic;
using Finance.Models.EF;


namespace Finance.Repository
{
        public interface IRepository : IDisposable
        {
            List<Quote> GetInsiderList();
            List<InsiderInfo> GetInsiderDateList();
            void FindAndStoreTicker(IEnumerable<string> quotes, Dictionary<string, string> tickers);
            void StoreInsiderInfo(IEnumerable<InsiderInfo> latestInsiderData, DateTime dateToGetData);
            Dictionary<string, Tuple<int, int>> FindStocksToBuyOrSell(int observableMonths, double observableAmount,
                                                                      DateTime forDate);

            List<Portfolio> GetPortfolio();
            List<Quote> GetTransactions();
            List<Ticker> GetTickers();
            void AddPostToPortfolio(Portfolio entity);
            void StoreTransaction(string stock, DateTime date, string transactionType, double number, double price, double amount);
        }
    
}
