﻿using Common.DTChange;
using Common.Excel;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalk.Models.ServerModels;
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
        /// <param name="Id">Id</param>
        /// <param name="ApplyManId">调用接口人员Id</param>
        /// <returns></returns>
        /// 测试数据: /CarMananger/Delete/
        /// data: JSON.stringify({ Id: "7" }),
        [Route("Delete")]
        [HttpGet]
        public object Delete(int Id, string ApplyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.RoleName.Contains("车辆管理员") && r.UserId == ApplyManId).ToList().Count > 0)
                    {
                        Car car = context.Car.Find(Convert.ToInt32(Id));
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
        /// <param name="key">车牌号、车名、颜色、</param>
        /// <returns></returns>
        [Route("Quary")]
        [HttpGet]
        public NewErrorModel Quary(string key)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        var Quary = context.Car.ToList();
                        return new NewErrorModel()
                        {
                            data = Quary,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                    else
                    {
                        var Quary = context.Car.Where(c => c.Name.Contains(key) ||
                          c.CarNumber.Contains(key) || c.Color.Contains(key)
                          || c.Type.Contains(key)).ToList();
                        return new NewErrorModel()
                        {
                            data = Quary,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
            }
        }

        /// <summary>
        /// 车辆查询(返回当前车辆状态)
        /// </summary>
        /// <param name="startTime">开始时间(2018-08-07 00:00:00)</param>
        /// <param name="endTime">结束时间(2018-08-27 00:00:00)</param>
        //[Route("QuaryByTime")]
        //[HttpGet]
        //public NewErrorModel QuaryByTime(string startTime, string endTime)
        //{
        //    try
        //    {
        //        using (DDContext context = new DDContext())
        //        {
        //            List<Car> cars = context.Car.ToList();
        //            List<CarTable> carTables = context.CarTable.Where(c => c.IsPublicCar == true && !string.IsNullOrEmpty(c.CarId)).ToList();
        //            List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskId("13"); //过滤审批后的流程
        //            List<Car> carsQuery = new List<Car>();//条件过滤集合
        //            List<Car> carsDic = new List<Car>();
        //            foreach (var ct in carTables)
        //            {
        //                if (tasks.Where(t => t.TaskId.ToString() == ct.TaskId).ToList().Count > 0)
        //                {
        //                    if (!(DateTime.Parse(endTime) < ct.StartTime || ct.EndTime < DateTime.Parse(startTime)))
        //                    {
        //                        Car car = cars.Find(c => c.Id.ToString() == ct.CarId);
        //                        if (car != null)
        //                        {
        //                            car.IsOccupyCar = true;
        //                            car.OccupyCarId = ct.Id.ToString();
        //                            car.UseMan = ct.DrivingMan;
        //                            car.UseTimes = ct.StartTime + "---" + ct.EndTime;
        //                            carsQuery.Add(car);
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (var c in cars)
        //            {
        //                if (carsQuery.Where(cq => cq.Id == c.Id).ToList().Count == 0)
        //                {
        //                    c.OccupyCarId = "";
        //                    c.IsOccupyCar = false;
        //                    c.UseTimes = "";
        //                    carsQuery.Add(c);
        //                }
        //            }

        //            Dictionary<string, List<Car>> keyValuePairs = new Dictionary<string, List<Car>>();

        //            foreach (var item in carsQuery)
        //            {
        //                if (!keyValuePairs.Keys.Contains(item.Name))
        //                {
        //                    keyValuePairs.Add(item.Name, new List<Car>() {
        //                     item
        //                  });
        //                }
        //                else
        //                {
        //                    List<Car> carsNew = new List<Car>();
        //                    carsNew = keyValuePairs[item.Name];
        //                    carsNew.Add(item);
        //                    keyValuePairs[item.Name] = carsNew;
        //                }
        //            }
        //            //2019 09 29 end
        //            return new NewErrorModel()
        //            {
        //                data = keyValuePairs,
        //                error = new Error(0, "读取成功！", "") { },
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new NewErrorModel()
        //        {
        //            error = new Error(1, ex.Message, "") { },
        //        };
        //    }
        //}


        /// <summary>
        /// 车辆查询(返回当前车辆状态)
        /// </summary>
        /// <param name="startTime">开始时间(2018-08-07 00:00:00)</param>
        /// <param name="endTime">结束时间(2018-08-27 00:00:00)</param>
        [Route("QuaryByTimeNew")]
        [HttpGet]
        public NewErrorModel QuaryByTimeNew(string startTime, string endTime)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<Car> cars = context.Car.ToList();
                    List<CarTable> carTables = context.CarTable.Where(c => c.IsPublicCar == true && !string.IsNullOrEmpty(c.CarId)).ToList();
                    List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskId("13"); //过滤审批后的流程
                    List<Car> carsQuery = new List<Car>();//条件过滤集合
                    List<Car> carsDic = new List<Car>();
                    foreach (var ct in carTables)
                    {
                        if (tasks.Where(t => t.TaskId.ToString() == ct.TaskId).ToList().Count > 0)
                        {
                            if (!(DateTime.Parse(endTime) < ct.StartTime || ct.EndTime < DateTime.Parse(startTime)))
                            {
                                Car car = cars.Find(c => c.Id.ToString() == ct.CarId);
                                if (car != null)
                                {
                                    car.IsOccupyCar = true;
                                    car.OccupyCarId = ct.Id.ToString();
                                    car.UseMan = ct.DrivingMan;
                                    car.UseTimes = ct.StartTime + "---" + ct.EndTime;
                                    carsQuery.Add(car);
                                }
                            }
                        }
                    }

                    foreach (var c in cars)
                    {
                        if (carsQuery.Where(cq => cq.Id == c.Id).ToList().Count == 0)
                        {
                            c.OccupyCarId = "";
                            c.IsOccupyCar = false;
                            c.UseTimes = "";
                            carsQuery.Add(c);
                        }
                    }
                    return new NewErrorModel()
                    {
                        data = carsQuery,
                        error = new Error(0, "查询成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                return new NewErrorModel()
                {
                    error = new Error(2, ex.Message, "") { },
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
        /// <param name="key">关键字(姓名、车辆信息、部门信息、流水号、车牌号)</param>
        /// <param name="IsSend">是否推送用户(默认否)</param>
        /// <param name="IsPublic">是否是公车(默认是)</param>
        /// <returns></returns>
        [Route("QuaryPrintExcel")]
        [HttpGet]
        public async Task<object> QuaryPrintExcel(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string applyManId, string key = "", bool IsSend = false, bool IsPublic = true)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    FlowInfoServer flowInfoServer = new FlowInfoServer();
                    List<Car> cars = context.Car.ToList();
                    List<Tasks> tasksNew = FlowInfoServer.ReturnUnFinishedTaskId(IsPublic == true ? "13" : "14"); //公车任务流13
                    List<TasksState> tasksStates = context.TasksState.ToList();
                    List<Tasks> tasks = new List<Tasks>();

                    //foreach (var item in tasksNew)
                    //{
                    //    if (flowInfoServer.GetTasksState(item.TaskId.ToString()) == "已完成")
                    //    {
                    //        tasks.Add(item);
                    //    }
                    //}

                    foreach (var item in tasksNew)
                    {
                        if (tasksStates.Where(t => t.TaskId.ToString() == item.TaskId.ToString()).FirstOrDefault().State == "已完成")
                        {
                            tasks.Add(item);
                        }
                    }

                    List<CarTable> carTables = context.CarTable.ToList();
                    if (IsPublic)
                    {
                        var Quary = from ct in carTables
                                    join t in tasks on ct.TaskId equals t.TaskId.ToString()
                                    join c in cars on ct.CarId equals c.Id.ToString()
                                    where t.NodeId.ToString() == "0" && ct.StartTime > startTime && ct.EndTime < endTime && ct.IsPublicCar == IsPublic
                                    && (!(string.IsNullOrEmpty(key)) ? (t.ApplyMan.Contains(key) || t.Dept.Contains(key) || t.TaskId.ToString() == key || c.Name.Contains(key)) : t.ApplyMan != null)
                                    select new
                                    {
                                        TaskId = t.TaskId,
                                        Dept = t.Dept,
                                        ApplyMan = t.ApplyMan,
                                        UseTime = ct.StartTime.ToString() + "---" + ct.EndTime.ToString(),
                                        Name = c.Name + "(" + c.CarNumber + ")",
                                        MainContent = ct.MainContent,
                                        UseKilometres = ct.UseKilometres,
                                        UnitPricePerKilometre = c.UnitPricePerKilometre,
                                        FactKilometre = ct.FactKilometre,
                                        AllPrice = float.Parse(ct.FactKilometre) * float.Parse(c.UnitPricePerKilometre.ToString()),
                                        //Remark = t.Remark
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

                            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet/用车通用模板(公车).xlsx");
                            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string newPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet") + "\\用车数据(公车)" + time + ".xlsx";
                            System.IO.File.Copy(path, newPath);
                            if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtReturn, 0, 2))
                            {
                                DingTalkServersController dingTalkServersController = new DingTalkServersController();
                                //上盯盘
                                var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/用车数据(公车)" + time + ".xlsx");
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
                        else
                        {
                            return new NewErrorModel()
                            {
                                count = Quary.Count(),
                                data = takeQuary,
                                error = new Error(0, "读取成功！", "") { },
                            };
                        }
                    }

                    else
                    {
                        var QuaryPri = from ct in carTables
                                       join t in tasks on ct.TaskId equals t.TaskId.ToString()
                                       where t.NodeId.ToString() == "0" && ct.StartTime > startTime && ct.EndTime < endTime && ct.IsPublicCar == IsPublic
                                       && (!(string.IsNullOrEmpty(key)) ? (t.ApplyMan.Contains(key) || t.Dept.Contains(key)) : t.ApplyMan != null)
                                       select new
                                       {
                                           TaskId = t.TaskId,
                                           Dept = t.Dept,
                                           ApplyMan = t.ApplyMan,
                                           UseTime = ct.StartTime.ToString() + "---" + ct.EndTime.ToString(),
                                           MainContent = ct.MainContent,
                                           UseKilometres = ct.UseKilometres,
                                           StartKilometres = ct.StartKilometres == null ? "" : ct.StartKilometres,
                                           EndKilometres = ct.EndKilometres == null ? "" : ct.EndKilometres,                      
                                       };
                        var takeQuaryPri = QuaryPri.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                        if (IsSend && QuaryPri.Count() > 0)  //生成报表推送用户
                        {
                            //DataTable dtpurchaseTables = DtLinqOperators.CopyToDataTable(Quary);

                            DataTable dtReturn = new DataTable();
                            // column names
                            PropertyInfo[] oProps = null;
                            // Could add a check to verify that there is an element 0
                            foreach (var rec in QuaryPri)
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
                            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet/用车通用模板(私车).xlsx");
                            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string newPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet") + "\\用车数据(私车)" + time + ".xlsx";
                            System.IO.File.Copy(path, newPath);
                            if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtReturn, 0, 2))
                            {
                                DingTalkServersController dingTalkServersController = new DingTalkServersController();
                                //上盯盘
                                var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/用车数据(私车)" + time + ".xlsx");
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
                        else
                        {
                            return new NewErrorModel()
                            {
                                count = QuaryPri.Count(),
                                data = takeQuaryPri,
                                error = new Error(0, "读取成功！", "") { },
                            };
                        }
                    }
                }
                return "";
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
