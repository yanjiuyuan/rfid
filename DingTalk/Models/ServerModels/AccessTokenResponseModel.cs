using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class AccessTokenResponseModel:ResponseBaseModel
    {
        public string access_token { get; set; }
    }
}