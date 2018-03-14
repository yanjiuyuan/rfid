
using DingTalk.DingTalkHelper;
using DingTalkServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebZhongZhi.Controllers
{
    public class LoginController : Controller
    {
        DingTalkConfig dtConfig = new DingTalkConfig();

        public ActionResult Index()
        {
            return View();
        }
        //
        // GET: /DD/
        public ActionResult GetUserInfo(string accessToken, string code, bool setCurrentUser = true)
        {
            try
            {
                string userId = DDApiService.Instance.GetUserId(accessToken, code);
                string jsonString = DDApiService.Instance.GetUserDetailJson(accessToken, userId);
                UserDetailInfo userInfo = DDApiService.Instance.GetUserDetailFromJson(jsonString);
                if (setCurrentUser)
                {
                    Session["AccessToken"] = accessToken;
                    Session["CurrentUser"] = userInfo;
                }
                return Content(jsonString);
            }
            catch (Exception ex)
            {
                return Content(string.Format("{{'errcode': -1, 'errmsg':'{0}'}}", ex.Message));
            }
        }

        public ActionResult Login()
        {
            BeginDDAutoLogin();
            return View();
        }

        private void BeginDDAutoLogin()
        {
            string nonceStr = "helloDD";//todo:随机
            ViewBag.NonceStr = nonceStr;
            string accessToken = DDApiService.Instance.GetAccessToken();
            ViewBag.AccessToken = accessToken;
            string ticket = DDApiService.Instance.GetJsApiTicket(accessToken);
            long timeStamp = DDHelper.GetTimeStamp();
            string url = Request.Url.ToString();
            string signature = DDApiService.Instance.GetSign(ticket, nonceStr, timeStamp, url);

            ViewBag.JsApiTicket = ticket;
            ViewBag.Signature = signature;
            ViewBag.NonceStr = nonceStr;
            ViewBag.TimeStamp = timeStamp;
            ViewBag.CorpId = dtConfig.CorpId;
            ViewBag.CorpSecret = dtConfig.CorpSecret;
            //ViewBag.AgentId = DDApiService.Instance.AgentId;
        }
    }
}