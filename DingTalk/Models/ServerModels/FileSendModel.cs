using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class FileSendModel
    {
        public string Media_Id { get; set; }

        public string UserId { get; set; }

        public string created_at { get; set; }

        public string type { get; set; }

        public string errmsg { get; set; }

        public string errcode { get; set; }
    }
}