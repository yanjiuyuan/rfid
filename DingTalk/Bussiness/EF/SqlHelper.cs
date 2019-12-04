using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DingTalk.Bussiness.EF
{
    public class SqlHelper
    {


        public string CreateTable(Tables table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($" CREATE TABLE [dbo].[{table.TableName}] (");
            sb.Append($" [Id] [numeric] (18, 0) IDENTITY(1,1) NOT NULL,");
            foreach (var item in table.tableInfos)
            {
                switch (item.ColumnProperty)
                {
                    case 0:  //string
                        sb.Append($" [{item.ColumnName}] [varchar]({item.ColumnLength}) {(item.IsNull ? "NULL" : "NOT NULL")},");
                        break;
                    case 1: //int
                        sb.Append($" [{item.ColumnName}] [int] {(item.IsNull ? "NULL" : "NOT NULL")},");
                        break;
                    case 2: //bool
                        sb.Append($" [{item.ColumnName}] [bit] {(item.IsNull ? "NULL" : "NOT NULL")},");
                        break;
                }
            }
            sb.Append("   ) ON[PRIMARY]");
            return sb.ToString();
        }

        /// <summary>
        /// 修改表名
        /// </summary>
        /// <param name="tableNew">新表</param>
        /// <param name="tableOld">旧表</param>
        /// <returns></returns>
        public string ModifyTable(Tables tableNew, Tables tableOld)
        {
            StringBuilder sb = new StringBuilder();
            switch (tableNew.operateType)
            {
                //case Models.OperateType.Add:
                //    sb.Append("");
                //    break;
                case OperateType.Modify:
                    sb.Append($" EXEC sp_rename '{tableOld.TableName}', '{tableNew.TableName}'");
                    break;
                case OperateType.Delete:
                    sb.Append($" drop table {tableNew.TableName}");
                    break;

            }
            return sb.ToString();
        }


        public string ModifyTableInfo(List<TableInfo> tablleInfos, Tables tablle)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tablleInfo in tablleInfos)
            {
                switch (tablleInfo.operateType)
                {
                    case OperateType.Add: //新增列
                        sb.Append($" alter table {tablle.TableName}  add  {tablleInfo.ColumnName}        {CheckColumnType(tablleInfo)};");
                        break;
                    case OperateType.Delete: //删除列
                        sb.Append($" alter table {tablle.TableName}  drop column {tablleInfo.ColumnName};");
                        break;
                    case OperateType.Modify:
                        //修改字段名
                        sb.Append($" EXEC sp_rename '{tablle.TableName}.{tablleInfo.ColumnNameOld}', '{tablleInfo.ColumnName}', 'column'");
                        //修改字段长度
                        if (tablleInfo.ColumnProperty == 0)
                        {
                            sb.Append($" alter table {tablle.TableName} alter {tablleInfo.ColumnName} test NVARCHAR({tablleInfo.ColumnLength})");
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        public string CheckColumnType(TableInfo tablleInfo)
        {
            switch (tablleInfo.ColumnProperty)
            {
                case 0:  //string
                    return $" varchar({tablleInfo.ColumnLength}) {(tablleInfo.IsNull ? "NULL" : "NOT NULL")}";
                case 1: //int
                    return $"  int  {(tablleInfo.IsNull ? "NULL" : "NOT NULL")}";
                case 2: //bool
                    return $"  bit  {(tablleInfo.IsNull ? "NULL" : "NOT NULL")}";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 通用读取
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public string CommomCURDRead(Tables tables)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($" select * from {tables.TableName} where 1=1 ");
            foreach (var item in tables.tableInfos)
            {
                if (item.IsSupportQuery)
                {
                    switch (item.ColumnProperty)
                    {
                        //string
                        case 0:
                            sb.Append($" and {item.ColumnName}='{item.Value}' ");
                            break;
                        //int
                        case 1:
                            sb.Append($" and {item.ColumnName}={item.Value} ");
                            break;
                        //bool
                        case 2:
                            sb.Append($" and {item.ColumnName}={item.Value} ");
                            break;
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 插入 
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public string Insert(Tables tables)
        {
            StringBuilder sb = new StringBuilder();
            string columnName = string.Empty;
            string columnValue = string.Empty;
            foreach (var item in tables.tableInfos)
            {
                if (item.ColumnProperty == 0) //string 
                {
                    if (tables.tableInfos.IndexOf(item) == tables.tableInfos.Count - 1)
                    {
                        columnName += item.ColumnName;
                        columnValue += "'" + item.Value + "'";
                    }
                    else
                    {
                        columnName += item.ColumnName + ",";
                        columnValue += "'" + item.Value + "'" + ",";
                    }
                }
                if (item.ColumnProperty == 1) //int
                {
                    if (tables.tableInfos.IndexOf(item) == tables.tableInfos.Count - 1)
                    {
                        columnName += item.ColumnName;
                        columnValue += item.Value;
                    }
                    else
                    {
                        columnName += item.ColumnName + ",";
                        columnValue += item.Value + ",";
                    }
                }

                if (item.ColumnProperty == 2) //bool
                {
                    if (tables.tableInfos.IndexOf(item) == tables.tableInfos.Count - 1)
                    {
                        columnName += item.ColumnName;
                        columnValue += (item.Value.ToLower()=="true"?"1":"0");
                    }
                    else
                    {
                        columnName += item.ColumnName + ",";
                        columnValue += (item.Value.ToLower() == "true" ? "1" : "0") + ",";
                    }
                }

            }
            sb.Append($" insert into {tables.TableName}({columnName}) values({columnValue})");
            return sb.ToString();
        }


        /// <summary>
        /// 记入Sql
        /// </summary>
        /// <param name="tablle"></param>
        /// <param name="strSql"></param>
        /// <param name="dataContext"></param>

        public void SaveSqlExe(Tables tablle, string strSql, DDContext dataContext)
        {
            dataContext.SqlExe.Add(new SqlExe()
            {
                TableName = tablle.TableName,
                ApplyMan = tablle.CreateMan,
                ApplyManId = tablle.CreateManId,
                Sql = strSql,
                DateTime = DateTime.Now
            });
        }
    }
}