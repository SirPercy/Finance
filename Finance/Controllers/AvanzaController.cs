using System;
using System.Web.Mvc;
using Finance.Core.Utilities;

namespace Finance.Controllers
{
    public class AvanzaController : Controller
    {
        //
        // GET: /Avanza/

        public ActionResult Index()
        {
            var model = new InsiderService().AvanzaGet(DateTime.Now.AddDays(-5));
            return View(model);
        }

    }
}
