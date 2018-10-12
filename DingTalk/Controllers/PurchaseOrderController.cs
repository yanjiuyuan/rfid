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
    /// 图纸下单管理
    /// </summary>
    [RoutePrefix("PurchaseOrder")]
    public class PurchaseOrderController : ApiController
    {
        /// <summary>
        /// 图纸下单数据查询
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public object Quary(string key)
        {
            try
            {
                using (DDContext context=new DDContext ())
                {
                    //context.Tasks.Where
                }

                return new NewErrorModel()
                {
                    data = "",
                    error = new Error(0, "复制成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
            }
        }
    }
}
