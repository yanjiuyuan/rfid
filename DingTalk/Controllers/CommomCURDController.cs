using DingTalk.Bussiness.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Data;
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
                if (cURDModel != null && cURDModel.SaveModels != null && cURDModel.SaveModels.Count > 0)
                {
                    int iCount = 0;
                    using (DDContext dataContext = new DDContext())
                    {
                        SqlHelper sqlHelper = new SqlHelper();
                        foreach (var item in cURDModel.SaveModels)
                        {
                            List<TableInfo> tableInfos = dataContext.TableInfo.Where(t => t.TableID ==
                            dataContext.Tables.Where(s => s.TableName == item.TableName).FirstOrDefault().ID
                            ).ToList();
                            string strSql = sqlHelper.Insert(item, tableInfos);
                            int iResult = dataContext.Database.ExecuteSqlCommand(strSql);
                            if (iResult != 1)
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, $"{item.TableName} 格式有误,插入失败！", "") { },
                                };
                            }
                            else
                            {
                                iCount += iResult;
                            }
                        }
                    }
                    return new NewErrorModel()
                    {
                        count = iCount,
                        error = new Error(0, $"批量插入成功！共计插入:{iCount.ToString()}条数据", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "参数有误！", "") { },
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 通用批量删除方法(目前只支持Id删除)
        /// </summary>
        /// <param name="cURDModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public NewErrorModel Delete(CURDModel cURDModel)
        {
            try
            {
                if (cURDModel != null && cURDModel.SaveModels != null && cURDModel.SaveModels.Count > 0)
                {
                    int iCount = 0;
                    using (DDContext dataContext = new DDContext())
                    {
                        SqlHelper sqlHelper = new SqlHelper();
                        foreach (var item in cURDModel.SaveModels)
                        {
                            string strSql = sqlHelper.Delete(item);
                            int iResult = dataContext.Database.ExecuteSqlCommand(strSql);
                            if (iResult == 0)
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, $"{item.TableName} 格式有误或者Id不存在,数据删除失败！", "") { },
                                };
                            }
                            else
                            {
                                iCount += iResult;
                            }
                        }
                    }
                    return new NewErrorModel()
                    {
                        count = iCount,
                        error = new Error(0, $"批量删除成功！共计删除:{iCount.ToString()}条数据 ", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "参数有误！", "") { },
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 通用修改方法
        /// </summary>
        /// <param name="cURDModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Modify")]
        public NewErrorModel Modify(CURDModel cURDModel)
        {
            try
            {
                if (cURDModel != null && cURDModel.SaveModels != null && cURDModel.SaveModels.Count > 0)
                {
                    int iCount = 0;
                    using (DDContext dataContext = new DDContext())
                    {
                        SqlHelper sqlHelper = new SqlHelper();
                        foreach (var item in cURDModel.SaveModels)
                        {
                            List<TableInfo> tableInfos = dataContext.TableInfo.Where(t => t.TableID ==
                            dataContext.Tables.Where(s => s.TableName == item.TableName).FirstOrDefault().ID
                            ).ToList();
                            string strSql = sqlHelper.Modify(item, tableInfos);
                            int iResult = dataContext.Database.ExecuteSqlCommand(strSql);
                            if (iResult != 1)
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, $"{item.TableName} 格式有误,插入失败！", "") { },
                                };
                            }
                            else
                            {
                                iCount += iResult;
                            }
                        }
                    }
                    return new NewErrorModel()
                    {
                        count = iCount,
                        error = new Error(0, $"批量更新成功！共计更新:{iCount.ToString()}条数据", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "参数有误！", "") { },
                    };
                }
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
                if (cURDModel != null && cURDModel.SaveModels != null && cURDModel.SaveModels.Count > 0)
                {
                    DDContext dataContext = new DDContext();
                    SqlHelper sqlHelper = new SqlHelper();
                    if (cURDModel.SaveModels.Count == 1) //单表
                    {
                        foreach (var item in cURDModel.SaveModels)
                        {
                            List<TableInfo> tableInfos = dataContext.TableInfo.Where(t => t.TableID ==
                            dataContext.Tables.Where(s => s.TableName == item.TableName).FirstOrDefault().ID
                            ).ToList();
                            string strSql = sqlHelper.Read(item, tableInfos);

                            DataTable result = SqlAdoHelper.ExecuteDataTable(strSql);

                            //List<TestTable> testTables = dataContext.Database.SqlQuery<TestTable>(strSql).ToList();

                            //var results = dataContext.Database.SqlQuery<object>(strSql).ToList();

                            //dynamic result = dataContext.Database.SqlQuery<object>(strSql).ToList();

                            return new NewErrorModel()
                            {
                                data = result,
                                error = new Error(0, $"{item.TableName} 读取成功！", "") { },
                            };
                        }
                    }
                    else  //多表
                    {

                    }
                  
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "参数有误！", "") { },
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class TestTable
    {
        public string ColumnName3 { get; set; }
        public string ColumnName4 { get; set; }
        public int ColumnName6 { get; set; }
        public bool ColumnName7 { get; set; }
    }

    public class CURDModel
    {
        /// <summary>
        /// 操作人Id(用于判断权限)
        /// </summary>
        public string ApplyManId { get; set; }

        /// <summary>
        /// 表数据
        /// </summary>
        //public List<Tables> tables { get; set; }

        /// <summary>
        /// 表数据
        /// </summary>
        public List<CURDModelSave> SaveModels { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 页容量(默认每页5条)
        /// </summary>

        public int pageSize { get; set; }
    }

    public class CURDModelSave
    {
        public string TableName { get; set; }

        public string SqlWhere { get; set; }

        public List<Dictionary<string, object>> Columns { get; set; }

    }

    //public class column
    //{
    //    /// <summary>
    //    /// 字段名
    //    /// </summary>
    //    public string ColumnName { get; set; }

    //    /// <summary>
    //    /// 字段属性( 0 : string  1 : int  2 : bool)
    //    /// </summary>
    //    public int ColumnProperty { get; set; }

    //    /// <summary>
    //    /// 字段值
    //    /// </summary>
    //    public string ColumnValue { get; set; }
    //}
}
