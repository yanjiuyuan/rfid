using DingTalk.Bussiness.FlowInfo;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 用车申请
    /// </summary>
    [RoutePrefix("CarTable")]
    public class CarTableController : ApiController
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
                    Car car = context.Car.Find(Int32.Parse(carTable.CarId));
                    //只保留五条最新数据
                    if (!string.IsNullOrEmpty(car.UseTimes))
                    {
                        if (car.UseTimes.Split(',').Length < 5)
                        {
                            car.UseTimes = car.UseTimes + "," + carTable.StartTime + "~" + carTable.EndTime;
                            car.UseMan = car.UseMan + "," + carTable.DrivingMan;
                        }
                        else
                        {
                            car.UseTimes = car.UseTimes.Substring(car.UseTimes.IndexOf(','), car.UseTimes.Length - car.UseTimes.IndexOf(','));
                            car.UseMan = car.UseMan.Substring(car.UseMan.IndexOf(','), car.UseMan.Length - car.UseMan.IndexOf(','));
                            car.UseTimes = car.UseTimes + "," + carTable.StartTime + "~" + carTable.EndTime;
                            car.UseMan = car.UseMan + "," + carTable.DrivingMan;
                        }
                    }
                    else
                    {
                        car.UseTimes = carTable.StartTime + "-" + carTable.EndTime;
                        car.UseMan = carTable.DrivingMan;
                    }
                    car.OccupyCarId = carTable.CarId;
                    car.FinnalStartTime = carTable.StartTime;
                    car.FinnalEndTime = carTable.EndTime;
                    context.Entry<Car>(car).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "添加成功"
                };
            }
            catch (Exception ex)
            {
                return new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                };
            }
        }


        /// <summary>
        /// 用车表单读取接口
        /// </summary>
        /// <param name="TaskId">流水号</param>
        /// <returns></returns>
        [Route("TableQuary")]
        [HttpGet]
        public object TableQuary(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    var Quary = context.CarTable.Where(c => c.TaskId == TaskId).ToList();
                    return Quary;
                }
            }
            catch (Exception ex)
            {
                return new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 用车表单修改接口
        /// </summary>
        /// <param name="carTable"></param>
        /// <returns></returns>
        [Route("TableModify")]
        [HttpPost]
        public object TableModify(CarTable carTable)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<CarTable>(carTable).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "修改成功"
                };
            }
            catch (Exception ex)
            {
                return new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取时间段内的车辆统计报表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        [Route("GetReport")]
        [HttpGet]
        public object GetMonthReport(string StartTime, string EndTime, string UserId)
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
                    return JsonConvert.SerializeObject(Quary);
                }
            }
            catch (Exception ex)
            {
                return new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                };
            }
        }
    }
}
