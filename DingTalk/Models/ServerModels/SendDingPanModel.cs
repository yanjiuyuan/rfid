using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.ServerModels
{
    public class SendDingPanModel
    {
        public string access_token { get; set; }
        public string agent_id { get; set; }
        public string userid { get; set; }
        public string media_id { get; set; }
        public string file_name { get; set; }

    }
}