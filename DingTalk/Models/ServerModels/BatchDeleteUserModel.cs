using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class BatchDeleteUserModel
    {
        public IEnumerable<string> Useridlist { get; set; }
        public BatchDeleteUserModel(string[] idList)
        {
            Useridlist = idList.AsEnumerable();
        }
        public BatchDeleteUserModel(IEnumerable<string> idList)
        {
            Useridlist = idList.AsEnumerable();
        }
    }
}