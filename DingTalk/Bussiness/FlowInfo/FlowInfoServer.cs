using DingTalk.Models.DbModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Bussiness.FlowInfo
{
    public class FlowInfoServer
    {
        /// <summary>
        /// 流程大类及小类读取
        /// </summary>
        /// <returns></returns>
        public string GetFlowInfo()
        {
            string strJson = string.Empty;
            using (DDContext context = new DDContext())
            {
                var Flows = context.Flows.Where(u => u.IsEnable == 1 && u.State == 1);
                var FlowSort = context.FlowSort.Where(u => u.IsEnable == 1 && u.State == 1 && u.DEPT_ID == "ALL");
                var Quary = from a in Flows
                            join b in FlowSort
                            on (int)a.SORT_ID equals (int)b.SORT_ID
                            select new
                            {
                                sortId = a.SORT_ID,
                                sortName = b.SORT_NAME,
                                flowId = a.FlowId,
                                flowName = a.FlowName,
                                flowCreateTime = b.CreateTime
                            };
                strJson = JsonConvert.SerializeObject(Quary);
            }
            return strJson;
        }

        /// <summary>
        /// 流程大类读取
        /// </summary>
        /// <returns></returns>
        public string GetFlowSort()
        {
            string strJson = string.Empty;
            using (DDContext context = new DDContext())
            {
                var FlowSort = context.FlowSort.Where(u => u.IsEnable == 1 && u.State == 1 && u.DEPT_ID == "ALL");
                strJson = JsonConvert.SerializeObject(FlowSort);
            }
            return strJson;
        }


        /// <summary>
        /// 找到当前最大的TaskId并返还TaskId+1
        /// </summary>
        /// <returns></returns>
        public int FindMaxTaskId()
        {
            using (DDContext context = new DDContext())
            {
                int TaskId = (int)context.Tasks.Where(u => u.TaskId != null).Max(x => x.TaskId);
                return TaskId + 1;
            }
        }

    }
}