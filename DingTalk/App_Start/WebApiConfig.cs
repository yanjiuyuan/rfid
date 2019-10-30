
using DingTalk.Utility.Attribute;
using DingTalk.Utility.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace DingTalk.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Filters.Add(new CustomBasicAuthorizeAttribute());//全局注册
            config.Filters.Add(new CustomExceptionFilterAttribute());

            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());//替换全局异常处理类

            // Web API 路由
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            
            //全局异常注入
            //config.Filters.Add(new WebApiExceptionFilterAttribute());
        }
    }
}