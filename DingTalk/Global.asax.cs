using DingTalk.App_Start;
using DingTalk.Models.DingModels;
using DingTalk.Utility;
using DingTalk.Utility.Attribute;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DingTalk
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private Logger logger = new Logger(typeof(MvcApplication));
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            //全局异常注入
            //GlobalConfiguration.Configuration.Filters.Add(new WebApiExceptionFilterAttribute());
            this.logger.Info($"~~~~~~~~~~~~~~~~~~~~~~~网站已启动~~~~~~~~~~~~~~~~~~~~~~~");
        }


        /// <summary>
        /// 全局式的异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception excetion = Server.GetLastError();
            this.logger.Error($"{base.Context.Request.Url.AbsoluteUri}出现异常");
            //Response.Write("System is Error....");
            //Server.ClearError();
            //Response.Redirect
            base.Context.RewritePath("/Login/Error");
        }
    }
}
