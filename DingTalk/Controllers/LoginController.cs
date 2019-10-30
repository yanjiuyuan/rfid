
using Common.JsonHelper;
using DingTalk.DingTalkHelper;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalkServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebZhongZhi.Controllers
{
    //[ValidateInput(false)]
    public class LoginController : Controller
    {
        DingTalkConfig dtConfig = new DingTalkConfig();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Error()
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
                throw ex;
            }
        }

        public ActionResult Login()
        {
            BeginDDAutoLogin();
            return View();
        }
        
        /// <summary>
        /// 免登数据返回接口
        /// </summary>
        public string BeginDDAutoLogin()
        {
            string nonceStr = "helloDD";//todo:随机
            ViewBag.NonceStr = nonceStr;
            string accessToken = DDApiService.Instance.GetAccessToken();
            ViewBag.AccessToken = accessToken;
            string ticket = DDApiService.Instance.GetJsApiTicket(accessToken);
            long timeStamp = DDHelper.GetTimeStamp();
            string url = dtConfig.Url;
            string signature = DDApiService.Instance.GetSign(ticket, nonceStr, timeStamp, url);
            string CompanyId = ConfigurationManager.AppSettings["hao"];  // 0 阿法迪 2 研究院
            ViewBag.Url = url;
            ViewBag.JsApiTicket = ticket;
            ViewBag.Signature = signature;
            ViewBag.NonceStr = nonceStr;
            ViewBag.TimeStamp = timeStamp;
            ViewBag.CorpId = DDApiService.Instance.CorpId;
            ViewBag.CorpSecret = dtConfig.CorpSecret;
            ViewBag.AgentId = DDApiService.Instance.AgentId;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Url", url);
            dic.Add("AgentId", DDApiService.Instance.AgentId);
            dic.Add("CorpId", DDApiService.Instance.CorpId);
            dic.Add("TimeStamp", timeStamp.ToString());
            dic.Add("NonceStr", nonceStr);
            dic.Add("Signature", signature);
            dic.Add("JsApiTicket", ticket);
            dic.Add("CompanyId", CompanyId);
            return JsonConvert.SerializeObject(dic);

        }

        /// <summary>
        /// 载入用户数据(Post)
        /// </summary>
        /// <returns>errorCode  0 载入测试用户数据成功,1 载入当前用户数据成功, 2 其他原因 </returns>

        [HttpPost]
        public string LoadUserInfo()
        {
            try
            {
                var sr = new StreamReader(Request.InputStream);
                var stream = sr.ReadToEnd();
                if (stream == null)
                {
                    string filepath = Server.MapPath("~/UserInfoConfig.json");
                    string json = JsonHelper.GetFileJson(filepath);
                    if (!string.IsNullOrEmpty(json))  //数据不为空存储对象到Session
                    {
                        UserInfo userInfo = JsonHelper.JsonToObject<UserInfo>(json);
                        Session["userInfo"] = userInfo;
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "载入测试用户数据成功"
                    });
                }
                else
                {
                    UserInfo userInfo = JsonHelper.JsonToObject<UserInfo>(stream);
                    userInfo.FinnalLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    //保存实体
                    using (DDContext context = new DDContext())
                    {
                        DbEntityEntry<UserInfo> entityEntry = context.Entry<UserInfo>(userInfo);
                        entityEntry.State = EntityState.Added;
                        context.SaveChanges();
                    }
                    Session["userInfo"] = userInfo;
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "载入用户数据成功"
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }
    }
}