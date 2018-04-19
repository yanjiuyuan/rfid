using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DingTalkServer
{
    public class DingTalkConfig
    {
        
        public string CorpId { get; set; } = ConfigurationManager.AppSettings["CorpId"];

        public string CorpSecret { get; set; } = ConfigurationManager.AppSettings["CorpSecret"];

        public string AgentId { get; set; } = ConfigurationManager.AppSettings["agentId"];
        public string hao { get; set; } = ConfigurationManager.AppSettings["hao"];

        public string CorpId_hao { get; set; } = ConfigurationManager.AppSettings["CorpId_hao"];

        public string CorpSecret_hao { get; set; } = ConfigurationManager.AppSettings["CorpSecret_hao"];

        public string AgentId_hao { get; set; } = ConfigurationManager.AppSettings["agentId_hao"];

        public string AccessToken { get; set; } = ConfigurationManager.AppSettings["AccessToken"].ToString();


        public DateTime LastUpdateTime { get; set; }

    }
}