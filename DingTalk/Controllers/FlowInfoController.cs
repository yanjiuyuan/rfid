using Common.JsonHelper;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.Models;
using DingTalk.Models.DbModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        #region 流程创建与提交
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
        //"State":"1"  // 1表示已审核，0表示未审核
        //"ImageUrl","图片路径",
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
                        context.Tasks.Add(tasks);
                        context.SaveChanges();
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
        //var FlowTest = {
        //"TaskId":"1",
        //"ApplyMan": "蔡兴桐",
        //"ApplyManId": "蔡兴桐",
        //"NodeId":"1",
        //"ApplyTime": "2018-04-12 14:40",
        //"IsEnable": "1",
        //"FlowId": "6",
        //"Remark":"审核通过",
        //"IsSend":"False",
        //"State":"0" 
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
                        context.Tasks.Add(tasks);
                        context.SaveChanges();
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
        /// 测试数据: FlowInfo/FindNextPeople?OldTaskId=1&IsNext=true&IsSend=False&FlowId=6&NodeId=1
        [HttpGet]
        public string FindNextPeople(string FlowId, bool IsNext = true, bool IsSend = false, int OldTaskId = 0, int NodeId = 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(FlowId) && NodeId != 0 && OldTaskId != 0)
                {
                    using (DDContext context = new DDContext())
                    {
                        string NodeName = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).NodeName;
                        string PeopleId = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).PeopleId;
                        string NodePeople = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).NodePeople;

                        //List<string> ListNodeName = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).Select(u => u.NodeName).ToList();
                        //List<string> ListPeopleId = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).Select(u => u.PeopleId).ToList();
                        //List<string> ListNodePeople = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).Select(u => u.NodePeople).ToList();

                        string[] ListNodeName = NodeName.Split(',');
                        string[] ListPeopleId = PeopleId.Split(',');
                        string[] ListNodePeople = NodePeople.Split(',');

                        for (int i = 0; i < ListPeopleId.Length; i++)
                        {
                            //保存任务流
                            Tasks newTask = new Tasks()
                            {
                                TaskId = OldTaskId,
                                ApplyMan = ListNodePeople[i],
                                //ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                                IsEnable = 1,
                                NodeId = NodeId,
                                FlowId = Int32.Parse(FlowId),
                                IsSend = IsSend,
                                ApplyManId = ListPeopleId[i],
                                State = 0 //0 表示未审核 1表示已审核
                            };
                            context.Tasks.Add(newTask);
                        }
                        context.SaveChanges();

                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("NodeName", NodeName);
                        dic.Add("NodePeople", NodePeople);
                        dic.Add("PeopleId", PeopleId);
                        return JsonConvert.SerializeObject(dic);
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
                    errorMessage = ex.Message,
                    Content = "未找到数据"
                });
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
        // "TaskId":"1",
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
                    int iApprove = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 1 && u.IsSend == false && u.State == 0).Count();
                    //我发起的
                    int iMyPost = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == false && u.State == 0).Count();
                    //抄送我的
                    int iSendMy = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == true && u.State == 0).Count();

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
                List<Tasks> ListTask = new List<Tasks>();
                using (DDContext context = new DDContext())
                {
                    switch (Index)
                    {
                        case 0:
                            //待审批的
                            ListTask = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 1 && u.IsSend == false && u.State == 0).ToList();
                            return JsonConvert.SerializeObject(ListTask);
                        case 1:
                            //我已审批
                            ListTask = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 1 && u.IsSend == false && u.State == 1).ToList();
                            return JsonConvert.SerializeObject(ListTask); ;
                        case 2:
                            //我发起的
                            ListTask = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == false && u.State == 0).ToList();
                            return JsonConvert.SerializeObject(ListTask);
                        case 3:
                            //抄送我的
                            ListTask = context.Tasks.Where(u => u.ApplyManId == ApplyManId && u.IsEnable == 1 && u.NodeId != 0 && u.IsSend == true && u.State == 0).ToList();
                            return JsonConvert.SerializeObject(ListTask);
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

        #endregion

        #region 审批意见数据读取

        /// <summary>
        /// 审批意见数据读取
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        /// 测试数据： /FlowInfo/GetSign?TaskId=1
        [HttpGet]
        public string GetSign(string TaskId)
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
                        List<Tasks> Tasks = context.Tasks.Where(u => u.TaskId.ToString() == TaskId).ToList();
                        List<NodeInfo> NodeInfo = context.NodeInfo.ToList();
                        var Quary = from t in Tasks
                                    join n in NodeInfo
                                    on t.NodeId equals n.NodeId
                                    select new
                                    {
                                        NodeName = n.NodeName,
                                        ApplyMan = t.ApplyMan,
                                        ApplyTime = t.ApplyTime,
                                        Remark = t.Remark,
                                        IsSend = n.IsSend
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

        #region MyRegion

        #endregion
    }
}