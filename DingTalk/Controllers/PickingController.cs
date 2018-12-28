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
    /// 领料申请
    /// </summary>
    [RoutePrefix("Picking")]
    public class PickingController : ApiController
    {
        /// <summary>
        /// 领料单批量保存
        /// </summary>
        /// <param name="pickings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Save")]
        public object Save([FromBody] List<Picking> pickings)
        {
            try
            {
                EFHelper<Picking> eFHelper = new EFHelper<Picking>();
                if (pickings.Count == 0)
                {
                    return new NewErrorModel()
                    {
                        error = new Error(2, "参数未传递！", "") { },
                    };
                }
                else
                {
                    foreach (var item in pickings)
                    {
                        eFHelper.Add(item);
                    }
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
        /// 领料单读取
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Read")]
        public object Read(string taskId)
        {
            try
            {
                EFHelper<Picking> eFHelper = new EFHelper<Picking>();
                var query = eFHelper.GetListBy(p => p.TaskId == taskId).ToList();
                return new NewErrorModel()
                {
                    data = query,
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

        /// <summary>
        /// 领料单批量修改
        /// </summary>
        /// <param name="pickings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Modify")]
        public object Modify([FromBody] List<Picking> pickings)
        {
            try
            {
                if (pickings.Count == 0)
                {
                    return new NewErrorModel()
                    {
                        error = new Error(2, "参数未传递！", "") { },
                    };
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        foreach (var item in pickings)
                        {
                            context.Entry<Picking>(item).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }

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
    }
}
