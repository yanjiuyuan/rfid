using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class AddDepartmentModel
    {
        public string Name { get; set; }= "事业部";
        public int Parentid { get; set; } = 1;
        public int? Order { get; set; }
        public bool? CreateDeptGroup { get; set; }
        public bool? DeptHiding { get; set; } 
        public string DeptPerimits { get; set; } //格式："dpt1|dept2"
        public string UserPerimits { get; set; }//格式："userid1|userid2"
        public bool? OuterDept { get; set; } 
        public string OuterPermitDepts { get; set; }//格式："dpt1|dpt2"
        public string OuterPermitUsers { get; set; }  //格式："userid1|userid2"
    }
}