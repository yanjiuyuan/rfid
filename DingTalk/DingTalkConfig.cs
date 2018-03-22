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

        public string AccessToken { get; set; } = ConfigurationManager.AppSettings["AccessToken"].ToString();


        public DateTime LastUpdateTime { get; set; }

    }
}