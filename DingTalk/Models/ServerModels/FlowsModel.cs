using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.ServerModels
{
    public class FlowsModel
    {
        public string applyManId { get; set; }
        public List<Flows> flowsList { get; set; }
    }
}