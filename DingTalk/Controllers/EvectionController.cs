using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Common.PDF;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using Common.Ionic;
using Common.ClassChange;
using DingTalk.Bussiness.FlowInfo;
using System.Web;
using Common.Excel;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 外出
    /// </summary>
    [RoutePrefix("Evection")]
    public class EvectionController : ApiController
    {
        /// <summary>
        /// 出差表单保存
        /// </summary>
        /// <param name="evection"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] Evection evection)
        {
            try
            {
                EFHelper<Evection> eFHelper = new EFHelper<Evection>();
                if (string.IsNullOrEmpty(evection.EvectionMan))
                {
                    evection.EvectionMan = "";
                    evection.EvectionManId = "";
                }
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
        /// 出差表单读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string taskId)
        {
            try
            {
                EFHelper<Evection> eFHelper = new EFHelper<Evection>();
                Evection evection = eFHelper.GetListBy(t => t.TaskId == taskId).ToList().First();

                return new NewErrorModel()
                {
                    data = evection,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 出差表单修改
        /// </summary>
        /// <param name="evection"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object Modify([FromBody] Evection evection)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<Evection>(evection).State = EntityState.Modified;
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
        /// 外出报表导出与查询
        /// </summary>
        /// <param name="dateStartTime">开始时间</param>
        /// <param name="dateEndTime">结束时间</param>
        /// <param name="userId">用户Id</param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route("GetReport")]
        [HttpPost]
        public async Task<NewErrorModel> GetReport(DateTime dateStartTime, DateTime dateEndTime, string userId, string key = "")
        {
            try
            {
                using (DDContext context = new DDContext())
                {

                    List<Evection> evectionsPro = new List<Evection>();
                    List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskIdByFlowName("外出申请").Where(t => t.NodeId == 0).ToList() ;
                    List<Evection> evections = context.Evection.ToList();
                    List<Evection> evectionPro = new List<Evection> ();
                    List<Evection> evectionProP = new List<Evection>();
                    DateTime dateTime = new DateTime();
                    foreach (var item in evections)
                    {
                        if (DateTime.TryParse(item.BeginTime, out dateTime))
                        {
                            if (DateTime.Parse(item.BeginTime) > dateStartTime && DateTime.Parse(item.EndTime) < dateEndTime)
                            {
                                evectionPro.Add(item);
                            }
                        }
                    }
                    foreach (var task in tasks)
                    {
                        foreach (var evection in evectionPro)
                        {
                            if (task.TaskId.ToString() == evection.TaskId)
                            {
                                //借用字段
                                evection.Duration = task.ApplyMan;
                                evectionProP.Add(evection);
                            }
                        }
                    }

                    DataTable dtpurchaseTables = ClassChangeHelper.ToDataTable(evectionProP, new List<string>() {
                    "Id","EvectionManId","LocationPlace"
                      });
                    string path = HttpContext.Current.Server.MapPath(string.Format("~/UploadFile/Excel/Templet/{0}.xlsx", "外出数据导出模板"));
                    string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\" + "外出数据" + time + ".xlsx";
                    File.Copy(path, newPath,true);

                    if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, 0, 1))
                    {
                        DingTalkServersController dingTalkServersController = new DingTalkServersController();
                        //上盯盘 
                        var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/" + "\\" + "外出数据"+ time + ".xlsx");
                        //推送用户
                        FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                        fileSendModel.UserId = userId;
                        var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                        //删除文件
                        File.Delete(newPath);
                    }
                    return new NewErrorModel()
                    {
                        error = new Error(0, "推送成功!", "") { },
                    };
                }
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
        public async Task<object> GetPrintPDF([FromBody]PrintAndSendModel printAndSendModel)
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
                    //判断流程是否已结束
                    List<Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.State == 0 && t.IsSend == false).ToList();
                    if (tasksList.Count > 0)
                    {
                        return JsonConvert.SerializeObject(new NewErrorModel
                        {
                            error = new Error(1, "流程尚未结束", "") { },
                        });
                    }

                    Evection ct = context.Evection.Where(u => u.TaskId == TaskId).FirstOrDefault();

                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                   
                    keyValuePairs.Add("外出人员", tasks.ApplyMan +  (string.IsNullOrEmpty(ct.EvectionMan)?"":","+ ct.EvectionMan));
                    keyValuePairs.Add("外出地点", ct.Place);
                    keyValuePairs.Add("开始时间", ct.BeginTime);
                    keyValuePairs.Add("结束时间", ct.EndTime);
                    keyValuePairs.Add("外出事由", ct.Content);
                    keyValuePairs.Add("时长", ct.Duration);

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
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
