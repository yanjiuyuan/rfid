using Common.DTChange;
using Common.Excel;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 图纸下单管理
    /// </summary>
    [RoutePrefix("PurchaseOrder")]
    public class PurchaseOrderController : ApiController
    {
        /// <summary>
        /// 图纸下单数据查询
        /// </summary>
        /// <param name="key">关键字(项目名、标题、申请人、流水号)</param>
        /// <returns></returns>
        [Route("Quary")]
        [HttpGet]
        public object Quary(string key)
        {
            try
            {
                DDContext context = new DDContext();
                List<object> list = new List<object>();
                List<Tasks> tasksList = FlowInfoServer.ReturnUnFinishedTaskId("6").Where(t => t.NodeId == 0).ToList();
                List<Tasks> tasksListNew = new List<Tasks>();
              List <TasksState> tasksState = context.TasksState.ToList();

                foreach (var item in tasksList)
                {
                    if (tasksState.Where(t => t.TaskId == item.TaskId.ToString()).FirstOrDefault().State == "已完成")
                    {
                        tasksListNew.Add(item);
                    }
                }


                var quaryList = context.Tasks.Where(t => (t.TaskId.ToString().Contains(key)
                 || t.ProjectName.Contains(key) || t.Title.Contains(key)
                   || t.ApplyMan.Contains(key)) && t.NodeId == 0 && t.FlowId == 6).OrderBy(t => t.TaskId).Select(t => new TasksPurcahse
                   {
                       Id = t.Id,
                       TaskId = t.TaskId,
                       ApplyMan = t.ApplyMan,
                       ApplyManId = t.ApplyManId,
                       ApplyTime = t.ApplyTime,
                       Dept = t.Dept,
                       FilePDFUrl = t.FilePDFUrl,
                       FileUrl = t.FileUrl,
                       FlowId = t.FlowId,
                       ImageUrl = t.ImageUrl,
                       IsBacked = t.IsBacked,
                       IsEnable = t.IsEnable,
                       IsPost = t.IsPost,
                       IsSend = t.IsSend,
                       MediaId = t.MediaId,
                       MediaIdPDF = t.MediaIdPDF,
                       NodeId = t.NodeId,
                       OldFilePDFUrl = t.OldFilePDFUrl,
                       OldFileUrl = t.OldFileUrl,
                       OldImageUrl = t.OldImageUrl,
                       PdfState = t.PdfState,
                       ProjectId = t.ProjectId,
                       ProjectName = t.ProjectName,
                       Title = t.Title,
                       PurchaseList = context.Purchase.Where(p => p.TaskId == t.TaskId.ToString()).ToList()
                   });

                foreach (var item in quaryList)
                {
                    foreach (var tasks in tasksListNew)
                    {
                        if (item.TaskId == tasks.TaskId)
                        {
                            list.Add(item);
                        }
                    }
                }

                return new NewErrorModel()
                {
                    data = list,
                    error = new Error(0, "复制成功！", "") { },
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 图纸下单表单保存
        /// </summary>
        /// <param name="purchaseOrderList"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save(List<PurchaseOrder> purchaseOrderList)
        {
            try
            {
                EFHelper<PurchaseOrder> eFHelper = new EFHelper<PurchaseOrder>();
                foreach (var item in purchaseOrderList)
                {
                    eFHelper.Add(item);
                }
                return new NewErrorModel()
                {
                    data = "",
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 图纸下单数据查询
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [Route("QuaryByTaskId")]
        [HttpGet]
        public NewErrorModel QuaryByTaskId(string taskId)
        {
            try
            {
                EFHelper<PurchaseOrder> eFHelper = new EFHelper<PurchaseOrder>();
                System.Linq.Expressions.Expression<Func<PurchaseOrder, bool>> expression = n => n.TaskId == taskId;
                List<PurchaseOrder> purchaseOrderList = eFHelper.GetListBy(expression).ToList();
                return new NewErrorModel()
                {
                    data = purchaseOrderList,
                    error = new Error(0, "查询成功！", "") { },
                };
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
                using (DDContext context = new DDContext())
                {
                    var SelectPurchaseList = from p in context.PurchaseOrder
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
