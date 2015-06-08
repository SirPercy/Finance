using System;
using System.Collections.Generic;
using System.Linq;
using Finance.Core.Utilities;
using Finance.Models.EF;
using Quartz;

namespace Finance.Core.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]   
    public class FindAndStoreActionJob : IJob
    {
        private const int DistinctByers = 4;
        private const int DistinctSelles = 0;
        private const double ObservableAmount = 54999;
        private const int ObservableMonths = 2;

        public void Execute(IJobExecutionContext context) {
            try
            {
                var portfolio = new Repository.Repository().GetPortfolio().Result;
                if (!portfolio.Any())
                {
                    for (int i = 45; i > 0; i--)
                    {
                        var date = DateTime.Now.AddDays(-i);
                        var buyOrSellInfo =
                            FindStocksToBuyOrSell(date).Where(
                                s => s.Value.Item1 >= DistinctByers || s.Value.Item2 >= DistinctSelles).ToDictionary(
                                    item => item.Key, item => item.Value);
                        UpdatePortfolio(buyOrSellInfo, date);
                    }
                }
                else
                {
                    var date = DateTime.Now.AddDays(-1);
                    var buyOrSellInfo =
                        FindStocksToBuyOrSell(date).Where(
                            i => i.Value.Item1 >= DistinctByers || i.Value.Item2 >= DistinctSelles).ToDictionary(
                                i => i.Key, i => i.Value);
                    UpdatePortfolio(buyOrSellInfo, date);
                }
                Logger.AddMessage("[OK] FindAndStoreActionJob " + DateTime.Now);

            }
            catch (Exception ex) {
                Logger.AddMessage("[ERROR] FindAndStoreActionJob " + ex.InnerException);

            }
        }
        private static Dictionary<string, Tuple<int, int>> FindStocksToBuyOrSell(DateTime forDate) {
            var repository = new Repository.Repository();
            return repository.FindStocksToBuyOrSell(ObservableMonths, ObservableAmount, forDate);
        }

        private static void UpdatePortfolio(Dictionary<string, Tuple<int, int>> buyOrSellInfo, DateTime forDate) {
            foreach (var item in buyOrSellInfo) {
                var stock = item.Key;
                var isSell = (item.Value.Item2 > DistinctSelles);
                var isBuy = (item.Value.Item1 >= DistinctByers) && !isSell;


                var repository = new Repository.Repository();
                var stockEntity = repository.GetPortfolio().Result.FirstOrDefault(s => s.Stock.Equals(stock));
                var ticker = repository.GetTickers().FirstOrDefault(t => t.FullName.Equals(stock));
                //post in not one of the stocks in list to follow
                if (ticker == null)
                    continue;
                var price = QuoteService.GetHistoricalPrice(ticker.TickerName, forDate);
                if (price.Last == null)
                    continue;

                var addBuyToPortfolio = (stockEntity == null && isBuy);
                var addSellToPortfolio = stockEntity != null && isSell;
                var calcPrice = Double.Parse(price.Last, System.Globalization.CultureInfo.InvariantCulture);
                var number = 10000 / Double.Parse(price.Last,
                                                   System.Globalization.CultureInfo.
                                                       InvariantCulture);

                if (addBuyToPortfolio)
                {
                    repository.StoreTransaction(stock, forDate, "Köp", number, calcPrice, 10000, 0);
                    repository.AddPostToPortfolio(new Portfolio
                                                      {
                                                          Stock = stock,
                                                          BuyPrice = calcPrice,
                                                          BuyAmount = 10000,
                                                          BuyNumber = number,
                                                          BuyDate = forDate,
                                                          CurrentPrice = calcPrice
                                                      }
                        );
                }
                else if (addSellToPortfolio) {
                    if (forDate > stockEntity.BuyDate)
                    {
                        repository.Context.Portfolio.Remove(stockEntity);
                        repository.SaveChanges();
                        var todaysPrice = Double.Parse(QuoteService.GetTodaysPrice(ticker.TickerName).Last, System.Globalization.CultureInfo.InvariantCulture);
                        repository.StoreTransaction(stock, forDate, "Försäljning", stockEntity.BuyNumber, calcPrice,
                                                    (calcPrice*stockEntity.BuyNumber),
                                                    (calcPrice*stockEntity.BuyNumber) - 10000);
                    }
                }
            }
        }

        //public IEnumerable<Portfolio> GetPortfolio(Dictionary<string, Tuple<int, int>> buyOrSellInfo, DateTime forDate, List<Portfolio> portfolioItems) {
        //    using (var ctx = new Context()) {
        //        foreach (var item in buyOrSellInfo) {
        //            var stock = item.Key;
        //            var isBuy = (item.Value.Item1 - item.Value.Item2) >= DistinctByers;

        //            var isInList = portfolioItems.Any(i => i.Stock.EndsWith(stock));
        //            var ticker = ctx.TickerList.FirstOrDefault(t => t.FullName.Equals(stock));
        //            //post in not one of the stocks in list to follow
        //            if (ticker == null)
        //                continue;
        //            var price = QuoteService.GetPrice(ticker.TickerName, forDate);
        //            if (price.Last == null)
        //                continue;
        //            if (!isInList && isBuy) {
        //                portfolioItems.Add(new Portfolio
        //                {
        //                    Stock = stock,
        //                    BuyPrice =
        //                        Double.Parse(price.Last,
        //                                     System.Globalization.CultureInfo.InvariantCulture),
        //                    BuyAmount = 10000,
        //                    BuyNumber = 10000 / Double.Parse(price.Last,
        //                                                   System.Globalization.CultureInfo.
        //                                                       InvariantCulture),
        //                    BuyDate = forDate,
        //                    SellDate = (DateTime)SqlDateTime.MinValue
        //                }
        //                    );

        //            }
        //        }
        //        return portfolioItems;
        //    }
        //}

    }
}