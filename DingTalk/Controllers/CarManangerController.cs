using Common.DTChange;
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
                throw ex;
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
                    if (context.Roles.Where(r => r.RoleName.Contains("车辆管理员") && r.UserId == ApplyManId).ToList().Count > 0 || context.Roles.Where(r => r.RoleName.Contains("超级管理员") && r.UserId == ApplyManId).ToList().Count > 0)
                    {
                        //判断车辆是否用完
                        string strSql = $"select count(*) from cartable a  left join TasksState b on a.TaskId=b.TaskId where carid={Id} and b.State='未完成'";
                        int count = context.Database.SqlQuery<int>(strSql).FirstOrDefault();
                        if (count > 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "该车辆有部分流程未走完，无法删除！", "") { },
                            };
                        }
                        Car car = context.Car.Find(Convert.ToInt32(Id));
                        context.Car.Remove(car);
                        context.SaveChanges();
                        return new NewErrorModel()
                        {
                            error = new Error(0, "删除成功！", "") { },
                        };
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "用户没有权限进行操作！", "") { },
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
                    if (context.Roles.Where(r => r.RoleName.Contains("车辆管理员") && r.UserId == car.ApplyManId).ToList().Count > 0 || context.Roles.Where(r => r.RoleName.Contains("超级管理员") && r.UserId == car.ApplyManId).ToList().Count > 0)
                    {
                        context.Entry<Car>(car).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        return new NewErrorModel()
                        {
                            error = new Error(0, "修改成功！", "") { },
                        };
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "用户没有权限进行操作！", "") { },
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
        /// 车辆查询
        /// </summary>
        /// <param name="key">车牌号、车名、颜色、</param>
        /// <param name="applyManId">用户Id</param>
        /// <returns></returns>
        [Route("Quary")]
        [HttpGet]
        public NewErrorModel Quary(string key, string applyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (string.IsNullOrEmpty(applyManId))
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "参数applyManId未传！", "") { },
                        };
                    }
                    else
                    {
                        //超管和车辆管理员可查询
                        if (context.Roles.Where(r => r.RoleName.Contains("车辆管理员") && r.UserId == applyManId).ToList().Count == 0 && context.Roles.Where(r => r.RoleName.Contains("超级管理员") && r.UserId == applyManId).ToList().Count == 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "用户没有权限进行操作！", "") { },
                            };
                        }
                    }
                    if (string.IsNullOrEmpty(key))
                    {
                        var Quary = context.Car.ToList();
                        return new NewErrorModel()
                        {
                            data = Quary.OrderByDescending(t => t.Id),
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
                            data = Quary.OrderByDescending(t => t.Id),
                            error = new Error(0, "读取成功！", "") { },
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
                    List<TasksState> tasksStates = context.TasksState.ToList();
                    List<CarTable> carTablesPro = new List<CarTable>();
                    foreach (var item in carTables)
                    {
                        TasksState tasksState = tasksStates.Where(t => t.TaskId == item.TaskId).FirstOrDefault();
                        if (tasksState != null && tasksState.State == "已完成")
                        {
                            carTablesPro.Add(item);
                        }
                    }
                    foreach (var item in cars)
                    {
                        foreach (var ct in carTablesPro)
                        {
                            if (ct.CarId.ToString() == item.Id.ToString())
                            {
                                if (!((DateTime.Parse(endTime) < ct.StartTime) || (DateTime.Parse(startTime) > ct.EndTime)))
                                {
                                    item.IsOccupyCar = true;
                                    //占用
                                    if (item.carTables != null)
                                    {
                                        item.carTables.Add(ct);
                                    }
                                    else
                                    {
                                        item.carTables = new List<CarTable>() {
                                            ct
                                        };
                                    }
                                    //旧格式
                                    if (string.IsNullOrEmpty(item.TaskId))
                                    {
                                        item.TaskId = ct.TaskId;
                                    }
                                    else
                                    {
                                        item.TaskId = item.TaskId + "," + ct.TaskId;
                                    }

                                    if (string.IsNullOrEmpty(item.UseMan))
                                    {
                                        item.UseMan = ct.DrivingMan;
                                    }
                                    else
                                    {
                                        item.UseMan = item.UseMan + "," + ct.DrivingMan;
                                    }
                                    if (string.IsNullOrEmpty(item.UseTimes))
                                    {
                                        item.UseTimes = ct.StartTime + "---" + ct.EndTime;
                                    }
                                    else
                                    {
                                        item.UseTimes = item.UseTimes + "," + ct.StartTime + "---" + ct.EndTime;
                                    }
                                }

                            }
                        }
                    }

                    return new NewErrorModel()
                    {
                        data = cars,
                        error = new Error(0, "查询成功！", "") { },
                    };
                    //List<Car> cars = context.Car.ToList();
                    //List<CarTable> carTables = context.CarTable.Where(c => c.IsPublicCar == true && !string.IsNullOrEmpty(c.CarId)).ToList();
                    //List<Tasks> tasks = FlowInfoServer.ReturnUnFinishedTaskId("13"); //过滤审批后的流程
                    //List<Car> carsQuery = new List<Car>();//条件过滤集合
                    //List<Car> carsDic = new List<Car>();
                    //foreach (var ct in carTables)
                    //{
                    //    if (tasks.Where(t => t.TaskId.ToString() == ct.TaskId).ToList().Count > 0)
                    //    {
                    //        if (!(DateTime.Parse(endTime) < ct.StartTime || ct.EndTime < DateTime.Parse(startTime)))
                    //        {
                    //            Car car = cars.Find(c => c.Id.ToString() == ct.CarId);
                    //            if (car != null)
                    //            {
                    //                car.IsOccupyCar = true;
                    //                car.OccupyCarId = ct.Id.ToString();
                    //                car.UseMan = ct.DrivingMan;
                    //                car.UseTimes = ct.StartTime + "---" + ct.EndTime;
                    //                carsQuery.Add(car);
                    //            }
                    //        }
                    //    }
                    //}

                    //foreach (var c in cars)
                    //{
                    //    if (carsQuery.Where(cq => cq.Id == c.Id).ToList().Count == 0)
                    //    {
                    //        c.OccupyCarId = "";
                    //        c.IsOccupyCar = false;
                    //        c.UseTimes = "";
                    //        //bool IsTrue = true;
                    //        //foreach (var item in carsQuery)
                    //        //{
                    //        //    if (item.Id == c.Id)
                    //        //    {
                    //        //        IsTrue = false;
                    //        //    }
                    //        //}
                    //        //if (IsTrue)
                    //        //{
                    //        //    carsQuery.Add(c);
                    //        //}
                    //    }
                    //}
                    //return new NewErrorModel()
                    //{
                    //    data = carsQuery.Distinct(),
                    //    error = new Error(0, "查询成功！", "") { },
                    //};
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                                // Use reflection to get property names, to create Table, Only first time, others will follow
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
                                // Use reflection to get property names, to create Table, Only first time, others will follow
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
                throw ex;
            }
        }
    }
}
