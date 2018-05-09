﻿using Common.JsonHelper;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.Models;
using DingTalk.Models.DbModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
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
        //var FlowTest = {
        //"ApplyMan": "蔡兴桐",
        //"ApplyManId": "蔡兴桐Id",
        //"NodeId":"0",
        //"ApplyTime": "2018-04-10 14:40",
        //"IsEnable": "1",
        //"FlowId": "6",
        //"Remark":"意见",
        //"IsSend":"False",
        //"State":"1"  
        //"OldImageUrl","原图片路径",
        //"ImageUrl","图片路径",
        //"OldFileUrl","原文件路径",
        //"FileUrl","文件路径",
        //"Title","标题",
        //"ProjectId","项目号",
        //}
        /// <returns>errorCode = 0 成功创建  Content(返回创建的TaskId)</returns>
        [HttpPost]
        public string CreateTaskInfo()
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
                    FlowInfoServer flowInfoServer = new FlowInfoServer();
                    int TaskId = flowInfoServer.FindMaxTaskId();
                    tasks.TaskId = TaskId;
                    using (DDContext context = new DDContext())
                    {
                        tasks.IsPost = true;
                        context.Tasks.Add(tasks);
                        context.SaveChanges();
                        //寻人推送
                        Dictionary<string, string> dic =
                        FindNextPeople(tasks.FlowId.ToString(), tasks.ApplyMan, true, false, tasks.TaskId, 0);
                        //推送OA消息
                        SentCommonMsg(dic["PeopleId"].ToString(), string.Format("您有一条待审批的流程(流水号:{0})，请及时登入研究院信息管理系统进行审批。", tasks.TaskId), tasks.ApplyMan, tasks.Remark, null);
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "创建成功！",
                        Content = tasks.TaskId.ToString()
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
        // var FlowTestApprove = {
        // "Id":"137",
        // "TaskId": "3",
        // "ApplyMan": "蔡兴桐",
        // "ApplyManId": "manager5312",
        // "NodeId": "1",
        // "ApplyTime": "2018-04-12 14:40",
        // "IsEnable": "1",
        // "FlowId": "6",
        // "Remark": "审核通过",
        // "IsSend": false,
        // "State": "0",
        // "OldImageUrl":"原图片路径",
        // "ImageUrl":"图片路径",
        // "OldFileUrl":"原文件路径",
        // "FileUrl":"文件路径",
        // "Title":"标题",
        // "ProjectId":"项目号"
        //}
        /// <returns>errorCode = 0 成功创建  Content(返回创建的TaskId)</returns>
        [HttpPost]
        public string SubmitTaskInfo()
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
                        //修改流程状态
                        tasks.State = 1;
                        tasks.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        context.Entry(tasks).State = EntityState.Modified;
                        context.SaveChanges();
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic = FindNextPeople(tasks.FlowId.ToString(), tasks.ApplyManId, true, tasks.IsSend,
                            tasks.TaskId, tasks.NodeId);
                        if (dic["NodeName"] == "结束")
                        {
                            JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 0,
                                errorMessage = "流程结束",
                                Content = tasks.TaskId.ToString()
                            });
                        }
                        else
                        {
                            //获取申请人提交表单信息
                            FlowInfoServer fServer = new FlowInfoServer();
                            Tasks taskNew = fServer.GetApplyManFormInfo(tasks.TaskId.ToString());
                            //推送OA消息
                            SentCommonMsg(dic["PeopleId"].ToString(),
                            string.Format("您有一条待审批的流程(流水号:{0})，请及时登入研究院信息管理系统进行审批。", taskNew.TaskId),
                            taskNew.ApplyMan, taskNew.Remark, null);
                        }
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "创建成功",
                        Content = tasks.TaskId.ToString()
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
        public string FlowBack()
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
                        //修改流程状态
                        tasks.IsBack = true;
                        tasks.State = 1;
                        tasks.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        context.Entry(tasks).State = EntityState.Modified;
                        context.SaveChanges();

                        //查找退回节点Id
                        string newBackNodeId = context.NodeInfo.Where
                            (u => u.FlowId == tasks.FlowId.ToString() && u.NodeId == tasks.NodeId)
                            .Select(u => u.BackNodeId).First();

                        //根据退回节点Id找人
                        if (newBackNodeId == "0")  //退回节点为发起人
                        {
                            Tasks newTask = new Tasks();
                            newTask = context.Tasks.Where(u => u.TaskId == tasks.TaskId && u.NodeId == 0).First();
                            newTask.IsBack = false;
                            newTask.ApplyTime = null;
                            newTask.State = 0;
                            newTask.Remark = null;
                            newTask.IsPost = true;
                            context.Tasks.Add(newTask);
                            context.SaveChanges();
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
                                newTask.IsBack = false;
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
                            });
                        return JsonConvert.SerializeObject(QuaryList);
                    }
                    else
                    {
                        var TasksList = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.FlowId.ToString() == FlowId);
                        var NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId);
                        var QuaryList = from a in TasksList
                                        join b in NodeInfoList
                                        on a.NodeId equals b.NodeId
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
                string NodeName = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).NodeName;
                string PeopleId = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).PeopleId;
                string NodePeople = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).NodePeople;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("NodeName", NodeName);
                dic.Add("NodePeople", NodePeople);
                dic.Add("PeopleId", PeopleId);
                if (NodeName == "结束")
                {
                    return dic;
                }
                else
                {
                    //List<string> ListNodeName = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).Select(u => u.NodeName).ToList();
                    //List<string> ListPeopleId = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).Select(u => u.PeopleId).ToList();
                    //List<string> ListNodePeople = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).Select(u => u.NodePeople).ToList();

                    //判断流程多人提交(当前步骤)
                    bool? IsAllAllow = context.NodeInfo.Where(u => u.NodeId == NodeId && u.FlowId == FlowId).First().IsAllAllow;
                    if (IsAllAllow == true)   //流程配置为所有人同时同意后提交
                    {
                        //查找当前是否还有人未审核
                        List<Tasks> ListTask = context.Tasks.Where(u => u.TaskId == OldTaskId && u.FlowId.ToString() == FlowId && u.NodeId == NodeId && u.NodeId != 0 && u.ApplyManId != ApplyManId && u.State == 1).ToList();
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
                                    //ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
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
                                //ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
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
        /// 选人及抄送接口(多人Post)（自选）
        /// </summary>
        /// <returns>errorCode = 0 成功 </returns>
        /// 测试数据：FlowInfo/ChoseOrSend
        ///var PeopleList = [{
        //   "TaskId":"1",
        //   "FlowId":"6",
        //   "NodeId":"1",
        //   "ApplyMan":"蔡兴桐",
        //   "ApplyManId":"123456",
        //   "IsEnable":"1",
        //   "IsSend":"True",
        //   "State":"0"
        //},{
        //   "TaskId":"1",
        //   "FlowId":"6",
        //   "NodeId":"1",
        //   "ApplyMan":"龙贤",
        //   "ApplyManId":"龙贤Id",
        //   "IsEnable":"1",
        //   "IsSend":"Flase",
        //   "State":"0"
        //}]
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
                        errorMessage = "选人或抄送成功"
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
                    return flowInfoServer.GetFlowInfo();
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
        public string GetNodeInfo(string FlowId, int NodeId = 0)
        {
            try
            {
                if (FlowId != null)
                {
                    using (DDContext context = new DDContext())
                    {
                        var NodeInfo = context.NodeInfo.Where(u => u.NodeId == NodeId && u.FlowId == FlowId);
                        return JsonConvert.SerializeObject(NodeInfo);
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
                    return flowInfoServer.GetFlowSort();
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
        /// <returns></returns>
        /// 测试数据： /FlowInfo/GetFlowStateDetail?Index=0&ApplyManId=蔡兴桐Id
        [HttpGet]
        public string GetFlowStateDetail(int Index, string ApplyManId)
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
                            ListTasks = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == false && u.State == 0 && u.IsPost != true && u.ApplyTime == null).Select(u => u.TaskId).ToList();
                            return Quary(context, ListTasks, ApplyManId);
                        case 1:
                            //我已审批
                            ListTasks = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == false && u.State == 1 && u.IsPost != true && u.ApplyTime != null).Select(u => u.TaskId).ToList();
                            return Quary(context, ListTasks, ApplyManId);
                        case 2:
                            //我发起的
                            ListTasks = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId == 0 && u.IsSend == false && u.State == 1 && u.IsPost == true && u.ApplyTime != null).Select(u => u.TaskId).ToList();
                            return Quary(context, ListTasks, ApplyManId);
                        case 3:
                            //抄送我的
                            ListTasks = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == true && u.State == 0 && u.IsPost != true && u.ApplyTime == null).Select(u => u.TaskId).ToList();
                            return Quary(context, ListTasks, ApplyManId);
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

        public string Quary(DDContext context, List<int?> ListTasks, string ApplyManId)
        {
            List<Object> listQuary = new List<object>();
            foreach (int TaskId in ListTasks)
            {
                int? NodeId = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.TaskId == TaskId).Select(u => u.NodeId).First();
                List<Tasks> ListTask = context.Tasks.ToList();
                List<Flows> ListFlows = context.Flows.ToList();
                listQuary.Add(from t in ListTask
                              join f in ListFlows
                              on t.FlowId.ToString() equals f.FlowId.ToString()
                              where t.NodeId == 0 && t.TaskId == TaskId
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
                                  Title = t.Title
                              });
            }
            return JsonConvert.SerializeObject(listQuary);
        }


        #endregion

        #region 审批意见数据读取

        /// <summary>
        /// 审批意见数据读取
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="FlowId">流程Id</param>
        /// <returns></returns>
        /// 测试数据： /FlowInfo/GetSign?TaskId=3&FlowId=6
        [HttpGet]
        public string GetSign(string TaskId, string FlowId)
        {
            try
            {
                if (string.IsNullOrEmpty(TaskId))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 2,
                        errorMessage = "TaskId不能为空"
                    });
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        string ApplyMan = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.IsPost == true && u.State == 1).First().ApplyMan;
                        List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId).ToList();
                        List<Tasks> TaskList = context.Tasks.Where(u => u.TaskId.ToString() == TaskId).ToList();
                        var Quary = from n in NodeInfoList
                                    join t in TaskList
                                    on n.NodeId equals t.NodeId
                                    into temp
                                    from tt in temp.DefaultIfEmpty()
                                    select new
                                    {
                                        NodeId = n.NodeId,
                                        NodeName = n.NodeName,
                                        IsBack = tt == null ? false : tt.IsBack,
                                        ApplyMan = (n.NodeName == "申请人发起") ? ApplyMan : n.NodePeople,
                                        ApplyTime = tt == null ? "" : tt.ApplyTime,
                                        Remark = tt == null ? "" : tt.Remark,
                                        IsSend = tt == null ? false : tt.IsSend
                                    };

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
                        Tasks task = context.Tasks.Where(u => u.TaskId.ToString() == TaskId && u.ApplyManId == ApplyManId).First();
                        return JsonConvert.SerializeObject(task);
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
                List<PurchaseDown> purchaseDown = context.PurchaseDown.ToList();
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
                var NodeInfoList = context.NodeInfo.Where(n => n.PeopleId != null && n.NodePeople != null);
                var Quary = from n in NodeInfoList
                            select new
                            {
                                n.PeopleId,
                                n.NodePeople
                            };
                return JsonConvert.SerializeObject(Quary);
            }
        }

        #endregion

        #region 钉钉SDK再封装接口

        /// <summary>
        /// 发送OA消息
        /// </summary>
        /// 测试数据 /FlowInfo/TestSentOaMsg
        public string TestSentOaMsg()
        {
            TopSDKTest top = new TopSDKTest();
            OATextModel oaTextModel = new OATextModel();
            oaTextModel.message_url = "http://dingtalk.com";
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
                content = "一大段文字",
                image = "@lADOADmaWMzazQKA",
                file_count = "3",
                author = "李四"
            };
            return top.SendOaMessage("020821466340361583", oaTextModel);
        }

        /// <summary>
        /// 发送普通消息
        /// </summary>
        /// 测试数据 /FlowInfo/TestSentCommonMsg
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