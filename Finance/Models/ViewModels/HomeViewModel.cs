using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Finance.Models.EF;

namespace Finance.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Portfolio> Portfolio { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}