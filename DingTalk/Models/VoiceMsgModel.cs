using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class VoiceMsgModel:MessageRequestBaseModel
    {
        public VoiceMsgModel()
        {
            this.messageType = MessageType.Voice;
        }
        public Voice Voice { get; set; }

    }
    public class Voice
    {
        public string Media_id { get; set; }
        public string Duration { get; set; }
    }
}