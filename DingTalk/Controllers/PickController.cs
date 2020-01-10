using Common.ClassChange;
using Common.DTChange;
using Common.Excel;
using Common.Ionic;
using Common.PDF;
using DingTalk.Bussiness.FlowInfo;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 领料
    /// </summary>
    [RoutePrefix("Pick")]
    public class PickController : ApiController
    {
        /// <summary>
        /// 领料单批量保存
        /// </summary>
        /// <param name="pickList"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] List<Pick> pickList)
        {
            try
            {

                EFHelper<Pick> eFHelper = new EFHelper<Pick>();
                foreach (var pick in pickList)
                {
                    eFHelper.Add(pick);
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
        /// 默认读取已验收的数据
        /// </summary>
        /// <param name="ApplyManId"></param>
        /// <param name="TaskId">钉钉零部件申请流水号</param>
        /// <returns></returns>
        [Route("ReadDefault")]
        [HttpGet]
        public NewErrorModel ReadDefault(string ApplyManId, string TaskId)
        {
            try
            {
                //Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                //keyValuePairs.Add("FBillNo", "88");
                //string result = HttpPost("http://wuliao5222.55555.io:35705/api/GoDown/GetGodownInfoByFBillNo",
                //   keyValuePairs);


                //HttpWebResponse httpWebResponse = CreateGetHttpResponse("http://wuliao5222.55555.io:35705/api/Pick/GetAll", 5000, null, null);
                //StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                //string content = reader.ReadToEnd();
                //NewErrorModel newErrorModel = new NewErrorModel()
                //{
                //    data = new List<GodownModel>() { },
                //};
                //newErrorModel = JsonConvert.DeserializeObject<NewErrorModel>(content);

                //using (DDContext context = new DDContext())
                //{
                //    List<GodownModel> goDowns = JsonConvert.DeserializeObject<List<GodownModel>>(newErrorModel.data.ToString()); ;
                //    Flows flows = context.Flows.Where(f => f.FlowName.Contains("零部件")).First();
                //    List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskId(flows.FlowId.ToString());
                //    List<Tasks> taskQuery = tasks.Where(t => t.TaskId.ToString() == TaskId && t.NodeId == 1).ToList();
                //    List<PurchaseTable> PurchaseTables = new List<PurchaseTable>();
                //    foreach (var task in taskQuery)
                //    {
                //        PurchaseTables.AddRange(context.PurchaseTable.Where(g => g.TaskId == task.TaskId.ToString()));
                //    }
                //    List<GodownModel> GodownModelList = new List<GodownModel>();
                //    foreach (var goDown in goDowns)
                //    {
                //        foreach (var PurchaseTable in PurchaseTables)
                //        {
                //            if (goDown.fNumber == PurchaseTable.CodeNo)
                //            {
                //                GodownModelList.Add(goDown);
                //            }
                //        }
                //    }

                //    return new NewErrorModel()
                //    {
                //        count = GodownModelList.Count,
                //        data = GodownModelList,
                //        error = new Error(0, "读取成功！", "") { },
                //    };
                //}



                using (DDContext context = new DDContext())
                {
                    //判断流水号是否存在
                    if (context.TasksState.Where(t => t.TaskId == TaskId && t.FlowName.Contains("零部件")).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "该流水号不存在或者不是零部件采购流程！", "") { },
                        };
                    }


                    HttpWebResponse httpWebResponse = CreateGetHttpResponse("http://wuliao5222.55555.io:35705/api/Pick/ReadPickInfoSingle", 5000, null, null);
                    StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                    string content = reader.ReadToEnd();
                    NewErrorModel newErrorModel = new NewErrorModel()
                    {
                        data = new List<GodownModel>() { },
                    };
                    newErrorModel = JsonConvert.DeserializeObject<NewErrorModel>(content);
                    List<GodownModel> goDowns = JsonConvert.DeserializeObject<List<GodownModel>>(newErrorModel.data.ToString()); ;


                    Flows flows = context.Flows.Where(f => f.FlowName.Contains("零部件")).First();
                    List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskId(flows.FlowId.ToString());
                    List<Tasks> taskQuery = tasks.Where(t => t.TaskId.ToString() == TaskId && t.NodeId == 1).ToList();
                    List<PurchaseTable> PurchaseTables = new List<PurchaseTable>();
                    foreach (var task in taskQuery)
                    {
                        PurchaseTables.AddRange(context.PurchaseTable.Where(g => g.TaskId == task.TaskId.ToString()));
                    }
                    List<GodownModel> GodownModelList = new List<GodownModel>();
                    foreach (var goDown in goDowns)
                    {
                        foreach (var PurchaseTable in PurchaseTables)
                        {
                            if (goDown.fNumber == PurchaseTable.CodeNo)
                            {
                                GodownModelList.Add(goDown);
                            }
                        }
                    }

                    if (GodownModelList.Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "查询的物料暂无库存！", "") { },
                        };
                    }

                    return new NewErrorModel()
                    {
                        count = GodownModelList.Count,
                        data = GodownModelList,
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
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SocketTest")]
        public NewErrorModel SocketTest()
        {
            HttpWebResponse httpWebResponse = CreateGetHttpResponse("http://wuliao5222.55555.io:35705/api/SocketServer/SaveOutUrl?UserId=123", 5000, null, null);
            StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            return new NewErrorModel()
            {
                data= content
            };
        }

        /// <summary>
        /// 领料单读取
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string taskId)
        {
            try
            {
                EFHelper<Pick> eFHelper = new EFHelper<Pick>();
                List<Pick> pickList = eFHelper.GetListBy(t => t.TaskId == taskId).ToList();
                return new NewErrorModel()
                {
                    count = pickList.Count,
                    data = pickList,
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

                    List<Pick> GoDownList = context.Pick.Where(u => u.TaskId == TaskId).ToList();

                    string ProjectName = tasks.ProjectId;
                    string ProjectNo = tasks.ProjectName;

                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("领料用途", ProjectName + "--" + ProjectNo);

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
                                               g.fFullName
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
                    DataTable dtApproveView = Common.ClassChange.ClassChangeHelper.ToDataTable(NodeInfoList);
                    string FlowName = context.Flows.Where(f => f.FlowId.ToString() == FlowId).First().FlowName.ToString();

                    //绘制BOM表单PDF
                    List<string> contentList = new List<string>()
                        {
                            "序号","物料编码","物料名称","规格型号","单位","实收数量","单价","金额","供应商"
                        };

                    float[] contentWithList = new float[]
                    {
                        50, 60, 60, 60, 60, 60, 60, 50,80
                    };

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
                    List<Pick> purchaseTables = context.Pick.Where(p => p.TaskId == printAndSendModel.TaskId).ToList();

                    DataTable dtpurchaseTables = ClassChangeHelper.ToDataTable(purchaseTables);

                    string path = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet/领料导出模板.xlsx");
                    string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\领料单" + time + ".xlsx";
                    File.Copy(path, newPath);
                    if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, 0, 1))
                    {
                        DingTalkServersController dingTalkServersController = new DingTalkServersController();
                        //上盯盘
                        var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/领料单" + time + ".xlsx");
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

        /// <summary>
        /// 领料管理关键字查询
        /// </summary>
        /// <param name="applyManId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="IsSend">是否推送Excel</param> 
        /// <param name="projectId"></param>
        /// <param name="key">申请人、申请部门、物料名称</param>
        /// <returns></returns>
        [Route("Query")]
        [HttpGet]
        public async Task<object> Query(string applyManId, DateTime startTime, DateTime endTime, bool IsSend = false, string projectId = null, string key = null)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Pick> picks = context.Pick.ToList();
                    List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskIdByFlowName("领料申请");
                    List<Tasks> tasksNew = tasks.Where(t => t.NodeId.ToString() == "0").ToList();
                    tasksNew = tasksNew.Where(t =>
                    (projectId == null ? 1 == 1 : t.ProjectId == projectId) &&
                    (DateTime.Parse(t.ApplyTime) > startTime && DateTime.Parse(t.ApplyTime) < endTime)).ToList(); //过滤审批后的流程
                    List<Roles> roles = context.Roles.Where(r => r.RoleName == "领料管理人员" && r.UserId == applyManId).ToList();

                    if (roles.Count > 0 ? true : false)  //领料管理员
                    {
                        if (IsSend)
                        {
                            var Query = from t in tasksNew
                                        join p in picks on
                                        t.TaskId.ToString() equals p.TaskId
                                        where
                                        key != null ?
                                        (t.ApplyMan.Contains(key) || t.Dept.Contains(key) || p.fName.Contains(key)) : 1 == 1
                                        select new
                                        {
                                            t.ProjectName,
                                            t.ApplyMan,
                                            t.ApplyTime,
                                            t.TaskId,
                                            p.fName,
                                            p.fNumber,
                                            p.fModel,
                                            p.fFullName,
                                            p.fQty,
                                            p.fPrice,
                                            p.fAmount,
                                            p.unitName,
                                            t.Remark
                                        };
                            DataTable dtReturn = new DataTable();
                            PropertyInfo[] oProps = null;
                            foreach (var rec in Query)
                            {
                                if (oProps == null)
                                {
                                    oProps = ((Type)rec.GetType()).GetProperties();
                                    foreach (PropertyInfo pi in oProps)
                                    {
                                        Type colType = pi.PropertyType; if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                                        {
                                            colType = colType.GetGenericArguments()[0];
                                        }
                                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                                    }
                                }
                                DataRow dr = dtReturn.NewRow(); foreach (PropertyInfo pi in oProps)
                                {
                                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                                }
                                dtReturn.Rows.Add(dr);
                            }
                            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet/领料数据统计模板.xlsx");
                            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string newPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet") + "\\领料数据统计" + time + ".xlsx";
                            System.IO.File.Copy(path, newPath);
                            if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtReturn, 0, 1))
                            {
                                DingTalkServersController dingTalkServersController = new DingTalkServersController();
                                //上盯盘
                                var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/领料数据统计" + time + ".xlsx");
                                //推送用户
                                FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia.ToString());
                                fileSendModel.UserId = applyManId;
                                var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                                return new NewErrorModel()
                                {
                                    error = new Error(0, "已推送至钉钉", "") { },
                                };
                            }
                        }
                        else
                        {
                            var Query = from t in tasksNew
                                        join p in picks on
                                        t.TaskId.ToString() equals p.TaskId
                                        where
                                        key != null ?
                                        (t.ApplyMan.Contains(key) || t.Dept.Contains(key) || p.fName.Contains(key)) : 1 == 1
                                        select new
                                        {
                                            t.ProjectName,
                                            t.ApplyMan,
                                            t.ApplyTime,
                                            t.TaskId,
                                            p.fName,
                                            p.fNumber,
                                            p.fModel,
                                            p.fFullName,
                                            p.fQty,
                                            p.fPrice,
                                            p.fAmount,
                                            p.unitName,
                                            t.Remark
                                        };
                            return new NewErrorModel()
                            {
                                data = Query,
                                error = new Error(0, "查询成功", "") { },
                            };
                        }
                    }
                    else
                    {
                        List<ProjectInfo> projectInfos = context.ProjectInfo.
                            Where(p => p.ResponsibleManId == applyManId).ToList();
                        if (projectInfos.Count > 0)
                        {
                            if (IsSend)
                            {
                                var Query = from pi in projectInfos
                                            join t in tasksNew on pi.ProjectId equals t.ProjectId
                                            join p in picks on t.TaskId.ToString()
                                            equals p.TaskId
                                            where
                                       key != null ?
                                       (t.ApplyMan.Contains(key) || t.Dept.Contains(key) || p.fName.Contains(key)) : 1 == 1
                                            select new
                                            {
                                                t.ProjectName,
                                                t.ApplyMan,
                                                t.ApplyTime,
                                                t.TaskId,
                                                p.fName,
                                                p.fNumber,
                                                p.fModel,
                                                p.fFullName,
                                                p.fQty,
                                                p.fPrice,
                                                p.fAmount,
                                                p.unitName,
                                                t.Remark
                                            };
                                DataTable dtReturn = new DataTable();
                                PropertyInfo[] oProps = null;
                                foreach (var rec in Query)
                                {
                                    if (oProps == null)
                                    {
                                        oProps = ((Type)rec.GetType()).GetProperties();
                                        foreach (PropertyInfo pi in oProps)
                                        {
                                            Type colType = pi.PropertyType; if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                                            {
                                                colType = colType.GetGenericArguments()[0];
                                            }
                                            dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                                        }
                                    }
                                    DataRow dr = dtReturn.NewRow(); foreach (PropertyInfo pi in oProps)
                                    {
                                        dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                                    }
                                    dtReturn.Rows.Add(dr);
                                }
                                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet/领料数据统计模板.xlsx");
                                string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                                string newPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet") + "\\领料数据统计" + time + ".xlsx";
                                System.IO.File.Copy(path, newPath);
                                if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtReturn, 0, 1))
                                {
                                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                                    //上盯盘
                                    var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/领料数据统计" + time + ".xlsx");
                                    //推送用户
                                    FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia.ToString());
                                    fileSendModel.UserId = applyManId;
                                    var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                                    return new NewErrorModel()
                                    {
                                        error = new Error(0, "已推送至钉钉", "") { },
                                    };
                                }
                            }
                            else
                            {
                                var Query = from pi in projectInfos
                                            join t in tasksNew on pi.ProjectId equals t.ProjectId
                                            join p in picks on t.TaskId.ToString()
                                            equals p.TaskId
                                            where
                                       key != null ?
                                       (t.ApplyMan.Contains(key) || t.Dept.Contains(key) || p.fName.Contains(key)) : 1 == 1
                                            select new
                                            {
                                                t.ProjectName,
                                                t.ApplyMan,
                                                t.ApplyTime,
                                                t.TaskId,
                                                p.fName,
                                                p.fNumber,
                                                p.fModel,
                                                p.fFullName,
                                                p.fQty,
                                                p.fPrice,
                                                p.fAmount,
                                                p.unitName,
                                                t.Remark
                                            };
                                return new NewErrorModel()
                                {
                                    data = Query,
                                    error = new Error(0, "查询成功", "") { },
                                };
                            }
                        }
                        else
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "您无权访问该模块数据，请联系管理员！", "") { },
                            };
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// 同步Post
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Params"></param>
        /// <returns>返回json格式</returns>
        public static string HttpPost(string Url, IDictionary<string, string> Params)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "application/json";

            //发送POST数据  
            StringBuilder bufferParams = new StringBuilder();
            if (!(Params == null || Params.Count == 0))
            {
                int i = 0;
                foreach (string key in Params.Keys)
                {
                    if (i > 0)
                    {
                        bufferParams.AppendFormat("&{0}={1}", key, Params[key]);
                    }
                    else
                    {
                        bufferParams.AppendFormat("{0}={1}", key, Params[key]);
                        i++;
                    }
                }
            }
            request.ContentLength = Encoding.UTF8.GetByteCount(bufferParams.ToString());

            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(bufferParams);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            #region ContentType = "application/x-www-form-urlencoded"处理方式
            //服务器端返回的是一个XML格式的字符串，XML的Content才是我们所需要的数据
            //XmlTextReader Reader = new XmlTextReader(myResponseStream);
            //Reader.MoveToContent();
            //string retString = Reader.ReadInnerXml();//取出Content中的数据
            //Reader.Close();
            #endregion
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        ///// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            //request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }




    }

    public class GodownModel
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string fNumber { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string fName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string fModel { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string unitName { get; set; }

        /// <summary>
        /// 实收数量(可编辑)
        /// </summary>
        public Decimal fQty { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public Decimal fPrice { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public Decimal fAmount { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string fFullName { get; set; }
        
        /// <summary>
        /// 入库数量
        /// </summary>
        public Decimal fCommitQty { get; set; }

    }
}
