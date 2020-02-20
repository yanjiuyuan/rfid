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
    /// 公共区域消毒
    /// </summary>
    [RoutePrefix("PublicArea")]
    public class PublicAreaController : ApiController
    {
        /// <summary>
        /// 公共区域消毒保存
        /// </summary>
        /// <param name="publicArea"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save(PublicArea publicArea)
        {
            try
            {
                if (publicArea != null)
                {
                    using (DDContext context = new DDContext())
                    {
                        context.PublicArea.Add(publicArea);
                        context.SaveChanges();
                    }

                    return new NewErrorModel()
                    {
                        error = new Error(0, "保存成功！", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "格式有误！", "") { },
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 表单读取
        /// </summary>
        /// <returns></returns>

        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read()
        {
            try
            {
                List<PublicArea> publicAreas = new List<PublicArea>();

                using (DDContext context = new DDContext())
                {
                    publicAreas = context.PublicArea.ToList();
                }

                return new NewErrorModel()
                {
                    data = publicAreas,
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 公共区域消毒查询
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="IsPrint">是否导出数据</param>
        /// <returns></returns>
        [Route("Query")]
        [HttpGet]
        public NewErrorModel Query(DateTime startTime, DateTime endTime, bool IsPrint = false)
        {
            try
            {
                DDContext dDContext = new DDContext();
                List<PublicArea> publicAreas = new List<PublicArea>();
                if (IsPrint == false)
                {
                    publicAreas = dDContext.PublicArea.Where(p => p.Date >= startTime && p.Date <= endTime).ToList();
                    return new NewErrorModel()
                    {
                        data = publicAreas,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        data = publicAreas,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
             
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
