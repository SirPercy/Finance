using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Finance.Models.ViewModels
{
    public class InsiderListViewModel
    {
        public List<EF.Quote> InsiderListByQuote { get; set; }
        public List<EF.InsiderInfo> InsiderListByDate { get; set; }
    }
}