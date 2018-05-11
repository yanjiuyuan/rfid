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
        public string ProcedureId { get; set; }
        public string ProcedureName { get; set; }
        public string CreateTime { get; set; }
        public string ApplyMan { get; set; }
    }
}