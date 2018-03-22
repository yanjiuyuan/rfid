using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.DingTalkHelper
{
    public class DepartmentInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Parentid { get; set; }
        public bool CreateDeptGroup { get; set; }
        public bool AutoAddUser { get; set; }
    }
}