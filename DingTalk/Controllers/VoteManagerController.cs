using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalkServer;
using DingTalkServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 投票管理
    /// </summary>
    [RoutePrefix("VoteManager")]
    public class VoteManagerController : ApiController
    {
        public DingTalkConfig DTConfig { get; set; } = new DingTalkConfig();
        /// <summary>
        /// 发起投票
        /// </summary>
        /// <param name="vote"></param>
        [Route("LaunchVote")]
        [HttpPost]
        public object LaunchVote([FromBody] Vote vote)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    context.Vote.Add(vote);
                    context.SaveChanges();
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "成功发起"
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

        /// <summary>
        /// 修改投票
        /// </summary>
        /// <param name="vote"></param>
        [Route("ChangeVote")]
        [HttpPost]
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


        /// <summary>
        /// 读取投票信息
        /// </summary>
        /// <param name="Id"></param>
        [Route("QuaryVote")]
        [HttpGet]
        public object QuaryVote(int? Id=0)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    if (Id==0)
                    {
                        List<Vote> vote = context.Vote.ToList();
                        return vote;
                    }
                    else
                    {
                        Vote vote = context.Vote.Find(Id);
                        return vote;
                    }
                }
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

        /// <summary>
        /// 发送投票
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("SendVote")]
        [HttpGet]
        public async Task<Object> SendVote(string Url, int Id)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    Vote vote = context.Vote.Find(Id);
                    string title = vote.Title;
                    string ApplyMan = vote.ApplyMan;
                    DingTalkManager dtManager = new DingTalkManager();
                    var AccessToken = await dtManager.GetAccessToken();
                    AccessTokenResponseModel accessTokenResponseModel = JsonConvert.DeserializeObject<AccessTokenResponseModel>(AccessToken);
                    IDingTalkClient client = new DefaultDingTalkClient("https://eco.taobao.com/router/rest");
                    CorpMessageCorpconversationAsyncsendRequest req = new CorpMessageCorpconversationAsyncsendRequest();
                    req.Msgtype = "oa";
                    req.AgentId = long.Parse(DTConfig.AgentId);
                    req.DeptIdList = "1";
                    req.ToAllUser = true;
                    req.Msgcontent = "{\"message_url\": \"http://dingtalk.com\",\"head\": {\"bgcolor\": \"FFBBBBBB\",\"text\": \"投票通知\"},\"body\": {\"title\": \"正文标题\",\"form\": [],\"content\": \"11111111111\",\"author\": \"李四 \"}}";
                    CorpMessageCorpconversationAsyncsendResponse rsp = client.Execute(req, accessTokenResponseModel.access_token);
                    Console.WriteLine(rsp.Body);
                }
                return new ErrorModel()
                {
                    errorCode = 0,
                    errorMessage = "成功"
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
