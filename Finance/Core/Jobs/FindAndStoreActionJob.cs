using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Finance.Core.Utilities;
using Finance.Models.EF;

namespace Finance.Core.Jobs
{
    public class FindAndStoreActionJob
    {
        private const int DistinctByers = 4;
        private const int DistinctSelles = 3;
        private const double ObservableAmount = 30000;
        private const int ObservableMonths = 2;

        public IEnumerable<Portfolio> Execute()
        {            
            
            var portfolioItems = new List<Portfolio>();
            for (var x = 1; x < 31; x++)
            {
                var date = new DateTime(2014, 10, x);
                var buyOrSellInfo = FindStocksToBuyOrSell(date).Where(i => i.Value.Item1 >= DistinctByers || i.Value.Item2 >= DistinctSelles).ToDictionary(i => i.Key, i => i.Value);
                //UpdatePortfolio(buyOrSellInfo, date);
                GetPortfolio(buyOrSellInfo, date, portfolioItems);
                
            }
            return portfolioItems;
        }
        private static Dictionary<string, Tuple<int, int>> FindStocksToBuyOrSell(DateTime forDate)
        {
            var repository = new Repository.Repository();
            return repository.FindStocksToBuyOrSell(ObservableMonths, ObservableAmount, forDate);
        }
    
        private static void UpdatePortfolio(Dictionary<string, Tuple<int, int>> buyOrSellInfo, DateTime forDate) {
            foreach (var item in buyOrSellInfo)
            {
                var stock = item.Key;
                var isBuy = (item.Value.Item1 - item.Value.Item2) >= DistinctByers;
                var isSell = (item.Value.Item2 - item.Value.Item1) >= DistinctSelles;

                var repository = new Repository.Repository();
                var stockEntity = repository.GetPortfolio().FirstOrDefault(s => s.Stock.Equals(stock));
                var ticker = repository.GetTickers().FirstOrDefault(t => t.FullName.Equals(stock));
                //post in not one of the stocks in list to follow
                if (ticker == null)
                    continue;
                var price = QuoteService.GetPrice(ticker.TickerName, forDate);
                if (price.Last == null)
                    continue;

                var addBuyToPortfolio = stockEntity == null && isBuy;
                var addSellToPortfolio = stockEntity != null && isSell && !isBuy;
                var number = Double.Parse(price.Last, System.Globalization.CultureInfo.InvariantCulture);
                var calcPrice = 10000/Double.Parse(price.Last,
                                                   System.Globalization.CultureInfo.
                                                       InvariantCulture);
                repository.StoreTransaction(stock, forDate, addBuyToPortfolio ? "Köp" : "Försäljning", number, calcPrice, 10000);
                

                if (addBuyToPortfolio)
                {
                    repository.AddPostToPortfolio(new Portfolio
                                          {
                                              Stock = stock,
                                              BuyPrice = calcPrice,
                                              BuyAmount = 10000,
                                              BuyNumber = number,
                                              BuyDate = forDate,
                                              SellDate = (DateTime) SqlDateTime.MinValue
                                          }
                        );

                }
                else if (addSellToPortfolio)
                {
                    var parsedPrice = Double.Parse(price.Last, System.Globalization.CultureInfo.InvariantCulture);
                    var sellAmount = parsedPrice*stockEntity.BuyNumber;
                    var result = (sellAmount - stockEntity.BuyAmount)/stockEntity.BuyAmount;
                    stockEntity.SellDate = forDate;
                    stockEntity.SellPrice = parsedPrice;
                    stockEntity.SellAmount = sellAmount;
                    stockEntity.Result = result;
                    repository.SaveChanges();
                }
            }
        }

       public IEnumerable<Portfolio> GetPortfolio(Dictionary<string, Tuple<int, int>> buyOrSellInfo, DateTime forDate, List<Portfolio> portfolioItems) {
           using (var ctx = new Context()) {
               foreach (var item in buyOrSellInfo) {
                   var stock = item.Key;
                   var isBuy = (item.Value.Item1 - item.Value.Item2) >= DistinctByers;

                   var isInList = portfolioItems.Any(i => i.Stock.EndsWith(stock));
                   var ticker = ctx.TickerList.FirstOrDefault(t => t.FullName.Equals(stock));
                   //post in not one of the stocks in list to follow
                   if (ticker == null)
                       continue;
                   var price = QuoteService.GetPrice(ticker.TickerName, forDate);
                   if (price.Last == null)
                       continue;
                   if (!isInList && isBuy) {
                       portfolioItems.Add(new Portfolio
                       {
                           Stock = stock,
                           BuyPrice =
                               Double.Parse(price.Last,
                                            System.Globalization.CultureInfo.InvariantCulture),
                           BuyAmount = 10000,
                           BuyNumber = 10000 / Double.Parse(price.Last,
                                                          System.Globalization.CultureInfo.
                                                              InvariantCulture),
                           BuyDate = forDate,
                           SellDate = (DateTime)SqlDateTime.MinValue
                       }
                           );

                   }
               }
               return portfolioItems;
           }
       }
 
    }
}