using Common.ClassChange;
using Common.Ionic;
using Common.PDF;
using DingTalk.Bussiness.FlowInfo;
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
    /// 用车流程(新)
    /// </summary>
    [RoutePrefix("CarTableNew")]
    public class CarTableNewController : ApiController
    {
        /// <summary>
        /// 用车表单保存接口
        /// </summary>
        /// <param name="carTable"></param>
        /// <returns></returns>
        [Route("TableSave")]
        [HttpPost]
        public object TableSave(CarTable carTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.CarTable.Add(carTable);
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "添加成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 用车表单读取接口
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        [Route("TableQuary")]
        [HttpGet]
        public NewErrorModel TableQuary(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    var Quary = context.CarTable.Where(c => c.TaskId == TaskId).ToList();

                    return new NewErrorModel()
                    {
                        data = Quary,
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
        /// 用车表单修改接口
        /// </summary>
        /// <param name="carTable"></param>
        /// <returns></returns>
        [Route("TableModify")]
        [HttpPost]
        public NewErrorModel TableModify(CarTable carTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    carTable.FactKilometre = carTable.UseKilometres;
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == carTable.TaskId && t.State == 0 && t.IsEnable == 1).FirstOrDefault();
                    if (tasks.NodeId == 4)
                    {
                        if (!string.IsNullOrEmpty(carTable.OccupyCarId) && !string.IsNullOrEmpty(carTable.FactKilometre))
                        {
                            CarTable carTableOld = context.CarTable.Find(int.Parse(carTable.OccupyCarId));
                            carTableOld.FactKilometre = (float.Parse(carTableOld.FactKilometre) - float.Parse(carTable.FactKilometre)).ToString();
                            context.Entry<CarTable>(carTableOld).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    context.Entry<CarTable>(carTable).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 获取时间段内的车辆统计
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        [Route("GetReport")]
        [HttpGet]
        public NewErrorModel GetMonthReport(string StartTime, string EndTime, string UserId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    //查询时间段内的数据
                    List<CarTable> carTables = context.CarTable.Where(c => c.StartTime > DateTime.Parse(StartTime)
                      && c.EndTime < DateTime.Parse(EndTime)).ToList();
                    //更新实际行驶路程
                    foreach (CarTable carTable in carTables)
                    {
                        if (!string.IsNullOrEmpty(carTable.OccupyCarId))
                        {
                            CarTable catTb = context.CarTable.Find(carTable.OccupyCarId);
                            catTb.UseKilometres = (Int32.Parse(catTb.UseKilometres) - Int32.Parse(carTable.UseKilometres)).ToString();
                            context.Entry<CarTable>(catTb).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    //计算公里数
                    List<CarTable> carTableList = context.CarTable.ToList();
                    List<Tasks> tasksList = FlowInfoServer.ReturnUnFinishedTaskId("13"); //公车任务流
                    List<Tasks> taskList = new List<Tasks>();
                    List<Car> carList = context.Car.ToList();
                    foreach (Tasks tasks in tasksList)
                    {
                        if (tasks.NodeId == 0)
                        {
                            taskList.Add(tasks);
                        }
                    }
                    var Quary = from t in taskList
                                join c in carTableList
                                on t.TaskId.ToString() equals c.TaskId
                                join cars in carList
                                on c.CarId equals cars.Id.ToString()
                                select new
                                {
                                    t.Dept,
                                    t.ApplyMan,
                                    c.StartTime,
                                    c.EndTime,
                                    cars.Name,
                                    c.MainContent,
                                    c.UseKilometres,
                                    cars.UnitPricePerKilometre,
                                    Price = Int32.Parse(c.UseKilometres) * cars.UnitPricePerKilometre
                                };
                    return new NewErrorModel()
                    {
                        data = Quary,
                        error = new Error(0, "获取成功！", "") { },
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
        /// <param name="printAndSendModel"></param>
        /// <returns></returns>
        [Route("GetPrintPDF")]
        [HttpPost]
        public async Task<NewErrorModel> GetPrintPDF([FromBody]PrintAndSendModel printAndSendModel)
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

                    TasksState tasksState = context.TasksState.Where(t => t.TaskId == TaskId).FirstOrDefault();
                    if (tasksState.State != "已完成")
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, string.Format("流程{0}！", tasksState.State), "") { },
                        };
                    }
                    CarTable ct = context.CarTable.Where(u => u.TaskId == TaskId).FirstOrDefault();
                    if (printAndSendModel.IsPublic)
                    {
                        ct.CarId = context.Car.Where(c => c.Id.ToString() == ct.CarId).FirstOrDefault().CarNumber;
                    }
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                    if (printAndSendModel.IsPublic)
                    {
                        keyValuePairs.Add("驾驶人", ct.DrivingMan);
                        keyValuePairs.Add("车牌号", ct.CarId);
                        keyValuePairs.Add("同行人数", ct.PeerNumber);
                        keyValuePairs.Add("用车事由", ct.MainContent);
                        keyValuePairs.Add("计划行车路线", ct.PlantTravelWay);
                        keyValuePairs.Add("实际行车路线", ct.FactTravelWay);
                        keyValuePairs.Add("出发时间", ct.StartTime.ToString());
                        keyValuePairs.Add("归来时间", ct.EndTime.ToString());
                        keyValuePairs.Add("实际行驶公里数", ct.FactKilometre);
                        keyValuePairs.Add("总行驶公里数", ct.UseKilometres);
                    }
                    else
                    {
                        keyValuePairs.Add("驾驶人", ct.DrivingMan);
                        keyValuePairs.Add("同行人数", ct.PeerNumber);
                        keyValuePairs.Add("用车事由", ct.MainContent);
                        keyValuePairs.Add("计划行车路线", ct.PlantTravelWay);
                        keyValuePairs.Add("实际行车路线", ct.FactTravelWay);
                        keyValuePairs.Add("出发时间", ct.StartTime.ToString());
                        keyValuePairs.Add("归来时间", ct.EndTime.ToString());
                        keyValuePairs.Add("总行驶公里数", ct.UseKilometres);
                    }

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


                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    null, null, "2", 300, 650, null, null, null, dtApproveView, keyValuePairs);
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
                        data = result,
                        error = new Error(0, "打印盖章成功！", "") { },
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
