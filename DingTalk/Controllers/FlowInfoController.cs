using Common.JsonHelper;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DingTalk.Controllers
{
    public class FlowInfoController : Controller
    {
        // GET: FlowInfo
        public ActionResult Index()
        {
            return View();
        }

        #region 流程创建与提交、退回
        /// <summary>
        /// 流程创建接口(Post)
        /// </summary>
        /// 测试数据：/DrawingUpload/CreateTaskInfo
        //var FlowTest =
        //[{ "ApplyMan": "小威", "ApplyManId": "1209662535974958", "ApplyTime": null, "IsEnable": 1, "FlowId": 6, "NodeId": 0, "Remark": "6666", "IsSend": false, "State": 1, "ImageUrl": null, "FileUrl": null, "Title": "大型石板材扫描仪", "ProjectId": "2016ZL051", "IsPost": false, "OldImageUrl": null, "OldFileUrl": null, "IsBack": null },
        //{ "ApplyMan": "蔡兴桐", "ApplyManId": "073110326032521796", "ApplyTime": null, "IsEnable": 1, "FlowId": 6, "NodeId": 1, "Remark": null, "IsSend": false, "State": 0, "ImageUrl": null, "FileUrl": null, "Title": "大型石板材扫描仪", "ProjectId": "2016ZL051", "IsPost": false, "OldImageUrl": null, "OldFileUrl": null, "IsBack": null },
        //{ "ApplyMan": "张鹏辉", "ApplyManId": "100328051024695354", "ApplyTime": null, "IsEnable": 1, "FlowId": 6, "NodeId": 2, "Remark": null, "IsSend": false, "State": 0, "ImageUrl": null, "FileUrl": null, "Title": "大型石板材扫描仪", "ProjectId": "2016ZL051", "IsPost": false, "OldImageUrl": null, "OldFileUrl": null, "IsBack": null }
        //];
        /// <returns>errorCode = 0 成功创建  Content(返回创建的TaskId)</returns>
        [HttpPost]
        public async Task<string> CreateTaskInfo()
        {
            try
            {
                StreamReader sr = new StreamReader(Request.InputStream);
                var stream = sr.ReadToEnd();
                if (string.IsNullOrEmpty(stream))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "提交的数据不能为空！"
                    });
                }
                else
                {
                    List<Tasks> taskList = JsonHelper.JsonToObject<List<Tasks>>(stream);
                    FlowInfoServer flowInfoServer = new FlowInfoServer();
                    int TaskId = flowInfoServer.FindMaxTaskId();

                    #region 新版
                    using (DDContext context = new DDContext())
                    {
                        int? FlowId = taskList[0].FlowId;
                        foreach (var tasks in taskList)
                        {
                            tasks.TaskId = TaskId;
                            if (tasks.IsSend == true)
                            {
                                tasks.State = 0; tasks.IsEnable = 1;
                                //抄送推送
                                SentCommonMsg(tasks.ApplyManId.ToString(), string.Format("您有一条抄送的流程(流水号:{0})，请及时登入研究院信息管理系统进行审批。", TaskId), taskList[0].ApplyMan, taskList[0].Remark, null);
                            }
                            else
                            {
                                //State 1 表示流程已审批 0 表示未审批  IsEnable 1 表示流程生效  0 未生效
                                if (tasks.NodeId == 0)
                                {
                                    tasks.State = 1;
                                    tasks.IsEnable = 1;
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

                                        await SendOaMsgNew(tasks.FlowId, PeopleIdList[i].ToString(), TaskId.ToString(), tasksApplyMan.ApplyMan, tasksApplyMan.Remark, context);
                                        Thread.Sleep(500);

                                    }
                                    return JsonConvert.SerializeObject(new ErrorModel
                                    {
                                        errorCode = 0,
                                        errorMessage = "创建成功！",
                                        Content = TaskId.ToString()
                                    });
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
                                        //推送OA消息
                                        //SentCommonMsg(PeopleIdList[i].ToString(), string.Format("您有一条待审批的流程(流水号:{0})，请及时登入研究院信息管理系统进行审批。", TaskId), tasksApplyMan.ApplyMan, tasksApplyMan.Remark, null);

                                        await SendOaMsgNew(tasks.FlowId, PeopleIdList[i].ToString(), TaskId.ToString(), tasksApplyMan.ApplyMan, tasksApplyMan.Remark, context);
                                        Thread.Sleep(500);

                                        //特殊处理(暂时)
                                        if (tasks.FlowId.ToString() == "6")
                                        {
                                            NodeInfo nodeInfoCurrent = context.NodeInfo.Where(n => n.FlowId.ToString() == "6" && n.NodeId.ToString() == "2").FirstOrDefault();
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
                                                FlowId = 6
                                            };
                                            context.Tasks.Add(taskCurrent);
                                            context.SaveChanges();
                                            await SendOaMsgNew(tasks.FlowId, taskCurrent.ApplyManId, TaskId.ToString(), tasksApplyMan.ApplyMan, tasksApplyMan.Remark, context);
                                            Thread.Sleep(500);

                                            return JsonConvert.SerializeObject(new ErrorModel
                                            {
                                                errorCode = 0,
                                                errorMessage = "创建成功！",
                                                Content = TaskId.ToString()
                                            });
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
                                    && tasksChoosed.FlowId.ToString() != "26" && tasksChoosed.FlowId.ToString() != "27")  //临时处理
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

                                        await SendOaMsgNew(tasks.FlowId, tasksChoosed.ApplyManId.ToString(), TaskId.ToString(), tasksApplyMan.ApplyMan, tasksApplyMan.Remark, context);
                                        Thread.Sleep(500);
                                        //推送OA消息
                                        //SentCommonMsg(tasksChoosed.ApplyManId.ToString(), string.Format("您有一条待审批的流程(流水号:{0})，请及时登入研究院信息管理系统进行审批。", TaskId), tasksApplyMan.ApplyMan, tasksApplyMan.Remark, null);

                                        return JsonConvert.SerializeObject(new ErrorModel
                                        {
                                            errorCode = 0,
                                            errorMessage = "创建成功！",
                                            Content = TaskId.ToString()
                                        });
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "创建成功！",
                        Content = TaskId.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }


        /// <summary>
        /// 流程提交接口(Approve)
        /// </summary>
        /// 测试数据：/DrawingUpload/SubmitTaskInfo
        //var FlowTestApprove =
        //    var FlowTestApprove = [{ Id: "348", TaskId: "100", "ApplyMan": "小威", "ApplyManId": "1209662535974958", "ApplyTime": null, "IsEnable": 1, "FlowId": 6, "NodeId": 1, "Remark": "6666", "IsSend": false, "State": 1, "ImageUrl": null, "FileUrl": null, "Title": "大型石板材扫描仪", "ProjectId": "2016ZL051", "IsPost": false, "OldImageUrl": null, "OldFileUrl": null, "IsBack": null },
        //{ TaskId: "100", "ApplyMan": "蔡兴桐", "ApplyManId": "073110326032521796", "ApplyTime": null, "IsEnable": 1, "FlowId": 6, "NodeId": 3, "Remark": null, "IsSend": false, "State": 0, "ImageUrl": null, "FileUrl": null, "Title": "大型石板材扫描仪", "ProjectId": "2016ZL051", "IsPost": false, "OldImageUrl": null, "OldFileUrl": null, "IsBack": null },
        //{ TaskId: "100", "ApplyMan": "张鹏辉", "ApplyManId": "100328051024695354", "ApplyTime": null, "IsEnable": 1, "FlowId": 6, "NodeId": 3, "Remark": null, "IsSend": false, "State": 0, "ImageUrl": null, "FileUrl": null, "Title": "大型石板材扫描仪", "ProjectId": "2016ZL051", "IsPost": false, "OldImageUrl": null, "OldFileUrl": null, "IsBack": null }
        //];
        /// <returns>errorCode = 0 成功创建  Content(返回创建的TaskId)</returns>
        [HttpPost]
        public async Task<string> SubmitTaskInfo()
        {
            try
            {
                StreamReader sr = new StreamReader(Request.InputStream);
                var stream = sr.ReadToEnd();
                if (string.IsNullOrEmpty(stream))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "提交的数据不能为空！"
                    });
                }
                else
                {
                    List<Tasks> taskList = JsonHelper.JsonToObject<List<Tasks>>(stream);
                    //获取申请人提交表单信息
                    FlowInfoServer fServer = new FlowInfoServer();
                    Tasks taskNew = fServer.GetApplyManFormInfo(taskList[0].TaskId.ToString());

                    if (taskList.Count > 1)  //如果有选人
                    {
                        using (DDContext contexts = new DDContext())
                        {
                            foreach (var task in taskList)
                            {
                                if (taskList.IndexOf(task) > 0)
                                {
                                    if (task.IsSend == true)
                                    {
                                        //推送抄送消息
                                        SentCommonMsg(task.ApplyManId,
                                        string.Format("您有一条抄送信息(流水号:{0})，请及时登入研究院信息管理系统进行查阅。", task.TaskId),
                                        taskNew.ApplyMan, taskNew.Remark, null);
                                        task.IsEnable = 1;
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
                            if (dic["NodeName"] == "结束")
                            {
                                //修改流程状态
                                tasks.IsPost = false;
                                tasks.State = 1;
                                tasks.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                context.Entry(tasks).State = EntityState.Modified;
                                context.SaveChanges();

                                Tasks tasksApplyMan = context.Tasks.Where(t => t.TaskId.ToString() == tasks.TaskId.ToString()
                                  && t.NodeId == 0).First();
                                tasksApplyMan.ImageUrl = tasks.ImageUrl;
                                tasksApplyMan.OldImageUrl = tasks.OldImageUrl;
                                tasksApplyMan.ImageUrl = tasks.ImageUrl;
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


                                //推送发起人
                                SentCommonMsg(taskNew.ApplyManId,
                                string.Format("您发起的审批的流程(流水号:{0})，已审批完成请知晓。", tasks.TaskId),
                                taskNew.ApplyMan, taskNew.Remark, null);

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

                                    Tasks tasksApplyMan = context.Tasks.Where(t => t.TaskId.ToString() == tasks.TaskId.ToString()
                                    && t.NodeId == 0).First();
                                    tasksApplyMan.ImageUrl = tasks.ImageUrl;
                                    tasksApplyMan.OldImageUrl = tasks.OldImageUrl;
                                    tasksApplyMan.ImageUrl = tasks.ImageUrl;
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
                                    //当前节点所有任务流已完成
                                    if (fServer.GetTasksByNotFinished(tasks.TaskId.ToString(), tasks.NodeId.ToString()).Count == 0)
                                    {
                                        //推送OA消息(寻人)
                                        //SentCommonMsg(dic["PeopleId"].ToString(),
                                        //string.Format("您有一条待审批的流程(流水号:{0})，请及时登入研究院信息管理系统进行审批。", tasks.TaskId),
                                        //taskNew.ApplyMan, taskNew.Remark, null);

                                        await SendOaMsgNew(tasks.FlowId, dic["PeopleId"].ToString(), tasks.TaskId.ToString(), taskNew.ApplyMan, taskNew.Remark, context);
                                        Thread.Sleep(500);
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
                                                await SendOaMsgNew(tasks.FlowId, PeopleId, tasks.TaskId.ToString(), taskNew.ApplyMan, taskNew.Remark, context);
                                                Thread.Sleep(500);
                                                //     SentCommonMsg(PeopleId,
                                                //string.Format("您有一条待审批的流程(流水号:{0})，请及时登入研究院信息管理系统进行审批。", tasks.TaskId),
                                                //taskNew.ApplyMan, taskNew.Remark, null);
                                            }
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "创建成功"
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// 流程退回
        /// </summary>
        /// <returns></returns>
        /// 测试：/FlowInfo/FlowBack
        /// var FlowBackList={"Id":157,"TaskId":4,"ApplyMan":"蔡兴桐","ApplyManId":"manager5312","ApplyTime":null,"IsEnable":1,"FlowId":6,"NodeId":1,"Remark":null,"IsSend":false,"State":0,"ImageUrl":"","FileUrl":null,"Title":"图纸上传2018-04-23 16:41","ProjectId":"2018-04-23 16:41","IsPost":false,"OldImageUrl":"","OldFileUrl":null,"IsBack":true,"BackNodeId":0}

        [HttpPost]
        public async Task<string> FlowBack()
        {
            try
            {
                StreamReader sr = new StreamReader(Request.InputStream);
                var stream = sr.ReadToEnd();
                if (string.IsNullOrEmpty(stream))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "提交的数据不能为空！"
                    });
                }
                else
                {
                    Tasks tasks = JsonHelper.JsonToObject<Tasks>(stream);
                    using (DDContext context = new DDContext())
                    {
                        if (tasks.NodeId == 0)  //撤回
                        {
                            Tasks taskNew = context.Tasks.Find(tasks.Id);
                            taskNew.IsBacked = true;
                            context.Entry<Tasks>(taskNew).State = EntityState.Modified;
                            context.SaveChanges();
                            //找到当前未审核的人员修改状态
                            List<Tasks> taskList = context.Tasks.Where(t => t.TaskId.ToString() == tasks.TaskId.ToString() && t.State == 0 && t.IsSend != true).ToList();
                            foreach (var task in taskList)
                            {
                                context.Tasks.Remove(task);
                                context.SaveChanges();
                            }
                            return JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 0,
                                errorMessage = "撤回成功",
                                Content = tasks.TaskId.ToString()
                            });
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
                                await SendOaMsgNew(tasks.FlowId, taskApplyMan.ApplyManId, tasks.TaskId.ToString(), taskApplyMan.ApplyMan, tasks.Remark, context);
                                Thread.Sleep(500);
                            }
                            else
                            {
                                string PeopleId = context.NodeInfo.SingleOrDefault
                                    (u => u.NodeId.ToString() == newBackNodeId && u.FlowId == tasks.FlowId.ToString()).PeopleId;
                                string NodePeople = context.NodeInfo.SingleOrDefault
                                    (u => u.NodeId.ToString() == newBackNodeId && u.FlowId == tasks.FlowId.ToString()).NodePeople;
                                if (string.IsNullOrEmpty(PeopleId))
                                {
                                    return JsonConvert.SerializeObject(new ErrorModel
                                    {
                                        errorCode = 1,
                                        errorMessage = "退回节点尚未配置人员"
                                    });
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
                        return JsonConvert.SerializeObject(new ErrorModel
                        {
                            errorCode = 0,
                            errorMessage = "退回成功",
                            Content = tasks.TaskId.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }

        #endregion

        #region 审批过程节点数据读取

        /// <summary>
        /// 审批过程节点数据读取接口
        /// </summary>
        /// <param name="TaskId">任务Id(发起页面无需传此字段)</param>
        /// <param name="FlowId">流程Id</param>
        /// <returns></returns>
        /// 测试数据  /FlowInfo/GetFlowProgress?TaskId=1&FlowId=6
        public string GetFlowProgress(string TaskId, string FlowId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (string.IsNullOrEmpty(FlowId))
                    {
                        return JsonConvert.SerializeObject(new ErrorModel
                        {
                            errorCode = 2,
                            errorMessage = "FlowId不能为空！"
                        });
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
                        return JsonConvert.SerializeObject(QuaryList);
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
                        return JsonConvert.SerializeObject(QuaryList);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }

        #endregion

        #region 寻人、选人与抄送
        /// <summary>
        /// 寻人接口（默认）
        /// </summary>
        /// <param name="OldTaskId">任务Id</param>
        /// <param name="FlowId">流程Id</param>
        /// <param name="NodeId">节点Id</param>
        /// <param name="IsNext">是否找下一节点的人(默认True)</param>
        /// <param name="IsSend">抄送标识(默认False)</param>
        /// <returns>NodeName节点名称  NodePeople节点审批人 PeopleId审批人Id</returns>
        /// 测试数据: FlowInfo/FindNextPeople?OldTaskId=1&IsNext=true&IsSend=False&FlowId=6&NodeId=0&ApplyManId=0935455445756597

        [HttpGet]
        public Dictionary<string, string> FindNextPeople(string FlowId, string ApplyManId, bool IsNext = true,
            bool? IsSend = false, int? OldTaskId = 0, int? NodeId = -1)
        {
            using (DDContext context = new DDContext())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string FindNodeId = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == NodeId).PreNodeId;
                string NodeName = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId.ToString() == FindNodeId).NodeName;
                dic.Add("NodeName", NodeName);
                if (NodeName == "结束")
                {
                    return dic;
                }
                string PeopleId = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId.ToString() == FindNodeId).PeopleId;
                string NodePeople = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId.ToString() == FindNodeId).NodePeople;
                bool? IsNeedChose = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId.ToString() == FindNodeId).IsNeedChose;
                //判断流程多人提交(当前步骤)
                bool? IsAllAllow = context.NodeInfo.Where(u => u.NodeId == NodeId && u.FlowId == FlowId).First().IsAllAllow;
                dic.Add("NodePeople", NodePeople);
                dic.Add("PeopleId", PeopleId);

                if (NodeName == "抄送")
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
                        Task.ApplyMan, Task.Remark, null);

                        context.Tasks.Add(newTask);
                        context.SaveChanges();
                    }
                    if (IsSend == true)
                    {
                        //return FindNextPeople(FlowId, ApplyManId, true, false, OldTaskId, NodeId + 2);
                        return null;
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
                            Task.ApplyMan, Task.Remark, null);
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


        /// <summary>
        /// 选人及抄送接口(多人Post)
        /// </summary>
        /// <returns>errorCode = 0 成功 </returns>
        /// 测试数据：FlowInfo/ChoseOrSend
        //   var PeopleList = [{ "Id": 335.0, "TaskId": 4, "ApplyMan": "蔡兴桐", "ApplyManId": "073110326032521796", "ApplyTime": null, "IsEnable": 1, "FlowId": 6, "NodeId": 1, "Remark": null, "IsSend": true, "State": 0, "ImageUrl": null, "FileUrl": null, "Title": "大型石板材扫描仪", "ProjectId": "2016ZL051", "IsPost": false, "OldImageUrl": null, "OldFileUrl": null, "IsBack": null }, { "Id": 336.0, "TaskId": 4, "ApplyMan": "黄龙贤", "ApplyManId": "020821466340361583", "ApplyTime": null, "IsEnable": 1, "FlowId": 6, "NodeId": 3, "Remark": "44444444444444", "IsSend": false, "State": 0, "ImageUrl": null, "FileUrl": null, "Title": "大型石板材扫描仪", "ProjectId": "2016ZL051", "IsPost": false, "OldImageUrl": null, "OldFileUrl": null, "IsBack": null }];
        [HttpPost]
        public string ChoseOrSend()
        {
            try
            {
                StreamReader reader = new StreamReader(Request.InputStream);
                string List = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(List))
                {
                    List<Tasks> ListTasks = new List<Tasks>();
                    ListTasks = JsonHelper.JsonToObject<List<Tasks>>(List);
                    using (DDContext context = new DDContext())
                    {
                        foreach (Tasks tasks in ListTasks)
                        {
                            if (tasks.State != 1)
                            {
                                context.Tasks.Add(tasks);
                            }
                        }
                        context.SaveChanges();
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "选人、抄送成功"
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "参数未传递"
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }

        #endregion

        #region 修改抄送状态为已阅

        /// <summary>
        /// 修改抄送状态为已阅
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="UserId">用户Id</param>
        /// <returns>{"errorCode":0,"errorMessage":"修改成功","Content":null,"IsError":false}</returns>
        /// 测试数据：/FlowInfo/ChangeSendState?TaskId=24&UserId=1209662535974958
        public string ChangeSendState(string TaskId, string UserId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    Tasks task = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.ApplyManId == UserId
                     && t.IsSend == true && t.State == 0).OrderByDescending(u => u.Id).First();

                    task.State = 1;
                    task.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    context.Entry<Tasks>(task).State = EntityState.Modified;
                    context.SaveChanges();
                    return JsonConvert.SerializeObject(new ErrorModel()
                    {
                        errorCode = 0,
                        errorMessage = "修改成功"
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                });
            }
        }

        #endregion

        #region 流程类别及数据读取
        /// <summary>
        /// 流程界面信息读取接口
        /// </summary>
        /// <param name="id">用户Id，用于判断权限(预留，暂时不做)</param>
        /// <returns></returns>
        /// 测试数据： /FlowInfo/LoadFlowInfo?id=123
        [HttpGet]
        public string LoadFlowInfo(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    FlowInfoServer flowInfoServer = new FlowInfoServer();
                    return JsonConvert.SerializeObject(flowInfoServer.GetFlowInfo());
                }
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = "id不能为空"
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }


        /// <summary>
        /// 流程节点信息获取接口
        /// </summary>
        /// <param name="FlowId">流程Id</param>
        /// <param name="NodeId">节点Id</param>
        /// <returns></returns>
        /// 测试数据: FlowInfo/GetNodeInfo?FlowId=6&NodeId=0
        public string GetNodeInfo(string FlowId, string NodeId)
        {
            try
            {
                if (FlowId != null)
                {
                    using (DDContext context = new DDContext())
                    {
                        if (string.IsNullOrEmpty(NodeId))
                        {
                            var NodeInfo = context.NodeInfo.Where(u => u.FlowId == FlowId);
                            return JsonConvert.SerializeObject(NodeInfo);
                        }
                        else
                        {
                            var NodeInfo = context.NodeInfo.Where(u => u.NodeId.ToString() == NodeId && u.FlowId == FlowId);
                            return JsonConvert.SerializeObject(NodeInfo);
                        }
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "参数未传递"
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }


        /// <summary>
        /// 流程界面分类信息读取接口
        /// </summary>
        /// <param name="id">用户Id，用于判断权限(预留，暂时不做)</param>
        /// <returns></returns>
        /// 测试数据： /FlowInfo/LoadFlowSort?id=123
        [HttpGet]
        public string LoadFlowSort(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    FlowInfoServer flowInfoServer = new FlowInfoServer();
                    return JsonConvert.SerializeObject(flowInfoServer.GetFlowSort());
                }
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = "id不能为空"
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }
        #endregion

        #region 左侧审批菜单栏状态读取

        /// <summary>
        /// 左侧审批状态(数量)数据读取
        /// </summary>
        /// <param name="ApplyManId">用户名Id</param>
        /// <returns>返回待审批的、我发起的、抄送我的数量</returns>
        /// 测试数据 /FlowInfo/GetFlowStateCounts?ApplyManId=123456
        [HttpGet]
        public string GetFlowStateCounts(string ApplyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    //待审批的
                    int iApprove = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == false && u.State == 0 && u.IsPost == false).Count();
                    //我发起的
                    int iMyPost = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId == 0 && u.IsSend == false && u.State == 1 && u.IsPost == true).Count();
                    //抄送我的
                    int iSendMy = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == true && u.State == 0 && u.IsPost == false).Count();
                    Dictionary<string, int> dic = new Dictionary<string, int>();
                    dic.Add("ApproveCount", iApprove);
                    dic.Add("MyPostCount", iMyPost);
                    dic.Add("SendMyCount", iSendMy);

                    return JsonConvert.SerializeObject(dic);
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                });
            }
        }



        /// <summary>
        /// 左侧审批状态详细数据读取
        /// </summary>
        /// <param name="Index">(Index=0:待我审批 1:我已审批 2:我发起的 3:抄送我的)</param>
        /// <param name="ApplyManId">用户名Id</param>
        /// <param name="IsSupportMobile">是否是手机端调用接口(默认 false)</param>
        /// <returns> State 0 未完成 1 已完成 2 被退回</returns>
        /// 测试数据： /FlowInfo/GetFlowStateDetail?Index=1&ApplyManId=083452125733424957
        [HttpGet]
        public string GetFlowStateDetail(int Index, string ApplyManId, bool IsSupportMobile = false)
        {
            try
            {
                List<int?> ListTasks = new List<int?>();
                using (DDContext context = new DDContext())
                {
                    switch (Index)
                    {
                        case 0:
                            //待审批的
                            ListTasks = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == false && u.State == 0 && u.IsPost != true && u.ApplyTime == null).OrderByDescending(u => u.TaskId).Select(u => u.TaskId).ToList();
                            return Quary(context, ListTasks, ApplyManId, IsSupportMobile);
                        case 1:
                            //我已审批
                            ListTasks = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == false && u.State == 1 && u.IsPost != true && u.ApplyTime != null).OrderByDescending(u => u.TaskId).Select(u => u.TaskId).ToList();
                            return Quary(context, ListTasks, ApplyManId, IsSupportMobile);
                        case 2:
                            //我发起的
                            ListTasks = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId == 0 && u.IsSend == false && u.State == 1 && u.IsPost == true && u.ApplyTime != null).OrderByDescending(u => u.TaskId).Select(u => u.TaskId).ToList();
                            return Quary(context, ListTasks, ApplyManId, IsSupportMobile);
                        case 3:
                            //抄送我的
                            ListTasks = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == true && u.IsPost != true).OrderByDescending(u => u.TaskId).Select(u => u.TaskId).ToList();
                            return Quary(context, ListTasks, ApplyManId, IsSupportMobile);
                        default:
                            return JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 1,
                                errorMessage = "参数不正确"
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }

        public string Quary(DDContext context, List<int?> ListTasks, string ApplyManId, bool IsMobile)
        {
            FlowInfoServer flowInfoServer = new FlowInfoServer();
            List<object> listQuary = new List<object>();
            List<object> listQuaryPro = new List<object>();
            List<Tasks> ListTask = context.Tasks.ToList();
            List<Flows> ListFlows = context.Flows.ToList();
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
                            where t.NodeId == 0 && t.TaskId == TaskId
                            && (IsMobile == true ? f.IsSupportMobile == true : 1 == 1)
                            select new
                            {
                                Id = t.Id + 1,
                                TaskId = t.TaskId,
                                NodeId = NodeId,
                                FlowId = t.FlowId,
                                FlowName = f.FlowName,
                                ApplyMan = t.ApplyMan,
                                ApplyManId = t.ApplyManId,
                                ApplyTime = t.ApplyTime,
                                Title = t.Title,
                                State = GetTasksState(t.TaskId.ToString(), ListTask),
                                IsBack = t.IsBacked,
                                IsSupportMobile = f.IsSupportMobile
                            };

                if (query.Count() > 0)
                {
                    listQuary.Add(query);
                }
            }

            //foreach (var item in listQuary)
            //{
            //    string TaskId = item.GetType().GetProperty("TaskId").GetValue(item).ToString();

            //}

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
            return JsonConvert.SerializeObject(TaskFlowModelListQuery);
        }

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

        #region 审批意见数据读取

        /// <summary>
        /// 审批意见数据读取
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="FlowId">流程Id</param>
        /// <returns></returns>
        /// 测试数据： /FlowInfo/GetSign?TaskId=100&FlowId=6
        [HttpGet]
        public string GetSign(string TaskId, string FlowId)
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
                                        ChoseNodeId = n.ChoseNodeId
                                    };
                        return JsonConvert.SerializeObject(Quary);
                    }
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId).ToList();
                        List<Tasks> TaskList = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.IsBacked != false).ToList();
                        var Quary = from n in NodeInfoList
                                    join t in TaskList
                                    on n.NodeId equals t.NodeId
                                    into temp
                                    from tt in temp.DefaultIfEmpty()
                                    orderby n.NodeId
                                    select new
                                    {
                                        NodeId = n.NodeId,
                                        NodeName = n.NodeName,
                                        IsBack = tt == null ? false : tt.IsBacked,
                                        ApplyMan = tt == null ? n.NodePeople : tt.ApplyMan,
                                        ApplyTime = tt == null ? "" : tt.ApplyTime,
                                        Remark = tt == null ? "" : tt.Remark,
                                        IsSend = tt == null ? n.IsSend : tt.IsSend,
                                        ApplyManId = tt == null ? "" : tt.ApplyManId
                                    };
                        Quary = Quary.OrderBy(q => q.NodeId).ThenByDescending(h => h.ApplyTime);
                        return JsonConvert.SerializeObject(Quary);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }

        #endregion

        #region 流程节点人员配置

        /// <summary>
        /// 流程节点人员配置
        /// </summary>
        /// <returns></returns>
        /// 测试数据：/FlowInfo/UpdateNodeInfo
        ///var NodeInfoList = [{
        //   "Id":"2",
        //   "NodeId":"1",
        //   "FlowId":"6",
        //   "NodeName":"负责人审核",
        //   "NodePeople":"蔡兴桐",
        //   "PeopleId":"1",
        //   "PreNodeId":"2",
        //   "IsAllAllow":"1",
        //   "Condition":"1",
        //   "IsBack":false,
        //   "IsNeedChose":false,
        //   "IsSend":false
        //},{
        //   "Id":"3",
        //   "NodeId":"2",
        //   "FlowId":"6",
        //   "NodeName":"部门审核",
        //   "NodePeople":"渣渣辉",
        //   "PeopleId":"1",
        //   "PreNodeId":"3",
        //   "IsAllAllow":"1",
        //   "Condition":"1",
        //   "IsBack":false,
        //   "IsNeedChose":false,
        //   "IsSend":false
        //}]
        [HttpPost]
        public string UpdateNodeInfo()
        {
            try
            {
                StreamReader reader = new StreamReader(Request.InputStream);
                string List = reader.ReadToEnd();
                if (string.IsNullOrEmpty(List))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "请传递参数"
                    });
                }
                else
                {
                    List<NodeInfo> ListNodeInfo = new List<NodeInfo>();
                    ListNodeInfo = JsonHelper.JsonToObject<List<NodeInfo>>(List);
                    using (DDContext context = new DDContext())
                    {
                        foreach (NodeInfo nodeInfo in ListNodeInfo)
                        {
                            context.Entry<NodeInfo>(nodeInfo).State = EntityState.Modified;
                        }
                        context.SaveChanges();
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "保存成功"
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }

        #endregion

        #region 审批页面通用数据读取

        /// <summary>
        /// 审批页面通用数据读取
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="ApplyManId">用户Id</param>
        /// <returns></returns>
        /// 测试数据 /FlowInfo/GetApproveInfo?TaskId=7
        public string GetApproveInfo(string TaskId, string ApplyManId)
        {
            try
            {
                if (string.IsNullOrEmpty(TaskId))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "请传递参数"
                    });
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        Tasks task = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.ApplyManId == ApplyManId && u.IsEnable == 1).OrderByDescending(t => t.Id).First();
                        Tasks taskOld = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.NodeId == 0).First();
                        taskOld.Id = task.Id;
                        taskOld.NodeId = task.NodeId;
                        return JsonConvert.SerializeObject(taskOld);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }

        #endregion

        #region 测试数据读取

        /// <summary>
        /// 测试数据读取
        /// </summary>
        /// <returns></returns>
        /// 测试数据：/FlowInfo/GetTestInfo
        public string GetTestInfo()
        {
            using (DDContext context = new DDContext())
            {
                ProjectInfo purchaseDown = context.ProjectInfo.First();
                return JsonConvert.SerializeObject(purchaseDown);
            }
        }

        #endregion

        #region 系统已配置人员信息读取

        /// <summary>
        /// 系统已配置人员信息读取
        /// </summary>
        /// <returns></returns>
        /// 测试数据 /FlowInfo/GetUserInfo
        public string GetUserInfo()
        {
            using (DDContext context = new DDContext())
            {
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


                //var Quary = context.Database.SqlQuery<NodeInfo>
                //       ("select peopleid,NodePeople from NodeInfo  where NodePeople is  not null and charindex(',',NodePeople)=0   group by peopleid,NodePeople");

                return JsonConvert.SerializeObject(QuaryPro);

            }
        }

        #endregion

        #region 钉钉SDK再封装接口

        /// <summary>
        /// 发送OA消息
        /// </summary>
        /// 测试数据 /FlowInfo/TestSentOaMsg
        [HttpGet]
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


        public async Task<object> SendOaMsgNew(int? FlowId, string ApplyManId, string TaskId, string ApplyMan, string Remark, DDContext dDContext)
        {
            DingTalkServersController dingTalkServersController = new DingTalkServersController();
            //推送OA消息
            if (dDContext.Flows.Where(f => f.FlowId.ToString() == FlowId.ToString()).First().IsSupportMobile == true)
            {
                return await dingTalkServersController.sendOaMessage(ApplyManId,
                        string.Format("您有一条待审批的流程(流水号:{0})，请及点击进入研究院信息管理系统进行审批。", TaskId),
                        ApplyMan, "eapp://page/approve/approve");
            }
            else
            {
                SentCommonMsg(ApplyManId, string.Format("您有一条待审批的流程(流水号:{0})，请及点击进入研究院信息管理系统进行审批。", TaskId), ApplyMan, Remark, null);
                return dingTalkServersController.sendOaMessage("测试",
                       string.Format("您有一条待审批的流程(流水号:{0})，请及点击进入研究院信息管理系统进行审批。", TaskId),
                       ApplyMan, "eapp://page/approve/approve");
            }
        }

        /// <summary>
        /// 发送普通消息
        /// </summary>
        /// 测试数据 /FlowInfo/TestSentCommonMsg
        [HttpGet]
        public string SentCommonMsg(string SendPeoPleId, string Title, string ApplyMan,
            string Content, string Url)
        {
            TopSDKTest top = new TopSDKTest();
            OATextModel oaTextModel = new OATextModel();
            oaTextModel.head = new head
            {
                bgcolor = "FFBBBBBB",
                text = "您有一条待审批的流程，请登入OA系统审批"
            };
            oaTextModel.body = new body
            {
                form = new form[] {
                    new form{ key="申请人：",value=ApplyMan},
                    new form{ key="申请时间：",value=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                },
                //rich = new rich
                //{
                //    num = "15.6",
                //    unit = "元"
                //},
                title = Title,//"您有一条待审批的流程，请登入OA系统审批",
                content = Content//"我要请假~~~~123456",
                                 //image = "@lADOADmaWMzazQKA",
                                 //file_count = "3",
            };
            oaTextModel.message_url = Url;
            return top.SendOaMessage(SendPeoPleId, oaTextModel);
        }

        /// <summary>
        /// 测试发送数量上限接口
        /// </summary>
        /// <returns></returns>
        /// 测试数据：/FlowInfo/Test
        public int Test()
        {
            int j = 0;
            for (int i = 0; i < 10000; i++)
            {
                j++;
                SentCommonMsg("073110326032521796", "您有一条待审批的流程，请登入OA系统审批", "古天乐", string.Format("我要请假1~~~~{0}", i), "https://www.cnblogs.com/BraveBoy/p/7417972.html");
            }
            return j;
        }
        #endregion

    }
}