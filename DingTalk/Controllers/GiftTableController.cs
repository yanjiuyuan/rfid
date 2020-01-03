using Common.ClassChange;
using Common.DTChange;
using Common.Ionic;
using Common.PDF;
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
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 礼品招待酒领用申请
    /// </summary>
    [RoutePrefix("Gift")]
    public class GiftTableController : ApiController
    {

        /// <summary>
        /// 表单保存接口
        /// </summary>
        /// <param name="giftTable"></param>
        [Route("TableSave")]
        [HttpPost]
        public NewErrorModel TableSave([FromBody] List<GiftTable> giftTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var gift in giftTable)
                    {
                        context.GiftTable.Add(gift);
                    }
                    context.SaveChanges();
                    return new NewErrorModel()
                    {
                        error = new Error(0, "添加成功", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 表单读取接口
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        [Route("GetTable")]
        [HttpGet]
        public NewErrorModel GetTable(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<GiftTable> giftTable = context.GiftTable.Where(c => c.TaskId == TaskId).ToList();
                    return new NewErrorModel()
                    {
                        count = giftTable.Count,
                        data = giftTable,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 表单批量修改
        /// </summary>
        /// <param name="giftList"></param>
        /// <returns></returns>
        [Route("TableModify")]
        [HttpPost]
        public NewErrorModel TableModify([FromBody] List<GiftTable> giftList)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var gift in giftList)
                    {
                        context.Entry<GiftTable>(gift).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    return new NewErrorModel()
                    {
                        error = new Error(0, "修改成功", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打印盖章
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <param name="UserId">推送用户Id</param>
        /// <returns></returns>
        [Route("GetPrintPDF")]
        [HttpGet]
        public async Task<object> GetPrintPDF(string TaskId, string UserId)
        {
            try
            {
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
                        return JsonConvert.SerializeObject(new NewErrorModel
                        {
                            error = new Error(1, "流程尚未结束", "") { },
                        });
                    }
                    List<GiftTable> giftTables = context.GiftTable.Where(u => u.TaskId == TaskId).ToList();

                    var giftTableList = from g in giftTables
                                        select new
                                        {
                                            g.GiftName,
                                            g.GiftCount
                                        };


                    List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId != 0 && u.IsSend != true && u.NodeName != "结束").ToList();


                    //绘制BOM表单PDF
                    List<string> contentList = new List<string>()
                        {
                          "序号","礼品名称","数量"
                        };

                    float[] contentWithList = new float[]
                    {
                        50,500,100
                    };

                    //Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    //keyValuePairs.Add("用途及使用说明", tasks.Remark);

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

                    DataTable dtGiftTables = DtLinqOperators.CopyToDataTable(giftTableList);
                    string FlowName = context.Flows.Where(f => f.FlowId.ToString() == FlowId).First().FlowName.ToString();

                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    null, null, "2", 300, 650, contentList, contentWithList, dtGiftTables, dtApproveView, null);
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
        /// 库存批量保存接口
        /// </summary>
        /// <param name="giftTable"></param>
        [Route("StockSave")]
        [HttpPost]
        public NewErrorModel StockSave([FromBody] List<Gift> giftTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var gift in giftTable)
                    {
                        context.Gift.Add(gift);
                    }
                    context.SaveChanges();
                    return new NewErrorModel()
                    {
                        error = new Error(0, "添加成功", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 库存消减
        /// </summary>
        /// <param name="giftTable"></param>
        [Route("StockReduce")]
        [HttpPost]
        public NewErrorModel StockReduce([FromBody] List<GiftTable> giftTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var gift in giftTable)
                    {
                        Gift gifts = context.Gift.Find(Int32.Parse(gift.GiftNo));
                        gifts.Stock = (Int32.Parse(gifts.Stock) - Int32.Parse(gift.GiftCount)).ToString();
                        context.Entry<Gift>(gifts).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    context.SaveChanges();
                    return new NewErrorModel()
                    {
                        error = new Error(0, "消减成功", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 库存信息读取接口
        /// </summary>
        /// <param name="key">名称、类别</param>
        /// <returns></returns>
        [Route("GetStock")]
        [HttpGet]
        public NewErrorModel GetStock(string key = "")
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Gift> giftTable = new List<Gift>();
                    if (key == "")
                    {
                        giftTable = context.Gift.ToList();
                    }
                    else
                    {
                        giftTable = context.Gift.Where(g => g.GiftName.Contains(key)
                 || g.Type.Contains(key)).ToList();
                    }

                    return new NewErrorModel()
                    {
                        count = giftTable.Count,
                        data = giftTable,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 库存批量修改
        /// </summary>
        /// <param name="giftTable"></param>
        [Route("StockModify")]
        [HttpPost]
        public Object StockModify([FromBody] List<Gift> giftTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    foreach (var gift in giftTable)
                    {
                        context.Entry<Gift>(gift).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }

                    return new NewErrorModel()
                    {
                        error = new Error(0, "修改成功", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 库存删除
        /// </summary>
        /// <param name="gift"></param>
        [Route("StockMove")]
        [HttpPost]
        public Object StockMove(Gift gift)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<Gift>(gift).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();

                    return new NewErrorModel()
                    {
                        error = new Error(0, "修改成功", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
