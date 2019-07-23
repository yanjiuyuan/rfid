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


        //以下为表单字段
        public string OldTaskId { get; set; }

        public string BomId { get; set; }

        public string CodeNo { get; set; }

        public string Name { get; set; }

        public string Count { get; set; }

        public string MaterialScience { get; set; }

        public string Unit { get; set; }

        public string Brand { get; set; }
        public string Sorts { get; set; }
        public bool IsDown { get; set; }
        public string FlowType { get; set; }

        
    }
}