using DingTalkServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DingTalk.DingTalkHelper
{
    public class DDApiService
    {
        public static readonly DDApiService Instance = new DDApiService();
        DingTalkConfig dtConfig = new DingTalkConfig();
        public string CorpId { get; private set; }
        public string CorpSecret { get; private set; }
        public string AgentId { get; private set; }

        private DDApiService()
        {
            CorpId = dtConfig.CorpId;
            CorpSecret = dtConfig.CorpSecret;
            AgentId = dtConfig.AgentId;
        }

        /// <summary>
        /// 获取AccessToken
        /// 开发者在调用开放平台接口前需要通过CorpID和CorpSecret获取AccessToken。
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            if (dtConfig.type == "1")//测试环境(新应用)
            {
                return DDHelper.GetAccessToken(dtConfig.PcAppKey, dtConfig.PcAppSecret, dtConfig.type);
            }
            else
            {
                return DDHelper.GetAccessToken(CorpId, CorpSecret, dtConfig.type);
            }
        }

        public string GetJsApiTicket(string accessToken)
        {
            return DDHelper.GetJsApiTicket(accessToken);
        }

        public string GetUserId(string accessToken, string code)
        {
            return DDHelper.GetUserId(accessToken, code);
        }

        public UserDetailInfo GetUserDetail(string accessToken, string userId)
        {
            return DDHelper.GetUserDetail(accessToken, userId);
        }

        public string GetUserDetailJson(string accessToken, string userId)
        {
            return DDHelper.GetUserDetailJson(accessToken, userId);
        }

        public UserDetailInfo GetUserDetailFromJson(string jsonString)
        {
            UserDetailInfo model = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDetailInfo>(jsonString);

            if (model != null)
            {
                if (model.errcode == 0)
                {
                    return model;
                }
            }
            return null;
        }



        public string GetSign(string ticket, string nonceStr, long timeStamp, string url)
        {
            String plain = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", ticket, nonceStr, timeStamp, url);
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plain);
                byte[] digest = SHA1.Create().ComputeHash(bytes);
                string digestBytesString = BitConverter.ToString(digest).Replace("-", "");
                return digestBytesString.ToLower();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<DepartmentInfo> GetDepartmentList(string accessToken, int parentId = 1)
        {
            return DDHelper.GetDepartmentList(accessToken, parentId);
        }
    }
}