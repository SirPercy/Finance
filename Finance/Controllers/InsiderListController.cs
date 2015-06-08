using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Finance.Core.Jobs;
using Finance.Models.EF;
using Finance.Models.ViewModels;
using Finance.Repository;

namespace Finance.Controllers
{
    public class InsiderListController : Controller
    {
        private readonly IRepository _repository; 

        public InsiderListController()
        {
            _repository = new Repository.Repository();
        }

        public ActionResult Index()
        {
     
            var list = HttpContext.Cache["insider"] as List<Quote>;
            if (list == null)
            {
                list = _repository.GetInsiderList();
                HttpRuntime.Cache.Insert("insider", list, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
            }
                var listByDate = new List<InsiderInfo>();
                if (Request.QueryString["show"] == "date")
            {
                foreach (var insiderInfo in list)
                {
                    listByDate.AddRange(insiderInfo.InsiderInfoList);
                }
            }
            var model = new InsiderListViewModel
                            {
                                InsiderListByQuote = list,
                                InsiderListByDate = listByDate
                            };
                
            return View(model);
   
        }

    }
}
