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
    }
}