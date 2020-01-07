using Common.PDF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalkServer;
using DingTalkServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    [RoutePrefix("File")]
    public class FileController : ApiController
    {

        /// <summary>
        /// Base64文件流上传
        /// </summary>
        /// <param name="fileModel">文件流对象</param>
        /// <returns></returns>
        /// 测试数据：/File/PostFile
        [HttpPost]
        [Route("PostFile")]
        public async Task<object> PostFile(FileModel fileModel)
        {
            try
            {
                string result = "";
                if (fileModel != null)
                {
                    string Base64String = fileModel.Base64String.Replace("data:image/png;base64,", "");
                    byte[] FileContent = Convert.FromBase64String(Base64String);
                    string ImageFilePath = AppDomain.CurrentDomain.BaseDirectory + "UploadFile\\Images\\ChangeImages\\";
                    string PdfFilePath = AppDomain.CurrentDomain.BaseDirectory + "UploadFile\\PDF\\";
                    string FileName = Path.GetFileName(fileModel.FileName.Substring(0, fileModel.FileName.Length - 4));
                    string Err = "";
                    bool upres = WriteFile(ImageFilePath, FileContent, FileName + ".png", out Err);

                    if (upres)
                    {
                        //清除文件
                        File.Delete(PdfFilePath + FileName + ".PDF");
                        //生成PDF
                        PDFHelper.ConvertJpgToPdf(ImageFilePath + FileName + ".png", PdfFilePath + FileName + ".PDF");
                        //上盯盘
                        string fileName = PdfFilePath + FileName + ".PDF";
                        var uploadFileModel = new UploadMediaRequestModel()
                        {
                            FileName = fileName,
                            MediaType = UploadMediaType.File
                        };
                        DingTalkManager dingTalkManager = new DingTalkManager();
                        string UploadFileResult = await dingTalkManager.UploadFile(uploadFileModel);
                        FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(UploadFileResult);
                        //替换数据库MediaId
                        using (DDContext context = new DDContext())
                        {
                            Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == fileModel.TaskId && t.NodeId == 0).First();
                            tasks.MediaIdPDF = tasks.MediaIdPDF.Replace(fileModel.OldMediaId, fileSendModel.Media_Id);
                            result = tasks.MediaIdPDF;
                            context.Entry<Tasks>(tasks).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        result = "上传文件写入失败：" + Err;
                    }
                }
                else
                {
                    result = "上传的文件信息不存在！";
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "保存成功",
                    Content = result

                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 图纸PDF状态更新
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="PDFState">PDF状态</param>
        /// <returns></returns>
        /// 测试数据： /File/UpdatePDFState?TaskId=2&PDFState=0,1,0
        [HttpGet]
        [Route("UpdatePDFState")]
        public object UpdatePDFState(string TaskId, string PDFState)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.NodeId == 0).First();
                    tasks.PdfState = PDFState;
                    context.Entry<Tasks>(tasks).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return new ErrorModel()
                    {
                        errorCode = 0,
                        errorMessage = "修改成功"
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// 二进制流写入文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="filecontent">文件内容</param>
        /// <param name="FileName">文件名</param>
        /// <param name="Errmsg">错误消息</param>
        /// <returns></returns>
        public static bool WriteFile(string filepath, byte[] filecontent, string FileName, out string Errmsg)
        {

            DirectoryInfo di = new DirectoryInfo(filepath);
            if (!di.Exists)
            {
                di.Create();
            }
            FileStream fstream = null;
            try
            {
                fstream = File.Create(filepath + "\\" + FileName, filecontent.Length);
                fstream.Write(filecontent, 0, filecontent.Length);   //二进制转换成文件
            }
            catch (Exception ex)
            {
                Errmsg = ex.Message;
                //抛出异常信息
                return false;
            }
            finally
            {
                if (fstream != null)
                    fstream.Close();
            }
            Errmsg = "";
            return true;
        }
    }
}
