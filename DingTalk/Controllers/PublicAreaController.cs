using Common.Excel;
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
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 公共区域消毒
    /// </summary>
    [RoutePrefix("PublicArea")]
    public class PublicAreaController : ApiController
    {
        /// <summary>
        /// 公共区域消毒保存
        /// </summary>
        /// <param name="publicArea"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save(PublicArea publicArea)
        {
            try
            {
                if (publicArea != null)
                {
                    using (DDContext context = new DDContext())
                    {
                        List<PublicArea> publicAreas = context.PublicArea.Where(p => p.Power == publicArea.Power).ToList();

                        foreach (var item in publicAreas)
                        {
                            if (item.Date.ToString("yyyy-HH-mm") == publicArea.Date.ToString("yyyy-HH-mm"))
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, "当天已有监督人进行过操作！", "") { },
                                };
                            }
                        }

                        context.PublicArea.Add(publicArea);
                        context.SaveChanges();
                    }


                    return new NewErrorModel()
                    {
                        error = new Error(0, "保存成功！", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "格式有误！", "") { },
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 表单读取
        /// </summary>
        /// <returns></returns>

        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read()
        {
            try
            {
                List<PublicArea> publicAreas = new List<PublicArea>();

                using (DDContext context = new DDContext())
                {
                    publicAreas = context.PublicArea.ToList();
                }
                //数据格式转换
                List<PublicAreaModel> publicAreaModels = new List<PublicAreaModel>();
                List<string> stringDate = new List<string>();
                foreach (var item in publicAreas)
                {
                    if (!stringDate.Contains(item.Date.ToString()))
                    {
                        stringDate.Add(item.Date.ToString());
                    }
                }
                foreach (var item in stringDate)
                {
                    publicAreaModels.Add(new PublicAreaModel()
                    {
                        Date = item,
                        publicAreas = publicAreas.Where(p => p.Date.ToString() == item.ToString()).ToList()
                    });
                }


                return new NewErrorModel()
                {
                    data = publicAreaModels,
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 公共区域消毒查询
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="applyManId">用户Id</param>
        /// <param name="IsPrint">是否导出数据</param>
        /// <returns></returns>
        [Route("Query")]
        [HttpGet]
        public async Task<NewErrorModel> Query(DateTime startTime, DateTime endTime, string applyManId, bool IsPrint = false)
        {
            try
            {
                DDContext dDContext = new DDContext();
                List<PublicArea> publicAreas = new List<PublicArea>();
                publicAreas = dDContext.PublicArea.Where(p => p.Date >= startTime && p.Date <= endTime).OrderBy(p => p.Date).ThenBy(p => p.Power).ToList();

                //数据格式转换
                List<PublicAreaModel> publicAreaModels = new List<PublicAreaModel>();
                List<string> stringDate = new List<string>();
                foreach (var item in publicAreas)
                {
                    if (!stringDate.Contains(item.Date.ToString()))
                    {
                        stringDate.Add(item.Date.ToString());
                    }
                }
                foreach (var item in stringDate)
                {
                    publicAreaModels.Add(new PublicAreaModel()
                    {
                        Date = item,
                        publicAreas = publicAreas.Where(p => p.Date.ToString() == item.ToString()).ToList()
                    });
                }
                if (IsPrint == false)
                {
                    return new NewErrorModel()
                    {
                        data = publicAreaModels,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
                else
                {
                    if (publicAreaModels.Count > 0)
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Date", typeof(string));
                        dataTable.Columns.Add("Ten", typeof(string));
                        dataTable.Columns.Add("Tens", typeof(string));
                        dataTable.Columns.Add("Tenss", typeof(string));
                        dataTable.Columns.Add("Ele", typeof(string));
                        dataTable.Columns.Add("Eles", typeof(string));
                        dataTable.Columns.Add("Eless", typeof(string));
                        dataTable.Columns.Add("Two", typeof(string));
                        dataTable.Columns.Add("Twos", typeof(string));
                        dataTable.Columns.Add("Twoss", typeof(string));
                        dataTable.Columns.Add("Th", typeof(string));
                        dataTable.Columns.Add("Ths", typeof(string));
                        dataTable.Columns.Add("Thss", typeof(string));
                        dataTable.Columns.Add("Jdbg", typeof(string));
                        dataTable.Columns.Add("Jdbgs", typeof(string));
                        dataTable.Columns.Add("Jdbgss", typeof(string));
                        dataTable.Columns.Add("Jdss", typeof(string));
                        dataTable.Columns.Add("Jdsss", typeof(string));
                        dataTable.Columns.Add("Jdssss", typeof(string));
                        dataTable.Columns.Add("Bf", typeof(string));
                        dataTable.Columns.Add("Bfs", typeof(string));
                        dataTable.Columns.Add("Bfss", typeof(string));
                        dataTable.Columns.Add("Jdxx", typeof(string));
                        dataTable.Columns.Add("Jdxxs", typeof(string));
                        dataTable.Columns.Add("Jdxxss", typeof(string));

                        foreach (var publicAreaModel in publicAreaModels)
                        {
                            DataRow dataRow = dataTable.NewRow();
                            foreach (var item in publicAreaModel.publicAreas)
                            {
                                dataRow["Date"] = item.Date.ToString("yyyy-MM-dd");
                                switch (item.Power)
                                {
                                    case 0:
                                        dataRow["Ten"] = item.ClearPeople;
                                        dataRow["Tens"] = item.ControlPeople;
                                        dataRow["Tenss"] = item.State == true ? "合格" : "不合格";
                                        break;
                                    case 1:
                                        dataRow["Ele"] = item.ClearPeople;
                                        dataRow["Eles"] = item.ControlPeople;
                                        dataRow["Eless"] = item.State == true ? "合格" : "不合格";
                                        break;
                                    case 2:
                                        dataRow["Two"] = item.ClearPeople;
                                        dataRow["Twos"] = item.ControlPeople;
                                        dataRow["Twoss"] = item.State == true ? "合格" : "不合格";
                                        break;
                                    case 3:
                                        dataRow["Th"] = item.ClearPeople;
                                        dataRow["Ths"] = item.ControlPeople;
                                        dataRow["Thss"] = item.State == true ? "合格" : "不合格";
                                        break;
                                    case 4:
                                        dataRow["Jdbg"] = item.ClearPeople;
                                        dataRow["Jdbgs"] = item.ControlPeople;
                                        dataRow["Jdbgss"] = item.State == true ? "合格" : "不合格";
                                        break;
                                    case 5:  //基地宿舍四楼
                                        dataRow["Jdss"] = item.ClearPeople;
                                        dataRow["Jdsss"] = item.ControlPeople;
                                        dataRow["Jdssss"] = item.State == true ? "合格" : "不合格";
                                        break;
                                    case 7:   //北峰宿舍楼
                                        dataRow["Bf"] = item.ClearPeople;
                                        dataRow["Bfs"] = item.ControlPeople;
                                        dataRow["Bfss"] = item.State == true ? "合格" : "不合格";
                                        break;
                                    case 8:   //基地宿舍小楼
                                        dataRow["Jdxx"] = item.ClearPeople;
                                        dataRow["Jdxxs"] = item.ControlPeople;
                                        dataRow["Jdxxss"] = item.State == true ? "合格" : "不合格";
                                        break;
                                    default:
                                        break;
                                }
                            }
                            dataTable.Rows.Add(dataRow);
                        }
                        string path = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet/公共区域消毒导出模板.xlsx");
                        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\公共区域消毒单" + time + ".xlsx";
                        File.Copy(path, newPath);
                        if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dataTable, 0, 1))
                        {
                            DingTalkServersController dingTalkServersController = new DingTalkServersController();
                            //上盯盘
                            var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/公共区域消毒单" + time + ".xlsx");
                            //推送用户
                            FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                            fileSendModel.UserId = applyManId;
                            var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                            File.Delete(newPath);
                            return new NewErrorModel()
                            {
                                error = new Error(0, "导出成功，已推送至工作通知！", "") { },
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
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "未查询到数据！", "") { },
                        };
                    }
                }

            }

            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Clear")]

        public NewErrorModel Clear()
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.PublicArea.RemoveRange(context.PublicArea.ToList());
                    context.SaveChanges();
                }


                return new NewErrorModel()
                {
                    error = new Error(0, "清除成功！", "") { },
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class PublicAreaModel
    {
        public string Date { get; set; }

        public List<PublicArea> publicAreas { get; set; }
    }
}
