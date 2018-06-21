using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class LinkMsgModel:MessageRequestBaseModel
    {
        public LinkMsgModel()
        {
            this.messageType = MessageType.Link;
        }
        public Link Link { get; set; }

    }
    public class Link
    {
        public string MessageUrl { get; set; }
        public string PicUrl { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}