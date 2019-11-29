using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DingTalk.Bussiness.EF;

namespace DingTalk.Controllers
{
    [RoutePrefix("TableManager")]
    public class TableManagerController : ApiController
    {
        /// <summary>
        /// 数据库表读取 
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Read")]
        public NewErrorModel Read(int flowId)
        {
            try
            {
                using (DDContext dataContext = new DDContext())
                {
                    List<Tables> tablles = dataContext.Tables.Where(t => t.FlowId == flowId).ToList();
                    List<TableInfo> tablleInfos = dataContext.TableInfo.ToList();
                    foreach (var item in tablles)
                    {
                        item.tableInfos = tablleInfos.Where(t => t.TableID == item.ID).ToList();
                    }

                    return new NewErrorModel()
                    {
                        data = tablles,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 数据库表动态创建
        /// </summary>
        /// <param name="tablle"></param>
        /// <returns></returns>)]
        [HttpPost]
        [Route("Save")]
        public NewErrorModel Save(Tables tablle)
        {
            try
            {
                using (DDContext dataContext = new DDContext())
                {
                    if (tablle != null)
                    {
                        if (tablle.tableInfos == null || tablle.tableInfos.Count == 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "表参数格式有误！", "") { },
                            };
                        }
                        else
                        {
                            //动态拼接创建表Sql
                            SqlHelper sqlHelper = new SqlHelper();
                            string strSql = sqlHelper.CreateTable(tablle);
                            int iResult = dataContext.Database.ExecuteSqlCommand(strSql);


                            //记录执行Sql
                            sqlHelper.SaveSqlExe(tablle, strSql, dataContext);
                            dataContext.Tables.Add(tablle);
                            dataContext.SaveChanges();

                            foreach (var item in tablle.tableInfos)
                            {
                                item.TableID = tablle.ID;
                            }
                            dataContext.TableInfo.AddRange(tablle.tableInfos);
                            dataContext.SaveChanges();
                        }
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "表参数格式有误！", "") { },
                        };
                    }
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 数据库表动态修改
        /// </summary>
        /// <param name="tablle"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Mofify")]
        public NewErrorModel Mofify(Tables tablle)
        {
            try
            {
                using (DDContext dataContext = new DDContext())
                {
                    if (tablle != null)
                    {
                        //动态拼接创建表Sql
                        SqlHelper sqlHelper = new SqlHelper();
                        Tables tablleOld = dataContext.Tables.Find(tablle.ID);
                        string strSql = sqlHelper.ModifyTable(tablle, tablleOld);
                        string strResult = dataContext.Database.SqlQuery<string>(strSql).ToString();

                        //记录执行Sql
                        sqlHelper.SaveSqlExe(tablle, strSql, dataContext);
                        //修改实体信息
                        dataContext.Entry<Tables>(tablle).State =
                            System.Data.Entity.EntityState.Modified;
                        dataContext.SaveChanges();

                        string strSqltablleInfos = string.Empty;
                        if (tablle.tableInfos != null && tablle.tableInfos.Count > 0)
                        {
                            strSqltablleInfos = sqlHelper.ModifyTableInfo(tablle.tableInfos, tablle);
                            //记录执行Sql
                            sqlHelper.SaveSqlExe(tablle, strSqltablleInfos, dataContext);
                            //修改实体信息
                            foreach (var item in tablle.tableInfos)
                            {
                                switch (item.operateType)
                                {
                                    case OperateType.Add:
                                        dataContext.TableInfo.Add(item);
                                        break;
                                    case OperateType.Delete:
                                        dataContext.TableInfo.Remove(item);
                                        break;
                                    case OperateType.Modify:
                                        dataContext.Entry<TableInfo>(item).State =
                                            System.Data.Entity.EntityState.Modified;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            dataContext.SaveChanges();
                        }
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "表参数格式有误！", "") { },
                        };
                    }
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
