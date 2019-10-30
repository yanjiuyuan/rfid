using DingTalk.EF;
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
    /// 合同管理
    /// </summary>
    [RoutePrefix("ContractManager")]
    public class ContractManagerController : ApiController
    {
        /// <summary>
        /// 新增合同
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public object Add(Contract contract)
        {
            try
            {
                EFHelper<Contract> eFHelper = new EFHelper<Contract>();
                if (eFHelper.Add(contract) == 1)
                {
                    //return new 
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
