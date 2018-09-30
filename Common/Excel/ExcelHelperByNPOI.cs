using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Excel
{
    public class ExcelHelperByNPOI
    {
        #region 从datatable中将数据导出到excel
        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        static MemoryStream ExportDT(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = workbook.CreateSheet() as HSSFSheet;

            #region 右击文件 属性信息

            //{
            //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //    dsi.Company = "http://www.yongfa365.com/";
            //    workbook.DocumentSummaryInformation = dsi;

            //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //    si.Author = "柳永法"; //填加xls文件作者信息
            //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息
            //    si.LastAuthor = "柳永法2"; //填加xls文件最后保存者信息
            //    si.Comments = "说明信息"; //填加xls文件作者信息
            //    si.Title = "NPOI测试"; //填加xls文件标题信息
            //    si.Subject = "NPOI测试Demo"; //填加文件主题信息
            //    si.CreateDateTime = DateTime.Now;
            //    workbook.SummaryInformation = si;
            //}

            #endregion

            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet() as HSSFSheet;
                    }

                    #region 表头及样式

                    {
                        HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;

                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                    }

                    #endregion


                    #region 列头及样式

                    {
                        HSSFRow headerRow = sheet.CreateRow(1) as HSSFRow;


                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);


                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                        }
                        //headerRow.Dispose();
                    }

                    #endregion

                    rowIndex = 2;
                }

                #endregion

                #region 填充内容

                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            double result;
                            if (isNumeric(drValue, out result))
                            {

                                double.TryParse(drValue, out result);
                                newCell.SetCellValue(result);
                                break;
                            }
                            else
                            {
                                newCell.SetCellValue(drValue);
                                break;
                            }

                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }

                #endregion

                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet;
                //workbook.Dispose();

                return ms;
            }
        }

        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        public static void ExportDTtoExcel(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = ExportDT(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region 从excel2003 或 2007中将数据导出到datatable
        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName)
        {
            DataTable dt = new DataTable();
            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = hssfworkbook.GetSheetAt(0) as HSSFSheet;
            dt = ImportExcel2003InDt(sheet, 0, true);
            return dt;
        }

        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel2007toDt(string strFileName)
        {
            DataTable dt = new DataTable();
            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new XSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            dt = ImportExcel2007InDt(sheet, 0, true);
            return dt;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName, string SheetName, int HeaderRowIndex)
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2003InDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2007toDt(string strFileName, string SheetName, int HeaderRowIndex)
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2007InDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName, int SheetIndex, int HeaderRowIndex)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2003InDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2007toDt(string strFileName, int SheetIndex, int HeaderRowIndex)
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2007InDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName, string SheetName, int HeaderRowIndex, bool needHeader)
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2003InDt(sheet, HeaderRowIndex, needHeader);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName, int SheetIndex, int HeaderRowIndex, bool needHeader)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2003InDt(sheet, HeaderRowIndex, needHeader);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        static DataTable ImportExcel2003InDt(ISheet sheet, int HeaderRowIndex, bool needHeader)
        {
            DataTable table = new DataTable();
            HSSFRow headerRow;
            int cellCount;
            try
            {
                if (HeaderRowIndex < 0 || !needHeader)
                {
                    headerRow = sheet.GetRow(0) as HSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column);
                    }
                }
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex) as HSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                            }

                        }
                        else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        {
                            DataColumn column = new DataColumn(headerRow.GetCell(i).ToString());
                            table.Columns.Add(column);
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        HSSFRow row;
                        if (sheet.GetRow(i) == null)
                        {
                            row = sheet.CreateRow(i) as HSSFRow;
                        }
                        else
                        {
                            row = sheet.GetRow(i) as HSSFRow;
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)
                        {
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.String:
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[j] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[j] = null;
                                            }
                                            break;
                                        case CellType.Numeric:
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                            {
                                                dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.Boolean:
                                            dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.Error:
                                            dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.Formula:
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.String:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[j] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.Numeric:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.Boolean:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.Error:
                                                    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[j] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[j] = "";
                                            break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                //wl.WriteLogs(exception.ToString());
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        //wl.WriteLogs(exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
            }
            return table;
        }

        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        static DataTable ImportExcel2007InDt(ISheet sheet, int HeaderRowIndex, bool needHeader)
        {
            DataTable table = new DataTable();
            NPOI.XSSF.UserModel.XSSFRow headerRow;
            int cellCount;
            try
            {
                if (HeaderRowIndex < 0 || !needHeader)
                {
                    headerRow = sheet.GetRow(0) as NPOI.XSSF.UserModel.XSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column);
                    }
                }
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex) as NPOI.XSSF.UserModel.XSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                            }

                        }
                        else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        {
                            DataColumn column = new DataColumn(headerRow.GetCell(i).ToString());
                            table.Columns.Add(column);
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        NPOI.XSSF.UserModel.XSSFRow row;
                        if (sheet.GetRow(i) == null)
                        {
                            row = sheet.CreateRow(i) as NPOI.XSSF.UserModel.XSSFRow;
                        }
                        else
                        {
                            row = sheet.GetRow(i) as NPOI.XSSF.UserModel.XSSFRow;
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)
                        {
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.String:
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[j] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[j] = null;
                                            }
                                            break;
                                        case CellType.Numeric:
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                            {
                                                dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.Boolean:
                                            dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.Error:
                                            dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.Formula:
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.String:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[j] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.Numeric:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.Boolean:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.Error:
                                                    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[j] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[j] = "";
                                            break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                //wl.WriteLogs(exception.ToString());
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        //wl.WriteLogs(exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
            }
            return table;
        }
        #endregion 

        #region 更新excel中的数据
        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[] updateData, int coluid, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {
                try
                {
                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluid);
                    }

                    sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);
                }
                catch (Exception ex)
                {
                    // wl.WriteLogs(ex.ToString());
                    throw;
                }
            }
            try
            {
                readfile.Close();
                FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                // wl.WriteLogs(ex.ToString());
            }

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                        }
                        sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);
                    }
                    catch (Exception ex)
                    {
                        // wl.WriteLogs(ex.ToString());
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.ToString());
            }
        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        /// <param name="IsTrue">true：以横向输出; false：以横向输出</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[] updateData, int coluid, int rowid, bool IsTrue)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {
                try
                {
                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluid);
                    }

                    if (IsTrue)
                    {
                        sheet1.GetRow(rowid).GetCell(coluid + i).SetCellValue(updateData[i]);
                    }
                    else
                    {
                        sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);
                    }

                }
                catch (Exception ex)
                {
                    //wl.WriteLogs(ex.ToString());
                    throw;
                }
            }
            try
            {
                readfile.Close();
                FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.ToString());
            }

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                        }
                        sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);
                    }
                    catch (Exception ex)
                    {
                        //wl.WriteLogs(ex.ToString());
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.ToString());
            }
        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static bool UpdateExcel(string outputFile, string sheetname, DataTable updateData, int coluids, int rowid)
        {
            try
            {
                if (!File.Exists(outputFile))
                {
                    return false;
                }

                FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);
                XSSFWorkbook hssfworkbook = new XSSFWorkbook(readfile);
                ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
                for (int i = 0; i < updateData.Rows.Count; i++)
                {
                    for (int j = 0; j < updateData.Columns.Count; j++)
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(j + coluids) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(j + coluids);
                        }
                        sheet1.GetRow(i + rowid).GetCell(j + coluids).SetCellValue(updateData.Rows[i][j].ToString());
                    }
                }
                try
                {
                    readfile.Close();
                    FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                    hssfworkbook.Write(writefile);
                    writefile.Close();
                }
                catch (Exception ex)
                {
                    //wl.WriteLogs(ex.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion

        #region 读取excel中Sheet个数
        public static int GetSheetNumber(string outputFile)
        {
            int number = 0;
            try
            {
                FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

                HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
                number = hssfworkbook.NumberOfSheets;

            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
            }
            return number;
        }
        #endregion

        #region 读取Sheet名称
        public static ArrayList GetSheetName(string outputFile)
        {
            ArrayList arrayList = new ArrayList();
            try
            {
                FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

                HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
                for (int i = 0; i < hssfworkbook.NumberOfSheets; i++)
                {
                    arrayList.Add(hssfworkbook.GetSheetName(i));
                }
            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
            }
            return arrayList;
        }
        #endregion

        #region 判断格式
        public static bool isNumeric(String message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;
        }
        #endregion

    }
}
