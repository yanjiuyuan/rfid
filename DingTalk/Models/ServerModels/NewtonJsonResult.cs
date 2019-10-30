using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DingTalk.Models.ServerModels
{
    public class NewtonJsonResult : ActionResult
    {
        private object Data = null;
        public NewtonJsonResult(object data)
        {
            this.Data = data;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(this.Data));
        }
    }
}