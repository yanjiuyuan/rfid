using DingTalk.Controllers;
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
        /// 批量插入
        /// </summary>
        /// <param name="cURDModelSave"></param>
        /// <param name="tableInfos"></param>
        /// <returns></returns>
        public string Insert(CURDModelSave cURDModelSave, List<TableInfo> tableInfos)
        {

            StringBuilder sb = new StringBuilder();
            string tableName = cURDModelSave.TableName;
            string columnName = string.Empty;
            string columnValue = string.Empty;
            foreach (var columnsList in cURDModelSave.Columns)
            {
                int i = 0;
                foreach (var column in columnsList)
                {
                    i++;
                    int iColumnProperty = tableInfos.Where(t => t.ColumnName == column.Key.ToString()).FirstOrDefault().ColumnProperty;

                    if (iColumnProperty == 0) //string 
                    {
                        if (i == columnsList.Count)
                        {
                            columnName += column.Key;
                            columnValue += "'" + column.Value + "'";
                        }
                        else
                        {
                            columnName += column.Key + ",";
                            columnValue += "'" + column.Value + "'" + ",";
                        }
                    }
                    if (iColumnProperty == 1) //int
                    {
                        if (i == columnsList.Count)
                        {
                            columnName += column.Key;
                            columnValue += column.Value;
                        }
                        else
                        {
                            columnName += column.Key + ",";
                            columnValue += column.Value + ",";
                        }
                    }

                    if (iColumnProperty == 2) //bool
                    {
                        if (i == columnsList.Count)
                        {
                            columnName += column.Key;
                            columnValue += (column.Value.ToString().ToLower() == "true" ? "1" : "0");
                        }
                        else
                        {
                            columnName += column.Key + ",";
                            columnValue += (column.Value.ToString().ToLower() == "true" ? "1" : "0") + ",";
                        }
                    }
                }
            }
            sb.Append($" insert into {tableName}({columnName}) values({columnValue}) ;");
            return sb.ToString();
        }


        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="cURDModelSave"></param>
        /// <param name="tableInfos"></param>
        /// <returns></returns>
        public string Modify(CURDModelSave cURDModelSave, List<TableInfo> tableInfos)
        {
            StringBuilder sb = new StringBuilder();
            string tableName = cURDModelSave.TableName;
            List<string> columnNameAndValue = new List<string>();
            string id = string.Empty;
            foreach (var columnsList in cURDModelSave.Columns)
            {
                foreach (var column in columnsList)
                {
                    if (column.Key.ToString().ToLower() == "id")
                    {
                        id = column.Value.ToString();
                    }
                    else
                    {
                        TableInfo tableInfo = tableInfos.Where(t => t.ColumnName == column.Key.ToString()).FirstOrDefault();
                        int iColumnProperty = tableInfo.ColumnProperty;
                        bool IsModify = tableInfo.IsSupportModify;
                        if (IsModify) //当前字段是否支持修改
                        {
                            if (iColumnProperty == 0) //string 
                            {
                                columnNameAndValue.Add($"  {column.Key}='{column.Value }'");
                            }
                            if (iColumnProperty == 1) //int
                            {
                                columnNameAndValue.Add($"  {column.Key}={column.Value }");
                            }
                            if (iColumnProperty == 2) //bool
                            {
                                columnNameAndValue.Add($"  {column.Key}={(column.Value.ToString().ToLower() == "true" ? "1" : "0")}");
                            }
                        }
                    }
                }
            }
            sb.Append($" update {tableName} set  {string.Join(",", columnNameAndValue)} where id={id};");
            return sb.ToString();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cURDModelSave"></param>
        /// <returns></returns>
        public string Delete(CURDModelSave cURDModelSave)
        {

            StringBuilder sb = new StringBuilder();
            string tableName = cURDModelSave.TableName;
            string columnName = string.Empty;
            string columnValue = string.Empty;
            List<string> strWhereList = new List<string>();
            foreach (var columnsList in cURDModelSave.Columns)
            {
                foreach (var column in columnsList)
                {
                    if (column.Key.ToLower() == "id")
                    {
                        strWhereList.Add($" id={column.Value} ");
                    }
                }
            }
            sb.Append($" delete  from  {tableName} where {string.Join(" or ", strWhereList)}");
            return sb.ToString();
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="cURDModelSave"></param>
        /// <param name="tableInfos"></param>
        /// <returns></returns>
        public string Read(CURDModelSave cURDModelSave, List<TableInfo> tableInfos)
        {
            StringBuilder sb = new StringBuilder();
            string tableName = cURDModelSave.TableName;
            List<string> strWhereAnd = new List<string>();
            List<string> strWhereOr = new List<string>();
            string id = string.Empty;
            foreach (var columnsList in cURDModelSave.Columns)
            {
                foreach (var column in columnsList)
                {
                    if (column.Key.ToString().ToLower() == "id")
                    {
                        //直接返回
                        return $" select * from {tableName} where id={column.Value.ToString()}";
                    }
                    else
                    {
                        TableInfo tableInfo = tableInfos.Where(t => t.ColumnName == column.Key.ToString()).FirstOrDefault();
                        int iColumnProperty = tableInfo.ColumnProperty;
                        bool IsSupportQuery = tableInfo.IsSupportQuery;
                        bool IsSupporLikeQuery = tableInfo.IsSupporLikeQuery;
                        bool IsAnd = tableInfo.IsAnd;
                        if (IsSupportQuery) //当前字段是否支持查询
                        {
                            if (iColumnProperty == 2) //bit
                            {
                                if (IsAnd)
                                {
                                    strWhereAnd.Add($" {column.Key}={(column.Value.ToString().ToLower() == "true" ? "1" : "0")}");
                                }
                                else
                                {
                                    strWhereOr.Add($" {column.Key}={(column.Value.ToString().ToLower() == "true" ? "1" : "0")}");
                                }

                            }
                            if (IsSupporLikeQuery) //是否支持模糊查询
                            {
                                if (iColumnProperty == 0) //string 
                                {
                                    if (IsAnd)
                                    {
                                        strWhereAnd.Add($"  { column.Key } like  '%{ column.Value }%'");
                                    }
                                    else
                                    {
                                        strWhereOr.Add($"  { column.Key } like  '%{ column.Value }%'");
                                    }

                                }
                                if (iColumnProperty == 1) //int
                                {
                                    if (IsAnd)
                                    {
                                        strWhereAnd.Add($"  { column.Key } like '%{column.Value }%'");
                                    }
                                    else
                                    {
                                        strWhereOr.Add($"  { column.Key } like '%{column.Value }%'");
                                    }

                                }
                            }
                            else
                            {
                                if (iColumnProperty == 0) //string 
                                {
                                    if (IsAnd)
                                    {
                                        strWhereAnd.Add($"  { column.Key } =  '{ column.Value }' ");
                                    }
                                    else
                                    {
                                        strWhereOr.Add($"  { column.Key } =  '{ column.Value }' ");
                                    }

                                }
                                if (iColumnProperty == 1) //int
                                {
                                    if (IsAnd)
                                    {
                                        strWhereAnd.Add($"  { column.Key } = {column.Value } ");
                                    }
                                    else
                                    {
                                        strWhereOr.Add($"  { column.Key } = {column.Value } ");
                                    }

                                }
                                if (iColumnProperty == 2) //bool
                                {
                                    if (IsAnd)
                                    {
                                        strWhereAnd.Add($"  { column.Key } = {(column.Value.ToString().ToLower() == "true" ? "1" : "0")}");
                                    }
                                    else
                                    {
                                        strWhereOr.Add($"  { column.Key } = {(column.Value.ToString().ToLower() == "true" ? "1" : "0")}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            sb.Append($"  select * from  {tableName}  where 1=1  ");
            if (strWhereAnd.Count > 0)
            {
                sb.Append($" {" and" +"(" + string.Join("and", strWhereAnd) + ")"};");
            }
            if (strWhereOr.Count > 0)
            {
                sb.Append($" {" and" + "(" + string.Join("or", strWhereOr) + ")"};");
            }
            if (!string.IsNullOrEmpty(cURDModelSave.SqlWhere))
            {
                sb.Append(" " + cURDModelSave.SqlWhere);
            }
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