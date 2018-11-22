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
    /// 礼品招待酒领用申请
    /// </summary>
    [RoutePrefix("Gift")]
    public class GiftTableController : ApiController
    {
        /// <summary>
        /// 表单保存接口
        /// </summary>
        /// <param name="giftTable"></param>
        [Route("TableSave")]
        [HttpPost]
        public Object TableSave([FromBody] List<GiftTable> giftTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var gift in giftTable)
                    {
                        context.GiftTable.Add(gift);
                    }
                    context.SaveChanges();
                    return new NewErrorModel()
                    {
                        error = new Error(0,"添加成功", "") { },
                    };
                }
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
        /// 表单读取接口
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        [Route("GetTable")]
        [HttpGet]
        public Object GetTable(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<GiftTable> giftTable = context.GiftTable.Where(c => c.TaskId == TaskId).ToList();
                    return new NewErrorModel()
                    {
                        count = giftTable.Count,
                        data = giftTable,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
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
        /// 表单批量修改
        /// </summary>
        /// <param name="giftList"></param>
        /// <returns></returns>
        [Route("TableModify")]
        [HttpPost]
        public Object TableModify([FromBody] List<GiftTable> giftList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var gift in giftList)
                    {
                        context.Entry<GiftTable>(gift).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    return new NewErrorModel()
                    {
                        error = new Error(0, "修改成功", "") { },
                    };
                }
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
