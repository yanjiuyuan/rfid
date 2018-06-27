using DingTalk.Models;
using DingTalk.Models.DbModels;
using DingTalk.Models.KisModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DingTalk.Controllers
{
    [RoutePrefix("Purchase")]
    public class PurchaseManageController : ApiController
    {

        /// <summary>
        /// 采购表单批量保存
        /// </summary>
        /// <param name="purchaseTable"></param>
        /// <returns></returns>
        /// 测试数据: /Purchase/SavePurchaseTable
        /// data:[{ "Id": 1.0, "TaskId": "流水号", "CodeNo": "物料编码", "Name": "苹果", "Standard": "型号", "Unit": "单位", "Count": "数量", "Price": "单价", "Purpose": "用途", "UrgentDate": "需用日期", "Mark": "备注" }, { "Id": 2.0, "TaskId": "流水号2", "CodeNo": "物料编码2", "Name": "苹果2", "Standard": "型号2", "Unit": "单位2", "Count": "数量2", "Price": "单价2", "Purpose": "用途2", "UrgentDate": "需用日期2", "Mark": "备注2" }];
        /// contentType: 'application/json; charset=utf-8',
        [Route("SavePurchaseTable")]
        [HttpPost]
        public string SavePurchaseTable([FromBody] List<PurchaseTable> purchaseTableList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (PurchaseTable purchaseTable in purchaseTableList)
                    {
                        context.PurchaseTable.Add(purchaseTable);
                        context.SaveChanges();
                    }
                }
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 0,
                    errorMessage = "保存成功"
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                });
            }
        }


        /// <summary>
        /// 采购表单读取
        /// </summary>
        /// <returns></returns>
        /// 测试数据：/Purchase/ReadPurchaseTable?TaskId=3
        [Route("ReadPurchaseTable")]
        [HttpGet]
        public string PurseTableRead(string TaskId)
        {
            try
            {
                List<PurchaseTable> PurchaseTableList = new List<PurchaseTable>();
                using (DDContext context = new DDContext())
                {
                    PurchaseTableList = context.PurchaseTable.Where
                         (p => p.TaskId == TaskId).ToList();
                }
                return JsonConvert.SerializeObject(PurchaseTableList);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// 金蝶产品信息读取
        /// </summary>
        /// <param name="Key">查询关键字</param>
        /// <returns></returns>
        /// 测试数据 /Purchase/GetICItem?Key=电
        [Route("GetICItem")]
        [HttpGet]
        public string GetICItem(string Key)
        {
            try
            {
                using (KisContext context=new KisContext ())
                {
                    var ICItemList = context.t_ICItem.ToList();
                    var Quary = from t in ICItemList
                                where t.FName.Contains(Key) || t.FNumber.Contains(Key)
                                select new
                                {
                                    t.FNumber, //物料编码
                                    t.FName,  //物料名称
                                    t.FModel, //规格
                                    t.FOrderPrice  //单价
                                };
                    return JsonConvert.SerializeObject(Quary);
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel() {
                    errorCode=1,
                    errorMessage=ex.Message
                });
            }
        }



        public async Task<string> PrintReport()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            return "";
        }
    }
}
