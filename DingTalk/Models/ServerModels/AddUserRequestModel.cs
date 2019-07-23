using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class AddUserRequestModel
    {
         public string Userid { get; set; }
        public string Name { get; set; }
        public IDictionary<int,int> OrderInDepts { get; set; }
        public int[] Department { get; set; }
        public string Position { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string WorkPlace { get; set; }
        public string Remark { get; set; }
        public string Email { get; set; }
        public   string Jobnumber { get; set; }
        public bool? IsHide { get; set; }
        public bool? IsSenior { get; set; }
        public IDictionary<string, string> Extattr { get; set; }
    }
}