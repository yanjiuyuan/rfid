using DingTalk.App_Start;
using DingTalk.Models.DingModels;
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
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Database.SetInitializer(
            //    new CreateDatabaseIfNotExists<DDContext>());
            //using (var context = new DDContext())
            //{
            //    context.Database.Initialize(true);
            //}


//            1 CreateDatabaseIfNotExists
//CreateDatabaseIfNotExists方法会在没有数据库时创建一个，这是默认行为。

//                Database.SetInitializer(
//                    new CreateDatabaseIfNotExists<BreakAwayContext>());
//            using (var context = new BreakAwayContext())
//            {
//                context.Database.Initialize(true);
//            }
//            2 DropCreateDatabaseIfModelChanges
//            如果我们在在模型改变时，自动重新创建一个新的数据库，就可以用这个方法。在这开发过程中非常有用。

//                Database.SetInitializer(
//                    new DropCreateDatabaseIfModelChanges<BreakAwayContext>());
//            using (var context = new BreakAwayContext())
//            {
//                context.Database.Initialize(true);
//            }
//            3 DropCreateDatabaseAlways
//            如果你想在每次运行时都重新生成数据库就可以用这个方法。

//                Database.SetInitializer(
//                    new DropCreateDatabaseAlways<BreakAwayContext>());
//            using (var context = new BreakAwayContext())
//            {
//                context.Database.Initialize(true);
//            }
        }
    }
}
