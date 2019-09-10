
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
            type = ConfigurationManager.AppSettings["hao"].ToString();
            if (type == "1")  //1 为测试环境环境
            {
                PcAppKey = ConfigurationManager.AppSettings["PcAppKey"];
                PcAppSecret = ConfigurationManager.AppSettings["PcAppSecret"];

                CorpId = ConfigurationManager.AppSettings["CorpId"];
                CorpSecret = ConfigurationManager.AppSettings["CorpSecret"];
                AgentId = ConfigurationManager.AppSettings["AgentId"];
                AppAgentId = ConfigurationManager.AppSettings["appAgentId"];
                Url = ConfigurationManager.AppSettings["Url"];
                appkey = ConfigurationManager.AppSettings["appkey"];
                appsecret = ConfigurationManager.AppSettings["appsecret"];
            }
            if (type == "2")  //2 为开发环境
            {
                CorpId = ConfigurationManager.AppSettings["CorpId_hao"];
                CorpSecret = ConfigurationManager.AppSettings["CorpSecret_hao"];
                AgentId = ConfigurationManager.AppSettings["AgentId_hao"];
                Url = ConfigurationManager.AppSettings["Url_hao"];
                appkey = ConfigurationManager.AppSettings["appkey_hao"];
                AppAgentId = ConfigurationManager.AppSettings["appAgentId_hao"];
                appsecret = ConfigurationManager.AppSettings["appsecret_hao"];
            }
            if (type == "0")  //真实环境
            {
                CorpId = ConfigurationManager.AppSettings["CorpId"];
                CorpSecret = ConfigurationManager.AppSettings["CorpSecret"];
                AgentId = ConfigurationManager.AppSettings["AgentId"];
                AppAgentId = ConfigurationManager.AppSettings["appAgentId"];
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

        public string AppAgentId { get; set; }

        public string PcAppKey { get; set; }

        public string PcAppSecret { get; set; }

        /// <summary>
        /// 0 开发(旧应用) 1 测试(新应用) 2 真实(旧应用) 
        /// </summary>
        public string type { get; set; }

    }
}