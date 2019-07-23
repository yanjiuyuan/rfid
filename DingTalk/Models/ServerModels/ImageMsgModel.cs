using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class ImageMsgModel:MessageRequestBaseModel
    {

        public ImageMsgModel()
        {
            this.messageType = MessageType.Image;
        }
        [JsonIgnore]
        public string MediaId { get; set; }
        public IDictionary<string,string> Image { get { return new Dictionary<string, string>() { { "media_id", MediaId } }; } }
    }
}