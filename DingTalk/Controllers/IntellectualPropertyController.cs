using Common.ClassChange;
using Common.DTChange;
using Common.Ionic;
using Common.PDF;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalk.Models.ServerModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 知识产权申报
    /// </summary>
    [RoutePrefix("IntellectualProperty")]
    public class IntellectualPropertyController : ApiController
    {
        /// <summary>
        /// 表单保存
        /// </summary>
        /// <param name="evection"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save([FromBody] IntellectualProperty evection)
        {
            try
            {
                EFHelper<IntellectualProperty> eFHelper = new EFHelper<IntellectualProperty>();
                eFHelper.Add(evection);
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
        /// 表单读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read(string taskId)
        {
            try
            {
                EFHelper<IntellectualProperty> eFHelper = new EFHelper<IntellectualProperty>();
                IntellectualProperty materialRelease = eFHelper.GetListBy(t => t.TaskId == taskId).ToList().First();

                return new NewErrorModel()
                {
                    data = materialRelease,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 表单修改
        /// </summary>
        /// <param name="evection"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object Modify([FromBody] IntellectualProperty evection)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<IntellectualProperty>(evection).State = EntityState.Modified;
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
        /// 打印表单
        /// </summary>
        /// <param name="printAndSendModel"></param>
        /// <returns></returns>
        [Route("Print")]
        [HttpPost]
        public async Task<NewErrorModel> Print(PrintModelCom printAndSendModel)
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
                    string ProjectId = tasks.ProjectId;
                    //判断流程是否已结束
                    List<Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.State == 0 && t.IsSend == false).ToList();
                    if (tasksList.Count > 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "流程未结束！", "") { },
                        };
                    }

                    IntellectualProperty IntellectualPropertyList = context.IntellectualProperty.Where(u => u.TaskId == TaskId).FirstOrDefault();
                    
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
                    ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == ProjectId).First();
                    string ProjectName = projectInfo.ProjectName;
                    string ProjectNo = projectInfo.ProjectId;

                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("申请名称", IntellectualPropertyList.Name);
                    keyValuePairs.Add("申请类别", IntellectualPropertyList.Type);
                    keyValuePairs.Add("申请发明人", IntellectualPropertyList.Inventor);
                    keyValuePairs.Add("发明人或设计人", IntellectualPropertyList.ActualInventor);
                    keyValuePairs.Add("申报名称", IntellectualPropertyList.ActualName);
                    keyValuePairs.Add("申报单位", IntellectualPropertyList.Company);
                    keyValuePairs.Add("申报类别", IntellectualPropertyList.ActualType);

                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    ProjectName, ProjectNo, "2", 300, 650, null, null, null, dtApproveView, keyValuePairs);
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
                        error = new Error(0, result, "") { },
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
