using Common.JsonHelper;
using DingTalk.Models;
using DingTalk.Models.DbModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DingTalk.Controllers
{
    public class PurchaseManageController : Controller
    {
        // GET: PurchaseManage
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 采购申请单表单接口
        /// </summary>
        /// <returns></returns>
        /// 测试数据: /PurchaseManage/SubmitPurchaseTable
        /// 
        [HttpPost]
        public string SubmitPurchaseTable()
        {
            try
            {
                StreamReader reader = new StreamReader(Request.InputStream);
                string List = reader.ReadToEnd();
                if (string.IsNullOrEmpty(List))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "请传递参数"
                    });
                }
                else
                {
                    List<PurchaseTable> PurchaseTableList = new List<PurchaseTable>();
                    PurchaseTableList = JsonHelper.JsonToObject<List<PurchaseTable>>(List);
                    using (DDContext context = new DDContext())
                    {
                        foreach (PurchaseTable purchaseTableList in PurchaseTableList)
                        {
                            context.PurchaseTable.Add(purchaseTableList);
                            context.SaveChanges();
                        }
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "保存成功"
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