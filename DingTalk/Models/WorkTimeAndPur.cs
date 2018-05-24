using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class WorkTimeAndPur
    {
        public bool? IsFinish { get; set; }
        
        public string Worker { get; set; }
        
        public string WorkerId { get; set; }
        
        public string StartTime { get; set; }
        
        public string EndTime { get; set; }
        
        public string UseTime { get; set; }
        
        public string DrawingNo { get; set; }
        
        public string ProcedureInfoId { get; set; }
        
        public string CreateManId { get; set; }
        
        public string CreateTime { get; set; }
        
        public string TaskId { get; set; }

    }
}