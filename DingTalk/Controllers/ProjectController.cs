using Common.Flie;
using Common.JsonHelper;
using Common.PDF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalkServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace DingTalk.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 项目信息添加接口
        /// </summary>
        /// <returns></returns>
        /// 测试数据：/Project/AddProject
        //var ProjectTest = { ProjectName":"集成钉钉的信息管理系统","CreateTime":"2018-04-20 14:40","IsEnable":true,"ProjectState":"在研","DeptName":"智慧工厂事业部","ApplyMan":"蔡兴桐","ApplyManId":"073110326032521796","StartTime":"2017-10-23","EndTime":"2018-09-01","ProjectId":"2017ZL054","FilePath":"项目路径","ResponsibleMan":"负责人","ResponsibleManId":"负责人Id"}
        [HttpPost]
        public string AddProject(bool IsPower = false)
        {
            try
            {

                StreamReader sr = new StreamReader(Request.InputStream);
                var stream = sr.ReadToEnd();
                if (string.IsNullOrEmpty(stream))
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "提交的数据不能为空！"
                    });
                }
                else
                {
                    ProjectInfo projectInfo = JsonHelper.JsonToObject<ProjectInfo>(stream);
                    using (DDContext context = new DDContext())
                    {
                        if (IsPower == false)
                        {
                            //项目管理员
                            bool IsProjectControl = context.Roles.Where(r => r.UserId == projectInfo.ApplyManId && r.RoleName == "项目管理员" && r.IsEnable == true).ToList().Count() > 0 ? true : false;
                            if (IsProjectControl)
                            {
                                ProjectInfo pInfo = context.ProjectInfo.SingleOrDefault(u => u.ProjectId == projectInfo.ProjectId);

                                if (pInfo != null)
                                {
                                    return JsonConvert.SerializeObject(new ErrorModel
                                    {
                                        errorCode = 2,
                                        errorMessage = string.Format("已存在 项目编号{0}", pInfo.ProjectId)
                                    });
                                }
                                else
                                {
                                    //建立项目文件夹及其子文件
                                    string path = string.Format("\\UploadFile\\ProjectFile\\{0}\\{1}\\{2}",
                                        projectInfo.CompanyName, projectInfo.ProjectType, projectInfo.ProjectName);
                                    projectInfo.FilePath = path;
                                    context.ProjectInfo.Add(projectInfo);
                                    path = Server.MapPath(path);
                                    FileHelper.CreateDirectory(path);
                                    FileHelper.CreateDirectory(path + "\\1需求分析");
                                    FileHelper.CreateDirectory(path + "\\2进度计划");
                                    FileHelper.CreateDirectory(path + "\\3立项书");
                                    FileHelper.CreateDirectory(path + "\\4方案设计");
                                    FileHelper.CreateDirectory(path + "\\5机械图纸");
                                    FileHelper.CreateDirectory(path + "\\6电气图纸");
                                    FileHelper.CreateDirectory(path + "\\7采购单");
                                    FileHelper.CreateDirectory(path + "\\8源代码");
                                    FileHelper.CreateDirectory(path + "\\9中试");
                                    FileHelper.CreateDirectory(path + "\\10验收报告");
                                    FileHelper.CreateDirectory(path + "\\11使用说明书");
                                    FileHelper.CreateDirectory(path + "\\12协议书");
                                    FileHelper.CreateDirectory(path + "\\13合同");
                                    FileHelper.CreateDirectory(path + "\\14验收资料");
                                    FileHelper.CreateDirectory(path + "\\15其他资料");
                                    context.SaveChanges();
                                    return JsonConvert.SerializeObject(new ErrorModel
                                    {
                                        errorCode = 0,
                                        errorMessage = "创建成功！"
                                    });
                                }
                            }
                            else
                            {
                                //建立项目文件夹及其子文件
                                string path = string.Format("\\UploadFile\\ProjectFile\\{0}\\{1}\\{2}",
                                    projectInfo.CompanyName, projectInfo.ProjectType, projectInfo.ProjectName);
                                projectInfo.FilePath = path;
                                context.ProjectInfo.Add(projectInfo);
                                path = Server.MapPath(path);
                                FileHelper.CreateDirectory(path);
                                FileHelper.CreateDirectory(path + "\\1需求分析");
                                FileHelper.CreateDirectory(path + "\\2进度计划");
                                FileHelper.CreateDirectory(path + "\\3立项书");
                                FileHelper.CreateDirectory(path + "\\4方案设计");
                                FileHelper.CreateDirectory(path + "\\5机械图纸");
                                FileHelper.CreateDirectory(path + "\\6电气图纸");
                                FileHelper.CreateDirectory(path + "\\7采购单");
                                FileHelper.CreateDirectory(path + "\\8源代码");
                                FileHelper.CreateDirectory(path + "\\9中试");
                                FileHelper.CreateDirectory(path + "\\10验收报告");
                                FileHelper.CreateDirectory(path + "\\11使用说明书");
                                FileHelper.CreateDirectory(path + "\\12协议书");
                                FileHelper.CreateDirectory(path + "\\13合同");
                                FileHelper.CreateDirectory(path + "\\14验收资料");
                                FileHelper.CreateDirectory(path + "\\15其他资料");
                                context.SaveChanges();
                                return JsonConvert.SerializeObject(new ErrorModel
                                {
                                    errorCode = 0,
                                    errorMessage = "创建成功！"
                                });
                            }
                        }
                        else
                        {
                            //建立项目文件夹及其子文件
                            string path = string.Format("\\UploadFile\\ProjectFile\\{0}\\{1}\\{2}",
                                projectInfo.CompanyName, projectInfo.ProjectType, projectInfo.ProjectName);
                            projectInfo.FilePath = path;
                            context.ProjectInfo.Add(projectInfo);
                            path = Server.MapPath(path);
                            FileHelper.CreateDirectory(path);
                            FileHelper.CreateDirectory(path + "\\1需求分析");
                            FileHelper.CreateDirectory(path + "\\2进度计划");
                            FileHelper.CreateDirectory(path + "\\3立项书");
                            FileHelper.CreateDirectory(path + "\\4方案设计");
                            FileHelper.CreateDirectory(path + "\\5机械图纸");
                            FileHelper.CreateDirectory(path + "\\6电气图纸");
                            FileHelper.CreateDirectory(path + "\\7采购单");
                            FileHelper.CreateDirectory(path + "\\8源代码");
                            FileHelper.CreateDirectory(path + "\\9中试");
                            FileHelper.CreateDirectory(path + "\\10验收报告");
                            FileHelper.CreateDirectory(path + "\\11使用说明书");
                            FileHelper.CreateDirectory(path + "\\12协议书");
                            FileHelper.CreateDirectory(path + "\\13合同");
                            FileHelper.CreateDirectory(path + "\\14验收资料");
                            FileHelper.CreateDirectory(path + "\\15其他资料");
                            context.SaveChanges();
                            return JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 0,
                                errorMessage = "创建成功！"
                            });
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
        /// 项目信息读取接口
        /// </summary>
        /// <param name="ApplyManId">创建者Id(不传时默认查所有项目信息)</param>
        /// <returns></returns>
        /// 测试数据：/Project/GetAllProJect?ApplyManId=0935455445756597
        public string GetAllProJect(string ApplyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<ProjectInfo> listProjectInfo = new List<ProjectInfo>();
                    if (string.IsNullOrEmpty(ApplyManId))
                    {
                        listProjectInfo = context.ProjectInfo.Where(p => p.ProjectState == "在研").ToList();
                    }
                    else
                    {
                        listProjectInfo = context.ProjectInfo.Where(u => u.ApplyManId == ApplyManId && u.ProjectState == "在研").ToList();
                    }
                    return JsonConvert.SerializeObject(listProjectInfo);
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
        /// 测试数据 /Project/PDFReadTest
        public void PDFReadTest()
        {
            PDFHelper pdfHelper = new PDFHelper();
            pdfHelper.PDFWatermark(string.Format(@"{0}\UploadFile\PDF\123.PDF", AppDomain.CurrentDomain.BaseDirectory),
                string.Format(@"{0}\UploadFile\PDF\321.PDF", AppDomain.CurrentDomain.BaseDirectory),
                string.Format(@"{0}\Content\images\受控章.png", AppDomain.CurrentDomain.BaseDirectory),
                100, 100
            );
            //DownloadFile(string.Format("{0}.pdf", DateTime.Now.ToString("yyyyMMdd hh:mm:ss")), string.Format(@"{0}\UploadFile\PDF\321.PDF", AppDomain.CurrentDomain.BaseDirectory));
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="flieName">文件名</param>
        /// <param name="filePath">文件路径</param>
        /// 测试数据： /Project/DownloadFile?flieName=123&filePath=~\UploadFile\PDF\123.PDF
        [HttpPost]
        public string DownloadFile(FileBase64 fileBase64)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Server.MapPath(fileBase64.FilePath));
            if (fileInfo.Exists == true)
            {
                //const long ChunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力
                //byte[] buffer = new byte[ChunkSize];
                //Response.Clear();
                //System.IO.FileStream iStream = System.IO.File.OpenRead(Server.MapPath(filePath));
                //long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                //Response.ContentType = "application/octet-stream";
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(flieName));
                //while (dataLengthToRead > 0 && Response.IsClientConnected)
                //{
                //    int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小
                //    Response.OutputStream.Write(buffer, 0, lengthRead);
                //    Response.Flush();
                //    dataLengthToRead = dataLengthToRead - lengthRead;
                //}
                //Response.Close();
                FileStream filestream = new FileStream(Server.MapPath(fileBase64.FilePath), FileMode.Open);
                byte[] bt = new byte[filestream.Length];

                //调用read读取方法
                filestream.Read(bt, 0, bt.Length);
                string base64Str = Convert.ToBase64String(bt);
                filestream.Close();
                return base64Str;

                //return new NewErrorModel()
                //{
                //    data = "data:application/pdf;base64," + base64Str,
                //    error = new Error(0, "下载成功！", "") { },
                //};
            }
            else
            {
                return null;
            }

        }


        /// <summary>
        /// 获取项目目录下的文件夹信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetProjectFileMsg()
        {
            try
            {
                string Path = string.Format(@"{0}UploadFile\ProjectFile", AppDomain.CurrentDomain.BaseDirectory);
                return JsonConvert.SerializeObject(FileHelper.GetFileNames(Path));
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
        /// <returns>返回文件名数组</returns>
        /// 测试数据：/Project/GetFileMsg?Path=\UploadFile\ProjectFile\宝发
        [HttpGet]
        public string GetFileMsg(string path, string userId)
        {
            try
            {
                string[] AbPathList = FileHelper.GetFileNames(Server.MapPath(path));
                List<string> RePathList = new List<string>();
                foreach (var item in AbPathList)
                {
                    //绝对路径转相对
                    string RelativePath = FileHelper.RelativePath(AppDomain.CurrentDomain.BaseDirectory, item);
                    string FileName = Path.GetFileName(RelativePath);
                    RePathList.Add(FileName);
                }

                using (DDContext context = new DDContext())
                {
                    //项目管理员
                    bool IsProjectControl = context.Roles.Where(r => r.UserId == userId && r.RoleName == "项目管理员" && r.IsEnable == true).ToList().Count() > 0 ? true : false;
                    //院领导
                    bool IsLeader = context.Roles.Where(r => r.UserId == userId && r.RoleName == "院领导" && r.IsEnable == true).ToList().Count() > 0 ? true : false;

                    if (IsProjectControl || IsLeader)
                    {
                        return JsonConvert.SerializeObject(RePathList);
                    }
                    int AppearCount = SubstringCount(path, "\\");
                    if (AppearCount < 5)
                    {
                        return JsonConvert.SerializeObject(RePathList);
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
                            return JsonConvert.SerializeObject(RePathList);
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(new ErrorModel()
                            {
                                errorCode = 0,
                                errorMessage = "没有权限"
                            });
                        }
                    }
                    else
                    {
                        if (CheckPath.Contains("合同") && IsGroupMember) //小组成员没有权限看合同
                        {
                            return JsonConvert.SerializeObject(new ErrorModel()
                            {
                                errorCode = 0,
                                errorMessage = "没有权限"
                            });
                        }
                        return JsonConvert.SerializeObject(RePathList);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 修改项目文件
        /// </summary>
        /// <param name="Path">当前路径</param>
        /// <param name="MovePath">修改文件名时的新路径</param>
        /// <param name="ApplyMan">用户名</param>
        /// <param name="ApplyManId">用户Id</param>
        /// <param name="ChangeType">修改类型( 0:新建  1:删除  2:修改(需要多传一个MovePath参数) )</param>
        /// <param name="MediaId">盯盘唯一Id</param>
        /// <returns></returns>
        /// 测试数据：/Project/ChangeFile?Path=\UploadFile\ProjectFile\白金刚\123&MovePath=\UploadFile\ProjectFile\白金刚\321&ApplyManId=manager325&ApplyMan=黄浩伟&ChangeType=2

        [HttpGet]
        public string ChangeFile(string path, string MovePath, string ApplyMan, string ApplyManId, int ChangeType, string MediaId)
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
                    path = Server.MapPath(path);
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
                                Directory.Move(path, Server.MapPath(MovePath));
                                var fs = context.FileInfos.Where(u => u.FilePath == RePath).FirstOrDefault();
                                context.FileInfos.Remove(fs);
                                context.SaveChanges();
                                context.FileInfos.Add(fileInfos);
                                context.SaveChanges();
                                break;
                        }

                        return JsonConvert.SerializeObject(new ErrorModel
                        {
                            errorCode = 0,
                            errorMessage = "操作成功"
                        });
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
                                    Directory.Move(path, Server.MapPath(MovePath));
                                    var fs = context.FileInfos.Where(u => u.FilePath == RePath).FirstOrDefault();
                                    context.FileInfos.Remove(fs);
                                    context.SaveChanges();
                                    context.FileInfos.Add(fileInfos);
                                    context.SaveChanges();
                                    break;
                            }
                            return JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 0,
                                errorMessage = "操作成功"
                            });
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(new ErrorModel
                            {
                                errorCode = 1,
                                errorMessage = "用户没有权限进行操作"
                            });
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
        /// 文件下载接口(盯盘推送文件)
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        /// 测试数据: /Project/DownloadFileModel/
        /// data： {path:"~/测试媒体文件/测试文本123.txt",userId:"083452125733424957"}
        [HttpPost]
        public async Task<string> DownloadFileModel(string path, string userId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    //查找MediaId
                    string mediaId = context.FileInfos.Where(f => f.FilePath == path).First().MediaId;
                    if (string.IsNullOrEmpty(mediaId))
                    {
                        return JsonConvert.SerializeObject(new ErrorModel
                        {
                            errorCode = 0,
                            errorMessage = "未找到该文件信息"
                        });
                    }
                    else
                    {
                        var DingTaklServer = DependencyResolver.Current.GetService<DingTalkServersController>();
                        FileSendModel fileSendModel = new FileSendModel()
                        {
                            Media_Id = mediaId,
                            UserId = userId
                        };
                        string result = await DingTaklServer.SendFileMessage(fileSendModel);
                        return result;
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
        /// 测试数据：  /Project/ProjectInfoModify
        ///  data: {"Id":10039.0,"ProjectName":"DingTalk","CreateTime":"2018-07-10 16:20","IsEnable":true,"ProjectState":"在研","DeptName":"智慧工厂事业部","ApplyMan":"蔡兴桐","ApplyManId":"083452125733424957","StartTime":"2018-07-12","EndTime":"2018-07-13","ProjectId":"12333","FilePath":"\\UploadFile\\ProjectFile\\泉州华中科技大学智能制造研究院\\纵向项目\\DingTalk","ResponsibleMan":"蔡兴桐","ResponsibleManId":"083452125733424957","CompanyName":"泉州华中科技大学智能制造研究院","ProjectType":"纵向项目","TeamMembers":"张鹏辉,肖民生,詹姆斯,黄龙贤","TeamMembersId":"100328051024695354,073110326032521796,manager325,020821466340361583","CreateMan":null,"CreateManId":null}
        [HttpPost]
        public string ProjectInfoModify(ProjectInfo projectInfo)
        {
            try
            {
                if (projectInfo != null)
                {
                    using (DDContext context = new DDContext())
                    {
                        context.Entry<ProjectInfo>(projectInfo).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 0,
                        errorMessage = "修改成功"
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new ErrorModel
                    {
                        errorCode = 1,
                        errorMessage = "参数未传递"
                    });
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
        /// <param name="key">关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="projectState">项目状态</param>
        /// <returns></returns>
        [HttpGet]
        public string QuaryProjectInfo(string key, string startTime, string endTime, string projectState)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<ProjectInfo> ProjectInfoList = new List<ProjectInfo>();
                    if (string.IsNullOrEmpty(key))
                    {
                        ProjectInfoList = context.ProjectInfo.OrderBy(p => (p.ProjectId.Substring(0, 4) + p.ProjectId.Substring(p.ProjectId.Length - 3, 3))).ToList();
                    }
                    else
                    {
                        ProjectInfoList = context.ProjectInfo.Where(p => p.ApplyMan.Contains(key) ||
                      p.DeptName.Contains(key) || p.ProjectName.Contains(key) ||
                      p.ProjectId.Contains(key) || p.FilePath.Contains(key)).OrderBy(p => (p.ProjectId.Substring(0, 4) + p.ProjectId.Substring(p.ProjectId.Length - 3, 3))).ToList();
                    }


                    if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                    {

                        if (string.IsNullOrEmpty(projectState))
                        {
                            return JsonConvert.SerializeObject(ProjectInfoList);
                        }
                        else
                        {
                            string[] projectStateList = projectState.Split('_');

                            List<ProjectInfo> pro = new List<ProjectInfo>();
                            foreach (ProjectInfo projectInfo in ProjectInfoList)
                            {
                                foreach (string item in projectStateList)
                                {
                                    if (projectInfo.ProjectState == item)
                                    {
                                        pro.Add(projectInfo);
                                    }
                                }
                            }
                            return JsonConvert.SerializeObject(pro);
                        }
                    }
                    else
                    {
                        //var Quary =
                        //    from p in ProjectInfoList
                        //    where
                        //   (Convert.ToDateTime(startTime) >= Convert.ToDateTime(p.StartTime))
                        //    && (Convert.ToDateTime(endTime) <= Convert.ToDateTime(p.EndTime))
                        //    select p;

                        var Quary = ProjectInfoList.Where(p => (Convert.ToDateTime(p.StartTime) >= Convert.ToDateTime(startTime))
                                          && (Convert.ToDateTime(p.StartTime) <= Convert.ToDateTime(endTime)));

                        if (string.IsNullOrEmpty(projectState))
                        {
                            return JsonConvert.SerializeObject(Quary);
                        }
                        else
                        {
                            string[] projectStateList = projectState.Split('_');

                            List<ProjectInfo> pro = new List<ProjectInfo>();
                            foreach (ProjectInfo projectInfo in Quary)
                            {
                                foreach (string item in projectStateList)
                                {
                                    if (projectInfo.ProjectState == item)
                                    {
                                        pro.Add(projectInfo);
                                    }
                                }
                            }
                            return JsonConvert.SerializeObject(pro);
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

    public class FileBase64
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}