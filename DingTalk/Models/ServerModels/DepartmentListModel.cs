using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class DepartmentListModel
    {
        public string errmsg { get; set; }
        public List<department> department { get; set; }
        public int errcode { get; set; }
    }

    public class department
    {
        public string name { get; set; }

        public int id { get; set; }

        public int parentid { get; set; }
        
    }
}