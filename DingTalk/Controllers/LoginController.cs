
using Common.JsonHelper;
using DingTalk.DingTalkHelper;
using DingTalkServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebZhongZhi.Controllers
{
    [ValidateInput(false)]
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
                return Content(userId);
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
            string url = "http://q202800o84.iask.in/login/login";
            string signature = DDApiService.Instance.GetSign(ticket, nonceStr, timeStamp, url);
            ViewBag.Url = url;
            ViewBag.JsApiTicket = ticket;
            ViewBag.Signature = signature;
            ViewBag.NonceStr = nonceStr;
            ViewBag.TimeStamp = timeStamp;
            ViewBag.CorpId = dtConfig.CorpId;
            ViewBag.CorpSecret = dtConfig.CorpSecret;
            ViewBag.AgentId = DDApiService.Instance.AgentId;
        }

        /// <summary>
        /// 载入用户数据
        /// </summary>
        /// <returns>errorCode  0 载入测试用户数据成功,1 载入当前用户数据成功, 2 其他原因 </returns>
        /// 测试数据：Login/LoadUserInfo
        public string LoadUserInfo(string UserInfoJson)
        {
            try
            {
                //var UserInfoJson = Request.Form["UserInfoJson"].ToString();
                string filepath = Server.MapPath("~/UserInfoConfig.json");
                if (string.IsNullOrEmpty(UserInfoJson))  //UserInfoJson为空时载入测试账号数据
                {
                    string json = JsonHelper.GetFileJson(filepath);
                    if (!string.IsNullOrEmpty(json))  //数据不为空存储对象到Session
                    {
                        UserInfo userInfo = JsonHelper.JsonToObject<UserInfo>(json);
                        Session["userInfo"] = userInfo;
                    }
                    return "{\"errorCode\":\"0\",\"errorMessage\":\"载入测试用户数据成功\"}";
                }
                else
                {
                    UserInfo userInfo = JsonHelper.JsonToObject<UserInfo>(UserInfoJson);
                    Session["userInfo"] = userInfo;
                    return "{\"errorCode\":\"1\",\"errorMessage\":\"载入用户数据成功\"}";
                }
            }
            catch (Exception ex)
            {
                  return "{\"errorCode\":\"2\",\"errorMessage\":\""+ ex.Message+"\"}";
            }
        }
    }
}