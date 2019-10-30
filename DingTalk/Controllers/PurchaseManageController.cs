using Common.ClassChange;
using Common.DTChange;
using Common.Excel;
using Common.Ionic;
using Common.PDF;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalk.Models.KisLocalModels;
using DingTalk.Models.KisModels;
using DingTalk.Models.OfficeLocalModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 采购管理
    /// </summary>
    [RoutePrefix("Purchase")]
    public class PurchaseManageController : ApiController
    {

        /// <summary>
        /// 采购表单批量保存
        /// </summary>
        /// <param name="purchaseTableList"></param>
        /// <returns></returns>
        /// 测试数据: /Purchase/SavePurchaseTable
        /// data:[{ "Id": 1.0, "TaskId": "流水号", "CodeNo": "物料编码", "Name": "苹果", "Standard": "型号", "Unit": "单位", "Count": "数量", "Price": "单价", "Purpose": "用途", "UrgentDate": "需用日期", "Mark": "备注" }, { "Id": 2.0, "TaskId": "流水号2", "CodeNo": "物料编码2", "Name": "苹果2", "Standard": "型号2", "Unit": "单位2", "Count": "数量2", "Price": "单价2", "Purpose": "用途2", "UrgentDate": "需用日期2", "Mark": "备注2" }];
        /// contentType: 'application/json; charset=utf-8',
        [Route("SavePurchaseTable")]
        [HttpPost]
        public string SavePurchaseTable([FromBody]List<PurchaseTable> purchaseTableList)
        {
            try
            {
                string contentType = System.Web.HttpContext.Current.Request.ContentType;
                using (DDContext context = new DDContext())
                {
                    //foreach (PurchaseTable purchaseTable in purchaseTableList)
                    //{
                    //    context.PurchaseTable.Add(purchaseTable);
                    //    int i= context.SaveChanges();
                    //}
                    EFHelper<PurchaseTable> eFHelper = new EFHelper<PurchaseTable>();
                    if (purchaseTableList.Count == 0)
                    {
                        return JsonConvert.SerializeObject(new ErrorModel
                        {
                            Content = contentType,
                            errorCode = 1,
                            errorMessage = "未接收到传递参数"
                        });
                    }
                    else
                    {
                        foreach (var item in purchaseTableList)
                        {
                            eFHelper.Add(item);
                        }
                    }

                }
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    Content = contentType,
                    errorCode = 0,
                    errorMessage = "保存成功"
                });
            }
            catch (Exception ex)
            {
                throw ex;
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
                throw ex;
            }
        }


        #region 金蝶产品信息读取
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
                //using (KisContext context = new KisContext())
                //{
                //    var Quary = context.Database.SqlQuery<DingTalk.Models.KisModels.t_ICItem>
                //        (string.Format("SELECT * FROM t_ICItem WHERE FName like  '%{0}%' or  FNumber like '%{1}%'  or FModel  like '%{2}%'", Key, Key, Key)).ToList();

                //    return JsonConvert.SerializeObject(Quary);
                //}
                using (DDContext context = new DDContext())
                {
                    var Quary = context.KisPurchase.Where(k => k.FName.Contains(Key) ||
                    k.FNumber.Contains(Key) || k.FModel.Contains(Key)).ToList();
                    return JsonConvert.SerializeObject(Quary);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 金蝶产品信息读取(手机版分页)
        /// </summary>
        /// <param name="Key">查询关键字</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string Key, int pageIndex, int pageSize)
        {
            try
            {
                EFHelper<KisPurchase> eFHelper = new EFHelper<KisPurchase>();
                System.Linq.Expressions.Expression<Func<KisPurchase, bool>> expression = null;
                expression = n => n.FName.Contains(Key) || n.FNumber.Contains(Key) || n.FModel.Contains(Key);
                List<KisPurchase> t_ICItemListAll = eFHelper.GetListBy(expression);
                List<KisPurchase> t_ICItem = eFHelper.GetPagedList(pageIndex, pageSize,
                     expression, n => n.FItemID);
                return new NewErrorModel()
                {
                    count = t_ICItemListAll.Count,
                    data = t_ICItem,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过本地数据(内网)同步金蝶数据
        /// </summary>
        /// <param name="State">0表示金蝶物料数据,1表示金蝶办公用品数据</param>
        /// <returns></returns>
        [Route("SynchroData")]
        [HttpGet]
        public object SynchroData(int State)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                int count = 0;
                if (State == 0)
                {
                    using (KisLocalContext kisLocalContext = new KisLocalContext())
                    {
                        using (DDContext DDcontext = new DDContext())
                        {
                            List<DingTalk.Models.KisLocalModels.t_ICItem> t_ICItemList = kisLocalContext.t_ICItem.ToList();
                            List<DingTalk.Models.DingModels.KisPurchase> KisPurchaseList = new List<KisPurchase>();
                            //清理旧数据
                            EFHelper<DingTalk.Models.DingModels.KisPurchase> eFHelper = new EFHelper<KisPurchase>();
                            eFHelper.DelBy(k => k.FItemID != null || k.FItemID == null);
                            //构造数据
                            foreach (var item in t_ICItemList)
                            {
                                KisPurchase kisPurchase = new KisPurchase()
                                {
                                    FNumber = item.FNumber,
                                    FItemID = item.FItemID.ToString(),
                                    FNote = item.FNote,
                                    FModel = item.FModel,
                                    FName = item.FName
                                };
                                DDcontext.KisPurchase.Add(kisPurchase);
                                DDcontext.SaveChanges();
                                //KisPurchaseList.Add(new KisPurchase()
                                //{
                                //    FNumber = item.FNumber,
                                //    FItemID = item.FItemID.ToString(),
                                //    FNote = item.FNote,
                                //    FModel = item.FModel,
                                //    FName = item.FName
                                //});
                            }
                            //批量插入
                            //DDcontext.BulkInsert(KisPurchaseList);
                            //DDcontext.BulkSaveChanges();
                            count = t_ICItemList.Count();
                        }
                    }
                    watch.Stop();
                    return new NewErrorModel()
                    {
                        count = count,
                        data = "耗时：" + watch.ElapsedMilliseconds,
                        error = new Error(0, "同步成功！", "") { },
                    };
                }
                else
                {
                    using (OfficeLocalContext officeLocalContext = new OfficeLocalContext())
                    {
                        using (DDContext DDcontext = new DDContext())
                        {
                            List<DingTalk.Models.OfficeLocalModels.t_ICItem> t_ICItemList = officeLocalContext.t_ICItem.ToList();
                            List<DingTalk.Models.DingModels.KisOffice> KisOfficeList = new List<KisOffice>();
                            //清理旧数据
                            EFHelper<DingTalk.Models.DingModels.KisOffice> eFHelper = new EFHelper<KisOffice>();
                            eFHelper.DelBy(k => k.FItemID != null || k.FItemID == null);
                            //构造数据
                            foreach (var item in t_ICItemList)
                            {
                                //KisOfficeList.Add();
                                DDcontext.KisOffice.Add(new KisOffice()
                                {
                                    FNumber = item.FNumber,
                                    FItemID = item.FItemID.ToString(),
                                    FNote = item.FNote,
                                    FModel = item.FModel,
                                    FName = item.FName
                                });
                                DDcontext.SaveChanges();
                            }

                            //批量插入
                            //DDcontext.BulkInsert(KisOfficeList);
                            //DDcontext.BulkSaveChanges();
                            count = t_ICItemList.Count();
                        }
                    }
                    watch.Stop();
                    return new NewErrorModel()
                    {
                        count = count,
                        data = "耗时：" + watch.ElapsedMilliseconds,
                        error = new Error(0, "同步成功！", "") { },
                    };
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        /// <summary>
        /// 打印表单数据、盖章、推送
        /// </summary>
        /// 测试数据   /Purchase/PrintAndSend
        /// data: { "UserId":"083452125733424957","TaskId":"20"}
        [HttpPost]
        [Route("PrintAndSend")]
        public async Task<string> PrintAndSend([FromBody]PrintAndSendModel printAndSendModel)
        {
            try
            {
                string TaskId = printAndSendModel.TaskId;
                string UserId = printAndSendModel.UserId;
                PDFHelper pdfHelper = new PDFHelper();
                using (DDContext context = new DDContext())
                {
                    //获取表单信息
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.NodeId == 0).First();
                    string FlowId = tasks.FlowId.ToString();
                    string ProjectId = tasks.ProjectId;
                    //判断是否有权限触发按钮
                    //string PeopleId = context.Roles.Where(r=>r.RoleName=="采购管理员").First().UserId;
                    //if (UserId != PeopleId)
                    //{
                    //    return JsonConvert.SerializeObject(new ErrorModel
                    //    {
                    //        errorCode = 1,
                    //        errorMessage = "没有权限"
                    //    });
                    //}

                    //判断流程是否已结束
                    List<Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.State == 0 && t.IsSend == false).ToList();
                    if (tasksList.Count > 0)
                    {
                        return JsonConvert.SerializeObject(new ErrorModel
                        {
                            errorCode = 2,
                            errorMessage = "流程未结束"
                        });
                    }

                    List<PurchaseTable> PurchaseTableList = context.PurchaseTable.Where(u => u.TaskId == TaskId).ToList();

                    var SelectPurchaseList = from p in PurchaseTableList
                                             select new
                                             {
                                                 p.CodeNo,
                                                 p.Name,
                                                 p.Standard,
                                                 p.Unit,
                                                 p.Count,
                                                 p.Price,
                                                 p.Purpose,
                                                 p.UrgentDate,
                                                 p.Mark
                                             };
                    DataTable dtSourse = DtLinqOperators.CopyToDataTable(SelectPurchaseList);
                    //ClassChangeHelper.ToDataTable(SelectPurchaseList);
                    List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId != 0 && u.IsSend != true && u.NodeName != "结束").ToList();
                    foreach (NodeInfo nodeInfo in NodeInfoList)
                    {
                        if (string.IsNullOrEmpty(nodeInfo.NodePeople))
                        {
                            string strNodePeople = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyMan;
                            string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                            nodeInfo.NodePeople = strNodePeople + "  " + ApplyTime;
                        }
                        else
                        {
                            string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                            nodeInfo.NodePeople = nodeInfo.NodePeople + "  " + ApplyTime;
                        }
                    }
                    DataTable dtApproveView = ClassChangeHelper.ToDataTable(NodeInfoList);
                    string FlowName = context.Flows.Where(f => f.FlowId.ToString() == FlowId).First().FlowName.ToString();
                    ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == ProjectId).First();
                    string ProjectName = projectInfo.ProjectName;
                    string ProjectNo = projectInfo.ProjectId;

                    //绘制BOM表单PDF
                    List<string> contentList = new List<string>()
                        {
                            "序号","物料编码","物料名称","规格型号","单位","数量","单价","用途","需用日期","备注"
                        };

                    float[] contentWithList = new float[]
                    {
                        50, 60, 60, 60, 60, 60, 60, 60, 60,60
                    };

                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    ProjectName, ProjectNo, "2", 300, 650, contentList, contentWithList, dtSourse, dtApproveView, null);
                    string RelativePath = "~/UploadFile/PDF/" + Path.GetFileName(path);

                    List<string> newPaths = new List<string>();
                    RelativePath = AppDomain.CurrentDomain.BaseDirectory + RelativePath.Substring(2, RelativePath.Length - 2).Replace('/', '\\');
                    newPaths.Add(RelativePath);
                    string SavePath = string.Format(@"{0}\UploadFile\Ionic\{1}.zip", AppDomain.CurrentDomain.BaseDirectory, FlowName + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //文件压缩打包
                    IonicHelper.CompressMulti(newPaths, SavePath, false);

                    //上传盯盘获取MediaId
                    SavePath = string.Format(@"~\UploadFile\Ionic\{0}", Path.GetFileName(SavePath));
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    var resultUploadMedia = await dingTalkServersController.UploadMedia(SavePath);
                    //推送用户
                    FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                    fileSendModel.UserId = UserId;
                    var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 采购管理查询并导出Excel
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> PrintExcel(string taskId, string UserId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<PurchaseTable> purchaseTables = context.PurchaseTable.Where(p => p.TaskId == taskId).ToList();
                    DataTable dtpurchaseTables = ClassChangeHelper.ToDataTable(purchaseTables);

                    string path = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet/采购导出模板.xlsx");
                    string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\采购单" + time + ".xlsx";
                    File.Copy(path, newPath);
                    if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, 0, 1))
                    {
                        DingTalkServersController dingTalkServersController = new DingTalkServersController();
                        //上盯盘
                        var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/采购单" + time + ".xlsx");
                        //推送用户
                        FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                        fileSendModel.UserId = UserId;
                        var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                        return new NewErrorModel()
                        {
                            error = new Error(0, result, "") { },
                        };
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "文件有误", "") { },
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
