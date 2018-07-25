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
    }
}
