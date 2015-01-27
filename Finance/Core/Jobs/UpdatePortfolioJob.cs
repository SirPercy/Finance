using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Finance.Core.Utilities;
using Quartz;

namespace Finance.Core.Jobs
{
    public class UpdatePortfolioJob: IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var repository = new Repository.Repository();
                var portfolio = repository.GetPortfolio();
                foreach (var item in portfolio)
                {
                    var currentItem = item;
                    var ticker = repository.GetTickers().FirstOrDefault(t => t.FullName.Equals(currentItem.Stock));
                    //post in not one of the stocks in list to follow
                    if (ticker == null)
                        continue;

                    var date = DateTime.Now.AddDays(-1);
                    if (date.DayOfWeek.Equals(DayOfWeek.Sunday))
                        date = DateTime.Now.AddDays(-3);
                    if (date.DayOfWeek.Equals((DayOfWeek.Saturday)))
                        date = DateTime.Now.AddDays(-2);

                    var price = QuoteService.GetPrice(ticker.TickerName, date);
                    if (price.Last == null)
                        continue;
                    var calcPrice = Double.Parse(price.Last, System.Globalization.CultureInfo.InvariantCulture);
                    if (item.BuyDate.AddMonths(3) < DateTime.Now)
                    {
                        AutomaticSell(item, calcPrice);
                    }
                    else
                    {
                        var entity = repository.Context.Portfolio.FirstOrDefault(i => i.Id.Equals(currentItem.Id));
                        entity.CurrentPrice = calcPrice;
                        repository.SaveChanges();
                    }
                }
                Logger.AddMessage("[OK] Updateportfoliojob " + DateTime.Now);
            }
            catch(Exception ex)
            {
                Logger.AddMessage("[ERROR] Updateportfoliojob " + ex.InnerException);
 
            }
        }

        private void AutomaticSell(Finance.Models.EF.Portfolio item, double calcPrice)
        {
            if (item.BuyDate.AddMonths(3) < DateTime.Now)
            {
                var repository = new Repository.Repository();
                var portfolio = new Finance.Models.EF.Portfolio() { Id = item.Id };
                repository.Context.Portfolio.Attach(item);
                repository.Context.Portfolio.Remove(portfolio);
                repository.SaveChanges();
                repository.StoreTransaction(item.Stock, DateTime.Now, "Försäljning", item.BuyNumber, calcPrice,
                                            (calcPrice * item.BuyNumber),
                                            (calcPrice * item.BuyNumber) - 10000);

            }
        }
    }
}