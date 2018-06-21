using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class TextMsgModel:MessageRequestBaseModel
    {
        //private string _content;
        public TextMsgModel()
        {
            this.messageType = MessageType.Text;
        }
        [JsonIgnore]
        public string Content
        {
            get;set;
            //get { return _content; }
            //set {
            //    _content = value;
                //if (Text == null)
                //{
                //    Text = new Dictionary<string, string>();
                //}
                //Text.Add("content", _content);
            //}
        }
        public IDictionary<string,string> Text { get { return new Dictionary<string, string>() { { "content", Content } }; } }
    }
}