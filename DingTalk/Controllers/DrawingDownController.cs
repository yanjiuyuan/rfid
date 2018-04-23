using DingTalk.Models;
using DingTalk.Models.DbModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DingTalk.Controllers
{
    public class DrawingDownController : Controller
    {
        // GET: DrawingDown
        public ActionResult Index()
        {
            return View();
        }

        #region 图纸上传数据读取
        /// <summary>
        /// 图纸上传数据读取
        /// </summary>
        /// <param name="ApplyManId">用户Id</param>
        /// <returns></returns>
        /// 测试数据：/DrawingDown/GetDrawingDownInfo?ApplyManId=胡工
        [HttpGet]
        public string GetDrawingDownInfo(string ApplyManId)
        {
            try
            {
                if (string.IsNullOrEmpty(ApplyManId))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "未传递参数ApplyManId"
                    });
                }
                else
                {
                    using (DDContext context = new DDContext())
                    {
                        var TaskList = context.Tasks.Where(u => u.State == 1 && u.FlowId == 6);
                        var NodeInfoList = context.NodeInfo.Where(u => u.FlowId == "6" && u.PeopleId == ApplyManId);
                        var Purchase = context.Purchase;
                        var TaskLists = from t in TaskList
                                        join n in NodeInfoList
                                        on t.FlowId.ToString() equals n.FlowId
                                        select new
                                        {
                                            TaskId = t.TaskId
                                        };
                        var Quary = from t in TaskLists
                                    join p in Purchase
                                    on t.TaskId.ToString() equals p.TaskId
                                    where p.IsDown == false
                                    select new
                                    {
                                        TaskId = p.TaskId,
                                        DrawingNo = p.DrawingNo,
                                        CodeNo = p.CodeNo,
                                        Name = p.Name,
                                        Count = p.Count,
                                        MaterialScience = p.MaterialScience,
                                        Unit = p.Unit,
                                        Brand = p.Brand,
                                        Sorts = p.Sorts,
                                        Mark = p.Mark,
                                        IsDown = p.IsDown
                                    };
                        return JsonConvert.SerializeObject(Quary);
                    }
                }

            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
            }
        }
        #endregion

        #region 图纸下发表单提交

        /// <summary>
        /// 图纸下发表单提交接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string SubmitDrawingDown()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            return "";
        }

        #endregion

    }
}