using Common.ClassChange;
using Common.DTChange;
using Common.Excel;
using Common.Ionic;
using Common.PDF;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// 入库
    /// </summary>
    [RoutePrefix("Godown")]
    public class GodownManagerController : ApiController
    {
        /// <summary>
        /// 入库单批量保存
        /// </summary>
        /// <param name="goDownList"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] List<GoDown> goDownList)
        {
            try
            {
                EFHelper<GoDown> eFHelper = new EFHelper<GoDown>();
                foreach (var goDown in goDownList)
                {
                    eFHelper.Add(goDown);
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 入库单读取
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string taskId)
        {
            try
            {
                EFHelper<GoDown> eFHelper = new EFHelper<GoDown>();
                List<GoDown> goDownList = eFHelper.GetListBy(t => t.TaskId == taskId).ToList();

                return new NewErrorModel()
                {
                    count = goDownList.Count,
                    data = goDownList,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 打印表单
        /// </summary>
        /// <param name="printAndSendModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintPDF")]
        public async Task<object> PrintAndSend([FromBody]PrintModel printAndSendModel)
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

                    //判断流程是否已结束
                    List<Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.State == 0 && t.IsSend == false).ToList();
                    if (tasksList.Count > 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "流程尚未结束", "") { },
                        };
                    }

                    List<GoDown> GoDownList = context.GoDown.Where(u => u.TaskId == TaskId).ToList();

                    var SelectGoDownList = from g in GoDownList
                                           select new
                                           {
                                               g.fNumber,
                                               g.fName,
                                               g.fModel,
                                               g.unitName,
                                               g.fQty,
                                               g.fPrice,
                                               g.fAmount,
                                               //g.fFullName
                                           };
                    DataTable dtSourse = DtLinqOperators.CopyToDataTable(SelectGoDownList);
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

                    //绘制BOM表单PDF
                    List<string> contentList = new List<string>()
                        {
                            "序号","物料编码","物料名称","规格型号","单位","实收数量","单价","金额"
                        };

                    float[] contentWithList = new float[]
                    {
                        50, 60, 60, 60, 60, 60, 60, 60
                    };

                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    List<string> vs = new List<string>();
                    foreach (var item in GoDownList)
                    {
                        if (!vs.Contains(item.fFullName))
                        {
                            vs.Add(item.fFullName);
                        }
                    }

                    keyValuePairs.Add("供应商", string.Join(",", vs));
                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    null, null, "2", 300, 650, contentList, contentWithList, dtSourse, dtApproveView, null, keyValuePairs);
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
                    return new NewErrorModel()
                    {
                        error = new Error(0, result, "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 导出Excel数据
        /// </summary>
        /// <param name="printAndSendModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintExcel")]
        public async Task<object> PrintExcel([FromBody]PrintModel printAndSendModel)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<GoDown> purchaseTables = context.GoDown.Where(p => p.TaskId == printAndSendModel.TaskId).ToList();
                    DataTable dtpurchaseTables = ClassChangeHelper.ToDataTable(purchaseTables);

                    string path = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet/入库导出模板.xlsx");
                    string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\入库单" + time + ".xlsx";
                    File.Copy(path, newPath);
                    if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, 0, 1))
                    {
                        DingTalkServersController dingTalkServersController = new DingTalkServersController();
                        //上盯盘
                        var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/入库单" + time + ".xlsx");
                        //推送用户
                        FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                        fileSendModel.UserId = printAndSendModel.UserId;
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
