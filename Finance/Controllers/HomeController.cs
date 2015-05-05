using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Finance.Core.Jobs;
using Finance.Models.EF;
using Finance.Models.ViewModels;
using Finance.Repository;

namespace Finance.Controllers
{
    public class HomeController : Controller
    {
        readonly IRepository _repository;
        public HomeController()
        {
            _repository = new Repository.Repository();
        }



        public async Task<ActionResult> Index()
        {
            var portfolio = _repository.GetPortfolio();
            var transactions = _repository.GetTransactions();
            var index = _repository.GetIndex();
            await Task.WhenAll(portfolio, transactions, index);

            var model = new HomeViewModel
                            {
                                Portfolio = portfolio.Result,
                                Transactions = transactions.Result,
                                Index = index.Result
                            };
            return View(model);
        }





    }
}
