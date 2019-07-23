using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class ResponseBaseModel
    {
        public int Errcode { get; set; }
        public string Errmsg { get; set; }
    }
}