using Common.Excel;
using Common.JsonHelper;
using DingTalk.Models;
using DingTalk.Models.DbModels;
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
        /// <returns>返回文件保存路径</returns>
        [HttpPost]
        public string Upload(FormCollection form)
        {
            try
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
                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string Path = "";
                    string strPath = "";
                    string FileName = file.FileName;
                    string strExtension = "";
                    int nIndex = FileName.LastIndexOf('.');
                    if (nIndex >= 0)
                    {
                        strExtension = FileName.Substring(nIndex);
                    }

                    switch (strExtension)
                    {
                        //Image
                        case ".jpg":
                            strPath = @"~\UploadFile\Images\";
                            Path = Server.MapPath(strPath + newFileName + strExtension);
                            break;
                        case ".png":
                            strPath = @"~\UploadFile\Images\";
                            Path = Server.MapPath(strPath + newFileName + strExtension);
                            break;
                        //Excel
                        case ".xls":
                            strPath = @"~\UploadFile\Excel\";
                            Path = Server.MapPath(strPath + newFileName + strExtension);
                            break;
                        case ".xlsx":
                            strPath = @"~\UploadFile\Excel\";
                            Path = Server.MapPath(strPath + newFileName + strExtension);
                            break;
                        //其他文件
                        default:
                            strPath = @"~\UploadFile\Flies\";
                            Path = Server.MapPath(strPath + newFileName + strExtension);
                            break;
                    }
                    //保存文件
                    files.SaveAs(Path);
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "上传成功",
                        Content = strPath + newFileName + strExtension
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 3,
                    errorMessage = ex.Message
                });
            }

        }


        /// <summary>
        /// 上传Excel并读取数据接口
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadAndGetInfo(FormCollection form)
        {
            try
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
                    string Path = Server.MapPath(@"~\UploadFile\Excel\" + newFileName);
                    files.SaveAs(Path);
                    return LoadExcel(Path);
                    //    JsonConvert.SerializeObject(new ErrorModel
                    //{
                    //    errorCode = 0,
                    //    errorMessage = "上传成功",
                    //    Content = Server.MapPath(@"~\UploadFile\Excel\" + newFileName)
                    //});
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 3,
                    errorMessage = ex.Message
                });
            }

        }

        /// <summary>
        /// Excel读取
        /// </summary>
        /// <param name="Path">Excel路径</param>
        /// <returns></returns>
        /// 测试数据/DrawingUpload/LoadExcel?path=C:\\Users\\tong\\Source\\Repos\\DingTalk\\DingTalk\\UploadFile\\Excel\\20180408163044.xlsx
        [HttpGet]
        public string LoadExcel(string Path)
        {
            if (Path == null)  //测试暂用
            {
                Path = "C:\\Users\\tong\\Source\\Repos\\DingTalk\\DingTalk\\UploadFile\\Excel\\BOM表提交模板.xls";
            }

            DataTable db = ExcelHelperByNPOI.ImportExcel2003toDt(Path);
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            dic.Add(Path, db);
            return JsonConvert.SerializeObject(dic.ToArray());
        }



        /// <summary>
        /// BOM表信息查询
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        /// 测试数据：/DrawingUpload/GetPurchase?TaskId=2
        [HttpGet]
        public string GetPurchase(string TaskId)
        {
            try
            {
                if (string.IsNullOrEmpty(TaskId))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "TaskId不能为空！"
                    });
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        var PurchaseList = context.Purchase.Where(u => u.TaskId == TaskId);
                        if (PurchaseList != null)
                        {
                            return JsonConvert.SerializeObject(PurchaseList);
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 2,
                                errorMessage = "未查到数据！"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 3,
                    errorMessage = ex.Message
                });
            }
        }


        /// <summary>
        /// BOM表信息载入
        /// </summary>
        /// <returns></returns>
        /// 测试数据 /DrawingUpload/LoadPurchase
        ///  var PurchaseList = [{ "TaskId": "2", "DrawingNo": "BOM-2017QZL001-delta0505A机器人", "CodeNo": "1", "Name": "十字座D16", "Count": "1", "MaterialScience": "7075T6", "Unit": "件", "Brand": "ATMV", "Sorts": "自制", "Mark": "借用" },
        ///  { "TaskId": "2", "DrawingNo": "DTE-801B-WX-01C", "CodeNo": "2", "Name": "十字座套", "Count": "2", "MaterialScience": "7075T6", "Unit": "件", "Brand": "耐克", "Sorts": "自制", "Mark": "借用" },
        ///  { "TaskId": "2", "DrawingNo": "DTE-801B-WX-01B", "CodeNo": "3", "Name": "十字座D10", "Count": "1", "MaterialScience": "7075T6", "Unit": "件", "Brand": "阿迪", "Sorts": "自制", "Mark": "借用" }]
        [HttpPost]
        public string LoadPurchase()
        {
            try
            {
                StreamReader reader = new StreamReader(Request.InputStream);
                string PurchaseJson = reader.ReadToEnd();
                if (string.IsNullOrEmpty(PurchaseJson))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "载入数据不能为空！"
                    });
                }
                else
                {
                    List<Purchase> listPurchase = new List<Purchase>();
                    listPurchase = JsonHelper.JsonToObject<List<Purchase>>(PurchaseJson);
                    foreach (Purchase item in listPurchase)
                    {
                        using (DDContext context = new DDContext())
                        {
                            context.Purchase.Add(item);
                            context.SaveChanges();
                        }
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "载入成功!"
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }


    }
}