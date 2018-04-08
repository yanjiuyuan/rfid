using Common.Excel;
using DingTalk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DingTalk.Controllers
{
    public class DrawingUploadController : Controller
    {
        // GET: DrawingUpload
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 文件上传接口
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public string Upload(FormCollection form)
        {
            if (Request.Files.Count == 0)
            {
                //Request.Files.Count 文件数为0上传不成功
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = "文件数为0"
                });
            }

            var file = Request.Files[0];
            if (file.ContentLength == 0)
            {
                //文件大小大（以字节为单位）为0时，做一些操作
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = "文件大小大（以字节为单位）为0"
                });
            }
            else
            {
                //文件大小不为0
                HttpPostedFileBase files = Request.Files[0];
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                files.SaveAs(Server.MapPath(@"~\UploadFile\Excel\"+ newFileName));

                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 0,
                    errorMessage = "上传成功",
                    Content= Server.MapPath(@"~\UploadFile\Excel\" + newFileName)
                });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        /// 测试数据/DrawingUpload/LoadExcel?path=C:\\Users\\tong\\Source\\Repos\\DingTalk\\DingTalk\\UploadFile\\Excel\\20180408163044.xlsx
        [HttpGet]
        public string LoadExcel(string Path)
        {
            //Path = "C:\\Users\\tong\\Source\\Repos\\DingTalk\\DingTalk\\UploadFile\\Excel\\20180408164559.xls";
            DataTable db= ExcelHelperByNPOI.ImportExcel2003toDt(Path);
            return JsonConvert.SerializeObject(db);
        }

    }
}