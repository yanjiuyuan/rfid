using Common.ClassChange;
using Common.DTChange;
using Common.Ionic;
using Common.PDF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalk.Models.KisModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
                using (KisContext context = new KisContext())
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
                return JsonConvert.SerializeObject(new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                });
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
                    string PeopleId = context.NodeInfo.Where(n => n.NodeName == "院领导审核" && n.FlowId == FlowId).First().PeopleId;
                    if (UserId != PeopleId)
                    {
                        return JsonConvert.SerializeObject(new ErrorModel
                        {
                            errorCode = 1,
                            errorMessage = "没有权限"
                        });
                    }
                    //判断流程是否已结束
                    List<Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.State == 0 && t.IsSend==false).ToList();
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
                                                 p.CodeNo,p.Name,p.Standard,p.Unit,
                                                 p.Count,p.Price,p.Purpose,p.UrgentDate,p.Mark
                                             };
                    DataTable dtSourse = DtLinqOperators.CopyToDataTable(SelectPurchaseList);
                    //ClassChangeHelper.ToDataTable(SelectPurchaseList);
                    List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId != 0 && u.NodeName != "结束").ToList();
                    foreach (NodeInfo nodeInfo in NodeInfoList)
                    {
                        if (string.IsNullOrEmpty(nodeInfo.NodePeople))
                        {
                            string strNodePeople = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyMan;
                            nodeInfo.NodePeople = strNodePeople;
                        }
                    }
                    DataTable dtApproveView = ClassChangeHelper.ToDataTable(NodeInfoList);
                    string FlowName = context.Flows.Where(f => f.FlowId.ToString() == FlowId).First().FlowName.ToString();
                    string ProjectName = context.ProjectInfo.Where(p => p.ProjectId == ProjectId).First().ProjectName;
                    
                    //绘制BOM表单PDF
                    List<string> contentList = new List<string>()
                        {
                            "序号","物料编码","物料名称","规格型号","单位","数量","单价","用途","需用日期","备注"
                        };

                    float[] contentWithList = new float[]
                    {
                        50, 60, 60, 60, 60, 60, 60, 60, 60,60
                    };

                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.ApplyTime,
                    ProjectName, "2",300,650, contentList, contentWithList, dtSourse, dtApproveView);
                    string RelativePath = "~/UploadFile/PDF/" + Path.GetFileName(path);


                    List<string> newPaths = new List<string>();
                    RelativePath = AppDomain.CurrentDomain.BaseDirectory + RelativePath.Substring(2, RelativePath.Length - 2).Replace('/', '\\');
                    newPaths.Add(RelativePath);
                    string SavePath = string.Format(@"{0}\UploadFile\Ionic\{1}.zip", AppDomain.CurrentDomain.BaseDirectory, FlowName + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //文件压缩打包
                    IonicHelper.CompressMulti(newPaths, SavePath, false);

                    ///上传盯盘获取MediaId
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
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 3,
                    errorMessage = ex.Message
                });
            }
        }
    }
}
