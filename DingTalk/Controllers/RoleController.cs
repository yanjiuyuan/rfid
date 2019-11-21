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
        /// 同步旧数据(第一次用)
        /// </summary>
        [HttpGet]
        [Route("AddAsync")]

        public void AddAsync()
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<string> RoleNameList = context.Roles.Select(r => r.RoleName).ToList();
                    foreach (var item in RoleNameList.Distinct())
                    {
                        context.Role.Add(new Role()
                        {
                            RoleName = item,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            CreateMan = "超级管理员",
                            CreateManId = "083452125733424957",
                            IsEnable = true,
                        });
                        context.SaveChanges();
                    }

                    foreach (var item in context.Role.ToList())
                    {
                        context.Database.ExecuteSqlCommand($"update roles set roleid={item.Id} where rolename='{item.RoleName}'");
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取角色种类及相应权限信息
        /// </summary>
        /// <param name="applyManId">当前处理人Id</param>
        /// <returns></returns>
        [Route("GetRole")]
        [HttpGet]
        public NewErrorModel GetRole(string applyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.UserId == applyManId && r.RoleName == "超级管理员").ToList().Count() > 0)
                    {
                        List<Role> roles = context.Role.ToList();
                        List<Roles> rolesList = context.Roles.ToList();
                        foreach (var item in roles)
                        {
                            item.roles = rolesList.Where(r => r.RoleId == item.Id).ToList();
                        }
                        return new NewErrorModel()
                        {
                            data = roles,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有读取权限！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增角色种类
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        [Route("AddRole")]
        [HttpPost]
        public NewErrorModel AddRole([FromBody]List<Role> roles)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (roles != null && roles.Count > 0)
                    {
                        List<Role> roleQuery = context.Role.ToList();
                        foreach (var item in roles)
                        {
                            if (context.Roles.Where(r => r.UserId == item.CreateManId && r.RoleName == "超级管理员").ToList().Count() > 0)
                            {
                                if (roleQuery.Where(r => r.RoleName == item.RoleName).ToList().Count > 0)
                                {
                                    return new NewErrorModel()
                                    {
                                        error = new Error(1, $"角色名：{item.RoleName} 已存在！", "") { },
                                    };
                                }
                                else
                                {
                                    item.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    context.Role.Add(item);
                                    context.SaveChanges();
                                    Role role = context.Role.Where(r => r.RoleName == item.RoleName).FirstOrDefault();
                                    if (item.roles.Count > 0)
                                    {
                                        foreach (var r in item.roles)
                                        {
                                            r.RoleName = role.RoleName;
                                            r.RoleId = Int32.Parse(role.Id.ToString());
                                        }
                                        context.Roles.AddRange(item.roles);
                                    }
                                }
                            }
                            else
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, "没有添加权限！", "") { },
                                };
                            }
                        }
                        context.SaveChanges();
                        return new NewErrorModel()
                        {
                            error = new Error(0, "添加成功！", "") { },
                        };
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "格式有误！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改角色及其相应权限
        /// </summary>
        /// <param name="roleOperator"></param>
        /// <returns></returns>
        [Route("ModifyRole")]
        [HttpPost]
        public NewErrorModel ModifyRole(RoleOperator roleOperator)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (context.Roles.Where(r => r.UserId == roleOperator.applyManId && r.RoleName == "超级管理员").ToList().Count > 0)
                    {
                        foreach (var item in roleOperator.roles)
                        {
                            if (item.RoleName.Contains("超级管理员"))
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, $"角色 {item.RoleName} 不允许修改！", "") { },
                                };
                            }
                            context.Entry<Role>(item).State = System.Data.Entity.EntityState.Modified;
                        }
                        List<Roles> roles = context.Roles.ToList();
                        if (roleOperator.roles.Count > 0)
                        {
                            foreach (var item in roleOperator.roles)
                            {
                                List<Roles> rolesNew = roles.Where(t => t.RoleId == item.Id).ToList();
                                context.Roles.RemoveRange(rolesNew);
                                foreach (var items in item.roles)
                                {
                                    items.RoleName = item.RoleName;
                                    context.Roles.Add(items);
                                }
                            }
                        }
                        context.SaveChanges();
                        return new NewErrorModel()
                        {
                            error = new Error(0, "修改成功！", "") { },
                        };
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有修改权限！", "") { },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 新增角色权限
        /// </summary>
        /// <param name="roleOperator"></param>
        /// <returns></returns>
        //[Route("AddRolePower")]
        //[HttpPost]
        //public NewErrorModel AddRolePower(RoleOperatorRoles roleOperator)
        //{
        //    try
        //    {
        //        using (DDContext context = new DDContext())
        //        {
        //            if (context.Roles.Where(r => r.UserId == roleOperator.applyManId && r.RoleName == "超级管理员").ToList().Count > 0)
        //            {
        //                context.Roles.AddRange(roleOperator.roles);
        //                context.SaveChanges();
        //                return new NewErrorModel()
        //                {
        //                    error = new Error(0, "添加成功！", "") { },
        //                };
        //            }
        //            else
        //            {
        //                return new NewErrorModel()
        //                {
        //                    error = new Error(1, "没有添加权限！", "") { },
        //                };
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 修改角色权限
        /// </summary>
        /// <param name="roleOperator"></param>
        /// <returns></returns>
        //[Route("ModifyRolePower")]
        //[HttpPost]
        //public NewErrorModel ModifyRolePower(RoleOperatorRoles roleOperator)
        //{
        //    try
        //    {
        //        using (DDContext context = new DDContext())
        //        {
        //            if (context.Roles.Where(r => r.UserId == roleOperator.applyManId && r.RoleName == "超级管理员").ToList().Count > 0)
        //            {
        //                foreach (var item in roleOperator.roles)
        //                {
        //                    context.Entry<Roles>(item).State = System.Data.Entity.EntityState.Modified;
        //                }
        //                context.SaveChanges();
        //                return new NewErrorModel()
        //                {
        //                    error = new Error(0, "修改成功！", "") { },
        //                };
        //            }
        //            else
        //            {
        //                return new NewErrorModel()
        //                {
        //                    error = new Error(1, "没有修改权限！", "") { },
        //                };
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        /// <summary>
        /// 修改角色详细权限
        /// </summary>
        /// <param name="roleOperator"></param>
        /// <returns></returns>
        //[Route("ModifyRoleDetail")]
        //[HttpPost]
        //public NewErrorModel ModifyRoleDetail(RoleOperatorRoles roleOperator)
        //{
        //    try
        //    {
        //        using (DDContext context = new DDContext())
        //        {
        //            if (context.Roles.Where(r => r.UserId == roleOperator.applyManId && r.RoleName == "超级管理员").ToList().Count > 0)
        //            {
        //                foreach (var item in roleOperator.roles)
        //                {
        //                    context.Entry<Roles>(item).State = System.Data.Entity.EntityState.Modified;
        //                }
        //                context.SaveChanges();
        //                return new NewErrorModel()
        //                {
        //                    error = new Error(0, "修改成功！", "") { },
        //                };
        //            }
        //            else
        //            {
        //                return new NewErrorModel()
        //                {
        //                    error = new Error(1, "没有修改权限！", "") { },
        //                };
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        [Route("GetRoleInfoList")]
        [HttpPost]
        public NewErrorModel GetRoleInfoList()
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    Dictionary<string, List<Roles>> keyValuePairs = new Dictionary<string, List<Roles>>();
                    var RoleList = context.Roles.ToList();
                    foreach (var item in RoleList)
                    {
                        if (!keyValuePairs.Keys.Contains(item.RoleName))
                        {
                            keyValuePairs.Add(item.RoleName, new List<Roles>() {
                                item
                            });
                        }
                        else
                        {
                            List<Roles> roles = keyValuePairs[item.RoleName];
                            roles.Add(item);
                            keyValuePairs[item.RoleName] = roles;
                        }
                    }

                    return new NewErrorModel()
                    {
                        data = keyValuePairs,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="RoleName">角色名(不传默认全查)</param>
        /// <returns></returns>
        /// 测试数据: Role/GetRoleInfo?RoleName=图纸校对人员
        [Route("GetRoleInfo")]
        [HttpGet]
        public object GetRoleInfo(string RoleName = "")
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (RoleName != null && RoleName == "")
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
                                        RoleName = r.RoleName,
                                        name = r.UserName,
                                        emplId = r.UserId
                                    };
                        return Quary;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class RoleOperator
    {
        /// <summary>
        /// 当前操作人Id
        /// </summary>
        public string applyManId { get; set; }
        public List<Role> roles { get; set; }
    }

    //public class RoleOperatorRoles
    //{
    //    /// <summary>
    //    /// 当前操作人Id
    //    /// </summary>
    //    public string applyManId { get; set; }
    //    public List<Roles> roles { get; set; }
    //}



    //public class RoleListOperator
    //{
    //    public List<Role> roles { get; set; }
    //}    //public class RoleOperatorRoles
    //{
    //    /// <summary>
    //    /// 当前操作人Id
    //    /// </summary>
    //    public string applyManId { get; set; }
    //    public List<Roles> roles { get; set; }
    //}



    //public class RoleListOperator
    //{
    //    public List<Role> roles { get; set; }
    //}    //public class RoleOperatorRoles
    //{
    //    /// <summary>
    //    /// 当前操作人Id
    //    /// </summary>
    //    public string applyManId { get; set; }
    //    public List<Roles> roles { get; set; }
    //}



    //public class RoleListOperator
    //{
    //    public List<Role> roles { get; set; }
    //}

}
