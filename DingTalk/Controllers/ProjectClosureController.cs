using DingTalk.Bussiness.FlowInfo;
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
    /// 项目结题
    /// </summary>
    [RoutePrefix("ProjectClosure")]
    public class ProjectClosureController : ApiController
    {
        /// <summary>
        /// 项目结题保存
        /// </summary>
        /// <param name="projectClosureModel"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] ProjectClosureModel projectClosureModel)
        {
            try
            {
                using (DDContext dDContext = new DDContext())
                {
                    if (projectClosureModel.detailedLists.Count > 0)
                    {
                        dDContext.DetailedList.AddRange(projectClosureModel.detailedLists);
                    }
                    if (projectClosureModel.applicationUnitList.Count > 0)
                    {
                        dDContext.ApplicationUnit.AddRange(projectClosureModel.applicationUnitList);
                    }
                    if (projectClosureModel.projectFundingList.Count > 0)
                    {
                        dDContext.ProjectFunding.AddRange(projectClosureModel.projectFundingList);
                    }
                    if (projectClosureModel.longitudinalProject.Count > 0)
                    {
                        dDContext.LongitudinalProject.AddRange(projectClosureModel.longitudinalProject);
                    }
                    dDContext.ProjectClosure.Add(projectClosureModel.projectClosure);
                    dDContext.SaveChanges();
                }
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
        /// 项目结题读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string taskId)
        {
            try
            {
                ProjectClosureModel projectClosureModel = new ProjectClosureModel();
                DDContext dDContext = new DDContext();
                ProjectClosure projectClosure = dDContext.ProjectClosure.Where(p => p.TaskId == taskId).FirstOrDefault();
                List<DetailedList> detailedLists = dDContext.DetailedList.Where(d => d.TaskId == taskId).ToList();
                List<ApplicationUnit> applicationUnitList = dDContext.ApplicationUnit.Where(d => d.TaskId == taskId).ToList();
                List<ProjectFunding> projectFundingList = dDContext.ProjectFunding.Where(d => d.TaskId == taskId).ToList();
                List<LongitudinalProject> longitudinalProjects = dDContext.LongitudinalProject.Where(d => d.TaskId == taskId).ToList();

                projectClosureModel.projectClosure = projectClosure;
                projectClosureModel.detailedLists = detailedLists;
                projectClosureModel.applicationUnitList = applicationUnitList;
                projectClosureModel.projectFundingList = projectFundingList;
                projectClosureModel.longitudinalProject = longitudinalProjects;
                return new NewErrorModel()
                {
                    data = projectClosureModel,
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
        /// 立项书或建议书、PPT数据读取
        /// </summary>
        /// <param projectId="">项目Id</param>
        /// <returns></returns>
        [Route("ReadDefault")]
        [HttpGet]
        public NewErrorModel ReadDefault(string projectId)
        {
            try
            {
                DDContext context = new DDContext();
                //立项数据(附件)
                string FlowId = context.Flows.Where(t => t.FlowName == "立项申请").First().FlowId.ToString();
                List<Tasks> tasksList = FlowInfoServer.ReturnUnFinishedTaskId(FlowId);
                List<Tasks> tasksListQuery = tasksList.Where(t => t.FlowId.ToString() == FlowId && t.NodeId == 0).ToList();
                CreateProject createProject = context.CreateProject.Where(c => c.ProjectId == projectId).FirstOrDefault();
                if (createProject != null)
                {
                    Tasks tasks = tasksListQuery.Where(t => t.TaskId.ToString() == createProject.TaskId).FirstOrDefault();
                    FileUrlModel fileUrlModel = new FileUrlModel()
                    {
                        fileName = tasks.OldFileUrl,
                        fileUrl = tasks.FileUrl,
                        mediaId = tasks.MediaId,
                    };
                    return new NewErrorModel()
                    {
                        data = fileUrlModel,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        data = "",
                        error = new Error(0, "暂无数据！", "") { },
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
        /// 项目采购清单、借用清单、维修清单、受理知识产权清单  流程数据读取
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("ReadFlowInfo")]
        [HttpGet]
        public NewErrorModel ReadFlowInfo(string projectId)
        {
            try
            {
                DDContext context = new DDContext();
                List<FlowModel> FlowModelList = new List<FlowModel>();
                List<Flows> flowsList = context.Flows.Where(t => t.FlowName.Contains("采购") || t.FlowName.Contains("借入") ||
                t.FlowName.Contains("维修") || t.FlowName.Contains("知识产权"))
                    .ToList();
                foreach (var flows in flowsList)
                {
                    string FlowId = flows.FlowId.ToString();
                    string FlowName = flows.FlowName.ToString();
                    List<Tasks> tasksList = FlowInfoServer.ReturnUnFinishedTaskId(FlowId).Where(t => t.ProjectId == projectId).ToList();
                    List<Tasks> tasksListQuery = tasksList.Where(t => t.FlowId.ToString() == FlowId && t.NodeId == 0).ToList();
                    CreateProject createProject = context.CreateProject.Where(c => c.ProjectId == projectId).FirstOrDefault();
                    if (createProject != null)
                    {
                        foreach (var item in tasksListQuery)
                        {
                            FlowModelList.Add(new FlowModel()
                            {
                                Type = FlowName,
                                ApplyMan = item.ApplyMan,
                                ApplyManId = item.ApplyManId,
                                ApplyTime = item.ApplyTime,
                                OldtaskId = item.TaskId.ToString()
                            });
                        }
                    }
                }
                return new NewErrorModel()
                {
                    data = FlowModelList,
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
        /// 项目结题修改
        /// </summary>
        /// <param name="projectClosureModel"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public NewErrorModel Modify([FromBody] ProjectClosureModel projectClosureModel)
        {
            try
            {
                if (string.IsNullOrEmpty(projectClosureModel.NodeId))
                {
                    return new NewErrorModel()
                    {
                        data = projectClosureModel,
                        error = new Error(1, "NodeId 没传！", "") { },
                    };
                }
                DDContext dDContext = new DDContext();
                ProjectClosure projectClosure = projectClosureModel.projectClosure;
                dDContext.Entry<ProjectClosure>(projectClosure).State = System.Data.Entity.EntityState.Modified;
                projectClosureModel.detailedLists.ToList().ForEach(d =>
                {
                    dDContext.Entry(d).State = System.Data.Entity.EntityState.Modified;
                });
                projectClosureModel.applicationUnitList.ToList().ForEach(d =>
                {
                    dDContext.Entry(d).State = System.Data.Entity.EntityState.Modified;
                });
                projectClosureModel.projectFundingList.ToList().ForEach(d =>
                {
                    dDContext.Entry(d).State = System.Data.Entity.EntityState.Modified;
                });
                projectClosureModel.longitudinalProject.ToList().ForEach(d =>
                {
                    dDContext.Entry(d).State = System.Data.Entity.EntityState.Modified;
                });
                dDContext.SaveChanges();

                Flows flows = dDContext.Flows.Where(f => f.FlowName.Contains("结题")).FirstOrDefault();
                NodeInfo nodeInfo = dDContext.NodeInfo.Where(n => n.NodeName == "结束" && n.FlowId.ToString() == flows.FlowId.ToString()
                ).FirstOrDefault();

                //最后一步保存路径
                if (nodeInfo.NodeId == Int32.Parse(projectClosureModel.NodeId) + 1)
                {

                }

                return new NewErrorModel()
                {
                    data = projectClosureModel,
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
    }

    public class FlowModel
    {
        public string Type { get; set; }
        public string taskId { get; set; }
        public string ApplyMan { get; set; }
        public string ApplyManId { get; set; }
        public string ApplyTime { get; set; }
        /// <summary>
        /// 其他流程的TaskId
        /// </summary>
        public string OldtaskId { get; set; }
    }

    public class FileUrlModel
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string fileUrl { get; set; }
        /// <summary>
        /// 盯盘Id
        /// </summary>
        public string mediaId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }
    }

    public class ProjectClosureModel
    {
        /// <summary>
        /// 当前NodeId 用于判断流程是否结束
        /// </summary>
        public string NodeId { get; set; }
        public ProjectClosure projectClosure { get; set; }

        /// <summary>
        /// 项目采购清单、借用清单、维修清单、受理知识产权清单
        /// </summary>
        public List<DetailedList> detailedLists { get; set; }

        /// <summary>
        /// 转化/应用单位
        /// </summary>
        public List<ApplicationUnit> applicationUnitList { get; set; }

        /// <summary>
        /// 项目经费使用情况
        /// </summary>
        public List<ProjectFunding> projectFundingList { get; set; }

        /// <summary>
        /// 纵向项目基本情况表
        /// </summary>
        public List<LongitudinalProject> longitudinalProject { get; set; }

    }
}
