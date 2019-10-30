using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalk.Models.OfficeModels;
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
    /// 办公用品申请
    /// </summary>
    [RoutePrefix("OfficeSupplies")]
    public class OfficeSuppliesManageController : ApiController
    {
        /// <summary>
        /// 办公用品申请表单保存
        /// </summary>
        /// <param name="officeSuppliesTableList"></param>
        /// <returns></returns>
        [Route("SaveTable")]
        [HttpPost]
        public NewErrorModel SaveTable([FromBody] List<OfficeSupplies> officeSuppliesTableList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (OfficeSupplies officeSupplies in officeSuppliesTableList)
                    {
                        context.OfficeSupplies.Add(officeSupplies);
                        context.SaveChanges();
                    }
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
        /// 办公用品表单读取
        /// </summary>
        /// <returns></returns>
        [Route("ReadTable")]
        [HttpGet]
        public NewErrorModel ReadTable(string TaskId)
        {
            try
            {
                List<OfficeSupplies> OfficeSuppliesTableList = new List<OfficeSupplies>();
                using (DDContext context = new DDContext())
                {
                    OfficeSuppliesTableList = context.OfficeSupplies.Where
                         (p => p.TaskId == TaskId && p.IsDelete!=true).ToList();
                }

                return new NewErrorModel()
                {
                    data = OfficeSuppliesTableList,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 办公用品申请表单修改
        /// </summary>
        /// <param name="officeSuppliesTableList"></param>
        /// <returns></returns>
        [Route("ModifyTable")]
        [HttpPost]
        public NewErrorModel ModifyTable([FromBody] List<OfficeSupplies> officeSuppliesTableList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (OfficeSupplies officeSupplies in officeSuppliesTableList)
                    {
                        context.Entry<OfficeSupplies>(officeSupplies).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
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


        #region 金蝶产品信息读取
        /// <summary>
        /// 金蝶产品信息读取
        /// </summary>
        /// <param name="Key">查询关键字</param>
        /// <returns></returns>
        [Route("GetICItem")]
        [HttpGet]
        public string GetICItem(string Key)
        {
            try
            {
                //using (OfficeModel context = new OfficeModel())
                //{
                //    var Quary = context.Database.SqlQuery<t_ICItem>
                //        (string.Format("SELECT * FROM t_ICItem WHERE FName like  '%{0}%' or  FNumber like '%{1}%'", Key, Key)).ToList();
                //    return JsonConvert.SerializeObject(Quary);
                //}
                using (DDContext context = new DDContext())
                {
                    var Quary = context.KisOffice.Where(k => k.FName.Contains(Key) ||
                    k.FNumber.Contains(Key) || k.FModel.Contains(Key)).ToList();
                    return JsonConvert.SerializeObject(Quary);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
