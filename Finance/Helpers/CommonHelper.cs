using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Finance.Helpers
{
    public static class CommonHelper
    {

        public static string Roi(this HtmlHelper helper, IEnumerable<Models.EF.Portfolio> portfolio)
        {
            double invested = 0;
            double value = 0;
            foreach (var stock in portfolio)
            {
                value = value + stock.CurrentPrice*stock.BuyNumber;
                invested = invested + 10000;
            }

            return string.Format("{0:P2}", (value - invested)/invested);
        }

        public static string TotalRoi(this HtmlHelper helper, IEnumerable<Models.EF.Portfolio> portfolio, IEnumerable<Models.EF.Transaction> transactions)
        {
            double startInvestMoney = 60000;
            double saldo = startInvestMoney;
            double portfolioresult = 0;
            foreach (var stock in portfolio)
            {
                portfolioresult = portfolioresult + (stock.CurrentPrice * stock.BuyNumber);
            }
            foreach (var transaction in transactions.OrderBy(i => i.Date))
            {
                if (transaction.TransactionType.Equals("Försäljning"))
                {
                    saldo = saldo + transaction.Amount;
                }
                else
                {
                    saldo = saldo - transaction.Amount;
                }
            }
            return string.Format("{0:P2}", ((saldo + portfolioresult)-startInvestMoney)/ startInvestMoney);
        }
    }
}