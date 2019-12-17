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
    [RoutePrefix("ContextError")]
    public class ContextErrorController : ApiController
    {
        /// <summary>
        /// 错误日志读取
        /// </summary>
        /// <param name="pageSize">页容量</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Read")]
        public NewErrorModel Read(int pageSize = 5, int pageIndex = 1, string key = "")
        {
            using (DDContext context = new DDContext())
            {
                int count = 0;
                List<ContextError> contextErrors = new List<ContextError>();
                if (key == "")
                {
                    count = context.ContextError.ToList().Count;
                    contextErrors = context.ContextError.OrderByDescending(c=>c.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    count = context.ContextError.Where(c => c.RequestUrl.Contains(key)
                   || c.Id.ToString() == key).ToList().Count;
                    contextErrors = context.ContextError.Where(c => c.RequestUrl.Contains(key)
                   || c.Id.ToString() == key).OrderByDescending(c => c.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                return new NewErrorModel()
                {
                    count= count,
                    data = contextErrors,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
        }
    }
}
