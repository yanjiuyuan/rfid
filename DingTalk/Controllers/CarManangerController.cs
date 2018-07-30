using DingTalk.Models;
using DingTalk.Models.DingModels;
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
        /// <param name="car"></param>
        /// <returns></returns>
        [Route("Delete")]
        [HttpPost]
        public object Delete(Car car)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
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
        [HttpPost]
        public object Quary(string key)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    var Quary = context.Car.Where(c => c.Name.Contains(key) ||
                    c.CarNumber.Contains(key) || c.Color.Contains(key)
                    || c.Type.Contains(key)).ToList();
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
    }
}
