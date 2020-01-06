using Common.JsonHelper;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalk.Models.ServerModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 工作流通用接口
    /// </summary>
    [RoutePrefix("FlowInfoNew")]
    public class FlowInfoNewController : ApiController
    {

        #region 流程创建与提交、退回

        /// <summary>
        /// 流程创建接口(Post)
        /// </summary>
        /// <param name="taskList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateTaskInfo")]
        public async Task<NewErrorModel> CreateTaskInfo([FromBody]List<Tasks> taskList)
        {
            try
            {
                FlowInfoServer flowInfoServer = new FlowInfoServer();
                int TaskId = flowInfoServer.FindMaxTaskId();
                int? FlowId = taskList[0].FlowId;
                Flows flows = flowInfoServer.GetFlow(FlowId.ToString());
                #region 新版
                using (DDContext context = new DDContext())
                {
                    foreach (var tasks in taskList)
                    {
                        if (taskList.IndexOf(tasks) == 0)
                        {
                            //修改流程状态
                            context.TasksState.Add(new TasksState()
                            {
                                ApplyMan= tasks.ApplyMan,
                                TaskId = TaskId.ToString(),
                                State = "未完成"
                            });
                            context.SaveChanges();
                        }
                        tasks.TaskId = TaskId;
                        if (tasks.IsSend == true)
                        {
                            if (tasks.NodeId == 1)
                            {
                                tasks.State = 0; tasks.IsEnable = 1;
                                await SendOaMsgNew(tasks.FlowId, tasks.ApplyManId.ToString(), tasks.TaskId.ToString(),
                                    taskList[0].ApplyMan, taskList[0].Remark, context, flows.ApproveUrl,
                                    taskList[0].NodeId.ToString(),
                                    true, false);
                            }
                            else
                            {
                                tasks.State = 0; tasks.IsEnable = 0;
                            }

                        }
                        else
                        {
                            //State 1 表示流程已审批 0 表示未审批  IsEnable 1 表示流程生效  0 未生效
                            if (tasks.NodeId == 0)
                            {
                                tasks.State = 1;
                                tasks.IsEnable = 1;
                                tasks.IsSend = false;
                                tasks.IsPost = true;
                                tasks.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                tasks.IsPost = false;
                                tasks.State = 0;
                                tasks.IsEnable = 0;
                            }
                        }
                        context.Tasks.Add(tasks);
                        context.SaveChanges();
                    }

                    List<Tasks> tasksNewList = context.Tasks.Where(t => t.TaskId == TaskId).OrderBy(t => t.NodeId).ToList();
                    List<NodeInfo> nodeInfoList = context.NodeInfo.Where(f => f.FlowId == FlowId.ToString()).ToList();
                    //申请人数据
                    Tasks tasksApplyMan = tasksNewList.Where(t => t.NodeId == 0).FirstOrDefault();
                    string PreApplyManId = "";  //上一级处理人

                    int iSendCount = 0;
                    foreach (var tasks in tasksNewList)
                    {
                        PreApplyManId = tasks.ApplyManId;
                        //开始找人
                        string PreNodeId = nodeInfoList.Where(n => n.NodeId == tasks.NodeId + iSendCount).FirstOrDefault().PreNodeId;
                        NodeInfo nextNodeInfo = nodeInfoList.Where(n => n.NodeId.ToString() == PreNodeId).FirstOrDefault();
                        //找到节点表预先配置的人
                        if (!string.IsNullOrEmpty(nextNodeInfo.PeopleId))
                        {
                            string[] PeopleIdList = nextNodeInfo.PeopleId.Split(',');
                            string[] NodePeopleList = nextNodeInfo.NodePeople.Split(',');
                            if (nextNodeInfo.IsSend != true)  //非抄送
                            {
                                //遍历推送消息
                                for (int i = 0; i < PeopleIdList.Length; i++)
                                {
                                    Tasks Newtasks = tasks;
                                    Newtasks.Remark = "";
                                    Newtasks.NodeId = int.Parse(PreNodeId);
                                    Newtasks.ApplyMan = NodePeopleList[i];
                                    Newtasks.ApplyManId = PeopleIdList[i];
                                    Newtasks.ApplyTime = null;
                                    tasks.IsEnable = 1;
                                    tasks.State = 0;
                                    Newtasks.IsPost = false;
                                    context.Tasks.Add(Newtasks);
                                    context.SaveChanges();

                                    await SendOaMsgNew(tasks.FlowId, PeopleIdList[i].ToString(),
                                        TaskId.ToString(), tasksApplyMan.ApplyMan,
                                        tasksApplyMan.Remark, context, flows.ApproveUrl,
                                        nextNodeInfo.NodeId.ToString());
                                    Thread.Sleep(100);
                                }

                                return new NewErrorModel()
                                {
                                    data = TaskId.ToString(),
                                    error = new Error(0, "流程创建成功！", "") { },
                                };
                            }
                            else  //找到抄送
                            {
                                //遍历推送消息
                                iSendCount--;
                                for (int i = 0; i < PeopleIdList.Length; i++)
                                {
                                    Tasks Newtasks = tasks;
                                    Newtasks.IsPost = false;
                                    Newtasks.IsSend = true;
                                    Newtasks.Remark = "";
                                    Newtasks.NodeId = int.Parse(PreNodeId);
                                    Newtasks.ApplyMan = NodePeopleList[i];
                                    Newtasks.ApplyManId = PeopleIdList[i];
                                    Newtasks.ApplyTime = "";
                                    tasks.IsEnable = 1;
                                    tasks.State = 0;
                                    context.Tasks.Add(Newtasks);
                                    context.SaveChanges();

                                    await SendOaMsgNew(tasks.FlowId, PeopleIdList[i].ToString(),
                                        TaskId.ToString(), tasksApplyMan.ApplyMan,
                                        tasksApplyMan.Remark, context, flows.ApproveUrl,
                                         nextNodeInfo.NodeId.ToString(), false, true);
                                    Thread.Sleep(100);

                                    //特殊处理(暂时)
                                    if (tasks.FlowId.ToString() == "6" || tasks.FlowId.ToString() == "33")
                                    {
                                        NodeInfo nodeInfoCurrent = context.NodeInfo.Where(n => n.FlowId.ToString() == tasks.FlowId.ToString() && n.NodeId.ToString() == "2").FirstOrDefault();
                                        Tasks taskCurrent = new Tasks()
                                        {
                                            TaskId = tasks.TaskId,
                                            ApplyMan = nodeInfoCurrent.NodePeople,
                                            ApplyManId = nodeInfoCurrent.PeopleId,
                                            IsPost = false,
                                            State = 0,
                                            IsSend = false,
                                            NodeId = 2,
                                            IsEnable = 1,
                                            FlowId = tasks.FlowId
                                        };
                                        context.Tasks.Add(taskCurrent);
                                        context.SaveChanges();
                                        await SendOaMsgNew(tasks.FlowId, taskCurrent.ApplyManId,
                                            TaskId.ToString(), tasksApplyMan.ApplyMan,
                                            tasksApplyMan.Remark, context, flows.ApproveUrl,
                                             nextNodeInfo.NodeId.ToString());
                                        Thread.Sleep(100);

                                        return new NewErrorModel()
                                        {
                                            data = TaskId.ToString(),
                                            error = new Error(0, "流程创建成功！", "") { },
                                        };
                                    }
                                }
                            }
                        }
                        else  //节点表数据未找到人
                        {
                            //找到已选人数据
                            List<Tasks> tasksChoosedList = tasksNewList.Where(t => t.NodeId.ToString() == PreNodeId).OrderBy(t => t.NodeId).ToList();
                            foreach (var tasksChoosed in tasksChoosedList)
                            {
                                //与上一级处理人重复
                                if (tasksChoosed.ApplyManId == PreApplyManId && iSendCount == 0
                                    && tasksChoosed.FlowId.ToString() != "26" && tasksChoosed.FlowId.ToString() != "27" && tasksChoosed.FlowId.ToString() != "34" && tasksChoosed.FlowId.ToString() != "30")  //临时处理
                                {
                                    tasksChoosed.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    tasksChoosed.State = 1; //修改审批状态
                                    tasksChoosed.IsEnable = 1; //修改显示状态
                                    context.Entry<Tasks>(tasksChoosed).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    PreApplyManId = tasksChoosed.ApplyManId;
                                    //修改显示状态
                                    tasksChoosed.IsEnable = 1;
                                    context.Entry<Tasks>(tasksChoosed).State = EntityState.Modified;
                                    context.SaveChanges();

                                    await SendOaMsgNew(tasks.FlowId, tasksChoosed.ApplyManId.ToString(),
                                        TaskId.ToString(), tasksApplyMan.ApplyMan,
                                        tasksApplyMan.Remark, context, flows.ApproveUrl,
                                         nextNodeInfo.NodeId.ToString(),
                                        false, false);
                                    Thread.Sleep(100);
                                    return new NewErrorModel()
                                    {
                                        data = TaskId.ToString(),
                                        error = new Error(0, "流程创建成功！", "") { },
                                    };
                                }
                            }
                        }
                    }
                }

                #endregion
                return new NewErrorModel()
                {
                    data = TaskId.ToString(),
                    error = new Error(0, "流程创建成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FlowInfoServer flowInfoServer = new FlowInfoServer();
                int TaskId = flowInfoServer.FindMaxTaskId();
                //同步数据
                AsyncTasksState((TaskId - 1).ToString());
            };
        }


        /// <summary>
        /// 流程提交接口(Approve)
        /// </summary>
        /// <param name="taskList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SubmitTaskInfo")]
        public async Task<NewErrorModel> SubmitTaskInfo([FromBody]List<Tasks> taskList)
        {
            try
            {
                //获取申请人提交表单信息
                FlowInfoServer fServer = new FlowInfoServer();
                string taskId = taskList[0].TaskId.ToString();
                Tasks taskNew = fServer.GetApplyManFormInfo(taskId);
                Flows flows = fServer.GetFlow(taskNew.FlowId.ToString());
                DDContext contexts = new DDContext();
                //判断流程状态
                TasksState tasksState = contexts.TasksState.Where(t => t.TaskId == taskId).FirstOrDefault();

                if (tasksState.State != "未完成")
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, $"当前流程状态为:{tasksState.State},无法提交。", "") { },
                    };
                }


                if (taskList.Count > 1)  //如果有选人
                {
                    if (taskList[0].Id == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "流程有误请联系管理员！", "") { },
                        };
                    }

                    foreach (var task in taskList)
                    {
                        if (taskList.IndexOf(task) > 0)
                        {
                            if (task.IsSend == true)
                            {
                                if (taskList.IndexOf(task) == 1)
                                {
                                    await SendOaMsgNew(task.FlowId, task.ApplyManId.ToString(),
                                task.TaskId.ToString(), taskNew.ApplyMan,
                                task.Remark, contexts, flows.ApproveUrl,
                                task.NodeId.ToString(),
                                false, true);
                                    Thread.Sleep(100);
                                    task.IsEnable = 1;
                                    task.State = 0;
                                    task.ApplyTime = null;
                                }
                                else
                                {
                                    task.IsEnable = 0;
                                    task.State = 0;
                                    task.ApplyTime = null;
                                }
                            }
                            else
                            {
                                task.IsEnable = 0;
                            }
                            contexts.Tasks.Add(task);
                            contexts.SaveChanges();
                        }
                    }
                }

                //调用寻人接口
                Tasks Findtasks = taskList[0];
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic = FindNextPeople(Findtasks.FlowId.ToString(), Findtasks.ApplyManId, true, Findtasks.IsSend,
                Findtasks.TaskId, Findtasks.NodeId);
                int i = 1; //控制推送次数

                foreach (var tasks in taskList)
                {
                    using (DDContext context = new DDContext())
                    {
                        Tasks tasksApplyMan = context.Tasks.Where(t => t.TaskId.ToString() == tasks.TaskId.ToString()
                             && t.NodeId == 0).First();
                        if (!string.IsNullOrEmpty(tasks.ImageUrl))
                        {
                            tasksApplyMan.ImageUrl = tasks.ImageUrl;
                        }
                        if (!string.IsNullOrEmpty(tasks.OldImageUrl))
                        {
                            tasksApplyMan.OldImageUrl = tasks.OldImageUrl;
                        }

                        if (!string.IsNullOrEmpty(tasksApplyMan.FileUrl))
                        {
                            if (!string.IsNullOrEmpty(tasks.FileUrl))
                            {
                                tasksApplyMan.FileUrl = tasksApplyMan.FileUrl + "," + tasks.FileUrl;
                                tasksApplyMan.OldFileUrl = tasksApplyMan.OldFileUrl + "," + tasks.OldFileUrl;
                                tasksApplyMan.MediaId = tasksApplyMan.MediaId + "," + tasks.MediaId;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(tasks.FileUrl))
                            {
                                tasksApplyMan.FileUrl = tasks.FileUrl;
                                tasksApplyMan.OldFileUrl = tasks.OldFileUrl;
                                tasksApplyMan.MediaId = tasks.MediaId;
                            }
                        }
                        context.Entry(tasksApplyMan).State = EntityState.Modified;
                        context.SaveChanges();

                        if (dic["NodeName"] == "结束")
                        {
                            //修改流程状态
                            tasks.IsPost = false;
                            tasks.State = 1;
                            tasks.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            context.Entry(tasks).State = EntityState.Modified;
                            context.SaveChanges();

                            //修改流程状态
                            TasksState tasksStateNew = context.TasksState.Where(t => t.TaskId == tasksApplyMan.TaskId.ToString()).FirstOrDefault();
                            tasksStateNew.State = "已完成";
                            context.Entry<TasksState>(tasksStateNew).State = EntityState.Modified;
                            context.SaveChanges();

                            await SendOaMsgNew(taskNew.FlowId, taskNew.ApplyManId.ToString(), tasks.TaskId.ToString(),
                                       taskNew.ApplyMan, taskNew.Remark, context, flows.ApproveUrl,
                                       taskNew.NodeId.ToString(),
                                       false, false, true);
                            Thread.Sleep(100);
                            JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 0,
                                errorMessage = "流程结束",
                                Content = tasks.TaskId.ToString()
                            });
                        }
                        else
                        {
                            if (taskList.IndexOf(tasks) == 0)
                            {
                                //修改流程状态
                                tasks.IsPost = false;
                                tasks.State = 1;
                                tasks.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                context.Entry(tasks).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                            else
                            {
                                //创建流程推送(选人)
                                //tasks.IsPost = false;
                                //tasks.State = 0;
                                //context.Tasks.Add(tasks);
                                //context.SaveChanges();
                            }
                            if (taskList.Count == 1 && taskList.IndexOf(tasks) == 0)  //未选人
                            {
                                if (fServer.GetTasksByNotFinished(tasks.TaskId.ToString(), tasks.NodeId.ToString()).Count == 0)
                                {
                                    await SendOaMsgNew(tasks.FlowId, dic["PeopleId"].ToString(), tasks.TaskId.ToString(),
                                        taskNew.ApplyMan, taskNew.Remark, context, flows.ApproveUrl,
                                        taskNew.NodeId.ToString(),
                                        false, false);
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                if (dic["PeopleId"] != null)
                                {
                                    if (i == 1)  //防止重复推送
                                    {
                                        //推送OA消息
                                        string[] PeopleIdList = dic["PeopleId"].Split(',');
                                        foreach (var PeopleId in PeopleIdList)
                                        {
                                            await SendOaMsgNew(tasks.FlowId, PeopleId, tasks.TaskId.ToString(),
                                                taskNew.ApplyMan, taskNew.Remark, context, flows.ApproveUrl,
                                                taskNew.NodeId.ToString(),
                                                false, false);
                                            Thread.Sleep(100);
                                        }
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }

                return new NewErrorModel()
                {
                    error = new Error(0, "流程创建成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //同步数据
                AsyncTasksState(taskList[0].TaskId.ToString());
            }
        }

        /// <summary>
        /// 流程退回
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FlowBack")]
        public async Task<NewErrorModel> FlowBack(Tasks tasks)
        {
            try
            {
                DDContext context = new DDContext();
                FlowInfoServer fServer = new FlowInfoServer();
                Flows flows = fServer.GetFlow(tasks.FlowId.ToString());

                //判断流程状态
                TasksState tasksState = context.TasksState.Where(t => t.TaskId == tasks.TaskId.ToString()).FirstOrDefault();

                if (tasksState.State == "已完成")
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, $"当前流程状已完成，无法继续操作。", "") { },
                    };
                }

                if (tasks.NodeId == 0 && tasksState.State == "被退回")
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, $"当前流程状被退回，无法继续操作。", "") { },
                    };
                }

                if (tasks.NodeId != 0 && tasksState.State == "已撤回")
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, $"当前流程状已撤回，无法继续操作。", "") { },
                    };
                }

                if (tasks.NodeId == 0)  //撤回
                {
                    Tasks taskNew = context.Tasks.Find(tasks.Id);
                    taskNew.IsBacked = true;
                    context.Entry<Tasks>(taskNew).State = EntityState.Modified;
                    context.SaveChanges();
                    //找到当前未审核的人员修改状态
                    List<Tasks> taskList = context.Tasks.Where(t => t.TaskId.ToString() == tasks.TaskId.ToString()).ToList();
                    foreach (var task in taskList)
                    {
                        task.IsEnable = 0; task.State = 0;
                        context.Entry<Tasks>(task).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    tasksState.State = "已撤回";
                    tasksState.NodeId = "0";
                    tasksState.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    context.Entry<TasksState>(tasksState).State = EntityState.Modified;
                    context.SaveChanges();


                    return new NewErrorModel()
                    {
                        data = tasks.TaskId.ToString(),
                        error = new Error(0, "流程撤回成功！", "") { },
                    };

                }
                else
                {
                    //修改流程状态
                    tasks.State = 1;
                    tasks.IsBacked = true;
                    tasks.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    context.Entry(tasks).State = EntityState.Modified;
                    context.SaveChanges();
                    //查找退回节点Id
                    string newBackNodeId = context.NodeInfo.Where
                        (u => u.FlowId == tasks.FlowId.ToString() && u.NodeId == tasks.NodeId)
                        .Select(u => u.BackNodeId).First();
                    //根据退回节点Id找人
                    if (newBackNodeId == "0")  //退回节点为发起人
                    {
                        Tasks taskApplyMan = context.Tasks.Where(t => t.TaskId.ToString() == tasks.TaskId.ToString() && t.NodeId == 0).First();

                        tasksState.State = "被退回";
                        tasksState.NodeId = "0";
                        tasksState.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        context.Entry<TasksState>(tasksState).State = EntityState.Modified;
                        context.SaveChanges();

                        await SendOaMsgNew(tasks.FlowId, taskApplyMan.ApplyManId, tasks.TaskId.ToString(),
                            taskApplyMan.ApplyMan, tasks.Remark, context, flows.ApproveUrl,
                            taskApplyMan.NodeId.ToString(),
                            true, false);
                        Thread.Sleep(100);
                    }
                    else
                    {
                        string PeopleId = context.NodeInfo.SingleOrDefault
                            (u => u.NodeId.ToString() == newBackNodeId && u.FlowId == tasks.FlowId.ToString()).PeopleId;
                        string NodePeople = context.NodeInfo.SingleOrDefault
                            (u => u.NodeId.ToString() == newBackNodeId && u.FlowId == tasks.FlowId.ToString()).NodePeople;
                        if (string.IsNullOrEmpty(PeopleId))
                        {
                            return new NewErrorModel()
                            {
                                data = tasks.TaskId.ToString(),
                                error = new Error(2, "退回节点尚未配置人员！", "") { },
                            };
                        }
                        else
                        {
                            int iBackNodeIds = int.Parse(newBackNodeId);
                            //根据找到的人创建新任务流
                            Tasks newTask = new Tasks();
                            newTask = tasks;
                            newTask.IsBacked = false;
                            newTask.ApplyMan = NodePeople;
                            newTask.ApplyManId = PeopleId;
                            newTask.ApplyTime = null;
                            newTask.State = 0;
                            newTask.NodeId = iBackNodeIds;
                            newTask.Remark = null;
                            newTask.IsPost = false;
                            context.Tasks.Add(newTask);
                            context.SaveChanges();
                        }
                    }
                }
                return new NewErrorModel()
                {
                    data = tasks.TaskId.ToString(),
                    error = new Error(0, "退回成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 审批过程节点数据读取

        /// <summary>
        /// 审批过程节点数据读取接口
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="FlowId">流程号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetFlowProgress")]
        public NewErrorModel GetFlowProgress(string TaskId, string FlowId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (string.IsNullOrEmpty(FlowId))
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "FlowId不能为空！", "") { },
                        };
                    }
                    if (string.IsNullOrEmpty(TaskId))
                    {
                        var QuaryList = context.NodeInfo.Where(u => u.FlowId == FlowId)
                            .Select(T => new
                            {
                                NodeId = T.NodeId,
                                NodeName = T.NodeName,
                                NodePeople = T.NodePeople
                            }).OrderBy(t => t.NodeId);

                        return new NewErrorModel()
                        {
                            data = QuaryList,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                    else
                    {
                        var TasksList = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.FlowId.ToString() == FlowId);
                        var NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId);
                        var QuaryList = from a in TasksList
                                        join b in NodeInfoList
                                        on a.NodeId equals b.NodeId
                                        orderby b.NodeId ascending
                                        select new
                                        {
                                            NodeId = a.NodeId,
                                            NodeName = b.NodeName,
                                            NodePeople = b.NodePeople,
                                            ApplyTime = a.ApplyTime,
                                            ApplyMan = a.ApplyMan,
                                            IsSend = a.IsSend
                                        };

                        return new NewErrorModel()
                        {
                            data = QuaryList,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 寻人、选人与抄送

        /// <summary>
        /// 寻人接口（默认）
        /// </summary>
        /// <param name="FlowId">流程Id</param>
        /// <param name="ApplyManId">提交人Id</param>
        /// <param name="IsNext">是否找下一个</param>
        /// <param name="IsSend">是否抄送</param>
        /// <param name="OldTaskId">旧流水号</param>
        /// <param name="NodeId">当前节点Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("FindNextPeople")]
        public Dictionary<string, string> FindNextPeople(string FlowId, string ApplyManId, bool IsNext = true,
            bool? IsSend = false, int? OldTaskId = 0, int? NodeId = -1)
        {
            using (DDContext context = new DDContext())
            {
                Flows flows = context.Flows.Where(f => f.FlowId.ToString() == FlowId).FirstOrDefault();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string FindNodeId = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == NodeId).PreNodeId;
                string NodeName = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId.ToString() == FindNodeId).NodeName;
                dic.Add("NodeName", NodeName);
                if (NodeName == "结束")
                {
                    //修改流程状态
                    TasksState tasksState = context.TasksState.
                    Where(t => t.TaskId == OldTaskId.ToString()).FirstOrDefault();
                    tasksState.State = "已完成";
                    context.Entry<TasksState>(tasksState).State = EntityState.Modified;
                    context.SaveChanges();
                    return dic;
                }
                string PeopleId = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId.ToString() == FindNodeId).PeopleId;
                string NodePeople = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId.ToString() == FindNodeId).NodePeople;
                bool? IsNeedChose = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId.ToString() == FindNodeId).IsNeedChose;
                //判断流程多人提交(当前步骤)
                bool? IsAllAllow = context.NodeInfo.Where(u => u.NodeId == NodeId && u.FlowId == FlowId).First().IsAllAllow;
                dic.Add("NodePeople", NodePeople);
                dic.Add("PeopleId", PeopleId);

                if (NodeName.Contains("抄送"))
                {
                    if (!string.IsNullOrEmpty(PeopleId))
                    {
                        string[] ListNodeName = NodeName.Split(',');
                        string[] ListPeopleId = PeopleId.Split(',');
                        string[] ListNodePeople = NodePeople.Split(',');

                        Tasks Task = context.Tasks.Where(u => u.TaskId == OldTaskId).First();

                        //推送已选择的抄送
                        List<Tasks> TaskSendList = context.Tasks.Where(t => t.TaskId.ToString() == OldTaskId.ToString() && t.IsEnable == 0 && t.State == 0
                        && t.IsSend == true && t.NodeId.ToString() == FindNodeId).ToList();
                        if (TaskSendList.Count > 0)
                        {
                            foreach (var item in TaskSendList)
                            {
                                item.IsEnable = 1;
                                item.State = 0;
                                context.Entry<Tasks>(item).State = EntityState.Modified;

                                //推送抄送消息
                                SentCommonMsg(item.ApplyManId,
                                string.Format("您有一条抄送信息(流水号:{0})，请及时登入研究院信息管理系统进行查阅。", Task.TaskId),
                                Task.ApplyMan, Task.Remark, null, item.TaskId.ToString(), flows.FlowName);
                                context.SaveChanges();
                            }
                        }

                        for (int i = 0; i < ListPeopleId.Length; i++)
                        {
                            //保存任务流
                            Tasks newTask = new Tasks()
                            {
                                TaskId = OldTaskId,
                                ApplyMan = ListNodePeople[i],
                                IsEnable = 1,
                                NodeId = NodeId + 1,
                                FlowId = Int32.Parse(FlowId),
                                IsSend = true,
                                ApplyManId = ListPeopleId[i],
                                State = 0, //0 表示未审核 1表示已审核
                                FileUrl = Task.FileUrl,
                                OldFileUrl = Task.OldFileUrl,
                                ImageUrl = Task.ImageUrl,
                                OldImageUrl = Task.OldImageUrl,
                                Title = Task.Title,
                                IsPost = false,
                                ProjectId = Task.ProjectId,
                            };
                            //推送抄送消息
                            SentCommonMsg(ListPeopleId[i],
                            string.Format("您有一条抄送信息(流水号:{0})，请及时登入研究院信息管理系统进行查阅。", Task.TaskId),
                            Task.ApplyMan, Task.Remark, null, newTask.TaskId.ToString(), flows.FlowName);

                            context.Tasks.Add(newTask);
                            context.SaveChanges();
                        }
                    }
                    else  //已选的抄送
                    {
                        List<Tasks> TaskSendList = context.Tasks.Where(u => u.TaskId == OldTaskId && u.NodeId.ToString() == FindNodeId).ToList();
                        if (TaskSendList.Count > 0)
                        {
                            Tasks TasksApplyMan = context.Tasks.Where(t => t.TaskId.ToString() == OldTaskId.ToString() && t.NodeId == 0).FirstOrDefault();
                            foreach (var item in TaskSendList)
                            {
                                item.IsEnable = 1;
                                context.Entry<Tasks>(item).State = EntityState.Modified;
                                context.SaveChanges();
                                //推送抄送消息
                                SentCommonMsg(item.ApplyManId,
                                string.Format("您有一条抄送信息(流水号:{0})，请及时登入研究院信息管理系统进行查阅。", item.TaskId),
                                TasksApplyMan.ApplyMan, TasksApplyMan.Remark, null, TasksApplyMan.TaskId.ToString(), flows.FlowName);
                            }
                        }
                    }
                    if (IsSend == true)
                    {
                        if (!string.IsNullOrEmpty(PeopleId))
                        {
                            return null;
                        }
                        else
                        {
                            return FindNextPeople(FlowId, ApplyManId, true, false, OldTaskId, NodeId + 1);
                        }
                    }
                    else
                    {
                        return FindNextPeople(FlowId, ApplyManId, true, false, OldTaskId, NodeId + 1);
                    }
                }
                //查找当前是否还有人未审核
                List<Tasks> ListTask = context.Tasks.Where(u => u.TaskId == OldTaskId && u.FlowId.ToString() == FlowId && u.NodeId == NodeId && u.NodeId != 0 && u.ApplyManId != ApplyManId && u.State == 0 && u.IsSend != true).ToList();
                if (ListTask.Count > 0)  //还有人未审核
                {
                    return dic;
                }
                else
                {
                    if (NodeName == "抄送相应部门部长")
                    {
                        return FindNextPeople(FlowId, ApplyManId, true, false, OldTaskId, NodeId + 1);
                    }
                    if (NodeName == "抄送所有人")
                    {
                        List<Tasks> TasksList = context.Tasks.Where(t => t.TaskId == OldTaskId).ToList();
                        //context.Tasks.Where(t => t.TaskId == OldTaskId).Select(h => h.ApplyManId).Distinct().ToList();
                        List<string> AppplyManIdList = new List<string>();
                        foreach (var task in TasksList)
                        {
                            if (!AppplyManIdList.Contains(task.ApplyManId))
                            {
                                AppplyManIdList.Add(task.ApplyManId);
                                task.IsSend = true;
                                task.ApplyTime = "";
                                task.Remark = "";
                                task.NodeId = NodeId + 1;
                                task.IsEnable = 1;
                                task.State = 0;
                                context.Tasks.Add(task);
                                context.SaveChanges();
                            }
                            //推送抄送消息
                            Tasks Task = context.Tasks.Where(u => u.TaskId == OldTaskId).First();
                            SentCommonMsg(task.ApplyManId,
                            string.Format("您有一条抄送信息(流水号:{0})，请及时登入研究院信息管理系统进行查阅。", Task.TaskId),
                            Task.ApplyMan, Task.Remark, null, Task.TaskId.ToString(), flows.FlowName);
                        }
                        return FindNextPeople(FlowId, ApplyManId, true, false, OldTaskId, NodeId + 1);
                    }
                }


                //节点表找不到人，任务表找
                if (string.IsNullOrEmpty(NodePeople) && string.IsNullOrEmpty(PeopleId))
                {
                    string PreNodeId = context.NodeInfo.Where(f => f.NodeId == NodeId && f.FlowId == FlowId).First().PreNodeId;
                    List<Tasks> taskList = context.Tasks.Where(t => t.TaskId == OldTaskId && t.NodeId.ToString() == PreNodeId).ToList();
                    int iCount = 0;
                    foreach (var tasks in taskList)
                    {
                        if (tasks.IsSend == false)  //非抄送
                        {
                            //查找当前是否还有人未审核
                            if (ListTask.Count > 0)  //还有人未审核
                            {
                                return dic;
                            }
                            if (IsAllAllow != true)  //逐一审核
                            {
                                if (iCount == 0)
                                {
                                    dic["PeopleId"] = tasks.ApplyManId;
                                    dic["NodePeople"] = tasks.ApplyMan;
                                    tasks.IsEnable = 1;
                                }
                                else
                                {
                                    tasks.IsEnable = 0;
                                }
                                iCount++;
                            }
                            else  //同时审核
                            {
                                if (iCount == 0)
                                {
                                    iCount++;
                                    dic["PeopleId"] = tasks.ApplyManId;
                                    dic["NodePeople"] = tasks.ApplyMan;
                                    tasks.IsEnable = 1;
                                }
                                else
                                {
                                    dic["PeopleId"] += "," + tasks.ApplyManId;
                                    dic["NodePeople"] += "," + tasks.ApplyMan;
                                    tasks.IsEnable = 1;
                                }
                            }
                            context.Entry<Tasks>(tasks).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            return FindNextPeople(FlowId, ApplyManId, true, false, OldTaskId, Int32.Parse(FindNodeId) + 1);
                        }
                    }
                    return dic;
                }

                if (PeopleId == null && IsNeedChose == false)  //找不到人、且不需要找人时继续查找下一节点人员
                {
                    return FindNextPeople(FlowId, ApplyManId, true, false, OldTaskId, NodeId + 1);
                }
                else
                {
                    if (IsAllAllow == true)   //流程配置为所有人同时同意后提交
                    {
                        //查找当前是否还有人未审核
                        if (ListTask.Count > 0)  //还有人未审核
                        {
                            return dic;
                        }
                        else
                        {
                            string[] ListNodeName = NodeName.Split(',');
                            string[] ListPeopleId = PeopleId.Split(',');
                            string[] ListNodePeople = NodePeople.Split(',');
                            Tasks Task = context.Tasks.Where(u => u.TaskId == OldTaskId).First();
                            for (int i = 0; i < ListPeopleId.Length; i++)
                            {
                                //保存任务流
                                Tasks newTask = new Tasks()
                                {
                                    TaskId = OldTaskId,
                                    ApplyMan = ListNodePeople[i],
                                    IsEnable = 1,
                                    NodeId = NodeId + 1,
                                    FlowId = Int32.Parse(FlowId),
                                    IsSend = IsSend,
                                    ApplyManId = ListPeopleId[i],
                                    State = 0, //0 表示未审核 1表示已审核
                                    FileUrl = Task.FileUrl,
                                    OldFileUrl = Task.OldFileUrl,
                                    ImageUrl = Task.ImageUrl,
                                    OldImageUrl = Task.OldImageUrl,
                                    Title = Task.Title,
                                    IsPost = false,
                                    ProjectId = Task.ProjectId,
                                };
                                context.Tasks.Add(newTask);
                            }
                            context.SaveChanges();
                        }
                    }
                    else  //流程配置为任意一人同意后提交
                    {
                        string[] ListNodeName = NodeName.Split(',');
                        string[] ListPeopleId = PeopleId.Split(',');
                        string[] ListNodePeople = NodePeople.Split(',');
                        Tasks Task = context.Tasks.Where(u => u.TaskId == OldTaskId).First();
                        for (int i = 0; i < ListPeopleId.Length; i++)
                        {
                            //保存任务流
                            Tasks newTask = new Tasks()
                            {
                                TaskId = OldTaskId,
                                ApplyMan = ListNodePeople[i],
                                IsEnable = 1,
                                NodeId = NodeId + 1,
                                FlowId = Int32.Parse(FlowId),
                                IsSend = IsSend,
                                ApplyManId = ListPeopleId[i],
                                State = 0, //0 表示未审核 1表示已审核
                                FileUrl = Task.FileUrl,
                                OldFileUrl = Task.OldFileUrl,
                                ImageUrl = Task.ImageUrl,
                                OldImageUrl = Task.OldImageUrl,
                                Title = Task.Title,
                                IsPost = false,
                                ProjectId = Task.ProjectId,
                            };
                            context.Tasks.Add(newTask);
                        }
                        context.SaveChanges();
                    }
                    return dic;
                }
            }
        }

        #endregion

        #region 修改抄送状态为已阅

        /// <summary>
        /// 修改抄送状态为已阅
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChangeSendState")]
        public NewErrorModel ChangeSendState(string TaskId, string UserId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    Tasks task = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.ApplyManId == UserId
                     && t.IsSend == true).OrderByDescending(u => u.Id).First();
                    task.State = 1;
                    task.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    context.Entry<Tasks>(task).State = EntityState.Modified;
                    context.SaveChanges();
                    return new NewErrorModel()
                    {
                        error = new Error(0, "修改成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 流程类别及数据读取

        /// <summary>
        /// 流程界面信息读取接口
        /// </summary>
        /// <param name="userId">用户Id，用于判断权限(预留，暂时不做)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadFlowSort")]
        public NewErrorModel LoadFlowSort(string userId = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    FlowInfoServer flowInfoServer = new FlowInfoServer();
                    return new NewErrorModel()
                    {
                        data = flowInfoServer.GetFlowInfo(userId),
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
                return new NewErrorModel()
                {
                    error = new Error(1, "userId不能为空！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 流程分类批量修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("LoadFlowModify")]
        public NewErrorModel LoadFlowModify(FlowSortModel flowSortModel)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.RoleName == "超级管理员" && r.UserId == flowSortModel.applyManId).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有权限处理！", "") { },
                        };
                    }
                    else
                    {
                        foreach (var item in flowSortModel.FlowSortList)
                        {
                            context.Entry<FlowSort>(item).State = EntityState.Modified;
                            if (item.flows != null)
                            {
                                foreach (var flows in item.flows)
                                {
                                    context.Entry<Flows>(flows).State = EntityState.Modified;
                                }
                            }
                        }
                        context.SaveChanges();
                        return new NewErrorModel()
                        {
                            error = new Error(0, "修改成功！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 流程分类批量添加
        /// </summary>
        /// <param name="flowSortModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FlowSortAdd")]
        public NewErrorModel FlowSortAdd(FlowSortModel flowSortModel)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.RoleName == "超级管理员" && r.UserId == flowSortModel.applyManId).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有权限处理！", "") { },
                        };
                    }
                    List<FlowSort> flowSort = context.FlowSort.ToList();
                    foreach (var item in flowSortModel.FlowSortList)
                    {
                        if (flowSort.Where(f => f.Sort_ID.ToString().Contains(item.Sort_ID)).ToList().Count > 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, string.Format("Sort_ID 已存在 {0}！", item.Sort_ID.ToString()), "") { },
                            };
                        }
                    }
                    context.FlowSort.AddRange(flowSortModel.FlowSortList);
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "添加成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 流程分类批量删除
        /// </summary>
        /// <param name="flowSortModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FlowSortDelete")]
        public NewErrorModel FlowSortDelete(FlowSortModel flowSortModel)
        {
            try
            {
                DDContext context = new DDContext();

                if (context.Roles.Where(r => r.RoleName == "超级管理员" && r.UserId == flowSortModel.applyManId).ToList().Count == 0)
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "没有权限处理！", "") { },
                    };
                }
                foreach (var item in flowSortModel.FlowSortList)
                {
                    context.Entry<FlowSort>(item).State = EntityState.Deleted;
                }
                //context.FlowSort.RemoveRange(flowSortModel.FlowSortList);
                context.SaveChanges();

                return new NewErrorModel()
                {
                    error = new Error(0, "删除成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 流程批量添加
        /// </summary>
        /// <param name="flowsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FlowAdd")]
        public NewErrorModel FlowAdd(FlowsModel flowsModel)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.RoleName == "超级管理员" && r.UserId == flowsModel.applyManId).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有权限处理！", "") { },
                        };
                    }
                    List<FlowSort> flowSortList = context.FlowSort.ToList();
                    List<Flows> flowsListNew = context.Flows.ToList();
                    foreach (var item in flowsModel.flowsList)
                    {
                        if (flowSortList.Where(f => f.Sort_ID.ToString() == item.SORT_ID.ToString()).ToList().Count == 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, string.Format("不存在流程类别{0}！", item.SORT_ID.ToString()), "") { },
                            };
                        }
                        if (flowsListNew.Where(f => f.FlowId.ToString().Contains(item.FlowId.ToString())).ToList().Count > 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, string.Format("流程 FlowId {0} 已存在！", item.FlowId.ToString()), "") { },
                            };
                        }
                        context.Flows.Add(item);
                    }
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "添加成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 流程批量删除
        /// </summary>
        /// <param name="flowsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FlowDelete")]
        public NewErrorModel FlowDelete(FlowsModel flowsModel)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.RoleName == "超级管理员" && r.UserId == flowsModel.applyManId).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有权限处理！", "") { },
                        };
                    }
                    foreach (var item in flowsModel.flowsList)
                    {
                        context.Entry<Flows>(item).State = EntityState.Deleted;
                    }
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "删除成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 流程批量修改
        /// </summary>
        /// <param name="flowsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FlowModify")]
        public NewErrorModel FlowModify(FlowsModel flowsModel)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.RoleName == "超级管理员" && r.UserId == flowsModel.applyManId).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有权限处理！", "") { },
                        };
                    }
                    foreach (var item in flowsModel.flowsList)
                    {
                        context.Entry<Flows>(item).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 流程节点信息获取接口
        /// </summary>
        /// <param name="FlowId">流程Id</param>
        /// <param name="NodeId">节点Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNodeInfo")]
        public NewErrorModel GetNodeInfo(string FlowId, string NodeId)
        {
            try
            {
                if (FlowId != null)
                {
                    DDContext context = new DDContext();
                    if (string.IsNullOrEmpty(NodeId))
                    {
                        var NodeInfo = context.NodeInfo.Where(u => u.FlowId == FlowId);

                        return new NewErrorModel()
                        {
                            data = NodeInfo,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                    else
                    {
                        var NodeInfo = context.NodeInfo.Where(u => u.NodeId.ToString() == NodeId && u.FlowId == FlowId);

                        return new NewErrorModel()
                        {
                            data = NodeInfo,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "参数未传递！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 左侧审批菜单栏状态读取

        /// <summary>
        /// 左侧审批状态(数量)数据读取
        /// </summary>
        /// <param name="ApplyManId">用户名Id</param>
        /// <returns>返回待审批的、我发起的、抄送我的数量</returns>
        //[HttpGet]
        //[Route("GetFlowStateCounts")]
        //public NewErrorModel GetFlowStateCounts(string ApplyManId)
        //{
        //    try
        //    {
        //        using (DDContext context = new DDContext())
        //        {
        //            //待审批的
        //            int iApprove = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == false && u.State == 0 && u.IsPost == false).Count();
        //            //我发起的
        //            int iMyPost = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId == 0 && u.IsSend == false && u.State == 1 && u.IsPost == true).Count();
        //            //抄送我的
        //            int iSendMy = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == true && u.State == 0 && u.IsPost == false).Count();
        //            Dictionary<string, int> dic = new Dictionary<string, int>();
        //            dic.Add("ApproveCount", iApprove);
        //            dic.Add("MyPostCount", iMyPost);
        //            dic.Add("SendMyCount", iSendMy);

        //            return new NewErrorModel()
        //            {
        //                data = dic,
        //                error = new Error(0, "读取成功！", "") { },
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 获取流程状态(单条)
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetFlowStateSingle")]
        public NewErrorModel GetFlowStateSingle(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    return new NewErrorModel()
                    {
                        data = context.TasksState.Where(t => t.TaskId == TaskId).FirstOrDefault(),
                        error = new Error(0, "获取流程状态成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 左侧审批状态详细数据读取
        /// </summary>
        /// <param name="Index">(Index=0:待我审批 1:我已审批 2:我发起的 3:抄送我的)</param>
        /// <param name="ApplyManId">用户名Id</param>
        /// <param name="IsSupportMobile">是否是手机端调用接口(默认 false)</param>
        /// <param name="Key">关键字模糊查询(流水号、标题、申请人、流程类型)</param>
        /// <param name="pageIndex">页码(默认第一页)</param>
        /// <param name="OnlyReturnCount">仅返回总页数</param>
        /// <param name="pageSize">页容量(默认每页5条)</param>
        /// <returns> State 0 未完成 1 已完成 2 被退回</returns>
        [HttpGet]
        [Route("GetFlowStateDetail")]
        public NewErrorModel GetFlowStateDetail(int Index,
            string ApplyManId, bool IsSupportMobile = false,
            string Key = "", bool OnlyReturnCount = false, int pageIndex = 1, int pageSize = 99)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    string strWhere = string.Empty;
                    switch (Index)
                    {
                        case 0:
                            strWhere = " and isenable = 1 and ispost != 1 and  issend != 1 and d.state= 0 ";
                            break;
                        case 1:
                            strWhere = " and isenable = 1 and ( d.ispost is null  or d.ispost!=1)   and  issend != 1 and d.state= 1 ";
                            break;
                        case 2:
                            strWhere = " and ispost = 1 ";
                            break;
                        case 3:
                            strWhere = " and isenable = 1 and issend = 1 ";
                            break;
                        default:
                            break;
                    }

                    if (OnlyReturnCount)
                    {
                        List<string> whereList = new List<string>();
                        whereList.Add(" and isenable = 1 and ispost != 1 and  issend != 1 and d.state= 0 ");
                        whereList.Add(" and isenable = 1 and issend = 1 and d.state= 0");

                        Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
                        foreach (var item in whereList)
                        {
                            string strSqlCountNew = $" select count(*) from (select max(id) as id,taskid,FlowName,FlowId,Title,applyman,applyManId,FlowState,state,ApplyTime,CurrentTime,NodeId from(select d.id, d.taskid, c.FlowName, c.FlowId, c.Title, c.applyman, d.applyManId, c.State as FlowState,d.state,c.ApplyTime,c.CurrentTime,c.NodeId from tasks d left join  TasksState c on d.taskid = c.taskid   where d.taskid in (select distinct(a.TaskId) from tasks a left join TasksState b on a.TaskId = b.TaskId where (b.ApplyMan like '%{Key}%' or b.FlowName like '%{Key}%'  or b.taskid = '{Key}'))  {item}   and ApplyManId = '{ApplyManId}') newTable group by taskid, FlowName, FlowId, Title, applyman, applyManId, FlowState, state, ApplyTime, CurrentTime, NodeId ) ttt";
                            keyValuePairs.Add(whereList.IndexOf(item) == 0 ? "ApproveCount" : "SendMyCount", context.Database.SqlQuery<int>(strSqlCountNew).FirstOrDefault());
                        }
                        return new NewErrorModel()
                        {
                            data = keyValuePairs,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }

                    string strSqlQuery = $"select id,taskid,FlowName,FlowId,Title,applyman,applyManId,FlowState,state,ApplyTime,CurrentTime,NodeId from (select max(id) as id,taskid,FlowName,FlowId,Title,applyman,applyManId,FlowState,state,ApplyTime,CurrentTime,NodeId from(select top 100 percent  d.id,d.taskid,c.FlowName,c.FlowId,c.Title,c.applyman,d.applyManId,c.State as FlowState,d.state,c.ApplyTime,c.CurrentTime,c.NodeId from tasks d left join  TasksState c  on d.taskid = c.taskid   where d.taskid in (select distinct(a.TaskId) from tasks a left join TasksState b on a.TaskId = b.TaskId where (b.ApplyMan like '%{Key}%' or b.FlowName like '%{Key}%'  or b.taskid = '{Key}'))  {strWhere}  and ApplyManId = '{ApplyManId} ' order by d.taskid desc) newTable group by taskid, FlowName, FlowId, Title, applyman, applyManId, FlowState, state, ApplyTime, CurrentTime, NodeId ) newtt order by TaskId desc offset {pageSize * (pageIndex - 1)} rows fetch next {pageSize} rows only";


                    string strSqlCount = $" select count(*) from (select max(id) as id,taskid,FlowName,FlowId,Title,applyman,applyManId,FlowState,state,ApplyTime,CurrentTime,NodeId from(select d.id, d.taskid, c.FlowName, c.FlowId, c.Title, c.applyman, d.applyManId, c.State as FlowState,d.state,c.ApplyTime,c.CurrentTime,c.NodeId from tasks d left join  TasksState c on d.taskid = c.taskid   where d.taskid in (select distinct(a.TaskId) from tasks a left join TasksState b on a.TaskId = b.TaskId where (b.ApplyMan like '%{Key}%' or b.FlowName like '%{Key}%'  or b.taskid = '{Key}'))  {strWhere}   and ApplyManId = '{ApplyManId}') newTable group by taskid, FlowName, FlowId, Title, applyman, applyManId, FlowState, state, ApplyTime, CurrentTime, NodeId ) ttt";


                    int count = context.Database.SqlQuery<int>(strSqlCount).FirstOrDefault();

                    //string strSqlQuery = $"select d.id,d.taskid,c.FlowName,c.FlowId,c.Title,c.applyman,d.applyManId,c.State as FlowState,d.state,c.ApplyTime,c.CurrentTime,c.NodeId from tasks d left join  TasksState c  on  d.taskid=c.taskid   where d.taskid in (select distinct(a.TaskId) from tasks a  left join TasksState b on a.TaskId = b.TaskId where (b.ApplyMan like '%{Key}%' or b.FlowName like '%{Key}%'  or b.taskid = '{Key}')) {strWhere}  and ApplyManId = '{ApplyManId} ' order by d.taskid desc offset {pageIndex} - 1 rows fetch next {pageSize} rows only ";

                    //string strSqlCount = $"select count(*) from tasks d left join  TasksState c  on  d.taskid=c.taskid   where d.taskid in (select distinct(a.TaskId) from tasks a  left join TasksState b on a.TaskId = b.TaskId where (b.ApplyMan like '%{Key}%' or b.FlowName like '%{Key}%'  or b.taskid = '{Key}')) {strWhere}  and ApplyManId = '{ApplyManId}'";
                    //string strSql = $"select d.id,d.taskid,c.FlowName,c.Title,c.applyman,d.applyManId,c.State,c.ApplyTime,c.CurrentTime,c.NodeId from tasks d left join  TasksState c  on  d.taskid=c.taskid   where d.taskid in (select distinct(a.TaskId) from tasks a  left join TasksState b on a.TaskId = b.TaskId where (b.ApplyMan like '%@key%' or b.FlowName like '%@key%'  or b.taskid = '@key')) {strWhere}  and ApplyManId = '@applyManId' order by d.taskid desc offset @pageIndex - 1 rows fetch next @pageSize rows only ";
                    //List<SqlParameter> parameters = new List<SqlParameter>();
                    //parameters.Add(new SqlParameter("@applyManId", ApplyManId));
                    //parameters.Add(new SqlParameter("@key", Key));
                    //parameters.Add(new SqlParameter("@pageIndex", pageIndex));
                    //parameters.Add(new SqlParameter("@pageSize", pageSize));
                    //List<TasksQueryPro> tasksAll = context.Database.SqlQuery<TasksQueryPro>(strSql, parameters.ToArray()).ToList();

                    //context.Database.Log = (sql) =>
                    //{
                    //    if (string.IsNullOrEmpty(sql) == false)
                    //    {
                    //        Console.WriteLine(sql);
                    //    }
                    //};


                    List<TasksQueryPro> tasksAll = context.Database.SqlQuery<TasksQueryPro>(strSqlQuery).ToList();




                    foreach (var item in tasksAll)
                    {
                        if (Index == 3)
                        {
                            if (item.State == 1)
                            {
                                item.IsRead = true;
                            }
                            else
                            {
                                item.IsRead = false;
                            }
                        }
                    }

                    return new NewErrorModel()
                    {
                        count = count,
                        data = tasksAll,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  流程分类(后端用)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TasksSort")]
        public List<Tasks> TasksSort(int index, List<Tasks> tasks)
        {
            switch (index)
            {
                //待我审批
                case 0:
                    tasks = tasks.Where(t => t.State == 0 && t.IsEnable == 1
                    && t.IsPost != true && t.IsSend != true).ToList();
                    break;
                //我已审批
                case 1:
                    tasks = tasks.Where(t => t.State == 1 && t.IsEnable == 1
                    && t.IsSend != true && t.IsPost != true).ToList();
                    break;
                //我发起的
                case 2:
                    tasks = tasks.Where(t => t.IsPost == true).ToList();
                    break;
                //抄送我的
                case 3:
                    tasks = tasks.Where(t => t.IsEnable == 1
                    && t.IsSend == true).ToList();
                    break;
            }
            return tasks;
        }

        /// <summary>
        /// 辅助查询
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ListTasks"></param>
        /// <param name="ApplyManId"></param>
        /// <param name="IsMobile"></param>
        /// <param name="Key"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Quary")]
        public List<TaskFlowModel> Quary(DDContext context, List<int?> ListTasks,
            string ApplyManId, bool IsMobile, string Key, int Index)
        {
            FlowInfoServer flowInfoServer = new FlowInfoServer();
            List<object> listQuary = new List<object>();
            List<object> listQuaryPro = new List<object>();
            //List<Tasks> ListTask = context.Tasks.ToList();

            List<TasksQuery> ListTask = context.TasksQuery.SqlQuery("select id,taskid,nodeid,flowid,ApplyMan,ApplyManId,ApplyTime,Title,State,IsBacked,IsSend from tasks where taskid in " +
                "(select TaskId from tasks where ApplyManId = @applyManId)", new SqlParameter("@applyManId", ApplyManId)).ToList();


            List<Flows> ListFlows = context.Flows.ToList();
            List<TasksState> ListTasksState = context.TasksState.ToList();
            foreach (int TaskId in ListTasks)
            {
                int StateCount = ListTask.Where(t => t.TaskId.ToString() == TaskId.ToString() && t.State == 0 && t.IsSend != true).Count();
                int? NodeId = 0;
                if (StateCount == 0)
                {
                    NodeId = ListTask.Where(t => t.TaskId.ToString() == TaskId.ToString()).Max(n => n.NodeId);
                }
                else
                {
                    if (StateCount > 1)
                    {
                        NodeId = ListTask.Where(t => t.TaskId.ToString() == TaskId.ToString() && t.State == 0 && t.IsSend != true).OrderBy(u => u.NodeId).Select(u => u.NodeId).ToList().First();
                    }
                    else
                    {
                        NodeId = ListTask.Where(t => t.TaskId.ToString() == TaskId.ToString() && t.State == 0 && t.IsSend != true).Select(u => u.NodeId).ToList().First();
                    }
                }
                var query = from t in ListTask
                            join f in ListFlows
                            on t.FlowId.ToString() equals f.FlowId.ToString()
                            join ts in ListTasksState
                            on t.TaskId.ToString() equals ts.TaskId
                            where t.NodeId == 0 && t.TaskId == TaskId
                            && (IsMobile == true ? f.IsSupportMobile == true : 1 == 1)
                            && ((Key != "" ? f.FlowName.Contains(Key) : 1 == 1) ||
                                (Key != "" ? t.TaskId.ToString().Contains(Key) : 1 == 1) ||
                                 (Key != "" ? t.Title.ToString().Contains(Key) : 1 == 1) ||
                                (Key != "" ? t.ApplyMan.Contains(Key) : 1 == 1)
                            )
                            select new
                            {
                                Id = t.Id + 1,
                                TaskId = t.TaskId,
                                NodeId = NodeId,
                                FlowId = t.FlowId,
                                ApplyMan = t.ApplyMan,
                                ApplyManId = t.ApplyManId,
                                ApplyTime = t.ApplyTime,
                                Title = t.Title,
                                State = ts.State,
                                IsBack = t.IsBacked,
                                FlowName = f.FlowName,
                                IsSupportMobile = f.IsSupportMobile,
                                //当前审批状态
                                IsRead = Index == 3 ? (ListTask.Where(t => t.ApplyManId == ApplyManId && t.TaskId == TaskId && t.IsSend == true).FirstOrDefault().State == 1 ? true : false) : false,
                            };

                if (query.Count() > 0)
                {
                    listQuary.Add(query);
                }
            }

            string strJson = JsonConvert.SerializeObject(listQuary);
            List<List<TaskFlowModel>> TaskFlowModelListList = JsonConvert.DeserializeObject<List<List<TaskFlowModel>>>(strJson);
            List<TaskFlowModel> TaskFlowModelList = new List<TaskFlowModel>();
            List<TaskFlowModel> TaskFlowModelListQuery = new List<TaskFlowModel>();
            List<List<TaskFlowModel>> TaskFlowModelListListPro = new List<List<TaskFlowModel>>();
            foreach (var item in TaskFlowModelListList)
            {
                TaskFlowModelList.Add(item[0]);
            }

            foreach (var item in TaskFlowModelList)
            {
                if (!TaskFlowModelListQuery.Contains(item))
                {
                    List<TaskFlowModel> taskFlowModels = TaskFlowModelListQuery.Where(t => t.TaskId == item.TaskId).ToList();
                    if (taskFlowModels.Count == 0)
                    {
                        TaskFlowModelListQuery.Add(item);
                    }
                }
            }
            return TaskFlowModelListQuery;
        }
        /// <summary>
        /// 获取流程状态
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="ListTask"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTasksState")]
        public string GetTasksState(string TaskId, List<Tasks> ListTask)
        {
            List<Tasks> tasksListBack = ListTask.Where(t => t.TaskId.ToString() == TaskId && t.IsBacked == true).ToList();
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
            List<Tasks> tasksListFinished = ListTask.Where(t => t.TaskId.ToString() == TaskId && t.State == 0 && t.IsSend != true).ToList();
            if (tasksListFinished.Count > 0)
            {
                return "未完成";
            }
            else
            {
                return "已完成";
            }
        }

        #endregion

        #region 审批流修改

        /// <summary>
        /// 审批流修改
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        [Route("TaskModify")]
        [HttpPost]
        public NewErrorModel ModifyTable(Tasks tasks)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    Tasks tasksNew = context.Tasks.Where(t => t.TaskId == tasks.TaskId && t.NodeId == 0).First();
                    tasksNew.ImageUrl = tasks.ImageUrl;
                    context.Entry<Tasks>(tasksNew).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }

                return new NewErrorModel()
                {
                    error = new Error(0, "", "修改成功") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region 审批意见数据读取

        /// <summary>
        /// 审批意见数据读取
        /// </summary>
        /// <param name="FlowId">流程Id</param>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSign")]
        public NewErrorModel GetSign(string FlowId, string TaskId = "")
        {
            try
            {
                if (string.IsNullOrEmpty(TaskId))  //尚未发起流程
                {
                    using (DDContext context = new DDContext())
                    {
                        List<NodeInfo> NodeInfoList = context.NodeInfo.Where(n => n.FlowId == FlowId).ToList();

                        var Quary = from n in NodeInfoList
                                    orderby n.NodeId
                                    select new
                                    {
                                        NodeId = n.NodeId,
                                        NodeName = n.NodeName,
                                        IsBack = false,
                                        ApplyMan = n.NodePeople,
                                        ApplyTime = "",
                                        Remark = "",
                                        IsSend = "",
                                        IsNeedChose = n.IsNeedChose,
                                        ChoseNodeId = n.ChoseNodeId,
                                        IsMandatory = n.IsMandatory,
                                        IsSelectMore = n.IsSelectMore,
                                        n.ChoseType,
                                        n.RoleNames,
                                        RolesList = n.ChoseType == "1" ? GetRolesList(n.RoleNames) : n.RolesList
                                    };
                        return new NewErrorModel()
                        {
                            data = Quary.OrderByDescending(f => f.ApplyTime),
                            error = new Error(0, "读取成功！", "") { },
                        };

                    }
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId).ToList();
                        List<Tasks> TaskList = context.Tasks.Where(u => u.TaskId.ToString() == TaskId).ToList();

                        //List<SignModel> SignModels = new List<SignModel>();
                        //foreach (var nodeInfo in NodeInfoList)
                        //{
                        //    List<string> NodePeopleList = new List<string>();
                        //    List<string> NodePeopleIdList = new List<string>();
                        //    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                        //    if (nodeInfo.NodePeople != null && nodeInfo.PeopleId != null)
                        //    {
                        //        NodePeopleList.AddRange(nodeInfo.NodePeople.Split(','));
                        //        NodePeopleIdList.AddRange(nodeInfo.PeopleId.Split(','));
                        //    }
                        //    if (NodePeopleList.Count > 0)
                        //    {
                        //        //没配置人员
                        //        List<Tasks> tasks = TaskList.Where(t => t.NodeId == nodeInfo.NodeId).ToList();
                        //        if (tasks.Count > 0)
                        //        {
                        //            foreach (var item in tasks)
                        //            {
                        //                SignModels.Add(new SignModel()
                        //                {
                        //                    ApplyMan = item.ApplyMan,

                        //                    IsBack = item.IsBacked,
                        //                    IsMandatory = nodeInfo.IsMandatory,
                        //                    IsSelectMore = nodeInfo.IsSelectMore,
                        //                    IsSend = nodeInfo.IsSend,
                        //                    ApplyManId = item.ApplyManId,
                        //                    ApplyTime = item.ApplyTime,
                        //                    NodeId = nodeInfo.NodeId,
                        //                    NodeName = nodeInfo.NodeName,
                        //                    Remark = item.Remark
                        //                });
                        //            }
                        //        }

                        //        foreach (var item in NodePeopleIdList)
                        //        {
                        //            Tasks tasksNew = TaskList.Where(t => t.ApplyManId == item && t.NodeId == nodeInfo.NodeId).FirstOrDefault();
                        //            if (tasksNew != null)
                        //            {
                        //                SignModels.Add(new SignModel()
                        //                {
                        //                    ApplyMan = tasksNew.ApplyMan,

                        //                    IsBack = false,

                        //                    IsMandatory = nodeInfo.IsMandatory,
                        //                    IsSelectMore = nodeInfo.IsSelectMore,
                        //                    IsSend = nodeInfo.IsSend,
                        //                    ApplyManId = tasksNew.ApplyManId,
                        //                    ApplyTime = tasksNew.ApplyTime,
                        //                    NodeId = nodeInfo.NodeId,
                        //                    NodeName = nodeInfo.NodeName,
                        //                    Remark = tasksNew.Remark
                        //                });
                        //            }
                        //            else
                        //            {
                        //                SignModels.Add(new SignModel()
                        //                {

                        //                    IsBack = false,

                        //                    IsMandatory = nodeInfo.IsMandatory,
                        //                    IsSelectMore = nodeInfo.IsSelectMore,
                        //                    ApplyManId = NodePeopleIdList[NodePeopleIdList.IndexOf(item)],
                        //                    IsSend = nodeInfo.IsSend,
                        //                    NodeId = nodeInfo.NodeId,
                        //                    NodeName = nodeInfo.NodeName,
                        //                    ApplyMan= NodePeopleList[NodePeopleIdList.IndexOf(item)]
                        //                });
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //没配置人员
                        //        List<Tasks> tasks = TaskList.Where(t => t.NodeId == nodeInfo.NodeId).ToList();
                        //        if (tasks.Count > 0)
                        //        {
                        //            foreach (var item in tasks)
                        //            {
                        //                SignModels.Add(new SignModel()
                        //                {
                        //                    ApplyMan = item.ApplyMan,

                        //                    IsBack = item.IsBacked,
                        //                    IsMandatory = nodeInfo.IsMandatory,
                        //                    IsSelectMore = nodeInfo.IsSelectMore,
                        //                    IsSend = nodeInfo.IsSend,
                        //                    ApplyManId = item.ApplyManId,
                        //                    ApplyTime = item.ApplyTime,
                        //                    NodeId = nodeInfo.NodeId,
                        //                    NodeName = nodeInfo.NodeName,
                        //                    Remark = item.Remark
                        //                });
                        //            }
                        //        }
                        //        else
                        //        {
                        //            SignModels.Add(new SignModel()
                        //            {

                        //                IsBack = false,
                        //                IsMandatory = nodeInfo.IsMandatory,
                        //                IsSelectMore = nodeInfo.IsSelectMore,
                        //                ApplyManId = nodeInfo.PeopleId,
                        //                IsSend = nodeInfo.IsSend,
                        //                NodeId = nodeInfo.NodeId,
                        //                NodeName = nodeInfo.NodeName,
                        //            });
                        //        }
                        //    }
                        //}

                        //return new NewErrorModel()
                        //{
                        //    data = SignModels.OrderBy(q => q.NodeId).ThenBy(h => h.ApplyTime),
                        //    error = new Error(0, "读取成功！", "") { },
                        //};

                        var Quary = from n in NodeInfoList
                                    join t in TaskList
                                    on n.NodeId equals t.NodeId
                                    into temp
                                    from tt in temp.DefaultIfEmpty()
                                    orderby n.NodeId
                                    select new
                                    {
                                        //Id = tt == null ? 0 : tt.Id,
                                        NodeId = n.NodeId,
                                        NodeName = n.NodeName,
                                        IsBack = tt == null ? false : tt.IsBacked,
                                        ApplyMan = tt == null ? n.NodePeople : tt.ApplyMan,
                                        ApplyTime = tt == null ? "" : tt.ApplyTime,
                                        Remark = tt == null ? "" : tt.Remark,
                                        IsSend = tt == null ? n.IsSend : tt.IsSend,
                                        ApplyManId = tt == null ? "" : tt.ApplyManId,
                                        IsMandatory = n.IsMandatory,
                                        IsSelectMore = n.IsSelectMore
                                    };
                        Quary = Quary.OrderBy(q => q.NodeId).ThenBy(h => h.ApplyTime);
                        return new NewErrorModel()
                        {
                            data = Quary,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量获取角色信息
        /// </summary>
        /// <param name="rolenames"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRolesList")]
        public Dictionary<string, List<Roles>> GetRolesList(string rolenames)
        {
            Dictionary<string, List<Roles>> keyValuePairs = new Dictionary<string, List<Roles>>();
            DDContext context = new DDContext();
            string[] roles = rolenames.Split(',');
            foreach (var item in roles)
            {
                List<Roles> rolesListNew = context.Roles.Where(r => r.RoleName == item).ToList();
                keyValuePairs.Add(item, rolesListNew);
            }
            return keyValuePairs;
        }

        #endregion

        #region 流程节点人员配置

        /// <summary>
        /// 流程节点人员配置
        /// </summary>
        /// <param name="NodeInfoList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateNodeInfo")]
        public NewErrorModel UpdateNodeInfo([FromBody]List<NodeInfo> NodeInfoList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (NodeInfo nodeInfo in NodeInfoList)
                    {
                        context.Entry<NodeInfo>(nodeInfo).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 审批页面通用数据读取

        /// <summary>
        /// 审批页面通用数据读取
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="ApplyManId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetApproveInfo")]
        public NewErrorModel GetApproveInfo(string TaskId, string ApplyManId)
        {
            try
            {
                if (string.IsNullOrEmpty(TaskId))
                {
                    return new NewErrorModel()
                    {
                        error = new Error(0, "请传递参数！", "") { },
                    };
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        Tasks taskOld = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.NodeId == 0).First();
                        List<Tasks> tasksList = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.IsSend != true).OrderByDescending(t => t.Id).ToList();
                        if (tasksList.Count > 0)
                        {
                            taskOld.Id = tasksList[0].Id;
                            taskOld.NodeId = tasksList[0].NodeId;
                        }
                        else
                        {
                            List<Tasks> tasksListSend = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.IsSend == true).OrderByDescending(t => t.Id).ToList();
                            if (tasksListSend.Count > 0)
                            {
                                taskOld.Id = tasksListSend[0].Id;
                                taskOld.NodeId = tasksListSend[0].NodeId;
                            }
                        }

                        return new NewErrorModel()
                        {
                            data = taskOld,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 系统已配置人员信息读取

        /// <summary>
        /// 系统已配置人员信息读取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserInfo")]
        public NewErrorModel GetUserInfo()
        {
            try
            {
                DDContext context = new DDContext();
                var NodeInfoList = context.NodeInfo;
                var Quary = from n in NodeInfoList
                            where n.PeopleId != null && !n.PeopleId.Contains(",")
                            select new
                            {
                                n.PeopleId,
                                n.NodePeople
                            };
                var QuaryPro = from n in Quary
                               group n by new { PeopleId = n.PeopleId, NodePeople = n.NodePeople }
                               into g
                               select new { g.Key.PeopleId, g.Key.NodePeople };

                return new NewErrorModel()
                {
                    data = QuaryPro,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region 钉钉SDK再封装接口

        /// <summary>
        /// 发送OA消息
        /// </summary>
        [HttpGet]
        [Route("TestSentOaMsg")]
        public string TestSentOaMsg()
        {
            TopSDKTest top = new TopSDKTest();
            OATextModel oaTextModel = new OATextModel();
            oaTextModel.message_url = "https://www.baidu.com/";
            oaTextModel.head = new head
            {
                bgcolor = "FFBBBBBB",
                text = "头部标题111"
            };
            oaTextModel.body = new body
            {
                form = new form[] {
                    new form{ key="姓名",value="11张三"},

                    new form{ key="爱好",value="打球"},
                },
                rich = new rich
                {
                    num = "15.6",
                    unit = "元"
                },
                //title = "正文标题",
                content = "111一大段文字",
                image = "@lADOADmaWMzazQKA",
                file_count = "3",
                author = "李四"
            };
            return top.SendOaMessage("083452125733424957", oaTextModel);
        }

        /// <summary>
        /// 推送OA消息
        /// </summary>
        /// <param name="FlowId"></param>
        /// <param name="ApplyManId"></param>
        /// <param name="TaskId"></param>
        /// <param name="ApplyMan"></param>
        /// <param name="Remark"></param>
        /// <param name="dDContext"></param>
        /// <param name="LinkUrl"></param>
        /// <param name="NodeId"></param>
        /// <param name="IsBack"></param>
        /// <param name="IsSend"></param>
        /// <param name="IsFinnish">是否完结</param>
        /// <returns></returns>
        [HttpGet]
        [Route("SendOaMsgNew")]
        public async Task<object> SendOaMsgNew(int? FlowId, string ApplyManId, string TaskId, string ApplyMan,
            string Remark, DDContext dDContext,
            string LinkUrl, string NodeId,
            bool IsBack = false, bool IsSend = false, bool IsFinnish = false)
        {
            DingTalkServersController dingTalkServersController = new DingTalkServersController();

            string ApplyManNew= dDContext.TasksState.Where(t => t.TaskId == TaskId).FirstOrDefault().ApplyMan;
            if (!string.IsNullOrEmpty(ApplyManNew))
            {
                ApplyMan = ApplyManNew;
            }
            
            string strLink = LinkUrl + "?taskid=" + TaskId +
                            "&flowid=" + FlowId +
                            "&nodeid=" + NodeId;
            Flows flows = dDContext.Flows.Where(f => f.FlowId.ToString() == FlowId.ToString()).First();
            if (IsFinnish == false)
            {
                //推送OA消息(手机端)
                if (flows.IsSupportMobile == true)
                {
                    if (IsBack)
                    {
                        //strLink = strLink + "&index=2";
                        strLink = "eapp://page/approve/approve?index=2";
                        return await dingTalkServersController.sendOaMessage(ApplyManId,
                        string.Format(string.Format("您提交的{0}已退回，请知晓。", flows.FlowName), TaskId),
                        flows.FlowName, TaskId, strLink);
                    }
                    else
                    {
                        if (IsSend)
                        {
                            //strLink = strLink + "&index=3";
                            strLink = "eapp://page/approve/approve?index=3";
                            return await dingTalkServersController.sendOaMessage(ApplyManId,
                            string.Format(string.Format("{0}提交的{1}抄送给您，请知晓。", ApplyMan, flows.FlowName), TaskId),
                            flows.FlowName, TaskId, strLink);
                        }
                        else
                        {
                            //strLink = strLink + "&index=0";
                            strLink = "eapp://page/approve/approve?index=0";
                            return await dingTalkServersController.sendOaMessage(ApplyManId,
                            string.Format(string.Format("{0}提交的{1}需要您审批", ApplyMan, flows.FlowName), TaskId),
                            flows.FlowName, TaskId, strLink);
                        }
                    }
                }
                else    //推送OA消息(电脑端)
                {
                    if (IsBack)
                    {
                        SentCommonMsg(ApplyManId, string.Format("您提交的{0}流程被退回(流水号:{1})，请知晓。", flows.FlowName, TaskId), ApplyMan, Remark, null, TaskId, flows.FlowName);
                    }
                    else
                    {
                        if (IsSend)
                        {
                            SentCommonMsg(ApplyManId, string.Format("{0}提交的{1}流程抄送给您，请知晓。", ApplyMan, flows.FlowName, TaskId), ApplyMan, Remark, null, TaskId, flows.FlowName);
                        }
                        else
                        {
                            SentCommonMsg(ApplyManId, string.Format("{0}提交的{1}需要您审批", ApplyMan, flows.FlowName), ApplyMan, Remark, null, TaskId, flows.FlowName);
                        }
                    }
                    return dingTalkServersController.sendOaMessage("测试",
                           string.Format("您有一条待审批的流程(流水号:{0})，请进入研究院信息管理系统进行审批。", TaskId),
                           ApplyMan, "eapp://page/approve/approve");
                }
            }
            else
            {
                //推送OA消息(手机端)
                if (dDContext.Flows.Where(f => f.FlowId.ToString() == FlowId.ToString()).First().IsSupportMobile == true)
                {
                    strLink = "eapp://page/approve/approve?index=0";
                    return await dingTalkServersController.sendOaMessage(ApplyManId,
               string.Format("您发起的审批的流程(流水号:{0})，已审批完成请知晓。", TaskId), flows.FlowName,
                TaskId, strLink);
                }
                else
                {
                    SentCommonMsg(ApplyManId, string.Format("您发起的审批的流程(流水号:{0})，已审批完成请知晓。", TaskId), flows.FlowName, ApplyMan, Remark, TaskId, flows.FlowName);
                }
                return dingTalkServersController.sendOaMessage("测试",
                          string.Format("您有一条待审批的流程(流水号:{0})，请进入研究院信息管理系统进行审批。", TaskId),
                          ApplyMan, "eapp://page/approve/approve");
            }
        }

        /// <summary>
        /// 发送普通消息
        /// </summary>
        /// <param name="SendPeoPleId"></param>
        /// <param name="Title"></param>
        /// <param name="ApplyMan"></param>
        /// <param name="Content"></param>
        /// <param name="Url"></param>
        /// <param name="TaskId"></param>
        /// <param name="FlowName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SentCommonMsg")]
        public string SentCommonMsg(string SendPeoPleId, string Title, string ApplyMan,
            string Content, string Url, string TaskId, string FlowName)
        {
            TopSDKTest top = new TopSDKTest();
            OATextModel oaTextModel = new OATextModel();
            oaTextModel.head = new head
            {
                bgcolor = "FFBBBBBB",
            };
            oaTextModel.body = new body
            {
                form = new form[] {
                    new form{ key="流水号：",value=TaskId},
                      new form{ key="审批类型：",value=FlowName},
                    new form{ key="申请时间：",value=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                },
                title = Title,
                content = Content
            };
            oaTextModel.message_url = Url;
            return top.SendOaMessage(SendPeoPleId, oaTextModel);
        }


        #endregion

        #region 本人审批意见修改

        /// <summary>
        /// 本人审批意见修改
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChangeRemark")]
        public NewErrorModel ChangeRemark(string Id, string Remark)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    Tasks tasks = context.Tasks.Find(Int32.Parse(Id));
                    tasks.Remark = Remark;
                    context.Entry<Tasks>(tasks).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取流程状态

        /// <summary>
        /// 获取流程状态
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        public NewErrorModel GetTasksState(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    return new NewErrorModel()
                    {
                        data = context.TasksState.Where(t => t.TaskId == TaskId).FirstOrDefault(),
                        error = new Error(0, "获取成功！", "") { }
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 同步流程状态(新旧数据交替)

        /// <summary>
        /// 同步流程状态(新旧数据交替)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SynTasksState")]
        public NewErrorModel SynTasksState()
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Tasks> tasks = context.Tasks.ToList();
                    FlowInfoServer flowInfoServer = new FlowInfoServer();
                    EFHelper<TasksState> eFHelper = new EFHelper<TasksState>();
                    eFHelper.DelBy(t => t.TaskId != "");
                    List<string> taskIdList = new List<string>();
                    foreach (var item in tasks)
                    {
                        if (!taskIdList.Contains(item.TaskId.ToString()))
                        {
                            taskIdList.Add(item.TaskId.ToString());
                        }
                    }
                    List<TasksState> TasksStateList = new List<TasksState>();
                    foreach (var taskId in taskIdList)
                    {
                        TasksStateList.Add(new TasksState()
                        {
                            TaskId = taskId,
                            State = flowInfoServer.GetTasksState(taskId)
                        });
                    }

                    context.TasksState.AddRange(TasksStateList);
                    context.SaveChanges();

                    return new NewErrorModel()
                    {
                        count = TasksStateList.Count,
                        error = new Error(0, "同步成功！", "") { }
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 流程图数据读取

        /// <summary>
        /// 流程图数据读取
        /// </summary>
        /// <param name="flowId">流程Id</param>
        /// <returns></returns>
        [Route("ReadFlows")]
        [HttpGet]
        public NewErrorModel ReadFlows(string flowId)
        {
            try
            {
                DDContext context = new DDContext();
                List<NodeInfo> flows = context.NodeInfo.Where(f => f.FlowId.ToString() == flowId).ToList();
                return new NewErrorModel()
                {
                    data = flows,
                    error = new Error(0, "读取成功！", "") { },
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 人员管理

        /// <summary>
        /// 搜索人员信息
        /// </summary>
        /// <param name="applyManId"></param>
        /// <returns></returns>
        [Route("GetNodeInfoInfoByApplyManId")]
        public NewErrorModel GetNodeInfoInfoByApplyManId(string applyManId = "")
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Flows> flows = context.Flows.ToList();
                    List<NodeInfo> nodeInfos = context.NodeInfo.ToList();
                    Dictionary<string, List<NodeInfo>> keyValuePairs = new Dictionary<string, List<NodeInfo>>();
                    List<string> vs = new List<string>();
                    foreach (var item in nodeInfos)
                    {
                        if (!string.IsNullOrEmpty(item.PeopleId))
                        {
                            if (item.PeopleId.Contains(applyManId))
                            {
                                Flows flow = flows.Where(f => f.FlowId.ToString() == item.FlowId).FirstOrDefault();
                                if (flow != null)
                                {
                                    if (!keyValuePairs.ContainsKey(flow.FlowName))
                                    {
                                        keyValuePairs.Add(flow.FlowName, nodeInfos.Where(n => n.FlowId == item.FlowId).ToList());
                                    }
                                }
                            }
                        }
                    }
                    Dictionary<string, List<NodeInfo>> keyValuePairsPro = new Dictionary<string, List<NodeInfo>>();
                    foreach (var item in keyValuePairs)
                    {
                        keyValuePairsPro.Add(item.Key, item.Value.OrderBy(i => i.NodeId).ToList());
                    }
                    return new NewErrorModel()
                    {
                        data = keyValuePairsPro,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 批量新增流程节点信息
        /// </summary>
        /// <param name="nodeInfos"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("AddNodeInfoInfo")]
        //public NewErrorModel AddNodeInfoInfo(List<NodeInfo> nodeInfos)
        //{
        //    try
        //    {
        //        using (DDContext context = new DDContext())
        //        {
        //            context.NodeInfo.AddRange(nodeInfos);
        //            context.SaveChanges();
        //            return new NewErrorModel()
        //            {
        //                error = new Error(0, "新增成功！", "") { },
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 删除流程节点信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("DelNodeInfoInfoById")]
        //public NewErrorModel DelNodeInfoInfoById(int Id)
        //{
        //    try
        //    {
        //        using (DDContext context = new DDContext())
        //        {
        //            NodeInfo nodeInfo = context.NodeInfo.Find(Id);
        //            context.NodeInfo.Remove(nodeInfo);
        //            context.SaveChanges();
        //            return new NewErrorModel()
        //            {
        //                error = new Error(0, "删除成功！", "") { },
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        /// <summary>
        /// 获取流程节点信息
        /// </summary>
        /// <param name="flowId">流程Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNodeInfos")]
        public NewErrorModel GetNodeInfos(string flowId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    return new NewErrorModel()
                    {
                        data = context.NodeInfo.Where(n => n.FlowId == flowId).OrderBy(n => n.NodeId).ToList(),
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量更新流程节点信息
        /// </summary>
        /// <param name="nodeInfos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateNodeInfos")]
        public NewErrorModel UpdateNodeInfos(List<NodeInfo> nodeInfos)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    //校验数据
                    if (nodeInfos != null && nodeInfos.Count > 0)
                    {
                        nodeInfos = nodeInfos.OrderBy(n => n.NodeId).ToList();

                        //校验必填项目
                        //foreach (var item in nodeInfos)
                        //{
                        //    if (item.IsNeedChose == true)
                        //    {
                        //        if (string.IsNullOrEmpty(item.IsSelectMore) || string.IsNullOrEmpty       (item.ChoseNodeId) || string.IsNullOrEmpty(item.ChoseType))
                        //        {
                        //            return new NewErrorModel()
                        //            {
                        //                error = new Error(1, $"格式有误：                                         {JsonConvert.SerializeObject(item)}！", "") { },
                        //            };
                        //        }
                        //        else
                        //        {
                        //            if (item.ChoseType == "1")
                        //            {
                        //                if (string.IsNullOrEmpty(item.RoleNames))
                        //                {

                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        //校验流程是否能走完
                        if (CheckNodeInfo(nodeInfos[0], nodeInfos) != null)
                        {
                            string flowid = nodeInfos[0].FlowId.ToString();
                            //清空旧数据
                            List<NodeInfo> nodeInfosList = context.NodeInfo.Where(n => n.FlowId == flowid).ToList();
                            //新增数据
                            context.NodeInfo.RemoveRange(nodeInfosList);
                            foreach (var item in nodeInfos)
                            {
                                context.NodeInfo.Add(item);
                            }
                            context.SaveChanges();

                            return new NewErrorModel()
                            {
                                error = new Error(0, "修改成功！", "") { },
                            };
                        }
                        else
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "流程走不完啦！", "") { },
                            };
                        }
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "接收不到参数！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 数据校验(后端用)
        /// </summary>
        /// <param name="nodeInfo"></param>
        /// <param name="nodeInfos"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckNodeInfo")]
        public NodeInfo CheckNodeInfo(NodeInfo nodeInfo, List<NodeInfo> nodeInfos)
        {
            NodeInfo nodeInfoPro = nodeInfos.Where(n => n.NodeId.ToString() == nodeInfo.PreNodeId).FirstOrDefault();
            if (nodeInfoPro != null)
            {
                if (nodeInfoPro.NodeName != "结束")
                {
                    return CheckNodeInfo(nodeInfoPro, nodeInfos);
                }
                else
                {
                    return nodeInfoPro;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 旧数据迁移(2019.12.23)
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("AsyncTasksState")]
        public NewErrorModel AsyncTasksState(string taskId = "")
        {
            try
            {
                DDContext dDContext = new DDContext();
                List<TasksState> tasksStates = new List<TasksState>();
                List<Tasks> tasksAll = new List<Tasks>();
                if (taskId == "")
                {
                    tasksStates = dDContext.TasksState.ToList();
                    tasksAll = dDContext.Tasks.ToList();
                }
                else
                {
                    tasksStates = dDContext.TasksState.Where(t => t.TaskId == taskId).ToList();
                    tasksAll = dDContext.Tasks.Where(t => t.TaskId.ToString() == taskId).ToList();
                }
                List<TasksState> tasksStatesMove = new List<TasksState>();
                //清理重复数据
                List<string> taskIdList = new List<string>();
                foreach (var item in tasksStates)
                {
                    if (taskIdList.Contains(item.TaskId))
                    {
                        tasksStatesMove.Add(item);
                    }
                    else
                    {
                        taskIdList.Add(item.TaskId);
                    }
                }
                dDContext.TasksState.RemoveRange(tasksStatesMove);
                dDContext.SaveChanges();


                List<Tasks> tasks = tasksAll.Where(t => t.NodeId == 0).ToList();
                List<Flows> flows = dDContext.Flows.ToList();
                List<NodeInfo> nodeInfos = dDContext.NodeInfo.ToList();
                foreach (var item in tasks)
                {
                    Flows flowsNew = flows.Where(f => f.FlowId == item.FlowId).FirstOrDefault();
                    TasksState tasksState = tasksStates.Where(t => t.TaskId == item.TaskId.ToString()).FirstOrDefault();
                    tasksState.Title = item.Title;
                    tasksState.ApplyMan = item.ApplyMan;
                    tasksState.ApplyTime = item.ApplyTime;
                    tasksState.FlowName = flowsNew.FlowName;
                    if (flowsNew.FlowId != null && flowsNew.FlowId != 0)
                    {
                        tasksState.FlowId = flowsNew.FlowId.ToString();
                    }
                    Tasks tasksNew = tasksAll.Where(t => t.TaskId == item.TaskId && t.State == 0 && t.IsEnable == 1 && t.IsSend != true).OrderByDescending(t => t.NodeId).FirstOrDefault();
                    Tasks tasksPro = tasksAll.Where(t => t.TaskId == item.TaskId && t.State == 1 && t.IsEnable == 1 && t.IsSend != true).OrderByDescending(t => t.NodeId).FirstOrDefault();

                    if (tasksState.State == "已完成")
                    {
                        tasksState.NodeId = nodeInfos.Where(n => n.NodeName == "结束" && n.FlowId == item.FlowId.ToString()).FirstOrDefault().NodeId.ToString();
                        if (tasksPro != null)
                        {
                            tasksState.CurrentTime = tasksPro.ApplyTime;
                        }
                    }
                    else
                    {
                        if (tasksState.State == "未完成")
                        {
                            if (tasksPro != null && tasksNew != null)
                            {
                                tasksState.NodeId = tasksNew.NodeId.ToString();
                                tasksState.CurrentTime = tasksPro.ApplyTime;
                            }
                        }
                        if (tasksState.State == "被退回")
                        {
                            tasksState.NodeId = "0";
                            if (tasksPro != null)
                            {
                                tasksState.CurrentTime = tasksPro.ApplyTime;
                            }
                        }
                        if (tasksState.State == "已撤回")
                        {
                            tasksState.NodeId = "0";
                            if (tasksPro != null)
                            {
                                tasksState.CurrentTime = tasksPro.ApplyTime.ToString();
                            }
                        }
                    }
                    dDContext.Entry<TasksState>(tasksState).State = EntityState.Modified;
                }
                dDContext.SaveChanges();
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                return new NewErrorModel()
                {
                    error = new Error(0, ex.Message, "") { },
                };
            }
        }

        #endregion
    }
}
