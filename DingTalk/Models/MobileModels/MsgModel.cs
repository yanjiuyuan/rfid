using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.MobileModels
{
    public class MsgModel
    {
        public string msgtype { get; set; }
        public text text { get; set; }

        public linkTest link { get; set; }
    }

    public class text
    {
        public string content { get; set; }
    }

    public class linkTest
    {
        public string messageUrl { get; set; }
        public string picUrl { get; set; }
        public string title { get; set; }
        public string text { get; set; }
    }
}