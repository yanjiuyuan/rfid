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
    /// 角色管理
    /// </summary>
    [RoutePrefix("Role")]
    public class RoleController : ApiController
    {
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="RoleName">角色名(不传默认全查)</param>
        /// <returns></returns>
        /// 测试数据: Role/GetRoleInfo?RoleName=图纸校对人员
        [Route("GetRoleInfo")]
        [HttpGet]
        public object GetRoleInfo(string RoleName)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (string.IsNullOrEmpty(RoleName))
                    {
                        var RoleList = context.Roles.ToList();
                        return RoleList;
                    }
                    else
                    {
                        var RoleList = context.Roles.Where(r => r.RoleName.Contains(RoleName)).ToList();
                        var Quary = from r in RoleList
                                    select new
                                    {
                                        RoleName=r.RoleName,
                                        name = r.UserName,
                                        emplId = r.UserId
                                    };
                        return Quary;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = ex.Message
                };
            }
        }
    }
}
