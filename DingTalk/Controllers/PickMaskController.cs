using Common.ClassChange;
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
    /// 口罩领用
    /// </summary>
    [RoutePrefix("PickMask")]
    public class PickMaskController : ApiController
    {
        /// <summary>
        /// 领料单批量保存
        /// </summary>
        /// <param name="pickList"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save([FromBody] List<PickMask> pickList)
        {
            try
            {
                EFHelper<PickMask> eFHelper = new EFHelper<PickMask>();
                foreach (var pick in pickList)
                {
                    eFHelper.Add(pick);
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

        /// <summary>
        /// 领料单读取
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read(string taskId)
        {
            try
            {
                EFHelper<PickMask> eFHelper = new EFHelper<PickMask>();
                List<PickMask> pickList = eFHelper.GetListBy(t => t.TaskId == taskId).ToList();
                return new NewErrorModel()
                {
                    count = pickList.Count,
                    data = pickList,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打印盖章
        /// </summary>
        /// <param name="printAndSendModel"></param>
        /// <returns></returns>
        [Route("GetPrintPDF")]
        [HttpPost]
        public async Task<NewErrorModel> GetPrintPDF([FromBody]PrintAndSendModel printAndSendModel)
        {
            try
            {
                string TaskId = printAndSendModel.TaskId;
                string UserId = printAndSendModel.UserId;
                PDFHelper pdfHelper = new PDFHelper();
                using (DDContext context = new DDContext())
                {
                    //获取表单信息
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.NodeId == 0).First();
                    string FlowId = tasks.FlowId.ToString();

                    TasksState tasksState = context.TasksState.Where(t => t.TaskId == TaskId).FirstOrDefault();
                    if (tasksState.State != "已完成")
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, string.Format("流程{0}！", tasksState.State), "") { },
                        };
                    }
                    PickMask ct = context.PickMask.Where(u => u.TaskId == TaskId).FirstOrDefault();

                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                    keyValuePairs.Add("使用日期", ct.BeginTime+"-"+ct.EndTime);
                    keyValuePairs.Add("领用人数", ct.PickPeopleCount.ToString());
                    keyValuePairs.Add("领用口罩数量", ct.PickCount.ToString());
                    keyValuePairs.Add("备注", ct.Remark);
                 
                    List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId != 0 && u.IsSend != true && u.NodeName != "结束").ToList();
                    foreach (NodeInfo nodeInfo in NodeInfoList)
                    {
                        if (string.IsNullOrEmpty(nodeInfo.NodePeople))
                        {
                            string strNodePeople = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyMan;
                            string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                            nodeInfo.NodePeople = strNodePeople + "  " + ApplyTime;
                        }
                        else
                        {
                            string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                            nodeInfo.NodePeople = nodeInfo.NodePeople + "  " + ApplyTime;
                        }
                    }
                    DataTable dtApproveView = ClassChangeHelper.ToDataTable(NodeInfoList);
                    string FlowName = context.Flows.Where(f => f.FlowId.ToString() == FlowId).First().FlowName.ToString();


                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    null, null, "2", 300, 650, null, null, null, dtApproveView, keyValuePairs);
                    string RelativePath = "~/UploadFile/PDF/" + Path.GetFileName(path);

                    List<string> newPaths = new List<string>();
                    RelativePath = AppDomain.CurrentDomain.BaseDirectory + RelativePath.Substring(2, RelativePath.Length - 2).Replace('/', '\\');
                    newPaths.Add(RelativePath);
                    string SavePath = string.Format(@"{0}\UploadFile\Ionic\{1}.zip", AppDomain.CurrentDomain.BaseDirectory, FlowName + DateTime.Now.ToString("yyyyMMddHHmmss"));
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

                    return new NewErrorModel()
                    {
                        data = result,
                        error = new Error(0, "打印盖章成功！", "") { },
                    };
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
