using DingTalk.EF;
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
    /// 车量管理
    /// </summary>
    [RoutePrefix("CarMananger")]
    public class CarManangerController : ApiController
    {
        /// <summary>
        /// 车辆添加
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        [Route("Add")]
        [HttpPost]
        public object Add(Car car)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    car.FinnalStartTime = DateTime.Now;
                    car.FinnalEndTime = DateTime.Now;
                    context.Car.Add(car);
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
        /// 车辆删除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// 测试数据: /CarMananger/Delete/
        /// data: JSON.stringify({ Id: "7" }),
        [Route("Delete")]
        [HttpPost]
        public object Delete(dynamic obj)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    var Id = Convert.ToInt32(obj.Id);
                    Car car = context.Car.Find(Id);
                    context.Car.Remove(car);
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "删除成功"
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
        /// 车辆修改
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object Modify(Car car)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<Car>(car).State = System.Data.Entity.EntityState.Modified;
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
        /// 车辆查询
        /// </summary>
        /// <param name="key">查询关键字</param>
        /// <returns></returns>
        [Route("Quary")]
        [HttpGet]
        public object Quary(string key)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        var Quary = context.Car.ToList();
                        return Quary;
                    }
                    else
                    {
                        var Quary = context.Car.Where(c => c.Name.Contains(key) ||
                          c.CarNumber.Contains(key) || c.Color.Contains(key)
                          || c.Type.Contains(key)).ToList();
                        return Quary;
                    }
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
        /// 车辆查询(返回当前车辆状态)
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>IsOccupyCar 是否被占用 true 被占用 false 未被占用</returns>
        /// 测试数据：  /CarMananger/QuaryByTime?dateTime=2018-08-07 00:00:00&endTime=2018-08-27 00:00:00
        [Route("QuaryByTime")]
        [HttpGet]
        public object QuaryByTime(string startTime, string endTime)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Car> cars = context.Car.ToList();
                    foreach (Car car in cars)
                    {
                        if (!string.IsNullOrEmpty(car.UseTimes))
                        {
                            string[] UseTimesList = car.UseTimes.Split(',');
                            if (UseTimesList.Length > 0)
                            {
                                int i = 0;
                                List<string> UseManResult = new List<string>();
                                List<string> UseTimeResult = new List<string>();
                                string UseManSave = car.UseMan;
                                string UseTimeSave = car.UseTimes;
                                foreach (var UseTimes in UseTimesList)
                                {
                                    i++;
                                    if (UseTimes.Split('-').Length > 0)
                                    {
                                        string startT = UseTimes.Split('-')[0];
                                        string endT = UseTimes.Split('-')[1];
                                        //判断时间段是否出现重叠
                                        if (!(DateTime.Parse(startTime) > DateTime.Parse(endT) ||
                                           DateTime.Parse(endTime) < DateTime.Parse(startT)))
                                        {
                                            car.IsOccupyCar = true;
                                            UseManResult.Add(UseManSave.Split(',')[i - 1]);
                                            UseTimeResult.Add(UseTimeSave.Split(',')[i - 1]);
                                        }
                                    }
                                }
                                car.UseTimes = string.Join(",", UseTimeResult);
                                car.UseMan = string.Join(",", UseManResult);
                            }
                        }
                    }
                    return cars;
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
        /// 查询数据并推送Excel
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="key">关键字(姓名、车辆信息、部门信息)</param>
        /// <param name="IsSend">是否推送用户(默认否)</param>
        /// <returns></returns>
        [Route("QuaryPrintExcel")]
        [HttpGet]
        public object QuaryPrintExcel(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string key = "", bool IsSend = false)
        {
            try
            {
                //EFHelper<CarTable> eFHelper = new EFHelper<CarTable>();
                //System.Linq.Expressions.Expression<Func<CarTable, bool>> expression = c => c.StartTime > startTime && c.EndTime < endTime;
                //List<CarTable> CarTablesListAll = eFHelper.GetListBy(expression);
                //var CarTableList = eFHelper.GetPagedList(pageIndex, pageSize, expression, c => c.Id);
                using (DDContext context = new DDContext())
                {
                    List<Car> cars = context.Car.ToList();
                    List<Tasks> tasks = context.Tasks.ToList();
                    List<CarTable> carTables = context.CarTable.ToList();

                    var Quary = from ct in carTables
                                join t in tasks on ct.TaskId equals t.TaskId.ToString()
                                join c in cars on ct.CarId equals c.Id.ToString()
                                where t.NodeId.ToString() == "0" && ct.StartTime > startTime && ct.EndTime < endTime
                                select new
                                {
                                    Dept = t.Dept,
                                    ApplyMan = t.ApplyMan,
                                    UseTime = ct.StartTime.ToString() + "---" + ct.EndTime.ToString(),
                                    Remark = t.Remark,
                                    UseKilometres = ct.UseKilometres,
                                    UnitPricePerKilometre = c.UnitPricePerKilometre,
                                    AllPrice = float.Parse(ct.UseKilometres) * c.UnitPricePerKilometre
                                };
                }

                return new NewErrorModel()
                {
                    //count = CarTablesListAll.Count,
                    //data = CarTableList,
                    error = new Error(0, "读取成功！", "") { },
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
    }
}
