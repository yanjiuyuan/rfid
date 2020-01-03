using Common.ClassChange;
using Common.DTChange;
using Common.Excel;
using Common.Ionic;
using Common.PDF;
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
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 图纸审批
    /// </summary>
    [RoutePrefix("DrawingUploadNew")]
    public class DrawingUploadNewController : ApiController
    {
        /// <summary>
        /// BOM表信息查询
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPurchase")]
        public NewErrorModel GetPurchase(string TaskId)
        {
            try
            {
                DDContext context = new DDContext();

                var PurchaseList = context.Purchase.Where(u => u.TaskId == TaskId);
                if (PurchaseList != null)
                {
                    return new NewErrorModel()
                    {
                        data = PurchaseList,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        data = PurchaseList,
                        error = new Error(1, "未查到数据！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// BOM表信息载入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("LoadPurchase")]
        public NewErrorModel LoadPurchase([FromBody]List<Purchase> listPurchase)
        {
            try
            {
                foreach (Purchase item in listPurchase)
                {
                    using (DDContext context = new DDContext())
                    {
                        context.Purchase.Add(item);
                        context.SaveChanges();
                    }
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
        /// 打印表单数据、盖章、推送
        /// </summary>
        /// <param name="printAndSendModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintAndSend")]
        public async Task<NewErrorModel> PrintAndSend(PrintAndSendModel printAndSendModel)
        {
            try
            {
                string TaskId = printAndSendModel.TaskId;
                string UserId = printAndSendModel.UserId;
                string OldPath = printAndSendModel.OldPath;
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
                        return new NewErrorModel()
                        {
                            error = new Error(1, "流程未结束！", "") { },
                        };

                    }

                    List<Purchase> PurchaseList = context.Purchase.Where(u => u.TaskId == TaskId).ToList();

                    var SelectPurchaseList = from p in PurchaseList
                                             select new
                                             {
                                                 p.DrawingNo,
                                                 p.Name,
                                                 p.Count,
                                                 p.MaterialScience,
                                                 p.Unit,
                                                 p.SingleWeight,
                                                 p.AllWeight,
                                                 p.Sorts,
                                                 p.NeedTime,
                                                 p.Mark
                                             };

                    DataTable dtSourse = DtLinqOperators.CopyToDataTable(SelectPurchaseList);
                    //ClassChangeHelper.ToDataTable(SelectPurchaseList);
                    List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId != 0 && u.NodeName != "结束" && u.IsSend != true).ToList();
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
                    //绘制BOM表单PDF
                    List<string> contentList = new List<string>()
                        {
                            "序号","代号","名称","数量","材料","单位","单重","总重","类别","需用日期","备注"
                        };

                    float[] contentWithList = new float[]
                    {
                        50, 80, 80, 30, 60, 30, 60, 60, 60 , 60, 60
                    };

                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    ProjectName, ProjectNo, "1", 380, 710, contentList, contentWithList, dtSourse, dtApproveView, null);
                    string RelativePath = "~/UploadFile/PDF/" + Path.GetFileName(path);

                    string[] Paths = OldPath.Split(',');

                    List<string> newPaths = new List<string>();
                    RelativePath = AppDomain.CurrentDomain.BaseDirectory + RelativePath.Substring(2, RelativePath.Length - 2).Replace('/', '\\');
                    newPaths.Add(RelativePath);
                    foreach (string pathChild in Paths)
                    {
                        string AbPath = AppDomain.CurrentDomain.BaseDirectory + pathChild.Substring(2, pathChild.Length - 2);
                        //PDF盖章 保存路径
                        newPaths.Add(
                            pdfHelper.PDFWatermark(AbPath,
                        string.Format(@"{0}\UploadFile\PDFPrint\{1}",
                        AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(pathChild)),
                        string.Format(@"{0}\Content\images\受控章.png", AppDomain.CurrentDomain.BaseDirectory),
                        100, 100)
                        );
                    }
                    string SavePath = string.Format(@"{0}\UploadFile\Ionic\{1}.zip", AppDomain.CurrentDomain.BaseDirectory, "图纸审核" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //文件压缩打包
                    IonicHelper.CompressMulti(newPaths, SavePath, false);

                    //上传盯盘获取MediaId
                    DingTalkServersController otherController = new DingTalkServersController();             
                    SavePath = string.Format(@"~\UploadFile\Ionic\{0}", Path.GetFileName(SavePath));
                    var resultUploadMedia = await otherController.UploadMedia(SavePath);
                    //推送用户
                    FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                    fileSendModel.UserId = UserId;
                    var result = await otherController.SendFileMessage(fileSendModel);
   
                    return new NewErrorModel()
                    {
                        error = new Error(0, "已推送至钉钉！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取Excel Bom报表
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="applyManId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetExcelReport")]
        public async Task<NewErrorModel> GetExcelReport(string taskId, string applyManId)
        {
            try
            {
                //EFHelper<Purchase> eFHelper = new EFHelper<Purchase>();
                //System.Linq.Expressions.Expression<Func<Purchase, bool>> where = p => p.TaskId == taskId;
                //List<Purchase> purchases = eFHelper.GetListBy(where);
                using (DDContext context = new DDContext())
                {
                    var SelectPurchaseList = from p in context.Purchase
                                             where p.TaskId == taskId
                                             select new
                                             {
                                                 p.TaskId,
                                                 p.DrawingNo,
                                                 p.Name,
                                                 p.Count,
                                                 p.MaterialScience,
                                                 p.Unit,
                                                 p.SingleWeight,
                                                 p.AllWeight,
                                                 p.Sorts,
                                                 p.NeedTime,
                                                 p.Mark
                                             };

                    DataTable dtpurchaseTables = DtLinqOperators.CopyToDataTable(SelectPurchaseList);
                    string path = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet/图纸BOM导出模板.xlsx");
                    string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\图纸BOM数据" + time + ".xlsx";
                    System.IO.File.Copy(path, newPath);
                    if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, 0, 1))
                    {
                        DingTalkServersController dingTalkServersController = new DingTalkServersController();
                        //上盯盘
                        var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/图纸BOM数据" + time + ".xlsx");
                        //推送用户
                        FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                        fileSendModel.UserId = applyManId;
                        var result = await dingTalkServersController.SendFileMessage(fileSendModel);

                        return new NewErrorModel()
                        {
                            error = new Error(0, "已推送至钉钉！", "") { },
                        };
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "文件有误！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
