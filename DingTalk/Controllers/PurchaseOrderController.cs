using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                //var quaryList = from t in context.Tasks
                //               join p in context.Purchase
                //               on t.TaskId.ToString() equals p.TaskId
                //               where (t.TaskId.ToString().Contains(key) ||
                //               t.ProjectName.Contains(key) || t.Title.Contains(key)
                //               || t.ApplyMan.Contains(key)) && t.NodeId == 0
                //               select new
                //               {
                //                   tId = t.Id,
                //                   t.ApplyMan,
                //                   t.ApplyManId,
                //                   t.ApplyTime,
                //                   t.Dept,
                //                   t.FilePDFUrl,
                //                   t.FileUrl,
                //                   t.FlowId,
                //                   t.ImageUrl,
                //                   t.IsBacked,
                //                   t.IsEnable,
                //                   t.IsPost,
                //                   t.IsSend,
                //                   t.MediaId,
                //                   t.MediaIdPDF,
                //                   t.NodeId,
                //                   t.OldFilePDFUrl,
                //                   t.OldFileUrl,
                //                   t.OldImageUrl,
                //                   t.PdfState,
                //                   t.ProjectId,
                //                   t.ProjectName,
                //                   t.Title,
                //                   purchaseList = new List<Purchase>
                //                   {
                //                       new Purchase()
                //                       {
                //                          Id = p.Id,
                //                          AllWeight= p.AllWeight,
                //                          BomId=  p.BomId,
                //                          Brand= p.Brand,
                //                          CodeNo= p.CodeNo,
                //                          Count= p.Count,
                //                          DrawingNo= p.DrawingNo,
                //                          IsDown= p.IsDown,
                //                          Mark= p.Mark,
                //                          MaterialScience= p.MaterialScience,
                //                          Name= p.Name,
                //                          NeedTime=  p.NeedTime,
                //                          SingleWeight=p.SingleWeight,
                //                          Sorts = p.Sorts,
                //                          TaskId  = p.TaskId,
                //                          Unit= p.Unit
                //                       }
                //                   },
                //               };

                var quaryList = context.Tasks.Where(t => (t.TaskId.ToString().Contains(key)
                  || t.ProjectName.Contains(key) || t.Title.Contains(key)
                    || t.ApplyMan.Contains(key)) && t.NodeId == 0 && t.FlowId == 6).Select(t => new TasksPurcahse
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

                return new NewErrorModel()
                {
                    data = quaryList,
                    error = new Error(0, "复制成功！", "") { },
                };

            }
            catch (Exception ex)
            {
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
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
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
            }
        }


        /// <summary>
        /// 图纸下单数据查询
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [Route("QuaryByTaskId")]
        [HttpGet]
        public object QuaryByTaskId(string taskId)
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
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
            }
        }
    }
}
