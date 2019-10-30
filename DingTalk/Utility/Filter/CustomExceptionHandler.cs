using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace DingTalk.Utility.Filters
{
    /// <summary>
    /// WEBApi的全局异常处理
    /// </summary>
    public class CustomExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// 判断是否要进行异常处理，规则自己定
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            string url = context.Request.RequestUri.AbsoluteUri;
            return url.Contains("/api/");

            //return base.ShouldHandle(context);
        }
        /// <summary>
        /// 完成异常处理
        /// </summary>
        /// <param name="context"></param>
        public override void Handle(ExceptionHandlerContext context)
        {
            //Console.WriteLine(context);//log
            context.Result = new ResponseMessageResult(context.Request.CreateResponse(
                System.Net.HttpStatusCode.OK, new
                {
                    Result = false,
                    Msg = "出现异常，请联系管理员",
                    Debug = context.Exception.Message
                }));

            //if(context.Exception is HttpException)
        }
    }
}