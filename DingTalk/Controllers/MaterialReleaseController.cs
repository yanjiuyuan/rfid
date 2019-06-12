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

namespace DingTalk.Controllers
{
    /// <summary>
    /// 物资出行
    /// </summary>
    [RoutePrefix("MaterialRelease")]
    public class MaterialReleaseController : ApiController
    {
        /// <summary>
        /// 表单保存
        /// </summary>
        /// <param name="evection"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save([FromBody] MaterialRelease evection)
        {
            try
            {
                EFHelper<MaterialRelease> eFHelper = new EFHelper<MaterialRelease>();
                eFHelper.Add(evection);
                return new NewErrorModel()
                {
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
                EFHelper<MaterialRelease> eFHelper = new EFHelper<MaterialRelease>();
                MaterialRelease materialRelease = eFHelper.GetListBy(t => t.TaskId == taskId).ToList().First();

                return new NewErrorModel()
                {
                    data = materialRelease,
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
        /// 表单修改
        /// </summary>
        /// <param name="evection"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object Modify([FromBody] MaterialRelease evection)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<MaterialRelease>(evection).State = EntityState.Modified;
                    context.SaveChanges();
                }

                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
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
