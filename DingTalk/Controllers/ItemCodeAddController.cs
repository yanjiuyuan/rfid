using Common.DbHelper;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

                    //context.BulkInsert(kisPurchases);
                    //context.BulkSaveChanges();
                    foreach (var kis in kisPurchases)
                    {
                        context.KisPurchase.Add(kis);
                        context.SaveChanges();
                    }
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
                    //context.BulkInsert(KisOffices);
                    //context.BulkSaveChanges();

                    foreach (var kis in KisOffices)
                    {
                        context.KisOffice.Add(kis);
                        context.SaveChanges();
                    }
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

        /// <summary>
        /// 同步MySql数据
        /// </summary>
        /// <returns></returns>
        [Route("SynchroMySqldata")]
        [HttpGet]
        public object SynchroMySqldata()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (DDContext context = new DDContext())
                {

                    List<MaterialCode> MaterialCodeList = new List<MaterialCode>();
                    string strSql = "select * from materialcode";
                    DataSet dataSets = MySqlHelper.GetDataSet(strSql);
                    DataTable dataTable = dataSets.Tables[0];
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        MaterialCodeList.Add(new MaterialCode()
                        {
                            MaterialCodeNumber = dr["materialCodeNumber"].ToString(),
                            MaterialName = dr["materialName"].ToString(),
                            MateriaType = dr["materiaType"].ToString(),
                        });
                    }
                    context.BulkInsert(MaterialCodeList);
                };
                stopwatch.Stop();
                return new NewErrorModel()
                {
                    //count = KisOffices.Count,
                    //data = KisOffices,
                    error = new Error(0, string.Format("同步成功！耗时：{0}", stopwatch.ElapsedMilliseconds), "") { },
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
        /// 获取物料分类
        /// </summary>
        /// <param name="materialCodeNumber">不传参数默认返回所有大类</param>
        /// <returns></returns>
        [Route("GatMaterialCode")]
        [HttpGet]

        public object GatMaterialCode(string materialCodeNumber = "")
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            object data = new object();
            int count = 0;
            using (DDContext context = new DDContext())
            {
                if (string.IsNullOrEmpty(materialCodeNumber))
                {
                    List<MaterialCode> materialCodes = context.MaterialCode.Where(
                        m => m.MateriaType == "0").ToList();
                    count = materialCodes.Count();
                    data = materialCodes;
                }
                else
                {
                    List<MaterialCode> materialCodes = context.MaterialCode.Where(
                       m => m.MaterialCodeNumber.Substring(0, 2) == materialCodeNumber &&
                      m.MateriaType=="2").ToList();
                    count = materialCodes.Count();
                    foreach (var item in materialCodes)
                    {
                        if (item.MaterialCodeNumber.Length > 3)
                        {
                            item.MaterialCodeNumber = item.MaterialCodeNumber.Replace(item.MaterialCodeNumber.Substring(0, 3), "");
                        }
                    }
                    data = materialCodes;
                }
            };
            stopwatch.Stop();
            return new NewErrorModel()
            {
                count = count,
                data = data,
                error = new Error(0, string.Format("同步成功！耗时：{0}毫秒", stopwatch.ElapsedMilliseconds), "") { },
            };
        }
    }
}
