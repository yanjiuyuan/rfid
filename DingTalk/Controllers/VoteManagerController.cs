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
    /// <summary>
    /// 投票管理
    /// </summary>
    public class VoteManagerController : ApiController
    {
        /// <summary>
        /// 发起投票
        /// </summary>
        /// <param name="vote"></param>
        public object LaunchVote([FromBody] Vote vote)
        {
            try
            {
                using (DDContext context =new DDContext ())
                {
                    context.Vote.Add(vote);
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode=0,
                    errorMessage="成功发起"
                };
            }
            catch (Exception ex )
            {
                return new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 修改投票
        /// </summary>
        /// <param name="vote"></param>
        public object ChangeVote([FromBody] Vote vote)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Entry<Vote>(vote).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "修改投票成功"
                };
            }
            catch (Exception ex)
            {
                return new ErrorModel()
                {
                    errorCode = 1,
                    errorMessage = ex.Message
                };
            }
        }
    }
}
