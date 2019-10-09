using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.PDF
{
    public class PDFHelper
    {
        private static PDFHelper instance;
        public static PDFHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new PDFHelper();
            }
            return instance;
        }

        private static Document doc;
        private static BaseFont bf = BaseFont.CreateFont(@"C://Windows/Fonts/simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

        //定义字体
        private static Font fontBig = new Font(bf, 15, Font.BOLD);
        private static Font fontMiddle = new Font(bf, 15, Font.BOLD);
        private static Font fontSmall = new Font(bf, 13, Font.BOLD);
        private static Font fontSmallest = new Font(bf, 11, Font.BOLD);
        private static Font fontSmallNoBold = new Font(bf, 12);
        private static Font fontTableSmallNoBold = new Font(bf, 8);
        private static float IndentationLeft = 50;//距左边距
        private static float IndentationCenter = 220;//距左边距
        /// <summary>
        /// 绘制PDF
        /// </summary>
        /// <param name="FlowName">流程名</param>
        /// <param name="TaskId">流水号</param>
        /// <param name="ApplyName">申请人</param>
        /// <param name="Dept">申请部门</param>
        /// <param name="ApplyTime">申请时间</param>
        /// <param name="ProjectName">项目名</param>
        /// <param name="ProjectNo">项目编号</param>
        /// <param name="ImageNo">图片编号</param>
        /// <param name="ImageX">盖章X轴</param>
        /// <param name="ImageY">盖章Y轴</param>
        /// <param name="FlowId">流程Id</param>
        /// <param name="FilePath">水印图片路径</param>
        /// <param name="contentList">表单头数组</param>
        /// <param name="contentWithList">表单宽度数组</param>
        /// <param name="dtSourse">表单数据</param>
        /// <param name="dtApproveView">审批意见数据</param>
        ///  <param name="keyValuePairs">表单单列数据</param>
        ///  <param name="keyValuePairsHead">表单单列列头数据</param>
        public string GeneratePDF(string FlowName, string TaskId, string ApplyName, string Dept,
            string ApplyTime, string ProjectName, string ProjectNo, string ImageNo, float ImageX, float ImageY
            , List<string> contentList, float[] contentWithList
            , DataTable dtSourse, DataTable dtApproveView, Dictionary<string, string> keyValuePairs, Dictionary<string, string> keyValuePairsHead = null)
        {
            doc = new Document(PageSize.A4);
            try
            {
                string fileName = string.Format("数据_{0}.pdf", DateTime.Now.ToString("yyyyMMddHHmmss"));
                string filePath = string.Format("{0}UploadFile\\PDF\\{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
                FileStream fs = new FileStream(filePath, FileMode.Create);//创建临时文件，到时生成好后删除
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                writer.CloseStream = false;//把doc内容写入流中
                doc.Open();

                iTextSharp.text.Image imageLogo = iTextSharp.text.Image.GetInstance(
                    string.Format(@"{0}\Content\images\单位LOGO.jpg", AppDomain.CurrentDomain.BaseDirectory));
                //imageLogo.Width = 100;
                imageLogo.SetAbsolutePosition(85, 780);
                writer.DirectContent.AddImage(imageLogo);

                AddHeaderTitleContent("泉州华中科技大学智能制造研究院", fontSmallest, 80);
                CreateEmptyRow(1);//生成一行空行
                CreateLine();//生成一条下横线
                //CreateEmptyRow(1);//生成一行空行

                AddHeaderTitleContent(FlowName, fontSmallNoBold, IndentationCenter);//添加表头
                //CreateEmptyRow(1);//生成一行空行

                if (!string.IsNullOrEmpty(TaskId)) { AddPartnerContents("   流水号", TaskId, "申请人", ApplyName); }
                if (!string.IsNullOrEmpty(TaskId)) { AddPartnerContents("申请时间", ApplyTime.Substring(0, 10), "申请部门", Dept); }
                if (!string.IsNullOrEmpty(ProjectName)) { AddSinglePartnerContents("项目", ProjectNo + "-" + ProjectName); }
                if (keyValuePairsHead != null)
                {
                    foreach (var item in keyValuePairsHead.Keys)
                    {
                        AddSinglePartnerContents(item, keyValuePairsHead[item]);
                    }
                }
                AddPageNumberContent();//添加页码
                CreateEmptyRow(1);//生成一行空行

                //设置审批水印
                if (!string.IsNullOrEmpty(filePath))
                {
                    string ImgaePath = "";
                    if (ImageNo == "1")
                    {
                        ImgaePath = string.Format(@"{0}\Content\images\受控章.png", AppDomain.CurrentDomain.BaseDirectory);
                    }
                    if (ImageNo == "2")
                    {
                        ImgaePath = string.Format(@"{0}\Content\images\approveSmall.png", AppDomain.CurrentDomain.BaseDirectory);
                    }
                    if (ImageNo == "3")
                    {
                        ImgaePath = string.Format(@"{0}\Content\images\变更章.png", AppDomain.CurrentDomain.BaseDirectory);
                    }
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(ImgaePath);
                    //image.SetAbsolutePosition(300, 650);
                    image.SetAbsolutePosition(ImageX, ImageY);
                    writer.DirectContent.AddImage(image);
                }


                #region 生成表格数据


                if (contentList != null)
                {
                    PdfPTable table = new PdfPTable(contentList.Count);

                    //添加表格列头   
                    foreach (var item in contentList)
                    {
                        table.AddCell(GetPdfCell(item, fontTableSmallNoBold, Element.ALIGN_CENTER));
                    }
                    //添加表格列头宽度   
                    table.SetTotalWidth(contentWithList);

                    if (dtSourse.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtSourse.Rows.Count; i++)
                        {
                            table.AddCell(GetPdfCell((i + 1).ToString(), fontTableSmallNoBold, Element.ALIGN_CENTER));
                            for (int j = 0; j < dtSourse.Columns.Count; j++)
                            {
                                table.AddCell(GetPdfCell((dtSourse.Rows[i][j]).ToString(), fontTableSmallNoBold, Element.ALIGN_CENTER));
                            }
                        }
                    }
                    doc.Add(table);
                }
                #endregion

                #region 生成表格单行数据

                if (keyValuePairs != null)
                {
                    CreateEmptyRow(1);//生成一行空行
                    PdfPTable table = new PdfPTable(2);
                    float[] fList = { 60, 200 };
                    table.SetTotalWidth(fList);
                    foreach (var item in keyValuePairs.Keys)
                    {
                        table.AddCell(GetPdfCell(item, fontTableSmallNoBold, Element.ALIGN_CENTER));
                        table.AddCell(GetPdfCell(keyValuePairs[item], fontTableSmallNoBold, Element.ALIGN_LEFT));
                    }
                    doc.Add(table);
                }

                #endregion

                #region 打印审批人

                CreateEmptyRow(1);//生成一行空行

                //int iResult = 1;  //每行打印数量

                if (dtApproveView.Rows.Count > 0)
                {
                    //int j = 0;
                    Paragraph content = new Paragraph();
                    for (int i = 0; i < dtApproveView.Rows.Count; i++)
                    {
                        content.IndentationLeft = IndentationLeft;
                        Chunk chunkName = new Chunk(dtApproveView.Rows[i]["NodeName"].ToString() + ":", fontSmallNoBold);
                        Chunk chunkText = new Chunk(dtApproveView.Rows[i]["NodePeople"].ToString(), fontSmallNoBold);
                        content.Add(0, chunkName);
                        content.Add(1, chunkText);
                        content.Alignment = 30;
                        //j++;
                        //if (i != 0 && i % iResult == 0 && i!= iResult)
                        //{
                        //    doc.Add(content);
                        //    j = 0;
                        //    content.Clear();
                        //}
                        doc.Add(content);
                        content.Clear();
                    }
                    //if (dtApproveView.Rows.Count <= iResult)
                    //{
                    //    doc.Add(content);
                    //    content.Clear();
                    //}
                }

                #endregion

                #region 添加文字水印
                string waterMarkName = "";

                #endregion

                doc.Close();
                MemoryStream ms = new MemoryStream();
                if (fs != null)
                {
                    byte[] bytes = new byte[fs.Length];//定义一个长度为fs长度的字节数组
                    fs.Read(bytes, 0, (int)fs.Length);//把fs的内容读到字节数组中
                    ms.Write(bytes, 0, bytes.Length);//把字节内容读到流中
                    fs.Flush();
                    fs.Close();
                }
                MemoryStream waterMS = SetWaterMark(ms, filePath, waterMarkName);//先生成水印，再删除临时文件


                //if (File.Exists(filePath))//判断临时文件是否存在，如果存在则删除
                //{
                //    File.Delete(filePath);
                //    GC.Collect();//回收垃圾
                //}
                //SendFile(fileName, waterMS);//把PDF文件发送回浏览器

                return filePath;
            }
            catch (DocumentException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #region 图片水印
        /// <summary>
        /// 加图片水印
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="ModelPicName"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public string PDFWatermark(string inputfilepath, string outputfilepath, string ModelPicName, float top, float left)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                if (!File.Exists(inputfilepath))
                {
                    return inputfilepath + "不存在！";
                }

                pdfReader = new PdfReader(inputfilepath);

                int numberOfPages = pdfReader.NumberOfPages;

                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);

                float width = psize.Width;

                float height = psize.Height;

                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.OpenOrCreate));

                PdfContentByte waterMarkContent;

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(ModelPicName);

                image.GrayFill = 80;//透明度，灰色填充
                                    //image.Rotation = 40;//旋转
                                    //image.RotationDegrees = 40;//旋转角度
                                    //水印的位置 
                if (left < 0)
                {
                    left = width / 2 - image.Width + left;
                }

                //image.SetAbsolutePosition(left, (height - image.Height) - top);
                image.SetAbsolutePosition(left, (height / 2 - image.Height) - top);

                //image.SetAbsolutePosition(200, 200);
                //每一页加水印,也可以设置某一页加水印 
                for (int i = 1; i <= numberOfPages; i++)
                {
                    //waterMarkContent = pdfStamper.GetUnderContent(i);//内容下层加水印
                    waterMarkContent = pdfStamper.GetOverContent(i);//内容上层加水印
                    waterMarkContent.AddImage(image);
                }
                pdfStamper.Close();
                pdfReader.Close();
                return outputfilepath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //if (pdfStamper != null)
                //    pdfStamper.Close();
                //if (pdfReader != null)
                //    pdfReader.Close();
            }
        }
        #endregion

        #region 生成一条横线
        private static void CreateLine()
        {
            PdfPTable table = new PdfPTable(1);//一个单元格的
            table.TotalWidth = 500;
            PdfPCell cell = new PdfPCell();
            cell.BorderWidthBottom = 0.5f;
            table.AddCell(cell);
            doc.Add(table);
        }
        #endregion

        #region 生成N行空白行
        private static void CreateEmptyRow(int emptyRowNum)
        {
            for (int i = 0; i < emptyRowNum; i++)
            {
                doc.Add(new Paragraph(" "));
            }
        }
        #endregion

        #region 生成标题
        private static void AddHeaderTitleContent(string content, Font font, float indentationLeft)
        {
            Paragraph p = new Paragraph(content, font);
            p.IndentationLeft = indentationLeft;//距离左边距
            doc.Add(p);
        }
        #endregion

        #region 生成表单通用基本数据

        public static void AddSinglePartnerContents(string FieldNameOne, string FieldValueOne)
        {
            fontMiddle.SetStyle(Font.UNDERLINE);//文字下划线
                                                //IndentationLeft = IndentationLeft + 10;
            Paragraph content = new Paragraph();
            content.IndentationLeft = IndentationLeft;
            Chunk chunkNameOne = new Chunk(FieldNameOne + ":", fontSmallNoBold);
            //Chunk chunkTextOne = new Chunk(GetEmptyString(20, FieldValueOne), fontSmallNoBold);   
            Chunk chunkTextOne = new Chunk(FieldValueOne, fontSmallNoBold);
            content.Add(0, chunkNameOne);
            content.Add(1, chunkTextOne);
            content.Alignment = 10;
            doc.Add(content);
        }

        /// <summary>
        /// 生成表单通用基本数据
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="FieldValue">字段值</param>
        public static void AddPartnerContents(string FieldNameOne, string FieldValueOne,
           string FieldNameTwo, string FieldValueTwo)
        {
            fontMiddle.SetStyle(Font.UNDERLINE);//文字下划线
                                                //IndentationLeft = IndentationLeft + 10;
            Paragraph content = new Paragraph();
            content.IndentationLeft = IndentationLeft;
            Chunk chunkNameOne = new Chunk(FieldNameOne + ":", fontSmallNoBold);
            //Chunk chunkTextOne = new Chunk(GetEmptyString(20, FieldValueOne), fontSmallNoBold);
            Chunk chunkTextOne = new Chunk(FieldValueOne + "             ", fontSmallNoBold);
            Chunk chunkNameTwo = new Chunk(FieldNameTwo + ":", fontSmallNoBold);
            //Chunk chunkTextTwo = new Chunk(GetEmptyString(20, FieldValueTwo), fontSmallNoBold);
            Chunk chunkTextTwo = new Chunk(FieldValueTwo + "            ", fontSmallNoBold);
            content.Add(0, chunkNameOne);
            content.Add(1, chunkTextOne);
            content.Add(0, chunkNameTwo);
            content.Add(1, chunkTextTwo);
            content.Alignment = 10;
            doc.Add(content);
        }

        #endregion

        #region
        //居中显示内容
        private static string GetEmptyString(int maxlength, string text)
        {
            int padding = (maxlength - text.Length * 2) / 2;
            string empty = string.Empty;
            for (int i = 0; i < padding; i++)
            {
                empty += " ";
            }
            return string.Format("{0}{1}{0}", empty, text);
        }
        #endregion

        #region 生成页码
        private static void AddPageNumberContent()
        {
            //var content = new Paragraph("共   页  第   页", fontSmall);
            //content.IndentationRight = IndentationLeft + 20;
            //content.Alignment = 2;    //居左
            //doc.Add(content);
        }
        #endregion

        #region 生成单元格
        private static PdfPCell GetPdfCell(string content, Font font, int horizontalAlignment)
        {
            PdfPCell cell = new PdfPCell(new Paragraph(content, font));
            cell.HorizontalAlignment = horizontalAlignment;//水平位置
            cell.VerticalAlignment = Element.ALIGN_CENTER;//垂直居中
            cell.MinimumHeight = 20;//单元格的最小高度
            return cell;
        }
        #endregion

        #region 生成水印盖章
        private static MemoryStream SetWaterMark(MemoryStream ms, string filePath, string waterMarkName, string waterMarkAddr = null)
        {
            MemoryStream msWater = new MemoryStream();
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(filePath);
                pdfStamper = new PdfStamper(pdfReader, msWater);

                int total = pdfReader.NumberOfPages + 1;//获取PDF的总页数
                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);//获取第一页
                float width = psize.Width;//PDF页面的宽度，用于计算水印倾斜
                float height = psize.Height;
                PdfContentByte waterContent;
                BaseFont basefont = BaseFont.CreateFont(@"C://Windows/Fonts/simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                for (int i = 1; i < total; i++)
                {
                    waterContent = pdfStamper.GetOverContent(i);//在内容上方加水印
                                                                //透明度
                    waterContent.SetGState(gs);
                    //开始写入文本
                    waterContent.BeginText();

                    //waterContent.SetColorFill(BaseColor.RED);
                    waterContent.SetFontAndSize(basefont, 18);

                    waterContent.SetTextMatrix(0, 0);
                    if (waterMarkAddr == null || waterMarkAddr == "")
                    {
                        waterContent.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, width / 2, height / 2, 55);
                    }
                    else
                    {
                        waterContent.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, width / 2, height / 2 + 100, 55);
                        waterContent.ShowTextAligned(Element.ALIGN_CENTER, waterMarkAddr, width / 2, height / 2 - 100, 55);
                    }
                    waterContent.EndText();
                }
            }
            catch (Exception)
            {
                return ms;
            }
            finally
            {
                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
            return msWater;
        }
        #endregion

        /// <summary>
        /// 发送PDF文件回浏览器端
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ms"></param>
        /// <param name="encoding"></param>
        public static void SendFile(string fileName, MemoryStream ms, Encoding encoding = null)
        {
            fileName = (fileName + "").Replace(" ", "");
            encoding = encoding ?? Encoding.UTF8;
            if (ms != null && !string.IsNullOrEmpty(fileName))
            {
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.Charset = encoding.BodyName;// "utf-8";
                if (!HttpContext.Current.Request.UserAgent.Contains("Firefox") && !HttpContext.Current.Request.UserAgent.Contains("Chrome"))
                {
                    fileName = HttpUtility.UrlEncode(fileName, encoding);
                }
                response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                //为了解决打开，导出NPOI生成的xlsx文件时，提示发现不可读取内容。
                if (!(fileName + "").ToLower().EndsWith(".xlsx"))
                {
                    response.AddHeader("Content-Type", "application/octet-stream");
                    response.BinaryWrite(ms.GetBuffer());
                }
                else
                {
                    response.BinaryWrite(ms.ToArray());
                }
                ms.Close();
                ms = null;
                response.Flush();
                response.End();
            }
        }

        /// <summary>
        /// 图片转PDF
        /// </summary>
        /// <param name="sourse">原始图片路径</param>
        /// <param name="output">输出图片路径</param>
        public static void ConvertJpgToPdf(string sourse, string output)
        {
            try
            {
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(sourse);
                using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Document document = new Document(image);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(document, fs);
                    pdfWriter.SetFullCompression();
                    pdfWriter.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_7);
                    document.Open();
                    image.SetAbsolutePosition(0, 0);
                    document.SetPageSize(new iTextSharp.text.Rectangle(0, 0, image.Width, image.Height));
                    document.NewPage();
                    pdfWriter.DirectContent.AddImage(image, false);
                    document.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
