using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.ServerModels
{
    public class SignModel
    {
        public int? NodeId { get; set; }
        public string NodeName { get; set; }

        public bool? IsBack { get; set; }
        public string ApplyMan { get; set; }
        public string Remark { get; set; }
        public bool? IsSend { get; set; }
        public string ApplyManId { get; set; }
        public string IsMandatory { get; set; }
        public string IsSelectMore { get; set; }

        public string ApplyTime { get; set; }
    }
}