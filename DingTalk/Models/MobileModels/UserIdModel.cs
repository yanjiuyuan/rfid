using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.MobileModels
{
    public class UserIdModel
    {
        public string userid { get; set; }

        public int sys_level { get; set; }

        public string errmsg { get; set; }

        public bool is_sys { get; set; }

        public string deviceId { get; set; }

        public int errcode { get; set; }
    }
}