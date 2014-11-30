using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Finance.Core.Jobs;
using Finance.Models.EF;
using Finance.Models.ViewModels;

namespace Finance.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var job = new FindAndStoreActionJob();
            var portfolio = new List<Portfolio>();// job.Execute();
            return View(portfolio);


            //var importJob = new StoreInsiderInfoJob();
            //importJob.Execute();
            //return View();

        }





    }
}
