using Common.JsonHelper;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.Models;
using DingTalk.Models.DingModels;
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
        /// 测试数据：/DrawingDown/GetDrawingDownInfo?ProjectId=666&ApplyManId=123456
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
                                        p.BomId
                                    };

                        var PurchaseProcedureInfo = context.PurchaseProcedureInfo.ToList();
                        if (PurchaseProcedureInfo.Count == 0)
                        {
                            var QuaryEmpty = from q in Quary
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
                                                 BomId = q.BomId,
                                                 ProList = new List<Pro>()
                                                 {

                                                 },
                                             };
                            return JsonConvert.SerializeObject(QuaryEmpty);
                        }
                        var ProcedureInfo = context.ProcedureInfo.ToList();
                        var WorkTime = context.WorkTime.ToList();



                        var QuaryPro = from q in Quary
                                       join pp in PurchaseProcedureInfo
                                       on q.DrawingNo equals pp.DrawingNo
                                       into temp
                                       from tt in temp.DefaultIfEmpty()
                                           //join p in ProcedureInfo
                                           //on tt.ProcedureInfoId equals p.Id.ToString()
                                           //into temps
                                           //from ss in temps.DefaultIfEmpty()
                                           //join w in WorkTime
                                           //on pp.Id.ToString() equals w.PurchaseProcedureInfoId
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
                                           BomId = q.BomId,
                                           ProcedureInfoId = tt == null ? "" : tt.ProcedureInfoId
                                           //ProList = new List<Pro>()
                                           //{
                                           //new Pro(){
                                           //     ProcedureId= ss==null?"": ss.Id.ToString(),
                                           //     ProcedureName = ss==null?"":ss.ProcedureName,
                                           //     //WorkTimeList=new List<WorkTimes>(){
                                           //     //    new WorkTimes(){
                                           //     //       Worker= w.Worker,
                                           //     //       WorkerId=w.WorkerId,
                                           //     //       UseTime=w.UseTime
                                           //     //    }

                                           //     //}
                                           //}
                                           //},
                                       };

                        var QuaryPros = from q in QuaryPro
                                        join p in ProcedureInfo
                                        on q.ProcedureInfoId equals p.Id.ToString()
                                        into temp
                                        from tt in temp.DefaultIfEmpty()
                                        select new
                                        {
                                            DrawingNoId = q.DrawingNoId,
                                            DrawingNo = q.DrawingNo,
                                            Name = q.Name,
                                            Sorts = q.Sorts,
                                            TaskId = q.TaskId,
                                            MaterialScience = q.MaterialScience,
                                            Brand = q.Brand,
                                            Count = q.Count,
                                            Unit = q.Unit,
                                            Mark = q.Mark,
                                            BomId = q.BomId,
                                            ProcedureInfoId = q.ProcedureInfoId,

                                            ProList = new List<Pro>()
                                            {
                                                tt == null ? null:
                                                      new Pro(){
                                                 ProcedureId= tt==null?"": tt.Id.ToString(),
                                                 ProcedureName = tt==null?"":tt.ProcedureName,
                                                 //WorkTimeList=new List<WorkTimes>(){
                                                 //    new WorkTimes(){
                                                 //       Worker= w.Worker,
                                                 //       WorkerId=w.WorkerId,
                                                 //       UseTime=w.UseTime
                                                 //    }
                                                 //}
                                                }
                                            },
                                        };


                        //var oPProcInfo = from pp in PurchaseProcedureInfo
                        //                 join p in ProcedureInfo
                        //                 on pp.ProcedureInfoId equals p.Id.ToString()
                        //                 join w in WorkTime
                        //                 on pp.Id.ToString() equals w.PurchaseProcedureInfoId
                        //                 select new
                        //                 {
                        //                     DrawingNo = pp.DrawingNo,
                        //                     ProList = new List<Pro>() {
                        //                        new Pro()
                        //                        {
                        //                            ProcedureName = p.ProcedureName,
                        //                            ProcedureId = p.Id.ToString(),
                        //                            //WorkTimeList = new List<WorkTimes>()
                        //                            //{
                        //                            //    new WorkTimes(){
                        //                            //    Worker= w.Worker,
                        //                            //    WorkerId=w.WorkerId,
                        //                            //    UseTime= w.UseTime
                        //                            //    }
                        //                            //}
                        //                        }
                        //                    }
                        //                 };

                        //var QuaryPro = from q in Quary
                        //               join pp in oPProcInfo
                        //               on q.DrawingNo equals pp.DrawingNo
                        //               select new
                        //               {
                        //                   DrawingNo = q.DrawingNo,
                        //                   Name = q.Name,
                        //                   Sorts = q.Sorts,
                        //                   TaskId = q.TaskId,
                        //                   MaterialScience = q.MaterialScience,
                        //                   Brand = q.Brand,
                        //                   Count = q.Count,
                        //                   Unit = q.Unit,
                        //                   Mark = q.Mark,
                        //                   ProList = pp.ProList,
                        //               };

                        //var QuaryPros = from q in QuaryPro
                        //                group new
                        //                {
                        //                    q.DrawingNo,
                        //                    q.Name,
                        //                    q.Sorts,
                        //                    q.TaskId,
                        //                    q.MaterialScience,
                        //                    q.Brand,
                        //                    q.Count,
                        //                    q.Unit,
                        //                    q.Mark,
                        //                    q.ProList
                        //                } by q.DrawingNo into g
                        //                select g;

                        return JsonConvert.SerializeObject(QuaryPros);
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
                            context.SaveChanges();
                            //修改下发状态
                            Purchase purchase = context.Purchase.Where(u => u.DrawingNo == purchaseDown.DrawingNo).First();
                            purchase.IsDown = true;
                            context.Entry<Purchase>(purchase).State = EntityState.Modified;
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

        #endregion

        #region 添加、绑定、删除工序与工时

        /// <summary>
        /// 添加工序
        /// </summary>
        /// <returns></returns>
        /// 测试数据: /DrawingDown/AddProcedure
        ///  var PurchaseList = [{"ProcedureName": "中料", "DefaultWorkTime": "1", "State": "0", "CreateTime": "2018-04-24 15:48", "ApplyMan": "胡工", "ApplyManId": "123456"},
        ///  { "ProcedureName": "喷漆", "DefaultWorkTime": "1", "State": "0", "CreateTime": "2018-04-24 15:48", "ApplyMan": "胡工", "ApplyManId": "123456"},
        ///  { "ProcedureName": "切割", "DefaultWorkTime": "1", "State": "0", "CreateTime": "2018-04-24 15:48", "ApplyMan": "胡工", "ApplyManId": "123456"}] 
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
        /// 工序关系表Id查询
        /// </summary>
        /// <returns></returns>
        /// 测试数据：
        ///  /DrawingDown/GetProcedureId
        ///   var GetProcedureIdList =  [{"DrawingNo":"DTE-801B-PT-13","ProcedureInfoId":"1","TaskId":"3"}]


        public string GetProcedureId()
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

                    List<PurchaseProcedureInfo> procedureInfoList = new List<PurchaseProcedureInfo>();
                    procedureInfoList = JsonHelper.JsonToObject<List<PurchaseProcedureInfo>>(List);

                    List<string> stringList = new List<string>();
                    using (DDContext context = new DDContext())
                    {
                        foreach (var item in procedureInfoList)
                        {
                            string Id = context.PurchaseProcedureInfo.Where(u => u.DrawingNo == item.DrawingNo && u.ProcedureInfoId == item.ProcedureInfoId && u.TaskId == item.TaskId).Select(q => q.Id).DefaultIfEmpty().First().ToString();
                            if (!string.IsNullOrEmpty(Id))
                            {
                                stringList.Add(Id);
                            }
                        }
                    }
                    return JsonConvert.SerializeObject(stringList);
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
        /// 查询工序
        /// </summary>
        /// <returns></returns>
        /// 测试数据：/DrawingDown/QuaryAllProcedure
        [HttpGet]
        public string QuaryAllProcedure()
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<ProcedureInfo> procedureInfo = context.ProcedureInfo.ToList();
                    return JsonConvert.SerializeObject(procedureInfo);
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
        /// 绑定工时和工序以及表单
        /// </summary>
        /// <returns></returns>
        /// 测试数据:/DrawingDown/BindWorkTimeAndPro
        ///  var WorkTimeAndProList = [{  "IsFinish": false, "Worker": "小红", "WorkerId": "666", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "2","DrawingNo": "DTE-801B-WX-01A", "ProcedureInfoId": "1", "CreateManId": "123456","TaskId":"4","OldTaskId":"123","OldTaskId":"1","CodeNo":"1","Name":"11", "Count":"1",   "MaterialScience":"1","Unit":"1","Brand":"1","Sorts":"1","FlowType":"1","BomId":"123"},
        ///  {  "IsFinish": false, "Worker": "小滨", "WorkerId": "777", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "3","DrawingNo": "DTE-801B-WX-01A", "ProcedureInfoId": "2", "CreateManId": "123456","TaskId":"4","OldTaskId":"123", "OldTaskId":"1","CodeNo":"1","Name":"11", "Count":"1","MaterialScience":"1","Unit":"1","Brand":"1","Sorts":"1","FlowType":"1","BomId":"123"},
        ///  {  "IsFinish": true, "Worker": "小雨", "WorkerId": "888", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "3","DrawingNo": "DTE-801B-WX-01A", "ProcedureInfoId": "3", "CreateManId": "123456","TaskId":"4","OldTaskId":"123", "OldTaskId":"1","CodeNo":"1","Name":"11", "Count":"1","MaterialScience":"1","Unit":"1","Brand":"1","Sorts":"1","FlowType":"1","BomId":"123"}] 

        [HttpPost]
        public string BindWorkTimeAndPro()
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
                    List<WorkTimeAndPur> WorkTimeAndPurList = new List<WorkTimeAndPur>();
                    WorkTimeAndPurList = JsonHelper.JsonToObject<List<WorkTimeAndPur>>(List);
                    List<string> ProcedureIdList = new List<string>();

                    using (DDContext context = new DDContext())
                    {
                        foreach (var item in WorkTimeAndPurList)
                        {
                            //绑定工序
                            PurchaseProcedureInfo purchaseProcedureInfo = new PurchaseProcedureInfo
                            {
                                DrawingNo = item.DrawingNo,
                                ProcedureInfoId = item.ProcedureInfoId,
                                TaskId = item.TaskId,
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")
                            };
                            context.PurchaseProcedureInfo.Add(purchaseProcedureInfo);
                            context.SaveChanges();

                            //绑定工时
                            WorkTime workTime = new WorkTime()
                            {
                                IsFinish = false,
                                PurchaseProcedureInfoId = purchaseProcedureInfo.Id.ToString(),
                                StartTime = item.StartTime,
                                EndTime = item.EndTime,
                                UseTime = item.UseTime,
                                Worker = item.Worker,
                                WorkerId = item.WorkerId
                            };
                            context.WorkTime.Add(workTime);
                            context.SaveChanges();
                            

                            //绑定Bom表单
                            PurchaseDown purchaseDown = new PurchaseDown()
                            {
                                PurchaseProcedureInfoId= purchaseProcedureInfo.Id.ToString(),
                                DrawingNo=item.DrawingNo,
                                OldTaskId = item.OldTaskId,
                                TaskId=item.TaskId,
                                BomId = item.BomId,
                                CodeNo = item.CodeNo,
                                Name = item.Name,
                                Count = item.Count,
                                MaterialScience = item.MaterialScience,
                                Unit = item.Unit,
                                Brand = item.Brand,
                                Sorts = item.Sorts,
                                IsDown =true,
                                FlowType = item.FlowType
                            };
                            context.PurchaseDown.Add(purchaseDown);
                            context.SaveChanges();

                            //修改下发状态
                            Purchase purchase = context.Purchase.Where(u => u.DrawingNo == item.DrawingNo).First();
                            purchase.IsDown = true;
                            context.Entry<Purchase>(purchase).State = EntityState.Modified;
                            context.SaveChanges();
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
        /// 绑定工序
        /// </summary>
        /// <returns></returns>
        /// 测试数据: /DrawingDown/BindProcedure
        ///  var BindPurchaseList = [{ "DrawingNo": "DTE-801B-WX-01A", "ProcedureInfoId": "1", "CreateManId": "123456","TaskId":"4" }, { "DrawingNo": "DTE-801B-PT-14", "ProcedureInfoId": "2", "CreateManId": "123456","TaskId":"4"  }, { "DrawingNo": "DTE-801B-PT-13", "ProcedureInfoId": "4", "CreateManId": "123456","TaskId":"4"  }]
        [HttpPost]
        public string BindProcedure()
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
                    List<PurchaseProcedureInfo> procedureInfoList = new List<PurchaseProcedureInfo>();
                    procedureInfoList = JsonHelper.JsonToObject<List<PurchaseProcedureInfo>>(List);
                    List<string> ProcedureIdList = new List<string>();

                    using (DDContext context = new DDContext())
                    {

                        List<PurchaseProcedureInfo> QuaryprocedureInfoList = context.PurchaseProcedureInfo.ToList();
                        if (QuaryprocedureInfoList.Count == 0)
                        {
                            foreach (PurchaseProcedureInfo purchaseProcedureInfo in procedureInfoList)
                            {
                                purchaseProcedureInfo.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                context.PurchaseProcedureInfo.Add(purchaseProcedureInfo);
                                context.SaveChanges();
                                ProcedureIdList.Add(purchaseProcedureInfo.Id.ToString());
                            }
                        }
                        else
                        {
                            foreach (PurchaseProcedureInfo purchaseProcedureInfo in procedureInfoList)
                            {
                                foreach (var item in QuaryprocedureInfoList)
                                {
                                    if (item.TaskId == purchaseProcedureInfo.TaskId && item.ProcedureInfoId == purchaseProcedureInfo.ProcedureInfoId && item.DrawingNo == purchaseProcedureInfo.DrawingNo)
                                    {
                                        return JsonConvert.SerializeObject(new ErrorModel
                                        {
                                            errorCode = 1,
                                            errorMessage = "插入数据有误",
                                            Content = string.Format("Index:{0}", procedureInfoList.IndexOf(purchaseProcedureInfo).ToString())
                                        });
                                    }
                                    else
                                    {
                                        purchaseProcedureInfo.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        context.PurchaseProcedureInfo.Add(purchaseProcedureInfo);
                                        context.SaveChanges();
                                        ProcedureIdList.Add(purchaseProcedureInfo.Id.ToString());
                                    }
                                }
                            }
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
        ///  var WorkTimeList = [{ "PurchaseProcedureInfoId": "7", "IsFinish": false, "Worker": "小红", "WorkerId": "666", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "2"},
        ///  { "PurchaseProcedureInfoId": "8", "IsFinish": false, "Worker": "小滨", "WorkerId": "777", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "3"},
        ///  { "PurchaseProcedureInfoId": "9", "IsFinish": true, "Worker": "小雨", "WorkerId": "888", "StartTime": "2018-04-24 15:48", "EndTime": "2018-04-25 15:48", "UseTime": "3"}] 
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
        /// 修改工时状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ChangeWorkTimeState()
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
                            context.Entry<WorkTime>(workTime).State = EntityState.Modified;
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
        /// 测试数据: /DrawingDown/GetPersonInfo?ApplyManId=073110326032521796&TaskId=101
        [HttpGet]
        public string GetPersonInfo(string ApplyManId, int TaskId = 0)
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
                        List<PurchaseDown> PurchaseDownList = context.PurchaseDown.
                            Where(u => u.TaskId == TaskId.ToString()).ToList();
                        List<ProcedureInfo> ProcedureInfoList = context.ProcedureInfo.ToList();
                        List<PurchaseProcedureInfo> PurchaseProcedureInfoList = context.PurchaseProcedureInfo.ToList();
                        List<WorkTime> WorkTimeInfoList = context.WorkTime.ToList();
                        var Quary = from p in PurchaseDownList
                                    join s in PurchaseProcedureInfoList on
                                    p.PurchaseProcedureInfoId equals s.Id.ToString()
                                    join pd in ProcedureInfoList on
                                    s.ProcedureInfoId equals pd.Id.ToString()
                                    join w in WorkTimeInfoList on
                                    p.PurchaseProcedureInfoId equals w.PurchaseProcedureInfoId
                                    select new
                                    {

                                        p.TaskId,
                                        p.IsDown,
                                        p.Mark,
                                        p.MaterialScience,
                                        p.Name,
                                        p.Sorts,
                                        //s.ApplyMan,
                                        //s.ApplyManId,
                                        s.CreateTime,
                                        //s.DefaultWorkTime,
                                        w.IsFinish,
                                        //w.ProcedureId,
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
        /// var DrawingNoList=  [{ "Id": "1" }, { "DrawingNo": "2"}] 
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
                            List<ProcedureInfo> ListProcedureInfo = context.ProcedureInfo.Where(u => u.Id.ToString() == drawingNo.msg.Id).ToList();
                            dic.Add(drawingNo.msg.Id, ListProcedureInfo);
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

        #region 绑定数据读取(用于下发审批页面数据读取)
        /// <summary>
        /// 绑定数据读取
        /// </summary>s
        /// <param name="ApplyManId">当前用户Id<param>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        /// 测试数据：/DrawingDown/GetFinishInfo?ApplyManId=100328051024695354&TaskId=4
        [HttpGet]
        public string GetFinishInfo(string ApplyManId, string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<string> ListPeopleId = context.NodeInfo.Where(u => u.FlowId == "7" && (u.NodeId.ToString() == "2" || u.NodeId.ToString() == "3")).Select(u => u.PeopleId).ToList();
                    if (ListPeopleId.Contains(ApplyManId))
                    {
                        List<PurchaseDown> PurchaseDownList = context.PurchaseDown.
                             Where(u => u.TaskId == TaskId.ToString()).ToList();
                        List<ProcedureInfo> ProcedureInfoList = context.ProcedureInfo.ToList();
                        List<PurchaseProcedureInfo> PurchaseProcedureInfoList = context.PurchaseProcedureInfo.ToList();
                        List<WorkTime> WorkTimeInfoList = context.WorkTime.ToList();
                        var Quary = from p in PurchaseDownList
                                    join s in PurchaseProcedureInfoList on
                                    p.PurchaseProcedureInfoId equals s.Id.ToString()
                                    join pd in ProcedureInfoList on
                                    s.ProcedureInfoId equals pd.Id.ToString()
                                    join w in WorkTimeInfoList on
                                    p.PurchaseProcedureInfoId equals w.PurchaseProcedureInfoId
                                    select new
                                    {
                                        p.TaskId,
                                        p.IsDown,
                                        p.DrawingNo,
                                        p.Mark,
                                        p.MaterialScience,
                                        p.Name,
                                        p.Sorts,
                                        p.Count,
                                        PurchaseProcedureInfoId=s.Id,
                                        s.CreateTime,
                                        pd.ProcedureName,
                                        w.IsFinish,
                                        w.StartTime,
                                        w.EndTime,
                                        w.UseTime,
                                        w.Worker,
                                        w.WorkerId,
                                        Id = w.Id
                                    };
                        return JsonConvert.SerializeObject(Quary);
                    }
                    else
                    {
                        List<PurchaseDown> PurchaseDownList = context.PurchaseDown.
                             Where(u => u.TaskId == TaskId.ToString()).ToList();
                        List<ProcedureInfo> ProcedureInfoList = context.ProcedureInfo.ToList();
                        List<PurchaseProcedureInfo> PurchaseProcedureInfoList = context.PurchaseProcedureInfo.ToList();
                        List<WorkTime> WorkTimeInfoList = context.WorkTime.ToList();
                        var Quary = from p in PurchaseDownList
                                    join s in PurchaseProcedureInfoList on
                                    p.PurchaseProcedureInfoId equals s.Id.ToString()
                                    join pd in ProcedureInfoList on
                                    s.ProcedureInfoId equals pd.Id.ToString()
                                    join w in WorkTimeInfoList on
                                    p.PurchaseProcedureInfoId equals w.PurchaseProcedureInfoId
                                    where w.WorkerId == ApplyManId
                                    select new
                                    {
                                        p.TaskId,
                                        p.IsDown,
                                        p.DrawingNo,
                                        p.Mark,
                                        p.MaterialScience,
                                        p.Name,
                                        p.Sorts,
                                        p.Count,
                                        s.CreateTime,
                                        PurchaseProcedureInfoId = s.Id,
                                        pd.ProcedureName,
                                        w.IsFinish,
                                        w.StartTime,
                                        w.EndTime,
                                        w.UseTime,
                                        w.Worker,
                                        w.WorkerId,
                                        Id = w.Id
                                    };
                        return JsonConvert.SerializeObject(Quary);
                    }
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

        #region 图纸管理

        #endregion

    }
}
