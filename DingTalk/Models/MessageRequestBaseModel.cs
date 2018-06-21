using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class MessageRequestBaseModel
    {
        public string touser { get; set; }
        public string toparty { get; set; }
        [Required]
        public string agentid { get; set; }
        [JsonIgnore]
        public MessageType messageType { get; set; } = MessageType.Text;
        public string msgtype { get { return GetDispayName(messageType); } }
        public string GetDispayName(MessageType type)
        {
            return type.ToString().ToLower();
        }
    }

    public enum MessageType
    {
        [Display(Name ="text")]
        Text,
        [Display(Name ="image")]
        Image,
        [Display(Name ="voice")]
        Voice,
        [Display(Name ="file")]
        File,
        [Display(Name ="link")]
        Link,
        [Display(Name ="oa")]
        Oa
    }

    
}