using DingTalk.EF;
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
    /// 生产加工进度表
    /// </summary>
    [RoutePrefix("ProcessingProgress")]
    public class ProcessingProgressController : ApiController
    {
        /// <summary>
        /// 生产加工进度表批量保存
        /// </summary>
        /// <param name="processingProgressModel"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save(ProcessingProgressModel processingProgressModel)
        {
            try
            {
                DDContext dDContext = new DDContext();
                //Excel上传判断权限
                if (processingProgressModel.IsExcelUpload)
                {
                    if (dDContext.Roles.Where(r => r.RoleName == "生产加工进度发起人" && r.UserId == processingProgressModel.applyManId).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有权限上传！", "") { },
                        };
                    }
                }
                List<ProjectInfo> projectInfos = dDContext.ProjectInfo.ToList();
                foreach (var processingProgresse in processingProgressModel.processingProgresses)
                {
                    //校对数据
                    if (!string.IsNullOrEmpty(processingProgresse.TaskId))
                    {
                        if (projectInfos.Where(p => p.ProjectId == processingProgresse.ProjectId &&
                         p.ProjectName == p.ProjectName).ToList().Count == 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, string.Format("项目Id {0} 或者 项目名 {1} 与系统中数据不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
                            };
                        }
                        else
                        {
                            dDContext.ProcessingProgress.Add(processingProgresse);
                        }
                    }
                }
                dDContext.SaveChanges();
                return new NewErrorModel()
                {
                    error = new Error(0, "保存成功！", "") { },
                };
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
        /// 生产加工进度表批量读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read(string taskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<ProcessingProgress> processingProgresses = context.ProcessingProgress.Where(t => t.TaskId == taskId).ToList();
                    return new NewErrorModel()
                    {
                        count = processingProgresses.Count,
                        data = processingProgresses,
                        error = new Error(0, "保存成功！", "") { },
                    };
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
    }

    public class ProcessingProgressModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string applyManId { get; set; }
        /// <summary>
        /// 是否是Excel上传
        /// </summary>
        public bool IsExcelUpload { get; set; }
        public List<ProcessingProgress> processingProgresses { get; set; }
    }
}
