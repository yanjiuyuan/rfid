using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace DingTalk.Bussiness.Word
{
    public class WordHelper
    {
        public string GetPathByDocToHTML(string strFile)
        {
            if (!File.Exists(strFile))
            {
                return "文件不存在";
            }

            

            ApplicationClass word = new ApplicationClass();
            Type wordType = word.GetType();
            Microsoft.Office.Interop.Word.Documents docs = word.Documents;

            // 打开文件  
            Type docsType = docs.GetType();

            object fileName = strFile;

            Microsoft.Office.Interop.Word.Document doc = (Microsoft.Office.Interop.Word.Document)docsType.InvokeMember("Open",
            System.Reflection.BindingFlags.InvokeMethod, null, docs, new Object[] { fileName, true, true });

            // 转换格式，另存为html  
            Type docType = doc.GetType();
            //给文件重新起名
            string filename = System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() +
            System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString();

            string strFileFolder = "/html/";
            DateTime dt = DateTime.Now;
            //以yyyymmdd形式生成子文件夹名
            string strFileSubFolder = dt.Year.ToString();
            strFileSubFolder += (dt.Month < 10) ? ("0" + dt.Month.ToString()) : dt.Month.ToString();
            strFileSubFolder += (dt.Day < 10) ? ("0" + dt.Day.ToString()) : dt.Day.ToString();
            string strFilePath = strFileFolder + strFileSubFolder + "/";
            // 判断指定目录下是否存在文件夹，如果不存在，则创建 
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(strFilePath)))
            {
                // 创建up文件夹 
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(strFilePath));
            }

            //被转换的html文档保存的位置 
            // HttpContext.Current.Server.MapPath("html" + strFileSubFolder + filename + ".html")
            string ConfigPath = System.Web.HttpContext.Current.Server.MapPath(strFilePath + filename + ".html");
            object saveFileName = ConfigPath;

            /*下面是Microsoft Word 9 Object Library的写法，如果是10，可能写成： 
              * docType.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, 
              * null, doc, new object[]{saveFileName, Word.WdSaveFormat.wdFormatFilteredHTML}); 
              * 其它格式： 
              * wdFormatHTML 
              * wdFormatDocument 
              * wdFormatDOSText 
              * wdFormatDOSTextLineBreaks 
              * wdFormatEncodedText 
              * wdFormatRTF 
              * wdFormatTemplate 
              * wdFormatText 
              * wdFormatTextLineBreaks 
              * wdFormatUnicodeText 
            */

            //docType.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod,
            //null, doc, new object[] { saveFileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatFilteredHTML });


            docType.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod,
       null, doc, new object[] { saveFileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatFilteredHTML });
            
            //关闭文档  
            docType.InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod,
            null, doc, new object[] { null, null, null });

            // 退出 Word  
            wordType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, word, null);

            //转化HTML页面统一编码格式
            TransHTMLEncoding(ConfigPath);
            return (strFilePath + filename + ".html");
        }
        private void TransHTMLEncoding(string strFilePath)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(strFilePath, Encoding.GetEncoding(0));
                string html = sr.ReadToEnd();
                sr.Close();
                html = System.Text.RegularExpressions.Regex.Replace(html, @"<meta[^>]*>", "<meta http-equiv=Content-Type content='text/html; charset=gb2312'>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(strFilePath, false, Encoding.Default);
                sw.Write(html);
                sw.Close();
            }
            catch (Exception ex)
            {
                //  Page.ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), "myscript", "<script>alert('" + ex.Message + "')</script>");
            }
        }
    }
}