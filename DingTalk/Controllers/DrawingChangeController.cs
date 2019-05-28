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
    /// 图纸变更
    /// </summary>
    [RoutePrefix("DrawingChange")]
    public class DrawingChangeController : ApiController
    {
        /// <summary>
        /// 图纸BOM变更表单保存
        /// </summary>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save(DrawingChangeTable  drawingChangeTable)
        {
            try
            {
                EFHelper<DrawingChange> eFHelper = new EFHelper<DrawingChange>();
                foreach (var item in drawingChangeTable.DrawingChangeList)
                {
                    eFHelper.Add(item);
                }

                EFHelper<FileChange> eFHelpers = new EFHelper<FileChange>();
                eFHelpers.Add(drawingChangeTable.fileChange);

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
        /// 图纸BOM变更表单读取
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<DrawingChange> DrawingChangeList = context.DrawingChange.Where(c => c.TaskId == TaskId).ToList();

                    FileChange fileChange=context.FileChange.Where(c => c.TaskId == TaskId).FirstOrDefault();
                    
                    return new NewErrorModel()
                    {
                        data = new DrawingChangeTable() {
                            DrawingChangeList= DrawingChangeList,
                            fileChange= fileChange,
                        },
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
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
        /// 图纸BOM变更接口(流程结束后调用)
        /// </summary>
        /// <returns></returns>
        [Route("ChangeBom")]
        [HttpPost]
        public NewErrorModel ChangeBom(DrawingChangeTable drawingChangeTable)
        {
            try
            {
                DDContext context = new DDContext();
                foreach (var item in drawingChangeTable.DrawingChangeList)
                {
                    if (item.ChangeType == "1") //新增
                    {
                        Purchase purchase = new Purchase()
                        {
                            TaskId = item.OldId,
                            BomId = item.BomId,
                            DrawingNo = item.DrawingNo,
                            CodeNo = item.CodeNo,
                            Name = item.Name,
                            Count = item.Count,
                            MaterialScience = item.MaterialScience,
                            Unit = item.Unit,
                            Brand = item.Brand,
                            Sorts = item.Sorts,
                            Mark = item.Mark,
                            SingleWeight = item.SingleWeight,
                            AllWeight = item.AllWeight,
                            NeedTime = item.NeedTime,
                        };
                        context.Purchase.Add(purchase);
                        context.SaveChanges();
                    }

                    if (item.ChangeType == "2")  //删除
                    {
                        Purchase purchase = context.Purchase.Find(item.OldId);
                        purchase.ChangeType = item.ChangeType;
                        context.Entry<Purchase>(purchase).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString()
                      == drawingChangeTable.fileChange.TaskId.ToString() && t.NodeId == 0).FirstOrDefault();
                tasks.MediaId = drawingChangeTable.fileChange.MediaId;
                tasks.MediaIdPDF = drawingChangeTable.fileChange.MediaIdPDF;
                tasks.FilePDFUrl = drawingChangeTable.fileChange.FilePDFUrl;
                tasks.OldFilePDFUrl = drawingChangeTable.fileChange.OldFilePDFUrl;
                tasks.FileUrl = drawingChangeTable.fileChange.FileUrl;
                tasks.OldFileUrl = drawingChangeTable.fileChange.OldFileUrl;
                context.Entry<Tasks>(tasks).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return new NewErrorModel()
                {
                    data = "",
                    error = new Error(0, "变更成功！", "") { },
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

    public class DrawingChangeTable
    {
        public List<DrawingChange> DrawingChangeList { get; set; }

        public FileChange fileChange { get; set; }
    }
}
