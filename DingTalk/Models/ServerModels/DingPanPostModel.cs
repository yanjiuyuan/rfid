using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.ServerModels
{
    public class DingPanPostModel
    {
        public string access_token { get; set; }
        public string domain { get; set; }
        public string agent_id { get; set; }
    }
}