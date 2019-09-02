using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.ServerModels
{
    public class FlowSortModel
    {
        /// <summary>
        /// 当前操作人Id
        /// </summary>
        public string applyManId { get; set; }
        public List<FlowSort> FlowSortList { get; set; }
    }
}