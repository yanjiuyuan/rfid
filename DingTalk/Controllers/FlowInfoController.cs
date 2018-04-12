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
        /// 流程创建接口(Post)
        /// </summary>
        /// 测试数据：/DrawingUpload/LoadTaskInfo
        //var FlowTest = {
        //"ApplyMan": "蔡兴桐",
        //"NodeId":"0",
        //"ApplyTime": "2018-04-10 14:40",
        //"IsEnable": "1",
        //"FlowId": "6"
        //}
        /// <returns>errorCode = 0 成功创建  Content(返回创建的TaskId)</returns>
        [HttpPost]
        public string LoadTaskInfo()
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
        /// 寻人接口
        /// </summary>
        /// <param name="FlowId">流程Id</param>
        /// <param name="NodeId">节点Id</param>
        /// <param name="IsNext">是否找下一节点的人</param>
        /// <returns></returns>
        /// 测试数据: FlowInfo/FindNextPeople?IsNext=true&FlowId=6&NodeId=1
        public string FindNextPeople(bool IsNext, string FlowId, int NodeId = 0)
        {
            try
            {
                if (FlowId != null && NodeId != 0)
                {
                    using (DDContext context = new DDContext())
                    {
                        string PeopleId = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).PeopleId;
                        string NodePeople = context.NodeInfo.SingleOrDefault(u => u.FlowId == FlowId && u.NodeId == (IsNext ? NodeId + 1 : NodeId)).NodePeople;
                        Dictionary<string, string> dic = new Dictionary<string, string>();
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
        /// 流程节点信息获取接口
        /// </summary>
        /// <param name="FlowId">流程Id</param>
        /// <param name="NodeId">节点Id</param>
        /// <returns></returns>
        /// 测试数据: FlowInfo/GetNodeInfo?FlowId=6&NodeId=1
        public string GetNodeInfo(string FlowId, int NodeId = 0)
        {
            try
            {
                if (FlowId != null && NodeId != 0)
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
    }
}