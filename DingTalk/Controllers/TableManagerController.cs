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
        /// 外键搜索(用于赋值)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OukKeyRead")]
        public NewErrorModel OukKeyRead()
        {
            try
            {
                using (DDContext dataContext = new DDContext())
                {
                    List<Tables> tablles = dataContext.Tables.ToList();
                    List<TableInfo> tablleInfos = dataContext.TableInfo.ToList();

                    foreach (var item in tablles)
                    {
                        item.tableInfos = tablleInfos.Where(t => t.TableID == item.ID).ToList();
                    }
                    //ColumnNameOld赋值
                    foreach (var item in tablles)
                    {
                        foreach (var tbinfo in item.tableInfos)
                        {
                            tbinfo.ColumnNameOld = tbinfo.ColumnName;
                        }
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
        /// 外键绑定
        /// </summary>
        /// <param name="pks"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OukKeySave")]
        public NewErrorModel OukKeySave(Pks pks)
        {
            try
            {
                using (DDContext dataContext = new DDContext())
                {
                    TableInfo tableInfo = dataContext.TableInfo.Where(t => t.ID == pks.ColumnId).FirstOrDefault();

                    if (tableInfo.ColumnProperty == 0 || tableInfo.ColumnProperty == 1)
                    {
                        dataContext.Pks.Add(pks);
                        dataContext.Pks.Add(new Pks()
                        {
                            ColumnId = pks.OutColumnId,
                            OutColumnId = pks.ColumnId
                        });
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "bool类型的字段不能用于外键绑定！", "") { },
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
        /// 外键删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OukKeyDel")]
        public NewErrorModel OukKeyDel(int Id)
        {
            try
            {
                using (DDContext dataContext = new DDContext())
                {
                    Pks pks = dataContext.Pks.Find(Id);
                    Pks pksPro = dataContext.Pks.Where(p => p.ColumnId == pks.OutColumnId).FirstOrDefault();
                    dataContext.Pks.Remove(pks);
                    dataContext.Pks.Remove(pksPro);
                    dataContext.SaveChanges();
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
        /// 数据库表读取 
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Read")]
        public NewErrorModel Read(int flowId)
        {
            try
            {
                using (DDContext dataContext = new DDContext())
                {
                    List<Tables> tabllesAll = dataContext.Tables.ToList();
                    List<Tables> tablles = tabllesAll.Where(t => t.FlowId == flowId).ToList();
                    List<TableInfo> tablleInfos = dataContext.TableInfo.ToList();
                    List<Pks> pksList = dataContext.Pks.ToList();
                    foreach (var item in tablles)
                    {
                        item.tableInfos = tablleInfos.Where(t => t.TableID == item.ID).ToList();
                    }
                    //ColumnNameOld赋值
                    foreach (var item in tablles)
                    {
                        foreach (var tbinfo in item.tableInfos)
                        {
                            List<Pks> pks = pksList.Where(p => p.ColumnId == tbinfo.ID).ToList();

                            if (pks.Count > 0)
                            {
                                //查找关联的字段名和表名
                                foreach (var pk in pks)
                                {
                                    TableInfo tableInfo = tablleInfos.Where(t => t.ID == pk.OutColumnId).FirstOrDefault();
                                    pk.ColumName = tableInfo.ColumnName;
                                    Tables tables = tabllesAll.Where(t => t.ID == tableInfo.TableID).FirstOrDefault();
                                    pk.TableName = tables.TableName;
                                }
                                tbinfo.Pks = pks;
                            }
                            tbinfo.ColumnNameOld = tbinfo.ColumnName;
                        }
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
                        if (tablle.operateType == OperateType.None)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "请尝试修改OperateType参数！", "") { },
                            }; ;
                        }
                        if (tablle.operateType == OperateType.Add)
                        {
                            return Save(tablle);
                        }

                        //动态拼接创建表Sql
                        SqlHelper sqlHelper = new SqlHelper();
                        string strSql = string.Empty;
                        Tables tablleOld = dataContext.Tables.AsNoTracking().Where(t => t.ID == tablle.ID).FirstOrDefault();
                        if (tablle.operateType == OperateType.Modify)
                        {
                            strSql = sqlHelper.ModifyTable(tablle, tablleOld);
                        }

                        //删除表处理
                        if (tablle.operateType == OperateType.Delete)
                        {
                            //判断表是否存在防止误操作
                            if (dataContext.Tables.Where(t => t.TableName == tablle.TableName).ToList().Count == 0)
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(0, "表不存在！", "") { },
                                };
                            }
                        }

                        int iResult = dataContext.Database.ExecuteSqlCommand(strSql);

                        //删除表处理
                        if (tablle.operateType == OperateType.Delete)
                        {
                            dataContext.Tables.Remove(dataContext.Tables.Find(tablle.ID));
                            dataContext.TableInfo.RemoveRange(dataContext.TableInfo.Where(t => t.TableID == tablle.ID));
                            dataContext.SaveChanges();
                            return new NewErrorModel()
                            {
                                error = new Error(0, "删除成功！", "") { },
                            };
                        }

                        //记录执行Sql
                        sqlHelper.SaveSqlExe(tablle, strSql, dataContext);
                        //修改实体信息
                        tablleOld = tablle;
                        dataContext.Entry<Tables>(tablleOld).State =
                            System.Data.Entity.EntityState.Modified;
                        dataContext.SaveChanges();

                        string strSqltablleInfos = string.Empty;
                        if (tablle.tableInfos != null && tablle.tableInfos.Count > 0)
                        {
                            strSqltablleInfos = sqlHelper.ModifyTableInfo(tablle.tableInfos, tablle);
                            int iResultstrSqltablleInfos = dataContext.Database.ExecuteSqlCommand(strSqltablleInfos);
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
                                        dataContext.TableInfo.Remove(dataContext.TableInfo.Find(item.ID));
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
