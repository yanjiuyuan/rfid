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
    /// 用车申请
    /// </summary>
    [RoutePrefix("CarTable")]
    public class CarTableController : ApiController
    {
        /// <summary>
        /// 用车表单保存接口
        /// </summary>
        /// <param name="carTable"></param>
        /// <returns></returns>
        [Route("TableSave")]
        [HttpPost]
        public object TableSave(CarTable carTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.CarTable.Add(carTable);
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "添加成功"
                };
            }
            catch (Exception ex)
            {
                return new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                };
            }
        }
    }
}
