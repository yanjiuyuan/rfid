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
                            context.ProjectInfo.Add(projectInfo);
                            context.SaveChanges();

                            //建立项目文件夹及其子文件
                            string Path = string.Format("{0}UploadFile\\ProjectFile\\{1}",
                                AppDomain.CurrentDomain.BaseDirectory, projectInfo.ProjectName);
                            FileHelper.CreateDirectory(Path);
                            FileHelper.CreateDirectory(Path + "\\1需求分析");
                            FileHelper.CreateDirectory(Path + "\\2进度计划");
                            FileHelper.CreateDirectory(Path + "\\3立项书");
                            FileHelper.CreateDirectory(Path + "\\4方案设计");
                            FileHelper.CreateDirectory(Path + "\\5机械图纸");
                            FileHelper.CreateDirectory(Path + "\\6电气图纸");
                            FileHelper.CreateDirectory(Path + "\\7采购单");
                            FileHelper.CreateDirectory(Path + "\\8源代码");
                            FileHelper.CreateDirectory(Path + "\\9中试");
                            FileHelper.CreateDirectory(Path + "\\10验收报告");
                            FileHelper.CreateDirectory(Path + "\\11使用说明书");
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
        /// 获取项目目录下的文件信息
        /// </summary>
        /// <param name="Path">绝对路径</param>
        /// <returns></returns>
        /// 测试数据：/Project/GetFileMsg?Path=E:\Project\DingTalk\DingTalk\UploadFile\ProjectFile
        [HttpGet]
        public string GetFileMsg(string Path)
        {
            //string Path = string.Format(@"{0}UploadFile\Excel", AppDomain.CurrentDomain.BaseDirectory);           
            return JsonConvert.SerializeObject(FileHelper.GetFileNames(Path));
        }

        /// <summary>
        /// 获取参数路径下的所有文件信息
        /// </summary>
        /// <param name="Path">路径</param>
        /// <returns></returns>
        /// 测试数据：/Project/GetAllFilePath?Path=E:\Project\DingTalk\DingTalk\UploadFile\ProjectFile
        [HttpGet]
        public string GetAllFilePath(string Path)
        {
            return JsonConvert.SerializeObject(FileHelper.GetDirectories(Path));
        }

        /// <summary>
        /// 修改项目文件
        /// </summary>
        /// <param name="Path">当前路径</param>
        /// <param name="MovePath">修改文件名时的新路径</param>
        /// <param name="UserId">用户Id</param>
        /// <param name="ProjectId">项目Id</param>
        /// <param name="ChangeType">修改类型( 0:新建  1:删除  2:修改(需要多传一个MovePath参数) )</param>
        /// <returns></returns>
        /// 测试数据：/Project/ChangeFile?Path=E:\Project\DingTalk\DingTalk\UploadFile\ProjectFile\news&UserId=manager325&ChangeType=0&ProjectId=1111111
        [HttpGet]
        public string ChangeFile(string Path, string MovePath, string UserId, string ProjectId, int ChangeType)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    //判断权限
                    bool IsSuperPower = (context.Roles.Where(r => r.UserId == UserId && r.RoleName == "超级管理员").ToList().Count() >= 1) ? true : false;
                    if (IsSuperPower)
                    {
                        switch (ChangeType)
                        {
                            case 0:
                                FileHelper.CreateDirectory(Path);
                                break;
                            case 1:
                                FileHelper.DeleteDirectory(Path);
                                break;
                            case 2:
                                FileHelper.Move(Path, MovePath);
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
                        bool IsComPower = (context.ProjectInfo.Where(p => p.ProjectId == ProjectId && p.ApplyManId == UserId).ToList().Count() >= 1) ? true : false;
                        if (IsComPower)
                        {
                            switch (ChangeType)
                            {
                                case 0:
                                    FileHelper.CreateDirectory(Path);
                                    break;
                                case 1:
                                    FileHelper.DeleteDirectory(Path);
                                    break;
                                case 2:
                                    FileHelper.Move(Path, MovePath);
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

    }
}