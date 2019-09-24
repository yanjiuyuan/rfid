using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class TaskFlowModel
    {
        public float Id { get; set; }

        public int? TaskId { get; set; }
        public int? NodeId { get; set; }

        public int? FlowId { get; set; }

        public string ApplyMan { get; set; }
        public string ApplyManId { get; set; }
        public string ApplyTime { get; set; }
        public string Title { get; set; }
        public string FlowName { get; set; }
       
        public string State { get; set; }
        public bool? IsBack { get; set; }
        public bool? IsSupportMobile { get; set; }

        public bool? IsRead { get; set; }
    }

    public class TaskFlowModelList
    {
        public int count { get; set; }
        public List<TaskFlowModel> taskFlowModels { get; set; }
    }
}