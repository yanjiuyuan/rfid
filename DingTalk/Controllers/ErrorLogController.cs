using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 报错管理
    /// </summary>
    [RoutePrefix("ErrorLog")]
    public class ErrorLogController : ApiController
    {
        /// <summary>
        /// 错误保存
        /// </summary>
        /// <param name="ErrorLog"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public async Task<object> Save([FromBody] ErrorLogs ErrorLog)
        {
            try
            {
                EFHelper<ErrorLogs> eFHelper = new EFHelper<ErrorLogs>();
                ErrorLog.ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                eFHelper.Add(ErrorLog);
                //钉钉推送超管

                string[] SaIds = System.Configuration.ConfigurationManager.AppSettings["administrator"].Split(',');
                DingTalkServersController dingTalkServersController = new DingTalkServersController();

                foreach (var SaId in SaIds)
                {
                    await dingTalkServersController.sendOaMessage(SaId, "报错反馈", "系统报错", "eapp://util/errorPage/errorPage");
                }
                
                return new NewErrorModel()
                {
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 错误读取
        /// </summary>
        /// <returns></returns>
        [Route("Read")]
        [HttpPost]
        public object Read()
        {
            try
            {
                EFHelper<ErrorLogs> eFHelper = new EFHelper<ErrorLogs>();
                List<ErrorLogs> errorLogs = eFHelper.GetListBy(e=>e.ApplyTime!=null, e=>e.Id,false);
                return new NewErrorModel()
                {
                    data = errorLogs,
                    count = errorLogs.Count(),
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                return null;
                //throw ex;
            }
        }
    }
}
