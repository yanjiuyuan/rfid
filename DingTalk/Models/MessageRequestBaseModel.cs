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
        public string Touser { get; set; }
        public string Toparty { get; set; }
        [Required]
        public string Agentid { get; set; }
        [JsonIgnore]
        public MessageType MessageType { get; set; } = MessageType.Text;
        public string Msgtype { get { return GetDispayName(MessageType); } }
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