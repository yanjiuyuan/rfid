using DingTalkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.DingTalkHelper
{

    
    public static class DDHelper
    {
        public static string GetAccessToken(string corpId, string corpSecret,string type)
        {
            string url = "";
            if (type == "1") //新应用
            {
                url = string.Format("https://oapi.dingtalk.com/gettoken?appkey={0}&appsecret={1}", corpId, corpSecret);
            }
            else
            {
                url = string.Format("https://oapi.dingtalk.com/gettoken?corpid={0}&corpsecret={1}", corpId, corpSecret);
            }
            try
            {
                string response = HttpRequestHelper.Get(url);
                AccessTokenModel oat = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenModel>(response);

                if (oat != null)
                {
                    if (oat.errcode == 0)
                    {
                        return oat.access_token;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return string.Empty;
        }

        /* https://oapi.dingtalk.com/get_jsapi_ticket?access_token=79721ed2fc46317197e27d9bedec0425
         * 
         * errmsg    "ok"
         * ticket    "KJWkoWOZ0BMYaQzWFDF5AUclJOHgO6WvzmNNJTswpAMPh3S2Z98PaaJkRzkjsmT5HaYFfNkMdg8lFkvxSy9X01"
         * expires_in    7200
         * errcode    0
         */
        public static string GetJsApiTicket(string accessToken)
        {
            string url = string.Format("https://oapi.dingtalk.com/get_jsapi_ticket?access_token={0}", accessToken);
            try
            {
                string response = HttpRequestHelper.Get(url);
                JsApiTicketModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<JsApiTicketModel>(response);
                if (model != null)
                {
                    if (model.errcode == 0)
                    {
                        return model.ticket;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return string.Empty;
        }

        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static string GetUserId(string accessToken, string code)
        {
            string url = string.Format("https://oapi.dingtalk.com/user/getuserinfo?access_token={0}&code={1}", accessToken, code);
            try
            {
                string response = HttpRequestHelper.Get(url);
                GetUserInfoModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<GetUserInfoModel>(response);

                if (model != null)
                {
                    if (model.errcode == 0)
                    {
                        return model.userid;
                    }
                    else
                    {
                        throw new Exception(model.errmsg);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return string.Empty;
        }

        public static string GetUserDetailJson(string accessToken, string userId)
        {
            string url = string.Format("https://oapi.dingtalk.com/user/get?access_token={0}&userid={1}", accessToken, userId);
            try
            {
                string response = HttpRequestHelper.Get(url);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static UserDetailInfo GetUserDetail(string accessToken, string userId)
        {
            string url = string.Format("https://oapi.dingtalk.com/user/get?access_token={0}&userid={1}", accessToken, userId);
            try
            {
                string response = HttpRequestHelper.Get(url);
                UserDetailInfo model = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDetailInfo>(response);

                if (model != null)
                {
                    if (model.errcode == 0)
                    {
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public static List<DepartmentInfo> GetDepartmentList(string accessToken, int parentId = 1)
        {
            string url = string.Format("https://oapi.dingtalk.com/department/list?access_token={0}", accessToken);
            if (parentId >= 0)
            {
                url += string.Format("&id={0}", parentId);
            }
            try
            {
                string response = HttpRequestHelper.Get(url);
                GetDepartmentListModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<GetDepartmentListModel>(response);

                if (model != null)
                {
                    if (model.errcode == 0)
                    {
                        return null;// model.department.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }
    }
}