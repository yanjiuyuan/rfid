using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebZhongZhi.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Approval()
        {
            return View();
        }

        public ActionResult Main2()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Main3()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Main4()
        {
            return View();
        }

        public ActionResult Main5()
        {
            return View();
        }
        public ActionResult Main6()
        {
            return View();
        }
        public ActionResult Main7()
        {
            return View();
        }
        public ActionResult Main8()
        {
            return View();
        }
    }
}