using Common.DTChange;
using Common.Excel;
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
using System.Threading.Tasks;
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
                    if (context.Roles.Where(r => r.RoleName.Contains("车辆管理员") && r.UserId == car.ApplyManId).ToList().Count > 0)
                    {
                        car.FinnalStartTime = DateTime.Now;
                        car.FinnalEndTime = DateTime.Now;
                        context.Car.Add(car);
                        context.SaveChanges();
                        return new ErrorModel()
                        {
                            errorCode = 0,
                            errorMessage = "添加成功"
                        };
                    }
                    else
                    {
                        return new ErrorModel()
                        {
                            errorCode = 1,
                            errorMessage = "没有权限"
                        };
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
        /// 车辆删除
        /// </summary>
        /// <param name="obj">Id</param>
        /// <param name="ApplyManId">调用接口人员Id</param>
        /// <returns></returns>
        /// 测试数据: /CarMananger/Delete/
        /// data: JSON.stringify({ Id: "7" }),
        [Route("Delete")]
        [HttpGet]
        public object Delete(dynamic obj, string ApplyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.RoleName.Contains("车辆管理员") && r.UserId == ApplyManId).ToList().Count > 0)
                    {
                        var Id = Convert.ToInt32(obj.Id);
                        Car car = context.Car.Find(Id);
                        context.Car.Remove(car);
                        context.SaveChanges();
                        return new ErrorModel()
                        {
                            errorCode = 0,
                            errorMessage = "删除成功"
                        };
                    }
                    else
                    {
                        return new ErrorModel()
                        {
                            errorCode = 1,
                            errorMessage = "没有权限"
                        };
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
                    if (context.Roles.Where(r => r.RoleName.Contains("车辆管理员") && r.UserId == car.ApplyManId).ToList().Count > 0)
                    {
                        context.Entry<Car>(car).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        return new ErrorModel()
                        {
                            errorCode = 0,
                            errorMessage = "修改成功"
                        };
                    }
                    else
                    {
                        return new ErrorModel()
                        {
                            errorCode = 1,
                            errorMessage = "没有权限"
                        };
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
                                    if (UseTimes.Split('~').Length > 0)
                                    {
                                        string startT = UseTimes.Split('~')[0];
                                        string endT = UseTimes.Split('~')[1];
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
        /// <param name="applyManId">调用接口人Id</param>
        /// <param name="key">关键字(姓名、车辆信息、部门信息)</param>
        /// <param name="IsSend">是否推送用户(默认否)</param>
        /// <returns></returns>
        [Route("QuaryPrintExcel")]
        [HttpGet]
        public async Task<object> QuaryPrintExcel(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string applyManId, string key = "", bool IsSend = false)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Car> cars = context.Car.ToList();
                    List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskId("13"); //公车任务流
                    List<CarTable> carTables = context.CarTable.ToList();

                    var Quary = from ct in carTables
                                join t in tasks on ct.TaskId equals t.TaskId.ToString()
                                join c in cars on ct.CarId equals c.Id.ToString()
                                where t.NodeId.ToString() == "0" && ct.StartTime > startTime && ct.EndTime < endTime
                                && (key != "" ? (t.ApplyMan.Contains(key) || t.Dept.Contains(key) || c.Name.Contains(key)) : t.ApplyMan != null)
                                select new
                                {
                                    Dept = t.Dept,
                                    ApplyMan = t.ApplyMan,
                                    UseTime = ct.StartTime.ToString() + "---" + ct.EndTime.ToString(),
                                    Name = c.Name + "(" + c.CarNumber + ")",
                                    MainContent = ct.MainContent,
                                    UseKilometres = ct.UseKilometres,
                                    UnitPricePerKilometre = c.UnitPricePerKilometre,
                                    AllPrice = float.Parse(ct.UseKilometres) * c.UnitPricePerKilometre,
                                    Remark = t.Remark
                                };
                    var takeQuary = Quary.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                    if (IsSend && Quary.Count() > 0)  //生成报表推送用户
                    {
                        //DataTable dtpurchaseTables = DtLinqOperators.CopyToDataTable(Quary);

                        DataTable dtReturn = new DataTable();
                        // column names
                        PropertyInfo[] oProps = null;
                        // Could add a check to verify that there is an element 0
                        foreach (var rec in Quary)
                        {
                            // Use reflection to get property names, to create table, Only first time, others will follow
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

                        string path = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet/用车通用模板.xlsx");
                        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string newPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet") + "\\用车数据" + time + ".xlsx";
                        System.IO.File.Copy(path, newPath);
                        if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtReturn, 0, 2))
                        {
                            DingTalkServersController dingTalkServersController = new DingTalkServersController();
                            //上盯盘
                            var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/用车数据" + time + ".xlsx");
                            //推送用户
                            FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                            fileSendModel.UserId = applyManId;
                            var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                            return new NewErrorModel()
                            {
                                error = new Error(0, "已推送至钉钉！", "") { },
                            };
                        }
                    }

                    return new NewErrorModel()
                    {
                        count = Quary.Count(),
                        data = takeQuary,
                        error = new Error(0, "读取成功！", "") { },
                    };
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
