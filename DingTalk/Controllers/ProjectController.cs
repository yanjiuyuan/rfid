using Common.JsonHelper;
using DingTalk.Models;
using DingTalk.Models.DbModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DingTalk.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 项目信息添加接口
        /// </summary>
        /// <returns></returns>
        /// 测试数据：/Project/AddProject
        //var ProjectTest = {
        //"ProjectName": "集成钉钉的信息管理系统",
        //"CreateTime": "2018-04-20 14:40",
        //"IsEnable":true,
        //"IsFinish": false,
        //"DeptName": "智慧工厂事业部",
        //"ApplyMan": "蔡兴桐",
        //"ApplyManId":"99f00dfc7badd72b00da35f211060176ae044d8b3b420106bb6ef6345be1ba9b",
        //"StartTime":"2017-10-23",
        //"EndTime":"2018-09-01",
        //"ProjectNo":"2017ZL054"
        //}
        [HttpPost]
        public string AddProject()
        {
            try
            {
                StreamReader sr = new StreamReader(Request.InputStream);
                var stream = sr.ReadToEnd();
                if (string.IsNullOrEmpty(stream))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "提交的数据不能为空！"
                    });
                }
                else
                {
                    ProjectInfo projectInfo = JsonHelper.JsonToObject<ProjectInfo>(stream);
                    using (DDContext context = new DDContext())
                    {
                        ProjectInfo pInfo = context.ProjectInfo.SingleOrDefault(u => u.ProjectNo == projectInfo.ProjectNo);

                        if (pInfo!=null)
                        {
                            return JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 2,
                                errorMessage = string.Format("已存在 项目编号{0}", pInfo.ProjectNo)
                            });
                        }
                        else
                        {
                            context.ProjectInfo.Add(projectInfo);
                            context.SaveChanges();
                            return JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 0,
                                errorMessage = "创建成功！"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 3,
                    errorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// 项目信息读取接口
        /// </summary>
        /// <param name="ApplyManId">创建者Id(不传时默认查所有项目信息)</param>
        /// <returns></returns>
        /// 测试数据：/Project/GetAllProJect
        public string GetAllProJect(string ApplyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<ProjectInfo> listProjectInfo = new List<ProjectInfo>();
                    if (string.IsNullOrEmpty(ApplyManId))
                    {
                        listProjectInfo = context.ProjectInfo.ToList();
                    }
                    else
                    {
                        listProjectInfo = context.ProjectInfo.Where(u => u.ApplyManId == "ApplyManId").ToList();
                    }
                    return JsonConvert.SerializeObject(listProjectInfo);
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
    }
}