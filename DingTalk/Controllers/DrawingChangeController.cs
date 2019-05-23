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
        /// 图纸变更表单保存
        /// </summary>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save(List<DrawingChange> DrawingChangeList)
        {
            try
            {
                EFHelper<DrawingChange> eFHelper = new EFHelper<DrawingChange>();
                foreach (var item in DrawingChangeList)
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
        /// 图纸变更表单读取
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
                    return new NewErrorModel()
                    {
                        count = DrawingChangeList.Count,
                        data = DrawingChangeList,
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
        /// 图纸变更接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public NewErrorModel Change(List<DrawingChange> DrawingChangeList)
        {
            try
            {
                DDContext context = new DDContext();

                foreach (var item in DrawingChangeList)
                {
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString()
                      == item.TaskId && t.NodeId == 0).FirstOrDefault();
                    if (!string.IsNullOrEmpty(item.FilePDFUrl))
                    {
                        tasks.FilePDFUrl.Replace(item.OldFilePDFUrl, item.FilePDFUrl);
                        tasks.OldFilePDFUrl.Replace(item.OldPDFName, item.PDFName);
                        context.SaveChanges();
                    }

                    if (!string.IsNullOrEmpty(item.MediaId))
                    {
                        tasks.FilePDFUrl.Replace(item.OldMediaId, item.MediaId);
                        tasks.OldFileUrl.Replace(item.OldFileName, item.FileName);
                        context.SaveChanges();
                    }
                }

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
}
