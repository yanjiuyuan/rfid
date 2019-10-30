
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
                throw ex;
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
                throw ex;
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
                throw ex;
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
                throw ex;
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
                throw ex;
            }
        }

        /// <summary>
        /// 同步物料编码数据数据
        /// </summary>
        /// <returns></returns>
        //[Route("SynchroMySqldata")]
        //[HttpGet]
        //public object SynchroMySqldata()
        //{
        //    try
        //    {
        //        Stopwatch stopwatch = new Stopwatch();
        //        stopwatch.Start();
        //        using (DDContext context = new DDContext())
        //        {

        //            List<MaterialCode> MaterialCodeList = new List<MaterialCode>();
        //            string strSql = "select * from materialcode";
        //            DataSet dataSets = MySqlHelper.GetDataSet(strSql);
        //            DataTable dataTable = dataSets.Tables[0];
        //            foreach (DataRow dr in dataTable.Rows)
        //            {
        //                MaterialCodeList.Add(new MaterialCode()
        //                {
        //                    materialCodeNumber = dr["materialCodeNumber"].ToString(),
        //                    materialName = dr["materialName"].ToString(),
        //                    materiaType = dr["materiaType"].ToString(),
        //                });
        //            }
                    
        //            context.MaterialCode.RemoveRange(context.MaterialCode.ToList());
        //            context.MaterialCode.AddRange(MaterialCodeList);
        //            context.SaveChanges();
        //        };
        //        stopwatch.Stop();
        //        return new NewErrorModel()
        //        {
        //            //count = KisOffices.Count,
        //            //data = KisOffices,
        //            error = new Error(0, string.Format("同步成功！耗时：{0}", stopwatch.ElapsedMilliseconds), "") { },
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new NewErrorModel()
        //        {
        //            error = new Error(1, ex.Message, "") { },
        //        };
        //    }
        //}


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
                        m => m.materiaType == "0").OrderBy(x => x.materialCodeNumber).ToList();
                    count = materialCodes.Count();
                    data = materialCodes;
                }
                else
                {
                    List<MaterialCode> materialCodes = context.MaterialCode.Where(
                       m => m.materialCodeNumber.Substring(0, 2) == materialCodeNumber &&
                      m.materiaType == "2").ToList();
                    count = materialCodes.Count();
                    foreach (var item in materialCodes)
                    {
                        if (item.materialCodeNumber.Length > 3)
                        {
                            item.materialCodeNumber = item.materialCodeNumber.Replace(item.materialCodeNumber.Substring(0, 3), "");
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


        /// <summary>
        /// 获取物料大类和小类
        /// </summary>
        /// <returns></returns>
        [Route("GetAllMaterialCode")]
        [HttpGet]

        public object GetAllMaterialCode()
        {
            using (DDContext context = new DDContext())
            {
                List<MaterialCode> materialCodes = context.MaterialCode.ToList();
                List<BigMaterialCode> bigMaterialCodes = new List<BigMaterialCode>();
                List<MaterialCode> bigmaterialCodes = materialCodes.Where(m => m.materiaType == "0").ToList();
                foreach (var item in bigmaterialCodes)
                {
                    List<MaterialCode> materialCodesSmallList = materialCodes.Where(m => m.materialCodeNumber.Substring(0, 2) == item.materialCodeNumber &&
                    m.materiaType == "2").ToList();

                    List<SmallMaterialCode> SmallMaterialCodeList = new List<SmallMaterialCode>();
                    foreach (var materialCodesSmall in materialCodesSmallList)
                    {
                        SmallMaterialCodeList.Add(new SmallMaterialCode()
                        {
                            materialCodeNumber = materialCodesSmall.materialCodeNumber.Length > 3 ? materialCodesSmall.materialCodeNumber.Replace(materialCodesSmall.materialCodeNumber.Substring(0, 3), "") : materialCodesSmall.materialCodeNumber,
                            materialName = materialCodesSmall.materialName,
                            materiaType = materialCodesSmall.materiaType,
                        });
                    }

                    bigMaterialCodes.Add(new BigMaterialCode
                    {
                        materialCodeNumber = item.materialCodeNumber,
                        materialName = item.materialName,
                        materiaType = item.materiaType,
                        smallMaterialCodes = SmallMaterialCodeList,
                    });
                }
                return new NewErrorModel()
                {
                    count = bigMaterialCodes.Count,
                    data = bigMaterialCodes,
                    error = new Error(0, "读取成功", "") { },
                };
            }
        }


        /// <summary>
        /// 获取物料大类和小类(后端调用)
        /// </summary>
        /// <returns></returns>

        public List<BigMaterialCode> GetAllMaterialCodeBackstage()
        {
            using (DDContext context = new DDContext())
            {
                List<MaterialCode> materialCodes = context.MaterialCode.ToList();
                List<BigMaterialCode> bigMaterialCodes = new List<BigMaterialCode>();
                List<MaterialCode> bigmaterialCodes = materialCodes.Where(m => m.materiaType == "0").ToList();
                foreach (var item in bigmaterialCodes)
                {
                    List<MaterialCode> materialCodesSmallList = materialCodes.Where(m => m.materialCodeNumber.Substring(0, 2) == item.materialCodeNumber &&
                    m.materiaType == "2").ToList();

                    List<SmallMaterialCode> SmallMaterialCodeList = new List<SmallMaterialCode>();
                    foreach (var materialCodesSmall in materialCodesSmallList)
                    {
                        SmallMaterialCodeList.Add(new SmallMaterialCode()
                        {
                            materialCodeNumber = materialCodesSmall.materialCodeNumber.Length > 3 ? materialCodesSmall.materialCodeNumber.Replace(materialCodesSmall.materialCodeNumber.Substring(0, 3), "") : materialCodesSmall.materialCodeNumber,
                            materialName = materialCodesSmall.materialName,
                            materiaType = materialCodesSmall.materiaType,
                        });
                    }

                    bigMaterialCodes.Add(new BigMaterialCode
                    {
                        materialCodeNumber = item.materialCodeNumber,
                        materialName = item.materialName,
                        materiaType = item.materiaType,
                        smallMaterialCodes = SmallMaterialCodeList,
                    });
                }
                return bigMaterialCodes;
            }
        }


        /// <summary>
        /// 校对物料编码
        /// </summary>
        /// <param name="codeList"></param>
        /// <returns>IsQualified表示校对结果</returns>
        [HttpPost]
        [Route("CheckItemCode")]
        public NewErrorModel CheckItemCode([FromBody] List<Code> codeList)
        {
            List<BigMaterialCode> bigMaterialCodes = GetAllMaterialCodeBackstage();
            //List<CheckCode> CodeRerurn = new List<CheckCode>();
            List<Code> CodeRerurn = codeList;
            foreach (var code in CodeRerurn)
            {
                foreach (var bigMaterialCode in bigMaterialCodes)
                {
                    if (code.BigCode == bigMaterialCode.materialCodeNumber)
                    {
                        if (code.BigCodeName == bigMaterialCode.materialName)
                        {
                            foreach (var item in bigMaterialCode.smallMaterialCodes)
                            {
                                if (code.SmallCode == item.materialCodeNumber)
                                {
                                    if (code.SmallCodeName == item.materialName)
                                    {
                                        code.IsQualified = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new NewErrorModel()
            {
                count = CodeRerurn.Count,
                data = CodeRerurn,
                error = new Error(0, "校验成功", "") { },
            };
        }

    }




    public class BigMaterialCode
    {
        public string materialCodeNumber { get; set; }

        public string materialName { get; set; }

        public string materiaType { get; set; }

        public List<SmallMaterialCode> smallMaterialCodes { get; set; }
    }

    public class SmallMaterialCode
    {
        public string materialCodeNumber { get; set; }

        public string materialName { get; set; }

        public string materiaType { get; set; }
    }


    public partial class CheckCode
    {
        public decimal Id { get; set; }

        public string TaskId { get; set; }

        public string CodeNumber { get; set; }

        public string BigCode { get; set; }

        public string SmallCode { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public string Standard { get; set; }

        public string SurfaceTreatment { get; set; }

        public string PerformanceLevel { get; set; }

        public string StandardNumber { get; set; }

        public string Features { get; set; }

        public string purpose { get; set; }

        public string Remark { get; set; }

        public string FNote { get; set; }

        /// <summary>
        /// 校对物料编码类型是否合格
        /// </summary>
        public bool IsQualified { get; set; }
    }
}
