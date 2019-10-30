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
    /// 跨部门协作
    /// </summary>
    [RoutePrefix("Cooperate")]
    public class CooperateManagerController : ApiController
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="cooperate"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] Cooperate cooperate)
        {
            try
            {
                EFHelper<Cooperate> eFHelper = new EFHelper<Cooperate>();
                eFHelper.Add(cooperate);
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
        /// 读取
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string taskId)
        {
            try
            {
                EFHelper<Cooperate> eFHelper = new EFHelper<Cooperate>();
                Cooperate cooperate = eFHelper.GetListBy(t => t.TaskId == taskId).FirstOrDefault();

                return new NewErrorModel()
                {
                    count =1,
                    data = cooperate,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="cooperate"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object Modify([FromBody] Cooperate cooperate)
        {
            try
            {
                using (DDContext context=new DDContext ())
                {
                    context.Entry<Cooperate>(cooperate).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
