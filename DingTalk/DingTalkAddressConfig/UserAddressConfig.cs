using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer
{
    public partial class DingTalkServerAddressConfig
    {
        public string GetUserDetailUrl { get; private set; }
        public string CreateUserUrl { get; private set; }
        public string UpdateUserUrl { get; private set; }
        public string DeleteUserUrl { get;private set; }
        public string BatchDeleteUserUrl { get; private set; }
        public string GetDepartmentUserListUrl { get; private set; }
        public string GetDepartmentUserDetailListUrl { get; private set; }
    }
}