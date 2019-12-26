using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace DingTalk.Utility.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private Logger logger = new Logger(typeof(CustomExceptionFilterAttribute));

        /// <summary>
        /// 异常发生后(没有被catch)，会进到这里
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Stream stream = actionExecutedContext.Request.Content.ReadAsStreamAsync().Result;
            stream.Position = 0;
            var reader = new StreamReader(stream, Encoding.UTF8);
            string method = actionExecutedContext.Request.Method.Method; //请求类型
            string errorMsg = actionExecutedContext.Exception.Message; //错误信息
            string requstResult = reader.ReadToEnd();  //前端调用参数
            string requestUrl = actionExecutedContext.Request.RequestUri.ToString();// 请求路径
            string stackTrace = actionExecutedContext.Exception.StackTrace; //堆栈信息
            string requestIp = "";
            DDContext context = new DDContext();

            ContextError contextError = new ContextError()
            {
                ErrorMsg = errorMsg,
                RequstResult = requstResult,
                Method = method,
                RequestIp = requestIp,
                RequestUrl = requestUrl,
                StackTrace = stackTrace,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            context.ContextError.Add(contextError);
            context.SaveChanges();
            this.logger.Error($"在响应 {requestUrl} 时出现异常，信息：{errorMsg}，详见数据库日志Id：{contextError.Id}");

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
            System.Net.HttpStatusCode.OK, new NewErrorModel
            {
                error = new Error(1, $"出现异常，请联系管理员! 错误Id：{contextError.Id}", "") { },
            });

            //base.OnException(actionExecutedContext);
            //ExceptionHandler
        }
    }
}