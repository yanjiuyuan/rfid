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
    [RoutePrefix("ProjectNew")]
    public class ProjectNewController : ApiController
    {
        /// <summary>
        /// 项目信息读取接口
        /// </summary>
        /// <param name="ApplyManId">创建者Id(不传时默认查所有项目信息)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllProJect")]
        public NewErrorModel GetAllProJect(string ApplyManId=null)
        {
            try
            {
                DDContext context = new DDContext();
                List<ProjectInfo> listProjectInfo = new List<ProjectInfo>();
                if (string.IsNullOrEmpty(ApplyManId))
                {
                    listProjectInfo = context.ProjectInfo.Where(p => p.ProjectState == "在研").ToList();
                }
                else
                {
                    listProjectInfo = context.ProjectInfo.Where(u => u.ApplyManId == ApplyManId && u.ProjectState == "在研").ToList();
                }
                return new NewErrorModel()
                {
                    data = listProjectInfo,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                return new NewErrorModel()
                {
                    error = new Error(2, ex.Message, "") { },
                };
            }
        }
    }
}
