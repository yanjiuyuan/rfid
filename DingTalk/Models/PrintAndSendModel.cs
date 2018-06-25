using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class PrintAndSendModel
    {
        public string UserId { get; set; }
        public string TaskId { get; set; }
        public string OldPath { get; set; }
    }
}