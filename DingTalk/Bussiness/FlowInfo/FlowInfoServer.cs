using DingTalk.Models.DingModels;
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
        public object GetFlowInfo(string userId)
        {
            DDContext context = new DDContext();
            List<Flows> Flows = context.Flows.Where(u => u.IsEnable == 1 && u.State == 1).OrderBy(f => f.OrderBY).ToList();
            List<FlowSort> FlowSort = context.FlowSort.Where(u => u.IsEnable == 1 && u.State == 1).OrderBy(u => u.OrderBY).ToList();

            foreach (var flowSort in FlowSort)
            {
                flowSort.flows = Flows.Where(f => f.SORT_ID.ToString() == flowSort.Sort_ID.ToString()).ToList();
            }

            //判断超管权限
            bool IsSupperLeader = context.Roles.Where(r => r.UserId == userId && r.RoleName== "超级管理员").ToList().Count > 0 ? true : false;

            if (!string.IsNullOrEmpty(userId) && !IsSupperLeader)
            {
                foreach (var item in FlowSort)
                {
                    if (item.ApplyManId != null && item.ApplyManId != "")
                    {
                        if (!item.ApplyManId.Contains(userId))
                        {
                            item.IsEnable = 0;
                            //FlowSort.Remove(item);
                        }
                    }
                    foreach (var flow in item.flows)
                    {
                        if (flow.ApplyManId != null && flow.ApplyManId != "")
                        {
                            if (!flow.ApplyManId.Contains(userId))
                            {
                                flow.IsEnable = 0;
                            }
                        }
                    }
                }

                FlowSort = FlowSort.Where(f => f.IsEnable == 1).ToList();
                foreach (var item in FlowSort)
                {
                    item.flows = item.flows.Where(f => f.IsEnable == 1).ToList();
                }
            }

            return FlowSort;
        }

        /// <summary>
        /// 流程大类读取
        /// </summary>
        /// <returns></returns>
        public object GetFlowSort()
        {
            DDContext context = new DDContext();
            var FlowSort = context.FlowSort.Where(u => u.IsEnable == 1 && u.State == 1 && u.DEPT_ID == "ALL");
            return FlowSort;
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
                Tasks task = context.Tasks.Where(u => u.NodeId == 0 && u.TaskId.ToString() == TaskId).First();
                return task;
            }
        }

        /// <summary>
        /// 查找下一个处理节点(非抄送)
        /// </summary>
        /// <param name="nodeInfos"></param>
        /// <param name="currentNodeId"></param>
        /// <returns></returns>
        public string FindNextNodeId(List<NodeInfo> nodeInfos, string currentNodeId)
        {
            //排序
            nodeInfos.OrderBy(n => n.NodeId);
            bool IsNow = false;
            foreach (var item in nodeInfos)
            {
                if (IsNow && item.IsSend != true)
                {
                    return item.NodeId.ToString();
                }
                if (item.NodeId.ToString() == currentNodeId)
                {
                    IsNow = true;
                }
            }
            return "0";
        }


        /// <summary>
        /// 返回已审批完成的任务流
        /// </summary>
        /// <returns>ListTasks</returns>
        public static List<Tasks> ReturnUnFinishedTaskId(string FlowId)
        {
            List<Tasks> ListTaskFinall = new List<Tasks>();
            using (DDContext context = new DDContext())
            {
                List<Tasks> ListTask = context.Tasks.Where(u => u.State == 0 && u.IsSend != true && u.FlowId.ToString() == FlowId).ToList();
                List<Tasks> ListTaskFinished = context.Tasks.Where(u => u.State == 1 && u.IsBacked != true && u.FlowId.ToString() == FlowId).ToList();

                ListTaskFinall = (from tf in ListTaskFinished
                                  where
                                !(from t in ListTask select t.TaskId).Contains(tf.TaskId)
                                  select tf).ToList();
            }
            return ListTaskFinall;
        }

        /// <summary>
        /// 返回已审批完成的任务流
        /// </summary>
        /// <param name="FlowName"></param>
        /// <returns></returns>
        public static List<Tasks> ReturnUnFinishedTaskIdByFlowName(string FlowName)
        {
            List<Tasks> ListTaskFinall = new List<Tasks>();
            using (DDContext context = new DDContext())
            {
                string FlowId = context.Flows.Where(t => t.FlowName == FlowName).FirstOrDefault().FlowId.ToString();
                List<Tasks> ListTask = context.Tasks.Where(u => u.State == 0 && u.IsSend != true && u.FlowId.ToString() == FlowId).ToList();
                List<Tasks> ListTaskFinished = context.Tasks.Where(u => u.State == 1 && u.IsBacked != true && u.FlowId.ToString() == FlowId).ToList();

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

        /// <summary>
        /// 获取当前流程状态
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns>0 未完成 1 已完成 2 被退回</returns>
        public string GetTasksState(string TaskId)
        {
            using (DDContext context = new DDContext())
            {
                List<Tasks> tasksListBack = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.IsBacked == true).ToList();
                if (tasksListBack.Count > 0)
                {
                    foreach (Tasks task in tasksListBack)
                    {
                        if (task.NodeId == 0)
                        {
                            return "已撤回";
                        }
                        else
                        {
                            return "被退回";
                        }
                    }
                }
                List<Tasks> tasksListFinished = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.State == 0 && t.IsSend != true).ToList();
                if (tasksListFinished.Count > 0)
                {
                    return "未完成";
                }
                else
                {
                    return "已完成";
                }

            }
        }

        /// <summary>
        /// 获取流程信息
        /// </summary>
        /// <param name="FlowId"></param>
        /// <returns></returns>
        public Flows GetFlow(string FlowId)
        {
            using (DDContext context = new DDContext())
            {
                return context.Flows.Where(f => f.FlowId.ToString() == FlowId).FirstOrDefault();
            }
        }
    }
}