
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
            if (ConfigurationManager.AppSettings["hao"].ToString()== "2")
            {
                CorpId = ConfigurationManager.AppSettings["CorpId_hao"];
                CorpSecret = ConfigurationManager.AppSettings["CorpSecret_hao"];
                AgentId = ConfigurationManager.AppSettings["agentId_hao"];
            }
            else
            {
                CorpId = ConfigurationManager.AppSettings["CorpId"];
                CorpSecret = ConfigurationManager.AppSettings["CorpSecret"];
                AgentId = ConfigurationManager.AppSettings["agentId"];
            }
        }

        public string CorpId { get; set; }

        public string CorpSecret { get; set; }

        public string AgentId { get; set; }


        //public string CorpId_hao { get; set; } = ConfigurationManager.AppSettings["CorpId_hao"];

        //public string CorpSecret_hao { get; set; } = ConfigurationManager.AppSettings["CorpSecret_hao"];

        //public string AgentId_hao { get; set; } = ConfigurationManager.AppSettings["agentId_hao"];

        public string AccessToken { get; set; }

        public DateTime LastUpdateTime { get; set; }

    }
}