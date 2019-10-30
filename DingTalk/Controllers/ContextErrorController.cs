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
        /// <param name="key">关键字(调用接口路径)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Read")]
        public NewErrorModel Read(string key="")
        {
            using (DDContext context = new DDContext())
            {
                List<ContextError> contextErrors = new List<ContextError>();
                if (key == "")
                {
                    contextErrors = context.ContextError.ToList();
                }
                else
                {
                    contextErrors = context.ContextError.Where(c => c.RequestUrl.Contains(key)
                    ).ToList();
                }
                return new NewErrorModel()
                {
                    data = contextErrors,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
        }
    }
}
