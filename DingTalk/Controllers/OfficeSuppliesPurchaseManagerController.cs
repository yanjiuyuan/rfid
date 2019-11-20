using Common.ClassChange;
using Common.DTChange;
using Common.Ionic;
using Common.PDF;
using DingTalk.Bussiness.FlowInfo;
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
    /// 办公用品采购
    /// </summary>
    [RoutePrefix("OfficeSuppliesPurchase")]
    public class OfficeSuppliesPurchaseManagerController : ApiController
    {
        /// <summary>
        /// 办公用品申请数据读取
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [Route("GetTable")]
        [HttpGet]
        public object GetOfficeSuppliesTable(DateTime startTime, DateTime endTime)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskId("1").Where(t => t.NodeId == 0).ToList();

                    List<Tasks> tasksSucced = new List<Tasks>();
                    FlowInfoServer flowInfoServer = new FlowInfoServer();
                    foreach (var item in tasks)
                    {
                        if (flowInfoServer.GetTasksState(item.TaskId.ToString()) == "已完成")
                        {
                            tasksSucced.Add(item);
                        }
                    }

                    List<OfficeModels> officeModeldList = new List<OfficeModels>();
                    foreach (var task in tasksSucced)
                    {
                        //if (task.NodeId == 0)
                        //{
                        if (DateTime.Parse(task.ApplyTime) > startTime && DateTime.Parse(task.ApplyTime) < endTime)
                        {
                            officeModeldList.Add(new OfficeModels()
                            {
                                taskId = task.TaskId.ToString(),
                                applyMan = task.ApplyMan,
                                dept = task.Dept
                            });
                        }
                        //}
                    }
                    List<OfficeSupplies> officeSupplies = context.OfficeSupplies.Where(o => o.IsDelete != true).ToList();

                    var Quary = from t in officeModeldList
                                join o in officeSupplies
                                on t.taskId equals o.TaskId
                                select new
                                {
                                    t.taskId,
                                    t.applyMan,
                                    t.dept,
                                    o.CodeNo,
                                    o.Count,
                                    o.Unit,
                                    o.ExpectPrice,
                                    o.Id,
                                    o.Mark,
                                    o.Name,
                                    o.Price,
                                    o.Purpose,
                                    o.Standard,
                                    o.UrgentDate,
                                };
                    return Quary;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 办公用品采购表单保存
        /// </summary>
        /// <param name="officeSuppliesPurchaseTableList"></param>
        /// <returns></returns>
        [Route("SaveTable")]
        [HttpPost]
        public string SaveTable([FromBody] List<OfficeSuppliesPurchase> officeSuppliesPurchaseTableList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (OfficeSuppliesPurchase officeSuppliesPurchase in officeSuppliesPurchaseTableList)
                    {
                        context.OfficeSuppliesPurchase.Add(officeSuppliesPurchase);
                        context.SaveChanges();
                    }
                }
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 0,
                    errorMessage = "保存成功"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 办公用品采购表单读取
        /// </summary>
        /// <returns></returns>
        [Route("ReadTable")]
        [HttpGet]
        public string ReadTable(string TaskId)
        {
            try
            {
                List<OfficeSuppliesPurchase> OfficeSuppliesPurchaseTableList = new List<OfficeSuppliesPurchase>();
                using (DDContext context = new DDContext())
                {
                    OfficeSuppliesPurchaseTableList = context.OfficeSuppliesPurchase.Where
                         (p => p.TaskId == TaskId && p.IsDelete != true).ToList();
                }
                return JsonConvert.SerializeObject(OfficeSuppliesPurchaseTableList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 办公用品采购表单修改
        /// </summary>
        /// <param name="officeSuppliesPurchaseTableList"></param>
        /// <returns></returns>
        [Route("ModifyTable")]
        [HttpPost]
        public string ModifyTable([FromBody] List<OfficeSuppliesPurchase> officeSuppliesPurchaseTableList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (OfficeSuppliesPurchase officeSuppliesPurchase in officeSuppliesPurchaseTableList)
                    {
                        context.Entry<OfficeSuppliesPurchase>(officeSuppliesPurchase).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 0,
                    errorMessage = "修改成功"
                });
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
        [HttpPost]
        [Route("PrintPDF")]
        public async Task<object> PrintAndSend([FromBody]PrintModel printAndSendModel)
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
                        return new NewErrorModel()
                        {
                            error = new Error(1, "流程尚未结束", "") { },
                        };
                    }

                    List<OfficeSuppliesPurchase> OfficeSuppliesPurchaseList = context.OfficeSuppliesPurchase.Where(u => u.TaskId == TaskId && u.IsDelete != true).ToList();

                    var SelectGoDownList = from g in OfficeSuppliesPurchaseList
                                           select new
                                           {
                                               g.CodeNo,
                                               g.Name,
                                               g.Standard,
                                               g.Unit,
                                               g.Count,
                                               g.Price
                                           };
                    DataTable dtSourse = DtLinqOperators.CopyToDataTable(SelectGoDownList);
                    //ClassChangeHelper.ToDataTable(SelectPurchaseList);
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
                            Tasks tasksNew = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First();
                            nodeInfo.NodePeople = nodeInfo.NodePeople + "  " + tasksNew.ApplyTime + "   " + tasksNew.Remark;
                        }
                    }
                    DataTable dtApproveView = ClassChangeHelper.ToDataTable(NodeInfoList);
                    string FlowName = context.Flows.Where(f => f.FlowId.ToString() == FlowId).First().FlowName.ToString();

                    //绘制BOM表单PDF
                    List<string> contentList = new List<string>()
                        {
                            "序号","物料编码","物料名称","规格型号","单位","数量","预计单价"
                        };

                    float[] contentWithList = new float[]
                    {
                        50, 90,90,90,60,60,60
                    };

                    float sum = 0;

                    foreach (var item in OfficeSuppliesPurchaseList)
                    {
                        sum += (float.Parse(item.Price) * float.Parse(item.Count));
                    }
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("总价", sum.ToString());

                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    null, null, "2", 300, 650, contentList, contentWithList, dtSourse, dtApproveView, null, keyValuePairs);
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



    public class OfficeModels
    {
        public string taskId { get; set; }
        public string applyMan { get; set; }
        public string dept { get; set; }
    }
}
