﻿using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
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
                        List<ProcessingProgress> ProcessingProgressList = dDContext.ProcessingProgress.Where(p => p.TaskId == processingProgresse.TaskId).ToList();
                        if (ProcessingProgressList.Count > 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, string.Format("保存失败,系统中已存在流水号 {0} 的数据", processingProgresse.TaskId), "") { },
                            };
                        }
                        if (projectInfos.Where(p => p.ProjectId == processingProgresse.ProjectId &&
                         p.ProjectName == processingProgresse.ProjectName).ToList().Count == 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, string.Format("保存失败,项目Id {0} 、 项目名 {1} 与系统中的数据不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
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
                                    error = new Error(1, string.Format("保存失败,项目Id {0} 、 项目名 {1} 与系统中的大类、小类不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
                                };
                            }
                            else
                            {
                                List<Tasks> tasksDesigner = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.Designer)).ToList();
                                List<Tasks> tasksNoteTaker = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.NoteTaker)).ToList();
                                List<Tasks> tasksHeadOfDepartments = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.HeadOfDepartments)).ToList();
                                List<Tasks> tasksTabulator = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.Tabulator)).ToList();
                                if (tasksDesigner.Count == 0 || tasksNoteTaker.Count == 0 || tasksHeadOfDepartments.Count == 0 || tasksTabulator.Count == 0)
                                {
                                    if (tasksTabulator.Count == 0)
                                    {
                                        return new NewErrorModel()
                                        {
                                            error = new Error(1, string.Format("保存失败,系统中找不到：制表人 {0} 的Id   ！", processingProgresse.Tabulator), "") { },
                                        };
                                    }
                                    if (tasksDesigner.Count == 0)
                                    {
                                        return new NewErrorModel()
                                        {
                                            error = new Error(1, string.Format("保存失败,系统中找不到：设计员 {0} 的Id   ！", processingProgresse.NoteTaker), "") { },
                                        };
                                    }
                                    if (tasksNoteTaker.Count == 0)
                                    {
                                        return new NewErrorModel()
                                        {
                                            error = new Error(1, string.Format("保存失败,系统中找不到：记录员 {0} 的Id   ！", processingProgresse.Designer), "") { },
                                        };
                                    }
                                    if (tasksHeadOfDepartments.Count == 0)
                                    {
                                        return new NewErrorModel()
                                        {
                                            error = new Error(1, string.Format("保存失败,系统中找不到：部门负责人 {0} 的Id   ！", processingProgresse.HeadOfDepartments), "") { },
                                        };
                                    }
                                }
                                else
                                {
                                    processingProgresse.TabulatorId = tasksTabulator[0].ApplyManId;
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
                            , processingProgresse.TaskId, processingProgresse.SpeedOfProgress, eappUrl);

                        await dingTalkServersController.SendProcessingProgress(roles.UserId, 0, processingProgressModel.applyMan, processingProgresse.Bom
                          , processingProgresse.TaskId, processingProgresse.SpeedOfProgress, eappUrl);

                        processingProgresse.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                        ProcessingProgressList.Add(processingProgresse);
                    }
                    dDContext.ProcessingProgress.AddRange(ProcessingProgressList);
                }
                dDContext.SaveChanges();
                return new NewErrorModel()
                {
                    error = new Error(0, string.Format("保存成功！共计{0}条数据 ", processingProgressModel.processingProgresses.Count), "") { },
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
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="projectType">项目大类</param>
        /// <param name="projectSmallType">小类</param>
        /// <param name="taskId">流水号</param>
        /// <param name="key">关键字(项目名、BOM、设计员、记录人)</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read(string applyManId, int pageIndex, int pageSize, string projectType = "",
            string projectSmallType = "", string taskId = "", string key = "")
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<ProcessingProgress> processingProgresses =
                        context.ProcessingProgress.Where(t =>
                    t.TabulatorId.Contains(applyManId) ||
                   t.DesignerId.Contains(applyManId) || t.HeadOfDepartmentsId.Contains(applyManId)
                   || t.NoteTakerId.Contains(applyManId)).ToList();

                    processingProgresses = processingProgresses.Where(t =>
                   (taskId != "" ? t.TaskId == taskId : 1 == 1)).ToList() ;
                    processingProgresses = processingProgresses.Where(t =>
                   (key != "" ? (t.ProjectName.Contains(key) || (t.Bom.Contains(key) || (t.Designer.Contains(key) || (t.NoteTaker.Contains(key))))) : 1 == 1)).ToList();

                     processingProgresses = processingProgresses.Where(t =>
                  (projectType != "" ? t.ProjectType == projectType : 1 == 1)
                  || (projectSmallType != "" ? t.ProjectSmallType == projectSmallType : 1 == 1)).OrderBy(t => t.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    foreach (var item in processingProgresses)
                    {
                        NewErrorModel errorModel = GetPower(applyManId, item.TaskId);
                        item.Power = (List<int>)errorModel.data;
                    }
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
                using (DDContext context = new DDContext())
                {
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    foreach (var item in processingProgressModel.processingProgresses)
                    {
                        string eappUrl = string.Format("eapp://page/start/pushNotice/pushNotice?taskid={0}", item.TaskId);
                        //判断当前修改权限
                        NewErrorModel errorModel = GetPower(processingProgressModel.applyManId, item.TaskId);
                        List<int> vs = (List<int>)errorModel.data;

                        
                        if (vs.Contains(1) && vs.Contains(3)) //  0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人
                        {
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                            if (!string.IsNullOrEmpty(item.SpeedOfProgress)) //获取工作进度表状态
                            {
                                //推送制表人
                                await dingTalkServersController.SendProcessingProgress(item.TabulatorId, 0, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.SpeedOfProgress, eappUrl);
                                //推送设计人员
                                await dingTalkServersController.SendProcessingProgress(item.DesignerId, 0, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.SpeedOfProgress, eappUrl);
                            }
                        }

                        if (vs.Count==1 && vs.Contains(1)) //  0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人
                        {
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                            if (!string.IsNullOrEmpty(item.SpeedOfProgress)) //获取工作进度表状态
                            {
                                //推送实际记录人
                                await dingTalkServersController.SendProcessingProgress(item.NoteTakerId, 3, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.SpeedOfProgress, eappUrl);
                            }
                        }
                        if (vs.Count == 1 && vs.Contains(0)) //制表人 暂时不通知(添加的时候通知了)
                        {
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                        }
                        if (vs.Count == 1 && vs.Contains(2)) //  0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人
                        {
                            //修改已读状态
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                        }
                        if (vs.Count == 1 && vs.Contains(3)) //  0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人
                        {
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                            if (!string.IsNullOrEmpty(item.SpeedOfProgress)) //获取工作进度表状态
                            {
                                //推送制表人
                                await dingTalkServersController.SendProcessingProgress(item.TabulatorId, 0, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.SpeedOfProgress, eappUrl);
                                //推送设计人员
                                await dingTalkServersController.SendProcessingProgress(item.DesignerId, 0, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.SpeedOfProgress, eappUrl);
                                //推送分配人
                                await dingTalkServersController.SendProcessingProgress(item.HeadOfDepartmentsId, 0, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.SpeedOfProgress, eappUrl);
                            }
                        }

                    }
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
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
        /// 图纸审批默认数据读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DefaultRead")]
        public NewErrorModel DefaultRead(string taskId)
        {
            try
            {
                ProcessingProgress processingProgress = new ProcessingProgress();
                using (DDContext context = new DDContext())
                {
                    //判断流程是否已结束
                    List<Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.IsSend != true && t.State == 0).ToList();
                    if (tasksList.Count > 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "流程未结束！", "") { },
                        };
                    }
                    Tasks tasksFinish = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.NodeId == 5).FirstOrDefault();
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.NodeId == 0).FirstOrDefault();
                    ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == tasks.ProjectId.ToString()).FirstOrDefault();
                    Purchase purchase = context.Purchase.Where(p => p.TaskId == taskId).FirstOrDefault();
                    processingProgress.ProjectType = projectInfo.ProjectType;
                    processingProgress.ProjectSmallType = projectInfo.ProjectSmallType;
                    processingProgress.ProjectId = projectInfo.ProjectId;
                    processingProgress.ProjectName = projectInfo.ProjectName;
                    processingProgress.TaskId = taskId;
                    processingProgress.Bom = projectInfo.ProjectName + "(流水号" + taskId + ")";
                    processingProgress.Designer = JsonConvert.DeserializeObject<DesignerModel>(tasks.counts).Designer;
                    processingProgress.DesignerId = JsonConvert.DeserializeObject<DesignerModel>(tasks.counts).DesignerId;
                    processingProgress.BomTime = tasksFinish.ApplyTime;
                    processingProgress.TwoD = tasksFinish.ApplyTime;
                    processingProgress.ThreeD = tasksFinish.ApplyTime;
                    processingProgress.NeedTime = purchase.NeedTime;
                    processingProgress.NeedCount = tasks.Remark;
                }
                return new NewErrorModel()
                {
                    data = processingProgress,
                    error = new Error(0, "读取成功！", "") { },
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
        /// 用户Id(当前操作处理人Id)
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

    public class DesignerModel
    {
        public string Designer { get; set; }
        public string DesignerId { get; set; }
    }
}