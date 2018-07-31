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
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("Delete")]
        [HttpPost]
        public object Delete([FromBody]int Id)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
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
        /// <param name="dateTime">最后使用时间</param>
        /// <returns></returns>
        [Route("QuaryByTime")]
        [HttpGet]
        public object QuaryByTime(DateTime dateTime)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    var ListCar = context.Car.ToList();
                    return null;
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
