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
    /// 人才招聘管理
    /// </summary>
    [RoutePrefix("Job")]
    public class JobManangerController : ApiController
    {
        /// <summary>
        /// 人才招聘保存
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] Jobs jobs)
        {
            try
            {
                EFHelper<Jobs> eFHelper = new EFHelper<Jobs>();
                eFHelper.Add(jobs);
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
        /// 人才招聘读取
        /// </summary>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read()
        {
            try
            {
                EFHelper<Jobs> eFHelper = new EFHelper<Jobs>();
                List<Jobs> jobsList = eFHelper.GetList();
                return new NewErrorModel()
                {
                    count = jobsList.Count,
                    data = jobsList,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 人才招聘删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("DeleteById")]
        [HttpGet]
        public object DeleteById(int id)
        {
            try
            {
                EFHelper<Jobs> eFHelper = new EFHelper<Jobs>();
                Jobs jobs = eFHelper.GetListById(id);
                eFHelper.Del(jobs);
                return new NewErrorModel()
                {
                    data = jobs,
                    error = new Error(0, "删除成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
