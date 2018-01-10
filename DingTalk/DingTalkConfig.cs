using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DingTalkServer
{
    public class DingTalkConfig
    {
        public string CorpId { get; set; } = "ding1238d49a88c92de535c2f4657eb6378f";
        public string CorpSecret { get; set; } = "JEgMvDPWMWtJM2ZVzvup73_XBHsI-lL-lflOK_E1rW16PGVICvSkSrRDIZkuJbAT";
        public string AccessToken { get; set; } = ConfigurationManager.AppSettings["AccessToken"].ToString();
        public DateTime LastUpdateTime { get; set; }

    }
}