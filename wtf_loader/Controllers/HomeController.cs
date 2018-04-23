using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrillout.Web.Ui;

namespace wtf_loader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDummy _dummy;
        private readonly IDummyService _service;

        public HomeController(IDummy dummy, IDummyService service)
        {
            _dummy = dummy;
            _service = service;
        }

        public ActionResult Index()
        {
            ViewBag.Title = $"Home Page {_dummy.GetType().FullName} + {_service.GetType().FullName}";

            return View();
        }
    }

    public interface IDummy
    {
    }

    class Dummy : IDummy
    {
    }
}
