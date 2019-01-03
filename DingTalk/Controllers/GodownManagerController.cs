using DingTalk.EF;
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
    /// 入库
    /// </summary>
    [RoutePrefix("Godown")]
    public class GodownManagerController : ApiController
    {
        /// <summary>
        /// 入库单批量保存
        /// </summary>
        /// <param name="goDownList"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] List<GoDown> goDownList)
        {
            try
            {
                EFHelper<GoDown> eFHelper = new EFHelper<GoDown>();
                foreach (var goDown in goDownList)
                {
                    eFHelper.Add(goDown);
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "保存成功！", "") { },
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


        /// <summary>
        /// 入库单读取
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string taskId)
        {
            try
            {
                EFHelper<GoDown> eFHelper = new EFHelper<GoDown>();
                List<GoDown> goDownList = eFHelper.GetListBy(t => t.TaskId == taskId).ToList();

                return new NewErrorModel()
                {
                    count= goDownList.Count,
                    data = goDownList,
                    error = new Error(0, "读取成功！", "") { },
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
