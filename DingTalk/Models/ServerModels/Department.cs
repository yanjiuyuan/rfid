using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Parentid { get; set; }
        public bool CreateDeptGroup { get; set; }
        public bool AutoAddUser { get; set; }
    }
}