using Common.ClassChange;
using Common.DTChange;
using Common.Ionic;
using Common.PDF;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 收文
    /// </summary>
    [RoutePrefix("Receiving")]
    public class ReceivingManagerController : ApiController
    {
        /// <summary>
        /// 收文表单保存
        /// </summary>
        /// <param name="ReceivingList"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object SaveTable([FromBody] Receiving ReceivingList)
        {
            try
            {
                EFHelper<Receiving> eFHelper = new EFHelper<Receiving>();
                ReceivingList.ReceivingNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                eFHelper.Add(ReceivingList);
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


        /// <summary>
        /// 收文表单修改
        /// </summary>
        /// <param name="ReceivingList"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object ModifyTable([FromBody] Receiving ReceivingList)
        {
            try
            {
                EFHelper<Receiving> eFHelper = new EFHelper<Receiving>();
                Receiving QuaryReceiving = eFHelper.GetListBy(t => t.Id == ReceivingList.Id).First();
                QuaryReceiving.Leadership = ReceivingList.Leadership;
                QuaryReceiving.MainIdea = ReceivingList.MainIdea;
                QuaryReceiving.Suggestion = ReceivingList.Suggestion;
                QuaryReceiving.Leadership = ReceivingList.Leadership;
                if (!string.IsNullOrEmpty(ReceivingList.Review))
                {
                    if (string.IsNullOrEmpty(QuaryReceiving.Review))
                    {
                        QuaryReceiving.Review += ReceivingList.Review;
                    }
                    else
                    {
                        QuaryReceiving.Review += "~" + ReceivingList.Review;
                    }
                }

                if (!string.IsNullOrEmpty(ReceivingList.HandleImplementation))
                {
                    if (string.IsNullOrEmpty(QuaryReceiving.HandleImplementation))
                    {
                        QuaryReceiving.HandleImplementation += ReceivingList.HandleImplementation;
                    }
                    else
                    {
                        QuaryReceiving.HandleImplementation += "~" + ReceivingList.HandleImplementation;
                    }
                }
                eFHelper.Modify(QuaryReceiving);
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
        /// 收文表单读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object ReadTable(string taskId)
        {
            try
            {
                EFHelper<Receiving> eFHelper = new EFHelper<Receiving>();
                List<Receiving> receivings = eFHelper.GetListBy(t => t.TaskId == taskId);
                return new NewErrorModel()
                {
                    error = new Error(0, "读取成功！", "") { },
                    data = receivings
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 获取打印报表
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        [Route("GetReport")]
        [HttpGet]
        public async Task<string> GetReport(string TaskId, string UserId)
        {
            try
            {
                PDFHelper pdfHelper = new PDFHelper();
                using (DDContext context = new DDContext())
                {
                    //获取表单信息
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.NodeId == 0).First();
                    string FlowId = tasks.FlowId.ToString();
                    string ProjectId = tasks.ProjectId;

                    //判断流程是否已结束
                    List<Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.IsSend != true && t.State == 0).ToList();
                    if (tasksList.Count > 0)
                    {
                        return JsonConvert.SerializeObject(
                            new NewErrorModel()
                            {
                                error = new Error(0, "流程尚未结束", "") { },
                            });
                    }
                    
                    List<Receiving> ReceivingList = context.Receiving.Where(u => u.TaskId == TaskId).ToList();
                    DataTable dtSourse = DtLinqOperators.CopyToDataTable(ReceivingList);
                    List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId != 0 && u.NodeName != "结束" && !u.NodeName.Contains("抄送")).ToList();
                    foreach (NodeInfo nodeInfo in NodeInfoList)
                    {
                        if (string.IsNullOrEmpty(nodeInfo.NodePeople))
                        {

                            List<Tasks> taskList = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).ToList();
                            foreach (var task in taskList)
                            {
                                nodeInfo.NodePeople += "  " + task.ApplyMan + "  " + task.ApplyTime;
                            }
                        }
                        else
                        {
                            string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                            nodeInfo.NodePeople = nodeInfo.NodePeople + "  " + ApplyTime;
                        }
                    }

                    DataTable dtApproveView = ClassChangeHelper.ToDataTable(NodeInfoList);
                    string FlowName = context.Flows.Where(f => f.FlowId.ToString() == FlowId).First().FlowName.ToString();

                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    Receiving receiving = context.Receiving.Where(r => r.TaskId == TaskId).SingleOrDefault();
                    dic.Add("申请人", tasks.ApplyMan);
                    dic.Add("流水号", tasks.TaskId.ToString());
                    dic.Add("标题", tasks.Title);
                    dic.Add("收文编号",receiving.ReceivingNo);
                    dic.Add("来文单位", receiving.ReceivingUnit);
                    dic.Add("文件文号", receiving.FileNo);
                    dic.Add("收文时间", receiving.ReceivingTime);
                    dic.Add("主要内容", receiving.MainIdea);
                    dic.Add("拟办意见", receiving.Suggestion);
                    dic.Add("领导阅示", receiving.Leadership);
                    //dic.Add("承办部门阅办情况", receiving.Review.Replace("~", "     "));
                    //dic.Add("办理落实情况", receiving.HandleImplementation.Replace("~", "     "));
                    string path = pdfHelper.GeneratePDF(FlowName, null, tasks.ApplyMan,tasks.Dept,tasks.ApplyTime,
                    "","", "2", 380, 710, null, null, dtSourse, dtApproveView, dic);
                    string RelativePath = "~/UploadFile/PDF/" + Path.GetFileName(path);
                    List<string> newPaths = new List<string>();
                    RelativePath = AppDomain.CurrentDomain.BaseDirectory + RelativePath.Substring(2, RelativePath.Length - 2).Replace('/', '\\');
                    newPaths.Add(RelativePath);
                    string SavePath = string.Format(@"{0}\UploadFile\Ionic\{1}.zip", AppDomain.CurrentDomain.BaseDirectory, "文件阅办单" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //文件压缩打包
                    IonicHelper.CompressMulti(newPaths, SavePath, false);
                    //上传盯盘获取MediaId
                    SavePath = string.Format(@"~\UploadFile\Ionic\{0}", Path.GetFileName(SavePath));
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    var resultUploadMedia = await dingTalkServersController.UploadMedia(SavePath);
                    //推送用户
                    FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                    fileSendModel.UserId = UserId;
                    var result = await dingTalkServersController.SendFileMessage(fileSendModel);

                    return JsonConvert.SerializeObject(
                        new NewErrorModel()
                        {
                            error = new Error(0, "获取成功,请在钉钉工作通知中查收！", "") { },
                            data = result
                        });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
