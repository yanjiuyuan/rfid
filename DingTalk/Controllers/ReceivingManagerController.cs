using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 收文
    /// </summary>
    [RoutePrefix("Receiving")]
    public class ReceivingManagerController : ApiController
    {
        /// <summary>
        /// 收文表单保存
        /// </summary>
        /// <param name="ReceivingList"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object SaveTable([FromBody] Receiving ReceivingList)
        {
            try
            {
                EFHelper<Receiving> eFHelper = new EFHelper<Receiving>();
                ReceivingList.ReceivingNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                eFHelper.Add(ReceivingList);
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
        /// 收文表单修改
        /// </summary>
        /// <param name="ReceivingList"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object ModifyTable([FromBody] Receiving ReceivingList)
        {
            try
            {
                EFHelper<Receiving> eFHelper = new EFHelper<Receiving>();
                Receiving QuaryReceiving = eFHelper.GetListBy(t => t.Id == ReceivingList.Id).First();
                QuaryReceiving.Leadership = ReceivingList.Leadership;
                QuaryReceiving.MainIdea = ReceivingList.MainIdea;
                QuaryReceiving.Suggestion = ReceivingList.Suggestion;
                QuaryReceiving.Leadership = ReceivingList.Leadership;
                if (string.IsNullOrEmpty(QuaryReceiving.Review))
                {
                    QuaryReceiving.Review = ReceivingList.Review;
                }
                else
                {
                    QuaryReceiving.Review += "~" + ReceivingList.Review;
                }

                if (string.IsNullOrEmpty(QuaryReceiving.HandleImplementation))
                {
                    QuaryReceiving.HandleImplementation = ReceivingList.HandleImplementation;
                }
                else
                {
                    QuaryReceiving.HandleImplementation += "~" + ReceivingList.HandleImplementation;
                }
                eFHelper.Modify(QuaryReceiving);
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
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
        /// 收文表单读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object ReadTable(string taskId)
        {
            try
            {
                EFHelper<Receiving> eFHelper = new EFHelper<Receiving>();
                List<Receiving> receivings = eFHelper.GetListBy(t => t.TaskId == taskId);
                return new NewErrorModel()
                {
                    error = new Error(0, "读取成功！", "") { },
                    data = receivings
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
