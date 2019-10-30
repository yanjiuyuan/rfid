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
    [RoutePrefix("FileNew")]

    public class FileNewController : ApiController
    {

        /// <summary>
        /// 图纸PDF状态更新
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="PDFState"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("UpdatePDFState")]
        public NewErrorModel UpdatePDFState(string TaskId, string PDFState)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.NodeId == 0).First();
                    tasks.PdfState = PDFState;
                    context.Entry<Tasks>(tasks).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return new NewErrorModel()
                    {
                        error = new Error(0, "更新成功！", "") { },
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
