using Common.Flie;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.IO;
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
                return new NewErrorModel()
                {
                    error = new Error(2, ex.Message, "") { },
                };
            }
        }


        /// <summary>
        /// 获取目录下的文件夹信息
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <param name="userId">用户Id</param>
        /// <returns>返回文件名数组</returns>
        [Route("GetFileMsg")]
        [HttpGet]
        public NewErrorModel GetFileMsg(string path, string userId)
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

                    //绝对路径转相对
                    string RelativePath = FileHelper.RelativePath(AppDomain.CurrentDomain.BaseDirectory, item);
                    string FileName = Path.GetFileName(RelativePath);
                    //RePathList.Add(FileName);
                    FileModelsList.Add(new FileModels()
                    {
                        path = FileName,
                        count = fileCount,
                    });
                }

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
                    if (AppearCount < 5)
                    {
                        return new NewErrorModel()
                        {
                            data = FileModelsList,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }
                    string CheckPath = path;
                    if (AppearCount > 5)
                    {
                        int k = GetIndexOfString(path, "\\", 6);
                        path = path.Substring(0, k - 1);
                    }
                    //项目负责人
                    bool IsProjectLeader = context.ProjectInfo.Where(p => p.ResponsibleManId == userId && p.FilePath == path).ToList().Count() > 0 ? true : false;
                    //小组成员
                    bool IsGroupMember = context.ProjectInfo.Where(p => p.TeamMembersId.Contains(userId) && p.FilePath == path).ToList().Count() > 0 ? true : false;

                    if (AppearCount == 5)  //项目路径层级
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
                return new NewErrorModel()
                {
                    error = new Error(2, ex.Message, "") { },
                };
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
        public string path { get; set; }
        public int count { get; set; }
    }

}
