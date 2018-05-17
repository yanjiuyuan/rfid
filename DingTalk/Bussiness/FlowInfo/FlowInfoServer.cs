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


        /// <summary>
        /// 获取申请者表单信息
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        public Tasks GetApplyManFormInfo(string TaskId)
        {
            using (DDContext context = new DDContext())
            {
                Tasks task = context.Tasks.Where(u => u.NodeId == 0 && u.IsPost == true && u.TaskId.ToString()== TaskId).First();
                return task;
            }
        }


        /// <summary>
        /// 返回已审批完成的任务流
        /// </summary>
        /// <returns>List<Tasks></returns>
        public static List<Tasks> ReturnUnFinishedTaskId(string FlowId)
        {
            List<Tasks> ListTaskFinall = new List<Tasks>();
            using (DDContext context = new DDContext())
            {
                List<Tasks> ListTask = context.Tasks.Where(u => u.State == 0 && u.FlowId.ToString() == FlowId).ToList();
                List<Tasks> ListTaskFinished = context.Tasks.Where(u => u.State == 1 && u.FlowId.ToString() == FlowId).ToList();

                ListTaskFinall = (from tf in ListTaskFinished
                                  where
 !(from t in ListTask select t.TaskId).Contains(tf.TaskId)
                                  select tf).ToList();
            }
            return ListTaskFinall;
        }


        /// <summary>
        /// 修改任务流状态
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public Tasks TasksModify(Tasks tasks)
        {
            using (DDContext context = new DDContext())
            {
                tasks.IsPost = true;
                context.Tasks.Add(tasks);
                context.SaveChanges();
                return tasks;
            }
        }


        /// <summary>
        /// 返还未完成的任务流
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="NodeId"></param>
        /// <returns></returns>
        public List<Tasks> GetTasksByNotFinished(string TaskId, string NodeId)
        {
            using (DDContext context = new DDContext())
            {
                List<Tasks> TaskList = new List<Tasks>();
                TaskList = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.State == 0 && u.NodeId.ToString() == NodeId).ToList();
                return TaskList;
            }
        }

    }
}