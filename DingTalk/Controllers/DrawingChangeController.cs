using DingTalk.Bussiness.FlowInfo;
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
        /// 图纸数据查询
        /// </summary>
        /// <returns></returns>
        [Route("Query")]
        [HttpGet]
        public NewErrorModel Query(string ProjectName, string ProjectType)
        {
            try
            {
                DDContext context = new DDContext();
                List<Tasks> tasksList = FlowInfoServer.ReturnUnFinishedTaskId("6").Where(t => t.NodeId == 0 && t.ProjectName==ProjectName && t.projectType==ProjectType).ToList();

                List<Purchase> PurchaseList = context.Purchase.ToList();
                var Query = from t in tasksList
                            join p in PurchaseList
        on t.TaskId.ToString() equals p.TaskId
                            select new
                            {
                                Id = p.Id,
                                TaskId = p.TaskId,
                                BomId = p.BomId,
                                DrawingNo = p.DrawingNo,
                                CodeNo = p.CodeNo,
                                Name = p.Name,
                                Count = p.Count,
                                MaterialScience = p.MaterialScience,
                                Unit = p.Unit,
                                Brand = p.Brand,
                                Sorts = p.Sorts,
                                Mark = p.Mark,
                                IsDown = p.IsDown,
                                SingleWeight = p.SingleWeight,
                                AllWeight = p.AllWeight,
                                NeedTime = p.NeedTime,
                                ChangeType = p.ChangeType,
                            };



                return new NewErrorModel()
                {
                    data = Query,
                    error = new Error(0, "读取成功！", "") { },
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
        /// 图纸BOM变更表单保存
        /// </summary>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save(DrawingChangeTable drawingChangeTable)
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

                    FileChange fileChange = context.FileChange.Where(c => c.TaskId == TaskId).FirstOrDefault();

                    return new NewErrorModel()
                    {
                        data = new DrawingChangeTable()
                        {
                            DrawingChangeList = DrawingChangeList,
                            fileChange = fileChange,
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
        public NewErrorModel ChangeBom(List<DrawingChange> DrawingChangeList)
        {
            try
            {
                DDContext context = new DDContext();
                foreach (var item in DrawingChangeList)
                {
                    if (item.ChangeType == "1") //新增
                    {
                        Purchase purchase = new Purchase()
                        {
                            TaskId = item.TaskId,
                            AllWeight = item.AllWeight,
                            BomId = item.BomId,
                            Brand = item.Brand,
                            ChangeType = item.ChangeType,
                            CodeNo = item.CodeNo,
                            Count = item.Count,
                            DrawingNo = item.DrawingNo,
                            Mark = item.Mark,
                            MaterialScience = item.MaterialScience,
                            Name = item.Name,
                            NeedTime = item.NeedTime,
                            SingleWeight = item.SingleWeight,
                            Sorts = item.Sorts,
                            Unit = item.Unit
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

                //Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString()
                //      == drawingChangeTable.fileChange.TaskId.ToString() && t.NodeId == 0).FirstOrDefault();
                //tasks.MediaId = drawingChangeTable.fileChange.MediaId;
                //tasks.MediaIdPDF = drawingChangeTable.fileChange.MediaIdPDF;
                //tasks.FilePDFUrl = drawingChangeTable.fileChange.FilePDFUrl;
                //tasks.OldFilePDFUrl = drawingChangeTable.fileChange.OldFilePDFUrl;
                //tasks.FileUrl = drawingChangeTable.fileChange.FileUrl;
                //tasks.OldFileUrl = drawingChangeTable.fileChange.OldFileUrl;
                //context.Entry<Tasks>(tasks).State = System.Data.Entity.EntityState.Modified;
                //context.SaveChanges();

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
