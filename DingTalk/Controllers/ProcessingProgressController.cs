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
                string eappUrl = string.Format("eapp://page/start/pushNotice/pushNotice?taskid={0}", processingProgressModel.processingProgresses[0].TaskId);
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
                                    if (tasksDesigner.Count == 0)
                                    {
                                        return new NewErrorModel()
                                        {
                                            error = new Error(1, string.Format("系统中找不到：设计员 {0} 的Id   ！", processingProgresse.Designer), "") { },
                                        };
                                    }
                                    if (tasksNoteTaker.Count == 0)
                                    {
                                        return new NewErrorModel()
                                        {
                                            error = new Error(1, string.Format("系统中找不到：记录员 {0} 的Id   ！", processingProgresse.Designer), "") { },
                                        };
                                    }
                                    if (tasksHeadOfDepartments.Count == 0)
                                    {
                                        return new NewErrorModel()
                                        {
                                            error = new Error(1, string.Format("系统中找不到：部门负责人 {0} 的Id   ！", processingProgresse.Designer), "") { },
                                        };
                                    }
                                }
                                else
                                {
                                    processingProgresse.DesignerId = tasksDesigner[0].ApplyManId;
                                    processingProgresse.NoteTakerId = tasksNoteTaker[0].ApplyManId;
                                    processingProgresse.HeadOfDepartmentsId = tasksHeadOfDepartments[0].ApplyManId;
                                    processingProgresse.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                                    dDContext.ProcessingProgress.Add(processingProgresse);
                                }
                            }
                        }
                    }
                }
                if (!processingProgressModel.IsExcelUpload)  //操作界面添加
                {
                    List<ProcessingProgress> ProcessingProgressList = new List<ProcessingProgress>();
                    foreach (var processingProgresse in processingProgressModel.processingProgresses)
                    {
                        Roles roles = dDContext.Roles.Where(r => r.RoleName == "生产加工进度分配人").FirstOrDefault();
                        //推送钉钉消息给设计人员和部门负责人(胡工)
                        DingTalkServersController dingTalkServersController = new DingTalkServersController();
                        await dingTalkServersController.SendProcessingProgress(processingProgresse.DesignerId, 0, processingProgressModel.applyMan, processingProgresse.Bom
                            , processingProgresse.TaskId, eappUrl);

                        await dingTalkServersController.SendProcessingProgress(roles.UserId, 0, processingProgressModel.applyMan, processingProgresse.Bom
                          , processingProgresse.TaskId, eappUrl);

                        processingProgresse.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                        ProcessingProgressList.Add(processingProgresse);
                    }
                    dDContext.ProcessingProgress.AddRange(ProcessingProgressList);
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

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="processingProgressModel"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public async Task<NewErrorModel> Modify(ProcessingProgressModel processingProgressModel)
        {
            try
            {
                using (DDContext context=new DDContext ())
                {
                    return null;
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
        /// 获取用户权限(返回 0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人)
        /// </summary>
        /// <param name="applyManId">用户Id</param>
        /// <param name="taskId">可不传(传了只返回当前流水号对应的权限)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPower")]
        public NewErrorModel GetPower(string applyManId, string taskId = "")
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<int> vs = new List<int>();
                    if (string.IsNullOrEmpty(taskId))
                    {
                        if (context.Roles.Where(r => r.RoleName == "生产加工进度发起人" && r.UserId == applyManId).ToList().Count > 0)
                        {
                            vs.Add(0);
                        }
                        else
                        {
                            if (context.Roles.Where(r => r.RoleName == "生产加工进度分配人" && r.UserId == applyManId).ToList().Count > 0)
                            {
                                vs.Add(1);
                            }
                            else
                            {
                                vs.Add(2);
                            }
                        }
                    }
                    else
                    {
                        ProcessingProgress processingProgress = context.ProcessingProgress.Where(p => p.TaskId == taskId).FirstOrDefault();
                        if (applyManId == processingProgress.DesignerId)
                        {
                            vs.Add(2);
                        }
                        if (applyManId == processingProgress.NoteTakerId) //记录人
                        {
                            vs.Add(3);
                        }
                        if (applyManId == processingProgress.TabulatorId) //制表人
                        {
                            vs.Add(0);
                        }
                        if (applyManId == processingProgress.HeadOfDepartmentsId) //分配人
                        {
                            vs.Add(1);
                        }
                    }

                    return new NewErrorModel()
                    {
                        data = vs,
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
        /// 用户Id(当前操作处理人)
        /// </summary>
        public string applyManId { get; set; }
        /// <summary>
        /// 用户名(当前操作处理人)
        /// </summary>
        public string applyMan { get; set; }

        /// <summary>
        /// 公司Id  研究院 0 华数 1
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 是否是Excel上传
        /// </summary>
        public bool IsExcelUpload { get; set; }
        public List<ProcessingProgress> processingProgresses { get; set; }
    }
}
