﻿using DingTalk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        [HttpPost]
        [Route("PostFile")]
        public string PostFile(FileModel fileModel)
        {

            string result = "";
            if (fileModel != null)
            {
                byte[] FileContent = Convert.FromBase64String(fileModel.Base64String);
                string Tpath = "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
                string FilePath = fileModel.FileName + "\\" + Tpath + "\\";
                string Err = "";
                string FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                bool upres = WriteFile(FilePath, FileContent, FileName, out Err);
                if (upres)
                {
                    result = (Tpath + FileName).Replace("\\", "/");
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
            return result;
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
