
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DingTalkServer
{
    public class DingTalkConfig
    {
        public DingTalkConfig()
        {
            if (ConfigurationManager.AppSettings["hao"].ToString() == "2")  //2 为测试公司
            {
                CorpId = ConfigurationManager.AppSettings["CorpId_hao"];
                CorpSecret = ConfigurationManager.AppSettings["CorpSecret_hao"];
                AgentId = ConfigurationManager.AppSettings["AgentId_hao"];
                Url = ConfigurationManager.AppSettings["Url_hao"];
                appkey = ConfigurationManager.AppSettings["appkey_hao"];
                appsecret = ConfigurationManager.AppSettings["appsecret_hao"];
            }
            else
            {
                CorpId = ConfigurationManager.AppSettings["CorpId"];
                CorpSecret = ConfigurationManager.AppSettings["CorpSecret"];
                AgentId = ConfigurationManager.AppSettings["AgentId"];
                Url = ConfigurationManager.AppSettings["Url"];
                appkey = ConfigurationManager.AppSettings["appkey"];
                appsecret = ConfigurationManager.AppSettings["appsecret"];
            }
        }

        public string CorpId { get; set; }

        public string CorpSecret { get; set; }

        public string AgentId { get; set; }

        public string Url { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public string appkey { get; set; }

        public string appsecret { get; set; }

    }
}