using Common.Excel;
using Common.Flie;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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
        public NewErrorModel GetAllProJect(string ApplyManId = null)
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
                throw ex;
            }
        }

        /// <summary>
        /// 获取目录下的文件夹信息
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <param name="userId">用户Id</param>
        /// <param name="keyword">关键字查询</param>
        /// <returns>返回文件名数组</returns>
        [Route("GetFileMsg")]
        [HttpGet]
        public NewErrorModel GetFileMsg(string path, string userId, string keyword = "")
        {
            try
            {
                string[] AbPathList = FileHelper.GetFileNames(System.Web.Hosting.HostingEnvironment.MapPath(path));
                //List<string> RePathList = new List<string>();
                List<FileModels> FileModelsList = new List<FileModels>();
                foreach (var item in AbPathList)
                {
                    int fileCount = 0;
                    DirectoryInfo getFolder = new DirectoryInfo(item);
                    if (getFolder.Exists)
                    {
                        FileInfo[] getFileInfos = getFolder.GetFiles();
                        fileCount = getFileInfos.Length;
                    }
                    int i = 0;
                    bool IsFile = false;
                    //绝对路径转相对
                    string RelativePath = FileHelper.RelativePath(AppDomain.CurrentDomain.BaseDirectory, item);
                    if (File.Exists(item))
                    {
                        IsFile = true;
                    }
                    string FileName = Path.GetFileName(RelativePath);

                    DateTime createTime = getFolder.CreationTime;//获取目录或者文件的创建 日期

                    if (FileName.Length > 2)
                    {
                        if (!Int32.TryParse(FileName.Substring(0, 2), out i))
                        {
                            Int32.TryParse(FileName.Substring(0, 1), out i);
                        }
                    }
                    if (string.IsNullOrEmpty(keyword))
                    {
                        FileModelsList.Add(new FileModels()
                        {
                            createTime = createTime,
                            IsFile = IsFile,
                            order = i,
                            path = FileName,
                            count = fileCount,
                        });
                    }
                    else
                    {
                        if (FileName.Contains(keyword))
                        {
                            FileModelsList.Add(new FileModels()
                            {
                                createTime = createTime,
                                IsFile = IsFile,
                                order = i,
                                path = FileName,
                                count = fileCount,
                            });
                        }
                    }
                }

                FileModelsList = FileModelsList.OrderBy(f => f.order).ToList();
                using (DDContext context = new DDContext())
                {
                    //项目管理员
                    bool IsProjectControl = context.Roles.Where(r => r.UserId == userId && r.RoleName == "项目管理员" && r.IsEnable == true).ToList().Count() > 0 ? true : false;
                    //院领导
                    bool IsLeader = context.Roles.Where(r => r.UserId == userId && r.RoleName == "院领导" && r.IsEnable == true).ToList().Count() > 0 ? true : false;

                    if (IsProjectControl || IsLeader)
                    {
                        return new NewErrorModel()
                        {
                            data = FileModelsList,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }

                    int AppearCount = SubstringCount(path, "\\");
                    if (AppearCount < 6)
                    {
                        return new NewErrorModel()
                        {
                            data = FileModelsList,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                    string CheckPath = path;
                    //项目负责人
                    bool IsProjectLeader = context.ProjectInfo.Where(p => p.ResponsibleManId == userId && p.FilePath == path).ToList().Count() > 0 ? true : false;
                    //小组成员
                    bool IsGroupMember = context.ProjectInfo.Where(p => p.TeamMembersId.Contains(userId) && p.FilePath == path).ToList().Count() > 0 ? true : false;

                    if (AppearCount == 7)  //项目路径层级
                    {
                        if (IsProjectLeader || IsGroupMember)
                        {
                            return new NewErrorModel()
                            {
                                data = FileModelsList,
                                error = new Error(0, "读取成功！", "") { },
                            };
                        }
                        else
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "没有权限！", "") { },
                            };
                        }
                    }
                    else
                    {
                        if (CheckPath.Contains("合同") && IsGroupMember) //小组成员没有权限看合同
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "没有权限！", "") { },
                            };
                        }
                        return new NewErrorModel()
                        {
                            data = FileModelsList,
                            error = new Error(0, "读取成功！", "") { },
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
        /// 创建项目文件
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns></returns>
        [HttpGet]
        [Route("AddProjectFile")]
        public bool AddProjectFile(string path)
        {
            try
            {
                string pathOb = System.Web.HttpContext.Current.Server.MapPath(path);
                string[] ChildFileName = {  "立项书或建议书", "评审PPT", "需求规格说明书、产品总体设计书",
                "机械设计图纸","电气图纸","BOM表","软件源代码","使用说明书、操作手册、技术方案、规格说明书",
                "合作协议","产品（样机、成品）图片、影像","阶段性整理的问题的分析、解决方案及计划表","项目采购清单、借用清单、领料清单、入库清单",
                "受理知识产权清单及申请资料","纵向项目申请、中期检查、验收资料","其他过程文档",
                "项目终止情况报告","装箱单","客户验收单","转化、应用单位情况表","项目经费使用情况表"};
                FileHelper.CreateDirectory(pathOb);
                foreach (var item in ChildFileName)
                {
                    FileHelper.CreateDirectory(pathOb + (ChildFileName.ToList().IndexOf(item) + 1) + item);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 项目数据单条信息读取
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSingleProjectInfo")]
        public NewErrorModel GetSingleProjectInfo(string projectId)
        {
            DDContext dDContext = new DDContext();

            return new NewErrorModel()
            {
                data = dDContext.ProjectInfo.Where(p => p.ProjectId == projectId).FirstOrDefault(),
                error = new Error(1, "读取成功！", "") { },
            };
        }



        /// <summary>
        /// 批量生成项目文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CreatepProjectFiles")]

        public NewErrorModel CreatepProjectFiles()
        {
            DDContext dDContext = new DDContext();
            List<ProjectInfo> projectInfoList = dDContext.ProjectInfo.ToList();

            //矫正项目路径
            foreach (var item in projectInfoList)
            {
                item.FilePath = $"\\UploadFile\\ProjectFile\\{item.CompanyName}\\{item.ProjectType}\\{item.ProjectSmallType}\\{item.ProjectName}\\";
                dDContext.Entry<ProjectInfo>(item).State = System.Data.Entity.EntityState.Modified;
            }
            dDContext.SaveChanges();

            List<ProjectInfo> projectInfoListNew = dDContext.ProjectInfo.ToList();
            foreach (var item in projectInfoListNew)
            {
                AddProjectFile(item.FilePath);
            }
            return new NewErrorModel()
            {
                error = new Error(0, "创建成功！", "") { },
            };
        }


        /// <summary>
        /// 批量创建项目
        /// </summary>
        /// <param name="projectInfos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddProject")]
        public NewErrorModel AddProject(List<ProjectInfo> projectInfos)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    string ApplyManId = projectInfos[0].ApplyManId;
                    //项目管理员
                    bool IsProjectControl = context.Roles.Where(r => r.UserId == ApplyManId && r.RoleName == "项目管理员" && r.IsEnable == true).ToList().Count() > 0 ? true : false;
                    if (IsProjectControl)
                    {
                        foreach (var projectInfo in projectInfos)
                        {
                            if (projectInfo.ProjectName.Contains(" "))
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, "项目名不能包含空格！", "") { },
                                };
                            }
                            if (string.IsNullOrEmpty(projectInfo.CompanyName) || string.IsNullOrEmpty(projectInfo.ProjectType)
                                || string.IsNullOrEmpty(projectInfo.ProjectSmallType) || string.IsNullOrEmpty(projectInfo.ProjectName) || string.IsNullOrEmpty(projectInfo.ProjectId))
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, string.Format("项目数据不完整 {0}", projectInfo.ProjectId), "") { },
                                };
                            }

                            ProjectInfo pi = context.ProjectInfo.SingleOrDefault(u => u.ProjectId == projectInfo.ProjectId);
                            if (pi != null)
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, string.Format("已存在 项目编号{0}", pi.ProjectId), "") { },
                                };
                            }
                            else
                            {
                                //建立项目文件夹及其子文件
                                string path = string.Format("\\UploadFile\\ProjectFile\\{0}\\{1}\\{2}\\{3}\\",
                                    projectInfo.CompanyName, projectInfo.ProjectType, projectInfo.ProjectSmallType, projectInfo.ProjectName);
                                if (AddProjectFile(path))
                                {
                                    projectInfo.FilePath = path;
                                    context.ProjectInfo.Add(projectInfo);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    return new NewErrorModel()
                                    {
                                        error = new Error(1, string.Format("文件夹创建有误", pi.ProjectId), "") { },
                                    };
                                }
                            }
                        }
                        return new NewErrorModel()
                        {
                            error = new Error(0, string.Format("创建成功！,共计{0}个项目", projectInfos.Count), "") { },
                        };
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有操作权限！", "") { },
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
        /// 推送项目提醒
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("SendMsg")]
        public async Task<NewErrorModel> SendMsg(string projectId)
        {
            try
            {
                DDContext context = new DDContext();
                DingTalkServersController dingTalkServersController = new DingTalkServersController();
                ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == projectId).FirstOrDefault();
                await dingTalkServersController.SendProjectMsg(projectInfo.ResponsibleManId,
                    string.Format("亲，您的{0}项目即将在{1}到期，请尽快完成项目并申请结题。", projectInfo.ProjectName, projectInfo.EndTime), string.Format("eapp://page/approveDetail/projectDetail/projectDetail?projectId={0}", projectInfo.ProjectId));
                return new NewErrorModel()
                {
                    error = new Error(0, "通知成功！", "") { },
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 导出所有项目数据Excel
        /// </summary>
        /// <param name="applyManId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print")]
        public async Task<NewErrorModel> Print(string applyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    //项目管理员
                    bool IsProjectControl = context.Roles.Where(r => r.UserId == applyManId && r.RoleName == "项目管理员" && r.IsEnable == true).ToList().Count() > 0 ? true : false;
                    if (IsProjectControl)
                    {
                        List<ProjectInfo> projectInfos = context.ProjectInfo.ToList();
                        var Query = from p in projectInfos
                                    select new
                                    {
                                        p.ProjectName,
                                        p.ProjectType,
                                        p.ProjectSmallType,
                                        p.ProjectState,
                                        p.ApplyMan,
                                        p.ApplyManId,
                                        p.CompanyName,
                                        p.CreateTime,
                                        p.DeptName,
                                        p.StartTime,
                                        p.EndTime,
                                        p.ProjectId,
                                        p.ResponsibleMan,
                                        p.ResponsibleManId,
                                        p.FilePath,
                                        p.IsEnable,
                                        p.TeamMembers,
                                        p.TeamMembersId,
                                        p.CooperativeUnit
                                    };
                        DataTable dtReturn = new DataTable();
                        PropertyInfo[] oProps = null;
                        foreach (var rec in Query)
                        {
                            if (oProps == null)
                            {
                                oProps = ((Type)rec.GetType()).GetProperties();
                                foreach (PropertyInfo pi in oProps)
                                {
                                    Type colType = pi.PropertyType; if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                                    {
                                        colType = colType.GetGenericArguments()[0];
                                    }
                                    dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                                }
                            }
                            DataRow dr = dtReturn.NewRow(); foreach (PropertyInfo pi in oProps)
                            {
                                dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                            }
                            dtReturn.Rows.Add(dr);
                        }
                        string path = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet/项目数据统计模板.xlsx");
                        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string newPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Excel/Templet") + "\\项目数据统计" + time + ".xlsx";
                        System.IO.File.Copy(path, newPath);
                        if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtReturn, 0, 1))
                        {
                            DingTalkServersController dingTalkServersController = new DingTalkServersController();
                            //上盯盘
                            var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/项目数据统计" + time + ".xlsx");
                            //推送用户
                            FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia.ToString());
                            fileSendModel.UserId = applyManId;
                            var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                            return new NewErrorModel()
                            {
                                error = new Error(0, "已推送至钉钉", "") { },
                            };
                        }
                        else
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "Excel模板有误！", "") { },
                            };
                        }
                    }
                    else
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有操作权限！", "") { },
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
        /// 项目信息关键字查询
        /// </summary>
        /// <param name="applyManId"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="projectState"></param>
        /// <param name="projectType"></param>
        /// <param name="projectSmallType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("QuaryProjectInfo")]
        public NewErrorModel QuaryProjectInfo(string applyManId, string key = "", string startTime = "", string endTime = "", string projectState = "",
            string projectType = "", string projectSmallType = "")
        {
            try
            {
                List<ProjectInfo> ProjectInfoList = new List<ProjectInfo>();
                List<ProjectInfo> ProjectInfoListQuery = new List<ProjectInfo>();

                using (DDContext context = new DDContext())
                {
                    bool IsSa = context.Roles.Where(r => r.UserId == applyManId && r.RoleName == "超级管理员").ToList().Count > 0 ? true : false;

                    ProjectInfoList = context.ProjectInfo.ToList();
                    
                    if (IsSa)
                    {
                        foreach (var item in ProjectInfoList)
                        {
                            item.IsEdit = true;
                        }
                    }
                    else
                    {
                        foreach (var item in ProjectInfoList)
                        {
                            if (item.ResponsibleManId == applyManId)
                            {
                                item.IsEdit = true;
                            }
                            else
                            {
                                item.IsEdit = false;
                            }
                        }
                    }

                    if (key != "" && key != null)
                    {
                        ProjectInfoList = ProjectInfoList.Where(p =>
                          (!string.IsNullOrEmpty(p.ProjectName) ? p.ProjectName.Contains(key) : false) ||
                         (!string.IsNullOrEmpty(p.ProjectId) ? p.ProjectId.Contains(key) : false) ||
                         (!string.IsNullOrEmpty(p.DeptName) ? p.DeptName.Contains(key) : false) ||
                         (!string.IsNullOrEmpty(p.ResponsibleMan) ? p.ResponsibleMan.Contains(key) : false) ||
                          (!string.IsNullOrEmpty(p.TeamMembers) ? p.TeamMembers.Contains(key) : false)
                        ).ToList();
                    }
                    ProjectInfoList = ProjectInfoList.Where(p => startTime == "" ? 1 == 1 : (DateTime.Parse(startTime) <= DateTime.Parse(p.StartTime)) &&
                    endTime == "" ? 1 == 1 : (DateTime.Parse(endTime) >= DateTime.Parse(p.EndTime))).ToList();
                    ProjectInfoList = ProjectInfoList.Where(p => (projectType == "" ? 1 == 1 : p.ProjectType == projectType)
                    && (projectSmallType == "" ? 1 == 1 : p.ProjectSmallType == projectSmallType)).ToList();

                    if (projectState != "")
                    {
                        foreach (var item in projectState.Split('_'))
                        {
                            foreach (var pitem in ProjectInfoList)
                            {
                                if (pitem.ProjectState == item)
                                {
                                    ProjectInfoListQuery.Add(pitem);
                                }
                            }
                        }


                        return new NewErrorModel()
                        {
                            count = ProjectInfoListQuery.Count,
                            data = ProjectInfoListQuery.OrderByDescending(t => t.ProjectId.Substring(0, 4)).ThenByDescending(t => t.ProjectId.Substring(t.ProjectId.Length - 3, 3)),
                            error = new Error(0, "查询成功！", "") { },
                        };
                    }

                }
                return new NewErrorModel()
                {
                    count = ProjectInfoList.Count,
                    data = ProjectInfoList.OrderByDescending(t => t.ProjectId.Substring(0, 4)).ThenByDescending(t => t.ProjectId.Substring(t.ProjectId.Length - 3, 3)),
                    error = new Error(0, "查询成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 项目文件修改
        /// </summary>
        /// <param name="path">当前路径</param>
        /// <param name="MovePath">修改文件名时的新路径</param>
        /// <param name="ApplyMan">用户名</param>
        /// <param name="ApplyManId">用户Id</param>
        /// <param name="ChangeType">修改类型( 0:新建  1:删除  2:修改(需要多传一个MovePath参数))</param>
        /// <param name="MediaId">盯盘唯一Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChangeFile")]
        public NewErrorModel ChangeFile(string path, string MovePath, string ApplyMan, string ApplyManId, int ChangeType, string MediaId)
        {
            try
            {
                //判断是否是文件
                bool IsFile = Path.GetFileName(path).Contains(".");
                using (DDContext context = new DDContext())
                {
                    FileInfos fileInfos = new FileInfos()
                    {
                        ApplyMan = ApplyMan,
                        ApplyManId = ApplyManId,
                        FilePath = path,
                        LastModifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:hh:ss"),
                        LastModifyState = ChangeType.ToString()
                    };
                    //判断权限
                    bool IsSuperPower = (context.Roles.Where(r => r.UserId == ApplyManId && r.RoleName == "项目管理员").ToList().Count() >= 1) ? true : false;
                    string RePath = path;
                    path = System.Web.Hosting.HostingEnvironment.MapPath(path);
                    if (IsSuperPower)
                    {
                        switch (ChangeType)
                        {
                            case 0:
                                if (IsFile)
                                {
                                    System.IO.File.Create(path);
                                }
                                else
                                {
                                    Directory.CreateDirectory(path);
                                }
                                context.FileInfos.Add(fileInfos);
                                context.SaveChanges();
                                break;
                            case 1:
                                if (IsFile)
                                {
                                    System.IO.File.Delete(path);
                                }
                                else
                                {
                                    Directory.Delete(path);
                                }
                                var f = context.FileInfos.Where(u => u.FilePath == RePath).FirstOrDefault();
                                context.FileInfos.Remove(f);
                                context.SaveChanges();
                                break;
                            case 2:
                                Directory.Move(path, System.Web.Hosting.HostingEnvironment.MapPath(MovePath));
                                var fs = context.FileInfos.Where(u => u.FilePath == RePath).FirstOrDefault();
                                context.FileInfos.Remove(fs);
                                context.SaveChanges();
                                context.FileInfos.Add(fileInfos);
                                context.SaveChanges();
                                break;
                        }

                        return new NewErrorModel()
                        {
                            error = new Error(0, "操作成功", "") { },
                        };
                    }
                    else
                    {
                        //检测路径查询权限
                        int k = GetIndexOfString(RePath, "\\", 7);
                        string CheckPath = RePath.Substring(0, k - 1);
                        bool IsComPower = (context.ProjectInfo.Where(p => p.ResponsibleManId == ApplyManId && p.FilePath == CheckPath).ToList().Count() >= 1) ? true : false;
                        if (IsComPower)
                        {
                            switch (ChangeType)
                            {
                                case 0:
                                    if (IsFile)
                                    {
                                        System.IO.File.Create(path);
                                    }
                                    else
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                    context.FileInfos.Add(fileInfos);
                                    context.SaveChanges();
                                    break;
                                case 1:
                                    if (IsFile)
                                    {
                                        System.IO.File.Delete(path);
                                    }
                                    else
                                    {
                                        Directory.Delete(path);
                                    }
                                    var f = context.FileInfos.Where(u => u.FilePath == RePath).FirstOrDefault();
                                    context.FileInfos.Remove(f);
                                    context.SaveChanges();
                                    break;
                                case 2:
                                    Directory.Move(path, System.Web.Hosting.HostingEnvironment.MapPath(MovePath));
                                    var fs = context.FileInfos.Where(u => u.FilePath == RePath).FirstOrDefault();
                                    context.FileInfos.Remove(fs);
                                    context.SaveChanges();
                                    context.FileInfos.Add(fileInfos);
                                    context.SaveChanges();
                                    break;
                            }

                            return new NewErrorModel()
                            {
                                error = new Error(0, "操作成功", "") { },
                            };
                        }
                        else
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "用户没有权限进行操作", "") { },
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 项目信息修改
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ProjectInfoModify")]
        public NewErrorModel ProjectInfoModify(ProjectInfo projectInfo)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    ProjectInfo projectInfoQuery = context.ProjectInfo.AsNoTracking().Where(p => p.ProjectId == projectInfo.ProjectId).FirstOrDefault();
                    if (projectInfoQuery.ProjectName != projectInfo.ProjectName)
                    {
                        //修改项目路径
                        projectInfo.FilePath = projectInfo.FilePath.Replace(projectInfoQuery.ProjectName, projectInfo.ProjectName);
                        System.IO.Directory.Move(HttpContext.Current.Server.MapPath(projectInfoQuery.FilePath), HttpContext.Current.Server.MapPath(projectInfo.FilePath));
                        //projectInfoQuery = projectInfo;
                    }

                    context.Entry<ProjectInfo>(projectInfo).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// 计算字符串中子串出现的次数
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="substring">子串</param>
        /// <returns>出现的次数</returns>
        public int SubstringCount(string str, string substring)
        {
            if (str.Contains(substring))
            {
                string strReplaced = str.Replace(substring, "");
                return (str.Length - strReplaced.Length) / substring.Length;
            }
            return 0;
        }


        /// <summary>
        /// 文件下载接口(盯盘推送文件)
        /// </summary>
        /// <param name="downloadFileModel">下载文件实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("DownloadFileModel")]
        public NewErrorModel DownloadFileModel(DownloadFileModel downloadFileModel)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    string url = string.Empty;
                    if (ConfigurationManager.AppSettings["hao"].ToString() == "2")
                    {
                        url = "17e245o364.imwork.net:49415";
                    }
                    else
                    {
                        url = System.Web.HttpContext.Current.Request.Url.Authority;
                    }

                    //生成下载链接
                    string downLoadLink = string.Format("{0}/ProjectNew/DownLoad?path=~/{1}", url, downloadFileModel.path);
                    //推送盯盘下载链接

                    return new NewErrorModel()
                    {
                        data = downLoadLink,
                        error = new Error(0, "请复制链接到浏览器中下载！", "1") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DownLoad")]
        public async Task<HttpResponseMessage> DownLoad(string path)
        {
            try
            {
                string filename = Path.GetFileName(path);
                string pathOb = System.Web.Hosting.HostingEnvironment.MapPath(path);
                var stream = new FileStream(pathOb, FileMode.Open);
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(stream)
                };
                resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = filename
                };
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                resp.Content.Headers.ContentLength = stream.Length;
                return await Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// 发送普通消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SentCommonMsg")]
        public string SentCommonMsg(string SendPeoPleId, string msg)
        {
            TopSDKTest top = new TopSDKTest();
            OATextModel oaTextModel = new OATextModel();

            oaTextModel.head = new head
            {
                //bgcolor = "FFBBBBBB",
                //text = "您有一条待审批的流程，请登入OA系统审批"
            };
            oaTextModel.body = new body
            {
                form = new form[] {
                    new form{ key="下载链接：",value=msg},
                    //new form{ key="申请时间：",value=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                },
                //title = Title,//"您有一条待审批的流程，请登入OA系统审批",
                //content = Content//"我要请假~~~~123456",
                //image = "@lADOADmaWMzazQKA",
                //file_count = "3",
            };
            //oaTextModel.message_url = Url;
            return top.SendOaMessage(SendPeoPleId, oaTextModel);
        }






        /// <summary>
        /// 返回第n次字符出现的索引
        /// </summary>
        /// <param name="InputString"></param>
        /// <param name="CharString"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GetIndexOfString(string InputString, string CharString, int n)
        {
            int count = 0;
            int k = 0;
            for (int i = 0; i < n; i++)
            {
                int j = InputString.IndexOf(CharString);
                InputString = InputString.Substring(j + 1, InputString.Length - j - 1);
                count++;
                k = k + j + 1;
                if (count == n)
                {
                    return k;
                }
            }
            return 0;
        }
    }


    public class FileModels
    {
        /// <summary>
        /// 文件创建时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 是否是文件
        /// </summary>
        public bool IsFile { get; set; }
        public string path { get; set; }
        public int count { get; set; }
        public int order { get; set; }

    }

    public class DownloadFileModel
    {
        public string path { get; set; }
        public string userId { get; set; }
    }

}
