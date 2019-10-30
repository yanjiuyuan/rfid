using DingTalk.DingTalkHelper;
using DingTalk.Models;
using DingTalk.Models.MobileModels;
using DingTalkServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace DingTalk.Controllers
{

    [RoutePrefix("LoginMobile")]
    public class LoginMobileController : ApiController
    {
        DingTalkManager dtManager = new DingTalkManager();
        HttpsClient _client = new HttpsClient();
        public DingTalkConfig DTConfig { get; set; } = new DingTalkConfig();
        DingTalkServerAddressConfig _addressConfig = DingTalkServerAddressConfig.GetInstance();

        /// <summary>
        /// 钉钉移动端免登接口
        /// </summary>
        /// <param name="authCode">免登授权码(dd.getAuthCode)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Bintang")]
        public async Task<NewErrorModel> Bintang(string authCode)
        {
            try
            {
                DingTalkServersController dingTalkServersController = new DingTalkServersController();
                string accessToken = await GetAccessToken();
                string userId = await GetUserId(accessToken, authCode);
                string accessTokenT = await GetAccessToken();
                UserInfoMobileModel userInfo = await GetUserInfo(accessTokenT, userId);
                //string DeptInfo = await dingTalkServersController.departmentQuaryByUserId(userInfo.userid);
                //DeptModel dept = JsonConvert.DeserializeObject<DeptModel>(DeptInfo);

                //if (dept.errcode == 0)
                //{
                //    userInfo.dept = dept.name;
                //}
                //else
                //{
                //    userInfo.dept = dept.errmsg;
                //}
              
                return new NewErrorModel()
                {
                    data = userInfo,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAccessToken")]
        public async Task<string> GetAccessToken()
        {
            _client.QueryString.Add("appkey", DTConfig.appkey);
            _client.QueryString.Add("appsecret", DTConfig.appsecret);
            var url = _addressConfig.GetAccessTokenUrl;
            var result = await _client.Get(url);
            string accessToken = JsonConvert.DeserializeObject<DingTalk.Models.MobileModels.AccessTokenModel>(result).access_token;
            //SetAppSettings("AccessToken", accessTokenModel.access_token);
            return accessToken;
        }

        /// <summary>
        /// 获取UserId
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="authCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserId")]
        public async Task<string> GetUserId(string access_token, string authCode)
        {
            _client.QueryString.Add("access_token", access_token);
            _client.QueryString.Add("code", authCode);
            var url = _addressConfig.GetUserId;
            var result = await _client.Get(url);
            string userId = JsonConvert.DeserializeObject<DingTalk.Models.MobileModels.UserIdModel>(result).userid;
            //SetAppSettings("UserId", userId.userid);
            return userId;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserInfo")]
        public async Task<UserInfoMobileModel> GetUserInfo(string access_token, string userId)
        {
            _client.QueryString.Add("access_token", access_token);
            _client.QueryString.Add("userid", userId);
            var url = _addressConfig.GetUserDetailUrl;
            var result = await _client.Get(url);
            UserInfoMobileModel userInfo = JsonConvert.DeserializeObject<DingTalk.Models.MobileModels.UserInfoMobileModel>(result);
            return userInfo;
        }

        private void SetAppSettings(string name, string value)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            config.AppSettings.Settings[name].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
