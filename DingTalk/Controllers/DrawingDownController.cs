using Common.JsonHelper;
using DingTalk.Bussiness.FlowInfo;
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
        /// <param name="ProjectId">项目Id</param>
        /// <returns></returns>
        /// 测试数据：/DrawingDown/GetDrawingDownInfo?ProjectId=2016ZL051&ApplyManId=123456
        [HttpGet]
        public string GetDrawingDownInfo(string ProjectId, string ApplyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    string CheckApplyManId = context.NodeInfo.Where(u => u.FlowId == "7" && u.NodeId == 0).First().PeopleId;
                    if (ApplyManId != CheckApplyManId)
                    {
                        return JsonConvert.SerializeObject(new ErrorModel
                        {
                            errorCode = 1,
                            errorMessage = "没有权限发起次流程"
                        });
                    }
                    else
                    {
                        List<Tasks> TaskIdList = FlowInfoServer.ReturnUnFinishedTaskId("6");
                        var TaskIdLists = from t in TaskIdList
                                          where
                     t.ProjectId == ProjectId
                                          select t;
                        //获取并过滤已完成流程的TaskId
                        List<string> ListTaskId = new List<string>();
                        foreach (var item in TaskIdLists)
                        {
                            if (!ListTaskId.Contains(item.TaskId.ToString()))
                            {
                                ListTaskId.Add(item.TaskId.ToString());
                            }
                        }

                        var TaskList = from t in ListTaskId select t;

                        var Purchase = context.Purchase.Where(u => u.IsDown != true);
                        var Quary = from t in TaskList
                                    join p in Purchase
                                    on t.ToString() equals p.TaskId
                                    select new
                                    {
                                        p.Id,
                                        p.DrawingNo,
                                        p.Name,
                                        p.Sorts,
                                        p.TaskId,
                                        p.MaterialScience,
                                        p.Brand,
                                        p.Count,
                                        p.Unit,
                                        p.Mark,
                                    };
                        var Procedure = context.ProcedureInfo;
                        var QuaryPro = from q in Quary
                                       join p in Procedure
                                       on q.DrawingNo equals p.DrawingNo
                                       into temp
                                       from pp in temp.DefaultIfEmpty()
                                       select new
                                       {
                                           DrawingNoId = q.Id,
                                           DrawingNo = q.DrawingNo,
                                           Name = q.Name,
                                           Sorts = q.Sorts,
                                           TaskId = q.TaskId,
                                           MaterialScience = q.MaterialScience,
                                           Brand = q.Brand,
                                           Count = q.Count,
                                           Unit = q.Unit,
                                           Mark = q.Mark,

                                           ProList = new List<Pro>()
                                           {
                                               new Pro{
                                                   ProcedureId = pp == null ? "" : pp.Id.ToString(),
                                                   ProcedureName = pp == null ? "" : pp.ProcedureName,
                                                   CreateTime = pp == null ? "" : pp.CreateTime,
                                                   ApplyMan = pp == null ? "" : pp.ApplyMan
                                               }
                                           },
                                       };
                        

                        //List<DrowDownModel> DrowDownModelList = new List<DrowDownModel>();

                        //foreach (var item in QuaryPro)
                        //{
                        //    DrowDownModel drowDownModel = new DrowDownModel();
                        //    Pro pro = new Pro();
                        //    List<Pro> proList = new List<Pro>();
                        //    drowDownModel.DrawingNo = item.DrawingNo;
                        //    drowDownModel.Name = item.Name;
                        //    drowDownModel.Sorts = item.Sorts;
                        //    drowDownModel.TaskId = item.TaskId;
                        //    drowDownModel.MaterialScience = item.MaterialScience;
                        //    drowDownModel.Brand = item.Brand;
                        //    drowDownModel.Count = item.Count;
                        //    drowDownModel.Unit = item.Unit;
                        //    drowDownModel.Mark = item.Mark;
                        //    pro.ProcedureId = item.ProList[0].ProcedureId ;
                        //    pro.ProcedureName = item.ProList[0].ProcedureName;
                        //    pro.CreateTime = item.ProList[0].CreateTime;
                        //    pro.ApplyMan = item.ProList[0].ApplyMan;
                        //    proList.Add(pro);
                        //    drowDownModel.ProList = proList;
                        //    DrowDownModelList.Add(drowDownModel);
                        //}



                        //List<DrowDownModel> DrowDownModelListTest = new List<DrowDownModel>();

                        //foreach (DrowDownModel drowDownModel in DrowDownModelList)
                        //{
                        //    List<Pro> ProList = new List<Pro>();
                        //    foreach (DrowDownModel drowDownModelT in DrowDownModelList)
                        //    {
                        //        if (drowDownModel.DrawingNo == drowDownModelT.DrawingNo && drowDownModel!= drowDownModelT)
                        //        {
                        //            foreach (Pro pro in drowDownModel.ProList)
                        //            {
                        //                ProList.Add(pro);
                        //            }
                        //            foreach (Pro proT in drowDownModelT.ProList)
                        //            {
                        //                ProList.Add(proT);
                        //            }
                        //        }
                        //    }

                        //    DrowDownModel drowDownModelTest = new DrowDownModel();
                        //    drowDownModelTest.ProList = ProList;
                        //    drowDownModelTest.DrawingNo = drowDownModel.DrawingNo;
                        //    drowDownModelTest.Name = drowDownModel.Name;
                        //    drowDownModelTest.Sorts = drowDownModel.Sorts;
                        //    drowDownModelTest.TaskId = drowDownModel.TaskId;
                        //    drowDownModelTest.MaterialScience = drowDownModel.MaterialScience;
                        //    drowDownModelTest.Brand = drowDownModel.Brand;
                        //    drowDownModelTest.Count = drowDownModel.Count;
                        //    drowDownModelTest.Unit = drowDownModel.Unit;
                        //    drowDownModelTest.Mark = drowDownModel.Mark;
                        //    DrowDownModelListTest.Add(drowDownModelTest);
                        //}


                        return JsonConvert.SerializeObject(QuaryPro);
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
        /// 测试数据  /DrawingDown/SubmitDrawingDown
        /// var PurchaseDownList = [{"Id":1.0,"TaskId":"3","OldTaskId":"2","DrawingNo":"DTE-801B-WX-01C","CodeNo":"2","Name":"十字座套","Count":"2","MaterialScience":"7075T6","Unit":"件","Brand":"耐克","Sorts":"自制","Mark":"借用","IsDown":true,"ProcedureId":"1","FlowType":"0"},{"Id":2.0,"TaskId":"3","OldTaskId":"2","DrawingNo":"DTE-801B-WX-01B","CodeNo":"3","Name":"十字座D10","Count":"1","MaterialScience":"7075T6","Unit":"件","Brand":"阿迪","Sorts":"自制","Mark":"借用","IsDown":true,"ProcedureId":"2","FlowType":"1"}]
        [HttpPost]
        public string SubmitDrawingDown()
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
                    List<PurchaseDown> procedureInfoList = new List<PurchaseDown>();
                    procedureInfoList = JsonHelper.JsonToObject<List<PurchaseDown>>(List);
                    using (DDContext context = new DDContext())
                    {
                        foreach (PurchaseDown purchaseDown in procedureInfoList)
                        {
                            context.PurchaseDown.Add(purchaseDown);
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

        #region 添加、删除工序与工时

        /// <summary>
        /// 添加工序
        /// </summary>
        /// <returns></returns>
        /// 测试数据: /DrawingDown/AddProcedure
        ///  var PurchaseList = [{ "DrawingNo1": "DTE-801B-WX-01C", "ProcedureName": "中料", "DefaultWorkTime": "1", "State": "0", "CreateTime": "2018-04-24 15:48", "ApplyMan": "胡工", "ApplyManId": "123456"},
        ///  { "DrawingNo": "DTE-801B-WX-01C1", "ProcedureName": "喷漆", "DefaultWorkTime": "1", "State": "0", "CreateTime": "2018-04-24 15:48", "ApplyMan": "胡工", "ApplyManId": "123456"},
        ///  { "DrawingNo": "DTE-801B-WX-01D1", "ProcedureName": "切割", "DefaultWorkTime": "1", "State": "0", "CreateTime": "2018-04-24 15:48", "ApplyMan": "胡工", "ApplyManId": "123456"}] 
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
                    List<string> ProcedureIdList = new List<string>();
                    using (DDContext context = new DDContext())
                    {
                        foreach (ProcedureInfo procedureInfo in procedureInfoList)
                        {
                            context.ProcedureInfo.Add(procedureInfo);
                            context.SaveChanges();
                            ProcedureIdList.Add(procedureInfo.Id.ToString());
                        }
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "保存成功",
                        Content = JsonConvert.SerializeObject(ProcedureIdList)
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
        ///  var WorkTimeList = [{ "ProjectInfoId": "1", "IsFinish": false, "Worker": "小红", "WorkerId": "666", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "2"},
        ///  { "ProjectInfoId": "2", "IsFinish": false, "Worker": "小滨", "WorkerId": "777", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "3"},
        ///  { "ProjectInfoId": "2", "IsFinish": true, "Worker": "小雨", "WorkerId": "888", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "3"}] 
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
                    List<string> WorkTimeIdList = new List<string>();
                    List<WorkTime> WorkTimeInfoList = new List<WorkTime>();
                    WorkTimeInfoList = JsonHelper.JsonToObject<List<WorkTime>>(List);
                    using (DDContext context = new DDContext())
                    {
                        foreach (WorkTime workTime in WorkTimeInfoList)
                        {
                            context.WorkTime.Add(workTime);
                            context.SaveChanges();
                            WorkTimeIdList.Add(workTime.Id.ToString());
                        }
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "保存成功",
                        Content = JsonConvert.SerializeObject(WorkTimeIdList)
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
        /// 批量删除工序
        /// </summary>
        /// <param name="Id">(逗号隔开)</param>
        /// <returns></returns>
        /// 测试数据： /DrawingDown/DeleteProcedure?&Id=10004,10005
        [HttpGet]
        public string DeleteProcedure(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "请传递参数"
                    });
                }
                else
                {
                    string[] ListIds = Id.Split(',');
                    using (DDContext context = new DDContext())
                    {
                        foreach (var item in ListIds)
                        {
                            ProcedureInfo procedureInfo = new ProcedureInfo()
                            {
                                Id = decimal.Parse(item)
                            };
                            context.ProcedureInfo.Attach(procedureInfo);
                            context.ProcedureInfo.Remove(procedureInfo);
                            context.SaveChanges();
                        }
                    }

                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "删除成功"
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
        /// 批量删除工时
        /// </summary>
        /// <param name="Id">(逗号隔开)</param>
        /// <returns></returns>
        /// 测试数据： /DrawingDown/DeleteWorkTime?&Id=10002,10003,10004
        public string DeleteWorkTime(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "请传递参数"
                    });
                }
                else
                {
                    string[] ListIds = Id.Split(',');
                    using (DDContext context = new DDContext())
                    {
                        foreach (var item in ListIds)
                        {
                            WorkTime workTime = new WorkTime()
                            {
                                Id = decimal.Parse(item)
                            };
                            context.WorkTime.Attach(workTime);
                            context.WorkTime.Remove(workTime);
                            context.SaveChanges();
                        }
                    }

                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "删除成功"
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
                                    select new
                                    {
                                        p.TaskId,
                                        p.IsDown,
                                        p.Mark,
                                        p.MaterialScience,
                                        p.Name,
                                        p.Sorts,
                                        s.ApplyMan,
                                        s.ApplyManId,
                                        s.CreateTime,
                                        s.DefaultWorkTime,
                                        s.DrawingNo,
                                        w.IsFinish,
                                        w.ProjectInfoId,
                                        w.StartTime,
                                        w.EndTime,
                                        w.UseTime,
                                        w.Worker,
                                        w.WorkerId
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

        #region 工序信息查询

        /// <summary>
        /// 工序信息查询
        /// </summary>
        /// <param name="DrawingNo">零件编号</param>
        /// <returns></returns>
        /// 测试数据：/DrawingDown/GetProcedureInfo
        /// var DrawingNoList=  [{ "DrawingNo": "DTE-801B-WX-01C" }, { "DrawingNo": "DTE-801B-WX-01C"}] 
        [HttpPost]
        public string GetProcedureInfo()
        {
            try
            {
                StreamReader streamReader = new StreamReader(Request.InputStream);
                var List = streamReader.ReadToEnd();
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
                    using (DDContext context = new DDContext())
                    {
                        CommomModel commomModel = new CommomModel();
                        List<CommomModel> commomModelList = JsonHelper.JsonToObject<List<CommomModel>>(List);
                        Dictionary<string, List<ProcedureInfo>> dic = new Dictionary<string, List<ProcedureInfo>>();
                        foreach (var drawingNo in commomModelList)
                        {
                            List<ProcedureInfo> ListProcedureInfo = context.ProcedureInfo.Where(u => u.DrawingNo == drawingNo.msg.DrawingNo).ToList();
                            dic.Add(drawingNo.msg.DrawingNo, ListProcedureInfo);
                        }
                        return JsonConvert.SerializeObject(dic);
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

        #region 员工信息查询

        /// <summary>
        /// 员工信息查询
        /// </summary>
        /// <returns></returns>
        /// 测试数据：测试数据：/DrawingDown/GetWorkerInfo
        public string GetWorkerInfo()
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Worker> ListWorker = context.Worker.ToList();
                    return JsonConvert.SerializeObject(ListWorker);
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

        #endregion
    }
}
