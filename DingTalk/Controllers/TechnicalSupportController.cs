using Common.Flie;
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
    /// 技术支持单
    /// </summary>
    [RoutePrefix("TechnicalSupport")]
    public class TechnicalSupportController : ApiController
    {
        /// <summary>
        /// 图纸BOM变更表单保存
        /// </summary>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public NewErrorModel Save(TechnicalSupport technicalSupport)
        {
            try
            {
                EFHelper<TechnicalSupport> eFHelper = new EFHelper<TechnicalSupport>();
                technicalSupport.CompanyName = "泉州华中科技大学智能制造研究院";
                technicalSupport.ProjectState = "在研";
                technicalSupport.ProjectType = "测试项目";
                eFHelper.Add(technicalSupport);

                return new NewErrorModel()
                {
                    data = "",
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
        /// 图纸BOM变更表单读取
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public NewErrorModel Read(string TaskId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    TechnicalSupport technicalSupport = context.TechnicalSupport.Where(c => c.TaskId == TaskId).FirstOrDefault();
                    
                    return new NewErrorModel()
                    {
                        data = technicalSupport,
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
        /// 图纸BOM变更表单修改
        /// </summary>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public NewErrorModel Modify(TechnicalSupport technicalSupport)
        {
            try
            {
             
                EFHelper<TechnicalSupport> eFHelper = new EFHelper<TechnicalSupport>();
                eFHelper.Modify(technicalSupport);
                if (technicalSupport.IsCreateProject)
                {
                    using (DDContext context = new DDContext())
                    {
                        ProjectInfo projectInfo = new ProjectInfo();
                        projectInfo.ProjectName = technicalSupport.ProjectName;
                        projectInfo.CreateTime = DateTime.Now.ToString("yyyy-dd-mm HH:mm:ss");
                        projectInfo.IsEnable = true;
                        projectInfo.ProjectState = technicalSupport.ProjectState;
                        projectInfo.DeptName = technicalSupport.DeptName;
                        projectInfo.StartTime = technicalSupport.StartTime;
                        projectInfo.EndTime = technicalSupport.EndTime;
                        projectInfo.CompanyName = technicalSupport.CompanyName;
                     
                        //建立项目文件夹及其子文件
                        string path = string.Format("\\UploadFile\\ProjectFile\\{0}\\{1}\\{2}",
                            projectInfo.CompanyName, technicalSupport.ProjectType, technicalSupport.ProjectName);
                        projectInfo.FilePath = path;
                        projectInfo.ResponsibleMan = technicalSupport.ResponsibleMan;
                        projectInfo.ResponsibleManId = technicalSupport.ResponsibleManId;
                       
                        projectInfo.TeamMembers = technicalSupport.TeamMembers;
                        projectInfo.TeamMembersId = technicalSupport.TeamMembersId;

                        context.ProjectInfo.Add(projectInfo);
                        context.SaveChanges();
                        path = System.IO.Path.GetFullPath(path);
                        FileHelper.CreateDirectory(path);
                    }
                }
             

                return new NewErrorModel()
                {
                    data = "",
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
}
