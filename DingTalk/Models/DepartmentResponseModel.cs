using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class DepartmentResponseModel : ResponseBaseModel
    {
        public IEnumerable<Department> Department { get; set; }
    }
}