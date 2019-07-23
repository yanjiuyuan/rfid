using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class DepartmentListModel
    {
        public string errmsg { get; set; }
        public List<List<string>> department { get; set; }
        public int errcode { get; set; }
    }
}