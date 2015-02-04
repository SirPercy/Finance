using System.Collections.Generic;
using System.Web.Mvc;
using Finance.Core.Jobs;
using Finance.Models.EF;
using Finance.Models.ViewModels;
using Finance.Repository;

namespace Finance.Controllers
{
    public class HomeController : Controller
    {
        IRepository _repository;
        public HomeController()
        {
            _repository = new Repository.Repository();
        }



        public ActionResult Index()
        {

            var model = new HomeViewModel
                            {
                                Portfolio = _repository.GetPortfolio(),
                                Transactions = _repository.GetTransactions(),
                                Index = _repository.GetIndex()
                            };
            return View(model);
        }





    }
}
