using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.MobileModels
{
    public class AccessTokenModel
    {
        public int expires_in { get; set; }
        public string errmsg { get; set; }
        public string access_token { get; set; }
        public string errcode { get; set; }
    }
}