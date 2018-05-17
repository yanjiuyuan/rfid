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
        public ActionResult Approval_detail()
        {
            return View();
        }
        public ActionResult Approval_detail2()
        {
            return View();
        }

        public ActionResult Approval_list()
        {
            return View();
        }

        public ActionResult Approval_IDone()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Approval_fromMe()
        {
            return View();
        }

        public ActionResult Approval_copyMe()
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