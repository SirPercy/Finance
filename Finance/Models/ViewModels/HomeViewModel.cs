using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Finance.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Finance.Models.EF.Quote> Quotes { get; set; }
        public Dictionary<string, Tuple<int, int>> Result { get; set; }
    }
}