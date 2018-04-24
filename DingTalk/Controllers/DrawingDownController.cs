using Common.JsonHelper;
using DingTalk.Models;
using DingTalk.Models.DbModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DingTalk.Controllers
{
    public class DrawingDownController : Controller
    {
        // GET: DrawingDown
        public ActionResult Index()
        {
            return View();
        }

        #region 图纸上传数据读取
        /// <summary>
        /// 图纸上传数据读取(图纸上传流程已结束)
        /// </summary>
        /// <param name="ApplyManId">用户Id</param>
        /// <returns></returns>
        /// 测试数据：/DrawingDown/GetDrawingDownInfo?ApplyManId=123456
        [HttpGet]
        public string GetDrawingDownInfo(string ApplyManId)
        {
            try
            {
                if (string.IsNullOrEmpty(ApplyManId))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "未传递参数ApplyManId"
                    });
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        var TaskList = context.Tasks.Where(u => u.State == 1 && u.FlowId == 6);
                        var NodeInfoList = context.NodeInfo.Where(u => u.FlowId == "6" && u.PeopleId == ApplyManId);
                        var Purchase = context.Purchase;
                        var TaskLists = from t in TaskList
                                        join n in NodeInfoList
                                        on t.FlowId.ToString() equals n.FlowId
                                        select new
                                        {
                                            TaskId = t.TaskId
                                        };
                        var Quary = from t in TaskLists
                                    join p in Purchase
                                    on t.TaskId.ToString() equals p.TaskId
                                    where p.IsDown == false
                                    select new
                                    {
                                        TaskId = p.TaskId,
                                        DrawingNo = p.DrawingNo,
                                        CodeNo = p.CodeNo,
                                        Name = p.Name,
                                        Count = p.Count,
                                        MaterialScience = p.MaterialScience,
                                        Unit = p.Unit,
                                        Brand = p.Brand,
                                        Sorts = p.Sorts,
                                        Mark = p.Mark,
                                        IsDown = p.IsDown
                                    };
                        return JsonConvert.SerializeObject(Quary);
                    }
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
        #endregion

        //#region 已上传的BOM表查询

        ///// <summary>
        ///// 已上传的BOM表查询
        ///// </summary>
        ///// <param name="ApplyManId"></param>
        ///// <returns></returns>
        ///// /DrawingDown/GetPurchaseInfo?ApplyManId=123456
        //[HttpGet]
        //public string GetPurchaseInfo(string ApplyManId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(ApplyManId))
        //        {
        //            return JsonConvert.SerializeObject(new ErrorModel
        //            {
        //                errorCode = 1,
        //                errorMessage = "未传递参数ApplyManId"
        //            });
        //        }
        //        else
        //        {
        //            using (DDContext context = new DDContext())
        //            {

        //                string PeopleId = context.NodeInfo.Where(u => u.NodeId == 0 && u.FlowId == "7").Select(u => u.PeopleId).First();
        //                if (ApplyManId != PeopleId) //校对申请人
        //                {
        //                    return JsonConvert.SerializeObject(new ErrorModel
        //                    {
        //                        errorCode = 3,
        //                        errorMessage = "用户没有权限"
        //                    });
        //                }
        //                else
        //                {
        //                    List<Purchase> PurchaseList = context.Purchase.Where(u => u.IsDown == false).ToList();
        //                    return JsonConvert.SerializeObject(PurchaseList);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonConvert.SerializeObject(new ErrorModel
        //        {
        //            errorCode = 2,
        //            errorMessage = ex.Message
        //        });
        //    }
        //}

        //#endregion

        #region 图纸下发表单提交

        /// <summary>
        /// 图纸下发表单提交接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string SubmitDrawingDown()
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

        #endregion

        #region 添加工序与工时

        /// <summary>
        /// 添加工序
        /// </summary>
        /// <returns></returns>
        /// 测试数据: /DrawingDown/AddProcedure
        [HttpPost]
        public string AddProcedure()
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
                    List<ProcedureInfo> procedureInfoList = new List<ProcedureInfo>();
                    procedureInfoList = JsonHelper.JsonToObject<List<ProcedureInfo>>(List);
                    using (DDContext context = new DDContext())
                    {
                        foreach (ProcedureInfo procedureInfo in procedureInfoList)
                        {
                            context.ProcedureInfo.Add(procedureInfo);
                        }
                        context.SaveChanges();
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

        /// <summary>
        /// 添加工时
        /// </summary>
        /// <returns></returns>
        /// 测试数据: /DrawingDown/AddWorkTime
        [HttpPost]
        public string AddWorkTime()
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

                    List<ProcedureInfo> procedureInfoList = new List<ProcedureInfo>();
                    procedureInfoList = JsonHelper.JsonToObject<List<ProcedureInfo>>(List);
                    using (DDContext context = new DDContext())
                    {
                        foreach (ProcedureInfo procedureInfo in procedureInfoList)
                        {
                            context.ProcedureInfo.Add(procedureInfo);
                        }
                        context.SaveChanges();
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

        #endregion

        #region Bom表、工序、工时数据读取

        /// <summary>
        /// Bom表、工序、工时数据读取
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        /// 测试数据: /DrawingDown/GetAllInfo?TaskId=2
        [HttpGet]
        public string GetAllInfo(int TaskId = 0)
        {
            try
            {
                if (TaskId == 0)
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "请传递参数"
                    });
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        List<Purchase> PurchaseList = context.Purchase.
                            Where(u => u.TaskId == TaskId.ToString()).ToList();
                        List<ProcedureInfo> ProcedureInfoList = context.ProcedureInfo.ToList();
                        List<WorkTime> WorkTimeInfoList = context.WorkTime.ToList();
                        var Quary = from p in PurchaseList
                                    join s in ProcedureInfoList
                                    on p.DrawingNo equals s.DrawingNo
                                    join w in WorkTimeInfoList
                                    on s.Id.ToString() equals w.ProjectInfoId
                                    select w;
                        return JsonConvert.SerializeObject(Quary);
                    }
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

        #endregion
    }
}