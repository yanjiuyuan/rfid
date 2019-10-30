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
    /// 基地加班
    /// </summary>
    [RoutePrefix("OverTimeTable")]
    public class OverTimeTableController : ApiController
    {
        /// <summary>
        /// 加班表单保存
        /// </summary>
        /// <param name="overTime"></param>
        [Route("OverTimeTableSave")]
        [HttpPost]
        public object OverTimeTableSave([FromBody] OverTime overTime)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.OverTime.Add(overTime);
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "成功发起"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 加班表单查询
        /// </summary>
        /// <param name="TaskId">流水号</param>
        [Route("OverTimeTableQuary")]
        [HttpGet]
        public object OverTimeTableQuary(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    OverTime overTime = context.OverTime.Where(o => o.TaskId == TaskId).First();
                    return overTime;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 加班表单修改
        /// </summary>
        /// <param name="overTime"></param>
        [Route("OverTimeTableModify")]
        [HttpPost]
        public object OverTimeTableModify([FromBody] OverTime overTime)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<OverTime>(overTime).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "修改成功"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
