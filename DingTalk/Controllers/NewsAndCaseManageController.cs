using Common.Flie;
using Common.Ionic;
using DingTalk.Bussiness.Word;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 新闻与应用案例管理
    /// </summary>
    [RoutePrefix("NewsAndCases")]
    public class NewsAndCaseManageController : ApiController
    {
        /// <summary>
        /// 新闻与应用案例保存
        /// </summary>
        /// <param name="newsAndCases"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] NewsAndCases newsAndCases)
        {
            try
            {
                EFHelper<NewsAndCases> eFHelper = new EFHelper<NewsAndCases>();
                newsAndCases.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                eFHelper.Add(newsAndCases);
                return new NewErrorModel()
                {
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新闻与应用案例读取
        /// </summary>
        /// <param name="bigType">大类</param>
        /// <param name="type">小类</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string bigType, string type, int pageIndex, int pageSize)
        {
            try
            {
                EFHelper<NewsAndCases> eFHelper = new EFHelper<NewsAndCases>();
                System.Linq.Expressions.Expression<Func<NewsAndCases, bool>> expression = null;
                expression = n => n.BigType == bigType && n.Type == type;
                List<NewsAndCases> NewsAndCasesListAll = eFHelper.GetListBy(expression);
                List<NewsAndCases> newsAndCases = eFHelper.GetPagedList(pageIndex, pageSize,
                     expression, n => n.Id);
                return new NewErrorModel()
                {
                    count = NewsAndCasesListAll.Count,
                    data = newsAndCases,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新闻与应用案例详情读取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("ReadById")]
        [HttpGet]
        public object ReadById(int id)
        {
            try
            {
                EFHelper<NewsAndCases> eFHelper = new EFHelper<NewsAndCases>();
                NewsAndCases newsAndCases = eFHelper.GetListById(id);
                return new NewErrorModel()
                {
                    data = newsAndCases,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新闻与应用案例详情删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("DeleteById")]
        [HttpGet]
        public object DeleteById(int id)
        {
            try
            {
                EFHelper<NewsAndCases> eFHelper = new EFHelper<NewsAndCases>();
                NewsAndCases newsAndCases = new NewsAndCases();
                using (DDContext context = new DDContext())
                {
                    newsAndCases = context.NewsAndCases.Find(id);
                    context.NewsAndCases.Remove(newsAndCases);
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    data = newsAndCases,
                    error = new Error(0, "删除成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// word转html
        /// </summary>
        /// <param name="wordPath">文件路径</param>
        /// <returns></returns>
        /// 测试数据   wordPath=~/bin/1.doc
        [Route("WordPathToHtml")]
        [HttpGet]
        public object WordPathToHtml(string wordPath)
        {
            try
            {
                WordHelper wordHelper = new WordHelper();
                string filePath = HttpContext.Current.Server.MapPath(wordPath);
                string strPathHtml = wordHelper.GetPathByDocToHTML(filePath);
                //string strHtml = GetFileToString(strPathHtml);
                return new NewErrorModel()
                {
                    data = strPathHtml,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 拷贝文件到研究院项目下
        /// </summary>
        /// <param name="picPath">文件路径</param>
        /// <returns></returns>
        [HttpGet]
        public object CopyPic(string picPath)
        {
            try
            {
                string YjyWebPath = ConfigurationManager.AppSettings["YjyWebPath"];
                string filePath = HttpContext.Current.Server.MapPath(picPath);
                File.Copy(filePath, YjyWebPath + "html");

                return new NewErrorModel()
                {
                    data = "",
                    error = new Error(0, "复制成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 读取html文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        [Route("GetFileToString")]
        [HttpGet]
        public string GetFileToString(string filePath)
        {
            filePath = HttpContext.Current.Server.MapPath(filePath);
            string fileContent = string.Empty;
            using (var reader = new StreamReader(filePath))
            {
                fileContent = reader.ReadToEnd();
            }
            return fileContent;
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileBase64">文件流</param>
        /// <returns></returns>
        [Route("DownloadFile")]
        [HttpPost]
        public object DownloadFile(FileBase64 fileBase64)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(fileBase64.FilePath));
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
                FileStream filestream = new FileStream(HttpContext.Current.Server.MapPath(fileBase64.FilePath), FileMode.Open);
                byte[] bt = new byte[filestream.Length];

                //调用read读取方法
                filestream.Read(bt, 0, bt.Length);
                string base64Str = Convert.ToBase64String(bt);
                filestream.Close();
                return new NewErrorModel()
                {
                    data = "data:application/pdf;base64," + base64Str,
                    error = new Error(0, "下载成功！", "") { },
                };
            }
            else
            {
                return null;
            }

        }






        /// <summary>
        /// 推送所有图纸数据和附件数据
        /// </summary>
        /// <param name="taskId">流水号</param>
        /// <param name="applyManId">用户Id</param>
        /// <returns></returns>
        [Route("GetAllPDF")]
        [HttpGet]
        public async Task<object> GetAllPDF(string taskId, string applyManId)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    string strUrl = "";
                    Tasks tasksQuery = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.NodeId == 0).FirstOrDefault();
                    string strFilePDFUrl = tasksQuery.FilePDFUrl;
                    string strFileUrl = tasksQuery.FileUrl;

                    if (!string.IsNullOrEmpty(strFileUrl))
                    {
                        strUrl = strFilePDFUrl + "," + strFileUrl;
                    }
                    else
                    {
                        strUrl = strFilePDFUrl;
                    }

                    string[] FilePDFUrl = strUrl.Split(',');
                    if (FilePDFUrl.Length > 0)
                    {
                        List<string> ListPath = new List<string>(FilePDFUrl);

                        Dictionary<int,List<string>> keyValuePairs = CheckPathLength(ListPath);
                        int i = 0;
                        foreach (var item in keyValuePairs)
                        {
                            i++;
                            string SavePath = string.Format(@"{0}\UploadFile\Ionic\{1}.zip",
                                   AppDomain.CurrentDomain.BaseDirectory,
                                    "流水号" + taskId + "图纸打包第" + i + "份" +
                                    DateTime.Now.ToString("yyyyMMddHHmmss"));
                            //文件压缩打包
                            IonicHelper.CompressMulti(item.Value, SavePath, false);
                            DingTalkServersController dingTalkServersController = new DingTalkServersController();
                            SavePath = "~\\" + FileHelper.RelativePath(HttpContext.Current.Server.MapPath("~/"), SavePath);

                            //上盯盘
                            var resultUploadMedia = await dingTalkServersController.UploadMedia(SavePath);
                            //推送用户
                            FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                            fileSendModel.UserId = applyManId;
                            var result = await dingTalkServersController.SendFileMessage(fileSendModel);

                        }

                    }

                    return new NewErrorModel()
                    {
                        error = new Error(0, "已推送至钉钉！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Dictionary<int,List<string>> CheckPathLength(List<string> vs)
        {
            long listLength = 0;
            int i = 0;
            Dictionary<int, List<string>> keyValuePairs = new Dictionary<int, List<string>>();
            foreach (var item in vs)
            {
                List<string> vss = new List<string>();
                FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(item));
                listLength += fileInfo.Length;
                string newPathName = (HttpContext.Current.Server.MapPath(item).Substring(0, HttpContext.Current.Server.MapPath(item).Length - 18)) +
                    System.IO.Path.GetExtension(HttpContext.Current.Server.MapPath(item));
                File.Copy(HttpContext.Current.Server.MapPath(item), newPathName, true);

                if (listLength < 1024 * 1024 * 6) //9M
                {
                    if (keyValuePairs.Count == 0)
                    {
                        vss.Add(newPathName);
                        keyValuePairs.Add(i, vss);
                    }
                    else
                    {
                        vss = keyValuePairs[i];
                        vss.Add(newPathName);
                        keyValuePairs[i] = vss;
                    }
                }
                else
                {
                    i++;
                    vss.Add(newPathName);
                    keyValuePairs.Add(i, vss);
                }
            }
            return keyValuePairs;
        }
    }
}
