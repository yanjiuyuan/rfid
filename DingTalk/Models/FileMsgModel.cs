using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class FileMsgModel : MessageRequestBaseModel
    {
        //private string _content;
        public FileMsgModel()
        {
            this.messageType = MessageType.File;
        }
        //[JsonIgnore]
        //public string MediaId { get; set; }

        ////public file file { get; set; }
        //public IDictionary<string,string> File { get { return new Dictionary<string, string>() { { "media_id", MediaId } }; } }

        public file file { get; set; }
    }


    public class file
    {
        public string media_id { get; set; }
        
    }
}