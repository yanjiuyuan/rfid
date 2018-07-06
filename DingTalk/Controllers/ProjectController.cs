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
        //var ProjectTest = {
        //"ProjectName": "集成钉钉的信息管理系统",
        //"CreateTime": "2018-04-20 14:40",
        //"IsEnable":true,
        //"IsFinish": false,
        //"DeptName": "智慧工厂事业部",
        //"ApplyMan": "蔡兴桐",
        //"ApplyManId":"99f00dfc7badd72b00da35f211060176ae044d8b3b420106bb6ef6345be1ba9b",
        //"StartTime":"2017-10-23",
        //"EndTime":"2018-09-01",
        //"ProjectNo":"2017ZL054"
        //}
        [HttpPost]
        public string AddProject()
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
                            string path = string.Format("\\UploadFile\\ProjectFile\\{0}",
                                projectInfo.ProjectName);
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
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 3,
                    errorMessage = ex.Message
                });
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
                        listProjectInfo = context.ProjectInfo.ToList();
                    }
                    else
                    {
                        listProjectInfo = context.ProjectInfo.Where(u => u.ApplyManId == ApplyManId).ToList();
                    }
                    return JsonConvert.SerializeObject(listProjectInfo);
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 2,
                    errorMessage = ex.Message
                });
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
            DownloadFile(string.Format("{0}.pdf", DateTime.Now.ToString("yyyyMMdd hh:mm:ss")), string.Format(@"{0}\UploadFile\PDF\321.PDF", AppDomain.CurrentDomain.BaseDirectory));
        }

        public void DownloadFile(string flieName, string filePath)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            if (fileInfo.Exists == true)
            {
                const long ChunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力
                byte[] buffer = new byte[ChunkSize];
                Response.Clear();
                System.IO.FileStream iStream = System.IO.File.OpenRead(filePath);
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(flieName));
                while (dataLengthToRead > 0 && Response.IsClientConnected)
                {
                    int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小
                    Response.OutputStream.Write(buffer, 0, lengthRead);
                    Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                Response.Close();
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
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                });
            }

        }

        /// <summary>
        /// 获取目录下的文件夹信息
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns>返回文件名数组</returns>
        /// 测试数据：/Project/GetFileMsg?Path=\UploadFile\ProjectFile\宝发
        [HttpGet]
        public string GetFileMsg(string path)
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
                return JsonConvert.SerializeObject(RePathList);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// 获取参数路径下的所有文件信息
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        /// 测试数据：/Project/GetAllFilePath?Path=\UploadFile\ProjectFile\宝发
        //[HttpGet]
        //public string GetAllFilePath(string path)
        //{
        //    try
        //    {
        //        string[] AbPathList = FileHelper.GetDirectories(Server.MapPath(path));
        //        List<string> RePathList = new List<string>();
        //        foreach (var item in AbPathList)
        //        {
        //            //绝对路径转相对
        //            RePathList.Add(FileHelper.RelativePath(AppDomain.CurrentDomain.BaseDirectory, item));
        //        }
        //        return JsonConvert.SerializeObject(RePathList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonConvert.SerializeObject(new ErrorModel
        //        {
        //            errorCode = 1,
        //            errorMessage = ex.Message
        //        });
        //    }

        //}

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
                    bool IsSuperPower = (context.Roles.Where(r => r.UserId == ApplyManId && r.RoleName == "超级管理员").ToList().Count() >= 1) ? true : false;
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
                        bool IsComPower = (context.ProjectInfo.Where(p => p.ApplyManId == ApplyManId).ToList().Count() >= 1) ? true : false;
                        if (IsComPower)
                        {
                            switch (ChangeType)
                            {
                                case 0:
                                    FileHelper.CreateDirectory(path);
                                    break;
                                case 1:
                                    FileHelper.DeleteDirectory(path);
                                    break;
                                case 2:
                                    FileHelper.Move(path, Server.MapPath(MovePath));
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
                                errorCode = 0,
                                errorMessage = "用户没有权限进行操作"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ErrorModel
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// 根据路径上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns>返回文件路径</returns>
        /// 测试数据：/Project/Save
        [HttpPost]
        public static string Save(HttpPostedFileBase file, string path)
        {
            var phicyPath = HostingEnvironment.MapPath(path);
            Directory.CreateDirectory(phicyPath);
            var fileName = DateTime.Now.ToString();
            file.SaveAs(phicyPath + fileName);
            return fileName;
        }

    }
}