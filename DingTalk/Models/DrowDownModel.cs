using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class DrowDownModel
    {
        public string DrawingNo { get; set; }
        public string Name { get; set; }
        public string Sorts { get; set; }
        public string TaskId { get; set; }
        public string MaterialScience { get; set; }
        public string Brand { get; set; }
        public string Count { get; set; }
        public string Unit { get; set; }
        public string Mark { get; set; }

        public List<Pro> ProList { get; set; }
    }

    public class Pro
    {
        public string ProcedureName { get; set; }
        public string ProcedureId { get; set; }
        public List<WorkTimes> WorkTimeList { get; set; }
    }

    public class WorkTimes
    {
        public string Worker { get; set; }
        public string WorkerId { get; set; }
        public string UseTime { get; set; }
    }
}