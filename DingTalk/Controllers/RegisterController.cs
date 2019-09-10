using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebZhongZhi.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            ViewBag.needLogin = false;
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult flowInformation()
        {
            return View();
        }
        public ActionResult addFlow()
        {
            return View();
        }
    }
}