using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class DepartmentUserResponseModel:ResponseBaseModel
    {
        public bool HasMore { get; set; }
        public IEnumerable<DepartmentUser> UserList { get; set; }
    }
}