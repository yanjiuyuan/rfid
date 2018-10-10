using DingTalk.Bussiness.Word;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                eFHelper.Add(newsAndCases);
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
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
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
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
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
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
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
                return new NewErrorModel()
                {
                    error = new Error(1, ex.Message, "") { },
                };
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
                File.Copy(filePath, YjyWebPath+"html");

                return new NewErrorModel()
                {
                    data = "",
                    error = new Error(0, "复制成功！", "") { },
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

    }
}
