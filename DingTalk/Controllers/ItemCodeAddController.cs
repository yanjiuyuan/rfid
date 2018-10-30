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
    /// 物料编码新增流程
    /// </summary>
    [RoutePrefix("ItemCodeAdd")]
    public class ItemCodeAddController : ApiController
    {
        /// <summary>
        /// 表单保存接口
        /// </summary>
        /// <param name="codeList"></param>
        /// <returns>"{\"errorCode\":0,\"errorMessage\":\"保存成功\",\"Content\":null,\"IsError\":false}"</returns>
        /// 测试数据：{"TaskId":"流水号","CodeNumber":"物料编码","BigCode":"物料大类编码","SmallCode":"小类编码","Name":"物料名称","Unit":"单位","Standard":"型号规格","SurfaceTreatment":"表面处理","PerformanceLevel":"性能等级","StandardNumber":"标准号","Features":"典型特征","purpose":"用途","Remark":"备注"}
        [Route("TableSave")]
        [HttpPost]
        public Object TableSave([FromBody] List<Code> codeList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var code in codeList)
                    {
                        context.Code.Add(code);
                    }
                    context.SaveChanges();
                    return new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "保存成功"
                    };
                }
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

        /// <summary>
        /// 表单读取接口
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        /// 测试数据：ItemCodeAdd/GetTable?TaskId=1
        [Route("GetTable")]
        [HttpGet]
        public Object GetTable(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Code> codes = context.Code.Where(c => c.TaskId == TaskId).ToList();
                    return codes;
                }
            }
            catch (Exception ex)
            {
                return new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                };
            }
        }


        /// <summary>
        /// 物料编码批量修改
        /// </summary>
        /// <param name="codeList"></param>
        /// <returns></returns>
        [Route("TableModify")]
        [HttpPost]
        public Object TableModify([FromBody] List<Code> codeList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var code in codeList)
                    {
                        context.Entry<Code>(code).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    return new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "保存成功"
                    };
                }
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

        /// <summary>
        /// 物料批量新增接口(采购物料)
        /// </summary>
        /// <param name="kisPurchases"></param>
        /// <returns></returns>
        [Route("InsertPurcahse")]
        [HttpPost]
        public object InsertPurcahse(List<KisPurchase> kisPurchases)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.BulkInsert(kisPurchases);
                    context.BulkSaveChanges();
                }
                return new NewErrorModel()
                {
                    count = kisPurchases.Count,
                    data = kisPurchases,
                    error = new Error(0, "插入成功！", "") { },
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
        /// 物料批量新增接口(办公用品物料)
        /// </summary>
        /// <param name="KisOffices"></param>
        /// <returns></returns>
        [Route("InsertOffice")]
        [HttpPost]
        public object InsertOffice(List<KisOffice> KisOffices)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.BulkInsert(KisOffices);
                    context.BulkSaveChanges();
                }
                return new NewErrorModel()
                {
                    count = KisOffices.Count,
                    data = KisOffices,
                    error = new Error(0, "插入成功！", "") { },
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
