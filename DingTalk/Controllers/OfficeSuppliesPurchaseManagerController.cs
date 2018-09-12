using Common.PDF;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 办公用品采购
    /// </summary>
    [RoutePrefix("OfficeSuppliesPurchase")]
    public class OfficeSuppliesPurchaseManagerController : ApiController
    {
        /// <summary>
        /// 办公用品申请数据读取
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [Route("GetTable")]
        [HttpGet]
        public object GetOfficeSuppliesTable(DateTime startTime, DateTime endTime)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskId("1");
                    List<OfficeModels> officeModeldList = new List<OfficeModels>();
                    foreach (var task in tasks)
                    {
                        if (task.NodeId == 0)
                        {
                            if (DateTime.Parse(task.ApplyTime) > startTime && DateTime.Parse(task.ApplyTime) < endTime)
                            {
                                officeModeldList.Add(new OfficeModels()
                                {
                                    taskId = task.TaskId.ToString(),
                                    applyMan = task.ApplyMan,
                                    dept = task.Dept
                                });
                            }
                        }
                    }
                    List<OfficeSupplies> officeSupplies = context.OfficeSupplies.ToList();

                    var Quary = from t in officeModeldList
                                join o in officeSupplies
                                on t.taskId equals o.TaskId
                                select new
                                {
                                    t.taskId,
                                    t.applyMan,
                                    t.dept,
                                    o.CodeNo,
                                    o.Count,
                                    o.Unit,
                                    o.ExpectPrice,
                                    o.Id,
                                    o.Mark,
                                    o.Name,
                                    o.Price,
                                    o.Purpose,
                                    o.Standard,
                                    o.UrgentDate
                                };
                    return Quary;
                }
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
        /// 办公用品采购表单保存
        /// </summary>
        /// <param name="officeSuppliesPurchaseTableList"></param>
        /// <returns></returns>
        [Route("SaveTable")]
        [HttpPost]
        public string SaveTable([FromBody] List<OfficeSuppliesPurchase> officeSuppliesPurchaseTableList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (OfficeSuppliesPurchase officeSuppliesPurchase in officeSuppliesPurchaseTableList)
                    {
                        context.OfficeSuppliesPurchase.Add(officeSuppliesPurchase);
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
        /// 办公用品采购表单读取
        /// </summary>
        /// <returns></returns>
        [Route("ReadTable")]
        [HttpGet]
        public string ReadTable(string TaskId)
        {
            try
            {
                List<OfficeSuppliesPurchase> OfficeSuppliesPurchaseTableList = new List<OfficeSuppliesPurchase>();
                using (DDContext context = new DDContext())
                {
                    OfficeSuppliesPurchaseTableList = context.OfficeSuppliesPurchase.Where
                         (p => p.TaskId == TaskId).ToList();
                }
                return JsonConvert.SerializeObject(OfficeSuppliesPurchaseTableList);
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
        /// 办公用品采购表单修改
        /// </summary>
        /// <param name="officeSuppliesPurchaseTableList"></param>
        /// <returns></returns>
        [Route("ModifyTable")]
        [HttpPost]
        public string ModifyTable([FromBody] List<OfficeSuppliesPurchase> officeSuppliesPurchaseTableList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (OfficeSuppliesPurchase officeSuppliesPurchase in officeSuppliesPurchaseTableList)
                    {
                        context.Entry<OfficeSuppliesPurchase>(officeSuppliesPurchase).State = System.Data.Entity.EntityState.Modified;
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
    }



    public class OfficeModels
    {
        public string taskId { get; set; }
        public string applyMan { get; set; }
        public string dept { get; set; }
    }
}
