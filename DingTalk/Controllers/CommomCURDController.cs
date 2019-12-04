using DingTalk.Bussiness.EF;
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
    [RoutePrefix("CommomCURD")]
    public class CommomCURDController : ApiController
    {
        /// <summary>
        /// 通用保存方法
        /// </summary>
        /// <param name="cURDModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Save")]
        public NewErrorModel Save(CURDModel cURDModel)
        {
            try
            {
                //if (cURDModel != null && cURDModel.tables != null && cURDModel.tables.Count > 0)
                //{
                //    using (DDContext dataContext = new DDContext())
                //    {
                //        SqlHelper sqlHelper = new SqlHelper();
                //        foreach (var item in cURDModel.tables)
                //        {
                //            string strSql = sqlHelper.Insert(item);
                //            int iResult = dataContext.Database.ExecuteSqlCommand(strSql);
                //            if (iResult != 1)
                //            {
                //                return new NewErrorModel()
                //                {
                //                    error = new Error(1, $"{item.TableName} 格式有误,插入失败！", "") { },
                //                };
                //            }
                //        }
                //    }
                //    return new NewErrorModel()
                //    {
                //        error = new Error(1, "批量插入成功！", "") { },
                //    };
                //}
                //else
                //{
                //    return new NewErrorModel()
                //    {
                //        error = new Error(1, "参数有误！", "") { },
                //    };
                //}
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 通用读取方法
        /// </summary>
        /// <param name="cURDModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Read")]
        public NewErrorModel Read(CURDModel cURDModel)
        {
            try
            {
                //if (cURDModel != null && cURDModel.tables != null && cURDModel.tables.Count > 0)
                //{
                //    using (DDContext dataContext = new DDContext())
                //    {

                //    }
                //    SqlHelper sqlHelper = new SqlHelper();
                //    if (cURDModel.tables.Count == 1) //单表
                //    {
                //        string strSql = sqlHelper.CommomCURDRead(cURDModel.tables[0]);

                //    }
                //    return null;
                //}
                //else
                //{
                //    return new NewErrorModel()
                //    {
                //        error = new Error(1, "参数有误！", "") { },
                //    };
                //}

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class CURDModel
    {
        /// <summary>
        /// 操作人Id(用于判断权限)
        /// </summary>
        public string applyManId { get; set; }

        /// <summary>
        /// 表数据
        /// </summary>
        //public List<Tables> tables { get; set; }

        /// <summary>
        /// 表数据
        /// </summary>
        public List<CURDModelSave> CURDModelSave { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        //public int pageIndex { get; set; }

        /// <summary>
        /// 页容量(默认每页5条)
        /// </summary>

        //public int pageSize { get; set; }
    }

    public class CURDModelSave
    {
        public string TableName { get; set; }

        public List<Dictionary<string, string>> columns { get; set; }

    }

    public class column
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段属性( 0 : string  1 : int  2 : bool)
        /// </summary>
        public int ColumnProperty { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        public string ColumnValue { get; set; }
    }
}
