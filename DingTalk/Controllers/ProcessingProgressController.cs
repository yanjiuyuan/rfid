using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task<NewErrorModel> Save(ProcessingProgressModel processingProgressModel)
        {
            try
            {
                DDContext dDContext = new DDContext();
                string eappUrl = "eapp://page/start/pushNotice/pushNotice";
                if (dDContext.Roles.Where(r => r.RoleName == "生产加工进度发起人" && r.UserId == processingProgressModel.applyManId).ToList().Count == 0)
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "没有权限上传！", "") { },
                    };
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
                                error = new Error(1, string.Format("项目Id {0} 、 项目名 {1} 与系统中的数据不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
                            };
                        }
                        else
                        {
                            if (projectInfos.Where(p => p.ProjectId == processingProgresse.ProjectId &&
                          p.ProjectName == p.ProjectName && p.ProjectType == processingProgresse.ProjectType
                          && p.ProjectSmallType == processingProgresse.ProjectSmallType).ToList().Count == 0)
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, string.Format("项目Id {0} 、 项目名 {1} 与系统中的大类、小类不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
                                };
                            }
                            else
                            {
                                List<Tasks> tasksDesigner = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.Designer)).ToList();
                                List<Tasks> tasksNoteTaker = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.NoteTaker)).ToList();
                                List<Tasks> tasksHeadOfDepartments = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.HeadOfDepartmentsId)).ToList();
                                if (tasksDesigner.Count == 0 || tasksNoteTaker.Count == 0 || tasksHeadOfDepartments.Count == 0)
                                {
                                    return new NewErrorModel()
                                    {
                                        error = new Error(1, string.Format("系统中找不到：{0} 的Id   ！", processingProgresse.Designer), "") { },
                                    };
                                }
                                else
                                {
                                    processingProgresse.DesignerId = tasksDesigner[0].ApplyManId;
                                    processingProgresse.NoteTakerId= tasksNoteTaker[0].ApplyManId;
                                    processingProgresse.HeadOfDepartmentsId = tasksHeadOfDepartments[0].ApplyManId;
                                    dDContext.ProcessingProgress.Add(processingProgresse);
                                }
                            }
                        }
                    }
                }
                if (!processingProgressModel.IsExcelUpload)  //单条添加
                {
                    Roles roles = dDContext.Roles.Where(r => r.RoleName == "生产加工进度分配人").FirstOrDefault();
                    //推送钉钉消息给设计人员和部门负责人(胡工)
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    await dingTalkServersController.SendProcessingProgress(processingProgressModel.processingProgresses[0].DesignerId, 0, processingProgressModel.applyMan, processingProgressModel.processingProgresses[0].Bom
                        , processingProgressModel.processingProgresses[0].TaskId, eappUrl);

                    await dingTalkServersController.SendProcessingProgress(roles.UserId, 0, processingProgressModel.applyMan, processingProgressModel.processingProgresses[0].Bom
                      , processingProgressModel.processingProgresses[0].TaskId, eappUrl);
                    
                    dDContext.ProcessingProgress.Add(processingProgressModel.processingProgresses[0]);
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
        /// <param name="applyManId">查询人Id</param>
        /// <param name="taskId">不传查全部</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read(string applyManId, string taskId = "")
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<ProcessingProgress> processingProgresses = context.ProcessingProgress.Where(t =>
                   (taskId == "" ? t.TaskId == taskId : 1 == 2) || t.TabulatorId.Contains(applyManId) ||
                   t.DesignerId.Contains(applyManId) || t.HeadOfDepartmentsId.Contains(applyManId)
                   || t.NoteTakerId.Contains(applyManId)).ToList();
                    return new NewErrorModel()
                    {
                        count = processingProgresses.Count,
                        data = processingProgresses,
                        error = new Error(0, "读取成功！", "") { },
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
        /// 用户名
        /// </summary>
        public string applyMan { get; set; }
        /// <summary>
        /// 是否是Excel上传
        /// </summary>
        public bool IsExcelUpload { get; set; }
        public List<ProcessingProgress> processingProgresses { get; set; }
    }
}
