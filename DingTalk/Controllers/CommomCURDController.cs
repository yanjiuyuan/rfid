using DingTalk.Bussiness.EF;
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
    [RoutePrefix("CommomCURD")]
    public class CommomCURDController : ApiController
    {
        /// <summary>
        /// 通用读取方法
        /// </summary>
        /// <param name="cURDModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Read")]
        public NewErrorModel Read(CURDModel cURDModel)
        {
            try
            {
                if (cURDModel != null && cURDModel.tables != null && cURDModel.tables.Count > 0)
                {
                    using (DDContext dataContext = new DDContext())
                    {

                    }
                    SqlHelper sqlHelper = new SqlHelper();
                    if (cURDModel.tables.Count == 1) //单表
                    {
                        string strSql = sqlHelper.CommomCURDRead(cURDModel.tables[0]);

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class CURDModel
    {
        /// <summary>
        /// 操作人Id(用于判断权限)
        /// </summary>
        public string applyManId { get; set; }

        /// <summary>
        /// 表数据
        /// </summary>
        public List<Tables> tables { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 页容量(默认每页5条)
        /// </summary>

        public int pageSize { get; set; }
    }
}
