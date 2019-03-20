using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 立项
    /// </summary>
    [RoutePrefix("CreateProject")]
    public class CreateProjectController : ApiController
    {
        /// <summary>
        /// 立项表单保存
        /// </summary>
        /// <param name="createProject"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] CreateProject createProject)
        {
            try
            {
                EFHelper<CreateProject> eFHelper = new EFHelper<CreateProject>();

                eFHelper.Add(createProject);
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
        /// 立项表单读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string taskId)
        {
            try
            {
                EFHelper<CreateProject> eFHelper = new EFHelper<CreateProject>();
                CreateProject createProject = eFHelper.GetListBy(t => t.TaskId == taskId).ToList().First();

                return new NewErrorModel()
                {
                    data = createProject,
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
        ///立项表单修改
        /// </summary>
        /// <param name="createProject"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object Modify([FromBody] CreateProject createProject)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<CreateProject>(createProject).State = EntityState.Modified;
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
    }
}
