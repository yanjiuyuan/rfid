using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class SendOAModel
    {
        public long agent_id { get; set; }
        public string userid_list { get; set; }
        public string dept_id_list { get; set; }
        public bool to_all_user { get; set; }
        public NewOATestModel msg { get; set; }

        //public string msgtype { get; set; }
        //public oa oa { get; set; }
    }
}