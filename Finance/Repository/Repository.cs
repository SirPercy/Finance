using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Finance.Core.Utilities;
using Finance.Models.EF;
using Finance.Models.Entities;

namespace Finance.Repository
{
    public class Repository : IRepository
    {
        internal Context Context;

        public Repository() { Context = new Context(); }

        public void Dispose() { Context.Dispose(); }

        public void FindAndStoreTicker(IEnumerable<string> quotes, Dictionary<string, string> tickers)
        {
            foreach (var name in quotes)
            {
                var info = name.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var trimmedName = info[0].StartsWith("AB") ? info[0].Remove(0, 2) : info[0];
                var ticker =
                    tickers.FirstOrDefault(
                        i => i.Value.StartsWith(trimmedName, StringComparison.CurrentCultureIgnoreCase));
                if (ticker.Key != null)
                {
                    Context.TickerList.Add(new Ticker {FullName = name, TickerName = ticker.Key});
                }
            }
            SaveChanges();

        }

        public void StoreInsiderInfo(IEnumerable<InsiderInfo> latestInsiderData)
        {
                foreach (var insiderInfo in latestInsiderData) {
                    var info = insiderInfo;
                    var ticker = Context.TickerList.FirstOrDefault(t => t.FullName.Equals(info.CompanyName));
                    //post in not one of the stocks in list to follow
                    if (ticker == null)
                        continue;

                    var date = insiderInfo.Date;
                    if (date.DayOfWeek.Equals(DayOfWeek.Monday))
                        date = DateTime.Now.AddDays(-4);
                    if (date.DayOfWeek.Equals(DayOfWeek.Sunday))
                        date = DateTime.Now.AddDays(-3);
                    if (date.DayOfWeek.Equals((DayOfWeek.Saturday)))
                        date = DateTime.Now.AddDays(-2);


                    var price = QuoteService.GetHistoricalPrice(ticker.TickerName, date);
                    if (price.Last == null)
                    {                        
                        Logger.AddMessage("[ERROR] " + DateTime.Now + " Kan inte hämta kurs för " + info.CompanyName);
                        continue;
                    }
                    var number = Double.Parse(info.Number);
                    var lastPrice = Double.Parse(price.Last, System.Globalization.CultureInfo.InvariantCulture);
                    insiderInfo.Amount = (int)Math.Round(number * lastPrice);
                    var quote = Context.Quote.SingleOrDefault(q => q.Company.Equals(info.CompanyName));
                    if (quote == null) {
                        var newQuote = new Quote { Company = info.CompanyName };
                        newQuote.InsiderInfoList.Add(insiderInfo);
                        Context.Quote.Add(newQuote);
                    }
                    else {
                        quote.InsiderInfoList.Add(insiderInfo);
                    }
                    SaveChanges();
                }
            
        }

        public Dictionary<string, Tuple<int, int>> FindStocksToBuyOrSell(int observableMonths, double observableAmount, DateTime forDate)
        {
            var buyOrSellDict = new Dictionary<string, Tuple<int, int>>();
            var quotes = Context.Quote.Include("InsiderInfoList").ToList();
            foreach (var quote in quotes)
            {
                var lastBuysAndSells =
                    quote.InsiderInfoList.Where(i => i.Date > forDate.AddMonths(-observableMonths) && i.Date <= forDate).ToList();
                var personDict = new Dictionary<string, int>();
                foreach (var lastBuysAndSell in lastBuysAndSells)
                {
                    var key = lastBuysAndSell.Person + "|" + lastBuysAndSell.Position;
                    if (lastBuysAndSell.Amount < observableAmount && lastBuysAndSell.Amount > -observableAmount)
                        continue;

                    if (personDict.ContainsKey(key))
                    {
                        if (lastBuysAndSell.Transaction.Equals("Köp"))
                            personDict[key]++;
                        else
                            personDict[key]--;
                    }
                    else
                    {
                        personDict.Add(key, lastBuysAndSell.Transaction.Equals("Köp") ? 1 : -1);
                    }
                }
                var noOfBuys = personDict.Count(i => i.Value > 0);
                var noOfSells = personDict.Count(i => i.Value < 0);
                //if last three is sell increase sell to trigger
                if(lastBuysAndSells.Count > 2 && lastBuysAndSells[lastBuysAndSells.Count - 1].Transaction.Equals("Försäljning") &&
                   lastBuysAndSells[lastBuysAndSells.Count - 2].Transaction.Equals("Försäljning") &&
                   lastBuysAndSells[lastBuysAndSells.Count - 3].Transaction.Equals("Försäljning"))
                {
                    noOfSells = 99;
                }
                buyOrSellDict.Add(quote.Company, new Tuple<int, int>(noOfBuys, noOfSells));
     
            }
            return buyOrSellDict;
        }

        public async Task<List<Portfolio>> GetPortfolio()
        {
            using (var context = new Context())
            {
                return await (from p in context.Portfolio select p).ToListAsync();
            }
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            using (var context = new Context())
            {
                return await (from t in context.Transactions select t).ToListAsync();
            }   
        }
        public void StoreTransaction(string stock, DateTime date, string transactionType, double number, double price, double amount, double result)
        {
            var transaction = new Transaction
                                  {
                                      Date = date,
                                      Stock = stock,
                                      TransactionType = transactionType,
                                      Number = number,
                                      Price = price,
                                      Amount = amount,
                                      Result = result
                                  };
            Context.Transactions.Add(transaction);
            Context.SaveChanges();
        }

        public List<Ticker> GetTickers() {
            return Context.TickerList.ToList();
        }

        public void AddPostToPortfolio(Portfolio entity)
        {
            Context.Portfolio.Add(entity);
            Context.SaveChanges();
        }
        public List<Quote> GetInsiderList()
        {
            return Context.Quote.Include("InsiderInfoList").Where(q => q.InsiderInfoList.Count > 0).OrderBy(q => q.Company).ToList();
          
        }
        public List<InsiderInfo> GetInsiderDateList() {
            return Context.InsiderInfo.ToList();

        }

        public async Task<string> GetIndex()
        {
            var retval = HttpRuntime.Cache.Get("index") as string;
            if (string.IsNullOrEmpty(retval))
            {
                var historPrices = await QuoteService.GetHistoricalPrices("^OMXSPI", new DateTime(2014, 10, 24), DateTime.Now);

                var quoteInfos = historPrices as QuoteInfo[] ?? historPrices.ToArray();
                var last = quoteInfos.FirstOrDefault(p => p.Date.Contains(DateTime.Now.ToString("yyyy-MM")));
                var start = quoteInfos.Last();
                if (start.Last == null || last == null || last.Last == null)
                    return "";
                retval =  string.Format("{0:P2}",
                    (double.Parse(last.Last, System.Globalization.CultureInfo.InvariantCulture) -
                     double.Parse(start.Last, System.Globalization.CultureInfo.InvariantCulture))/
                    double.Parse(start.Last, System.Globalization.CultureInfo.InvariantCulture));
                HttpRuntime.Cache.Insert("index", retval, null, DateTime.Now.AddMinutes(30), TimeSpan.Zero);
            }
            return retval;


        }


        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}