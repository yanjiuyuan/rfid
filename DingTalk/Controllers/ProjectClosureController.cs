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
                projectClosureModel.projectClosure = projectClosure;
                projectClosureModel.detailedLists = detailedLists;
                projectClosureModel.applicationUnitList = applicationUnitList;
                projectClosureModel.projectFundingList = projectFundingList;
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
        /// 项目结题修改
        /// </summary>
        /// <param name="projectClosureModel"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object Modify([FromBody] ProjectClosureModel projectClosureModel)
        {
            try
            {
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
                dDContext.SaveChanges();
                
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

    public class ProjectClosureModel
    {
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
    }
}
