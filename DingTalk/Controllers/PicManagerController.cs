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
    /// 图片数据管理
    /// </summary>
    [RoutePrefix("PicManage")]
    public class PicManageController : ApiController
    {
        /// <summary>
        /// 图片数据保存
        /// </summary>
        /// <param name="picInfo"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] PicInfo picInfo)
        {
            try
            {
                EFHelper<PicInfo> eFHelper = new EFHelper<PicInfo>();
                picInfo.CreateTime = DateTime.Now.ToString("yyyy-dd-MM HH:hh:ss");
                eFHelper.Add(picInfo);
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
        /// 图片数据分页
        /// </summary>
        /// <param name="type">所属类型(必填)</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string type, int pageIndex, int pageSize)
        {
            try
            {
                EFHelper<PicInfo> eFHelper = new EFHelper<PicInfo>();
                System.Linq.Expressions.Expression<Func<PicInfo, bool>> expression = null;
                expression = n => n.Type == type;
                List<PicInfo> PicInfoListAll = eFHelper.GetListBy(expression);
                List<PicInfo> PicInfoList = eFHelper.GetPagedList(pageIndex, pageSize,
                     expression, n => n.Id);
                return new NewErrorModel()
                {
                    count = PicInfoListAll.Count,
                    data = PicInfoList,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 图片数据删除
        /// </summary>
        /// <param name="Id">唯一标识Id</param>
        /// <returns></returns>
        [Route("DeleteById")]
        [HttpGet]
        public object DeleteById(int Id)
        {
            try
            {
                EFHelper<PicInfo> eFHelper = new EFHelper<PicInfo>();
                int count = eFHelper.DelBy(p => p.Id == Id);
                return new NewErrorModel()
                {
                    count = count,
                    error = new Error(0, "删除成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 图片修改接口
        /// </summary>
        /// <param name="picInfo"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public object Modify(PicInfo picInfo)
        {
            try
            {
                EFHelper<PicInfo> eFHelper = new EFHelper<PicInfo>();
                picInfo.LastModifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int count = eFHelper.Modify(picInfo);
                return new NewErrorModel()
                {
                    count = count,
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将文件(图片)拷贝到研究院项目下
        /// </summary>
        /// <param name="wordPath">测试数据：~/UploadFile/Images/20180709142550.jpg</param>
        /// <returns></returns>
        [Route("FileRemove")]
        [HttpGet]
        public object FileRemove(string wordPath)
        {
            try
            {
                string filePath = HttpContext.Current.Server.MapPath(wordPath);
                string YjyWebPath = ConfigurationManager.AppSettings["YjyWebPath"];
                YjyWebPath = YjyWebPath+ "UploadFile\\Images\\" + Path.GetFileName(wordPath);

                File.Move(filePath, YjyWebPath);

                //string strHtml = GetFileToString(strPathHtml);
                return new NewErrorModel()
                {
                    data = "",
                    error = new Error(0, "拷贝成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
