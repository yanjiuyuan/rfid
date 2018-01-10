using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class AttendanceRequestModel
    {
        public string UserId { get; set; }
        
        public DateTime WorkDateFrom { get; set; }
        public DateTime WorkDateTo { get; set; }

    }
}