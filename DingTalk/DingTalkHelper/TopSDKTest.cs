using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using DingTalk.DingTalkHelper;
using DingTalk.Models;
using DingTalkServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DingTalk.Controllers
{
    public class TopSDKTest
    {
        public DingTalkConfig DTConfig { get; set; } = new DingTalkConfig();
        public string SendOaMessage(string ApplyManIds, OATextModel oaTextModel)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://eco.taobao.com/router/rest");
            CorpMessageCorpconversationAsyncsendRequest req = new CorpMessageCorpconversationAsyncsendRequest();
            req.Msgtype = "oa";
            req.AgentId = long.Parse(DTConfig.AgentId);
            req.UseridList = ApplyManIds;
            //req.DeptIdList = "123,456";
            req.ToAllUser = false;
            //req.Msgcontent = "{\"message_url\": \"http://dingtalk.com\",\"head\": {\"bgcolor\": \"FFBBBBBB\",\"text\": \"头部标题\"},\"body\": {\"title\": \"正文标题\",\"form\": [{\"key\": \"姓名:\",\"value\": \"张三\"},{\"key\": \"爱好:\",\"value\": \"打球、听音乐\"}],\"rich\": {\"num\": \"15.6\",\"unit\": \"元\"},\"content\": \"111大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本\",\"image\": \"@lADOADmaWMzazQKA\",\"file_count\": \"3\",\"author\": \"李四 \"}}";
            req.Msgcontent=JsonConvert.SerializeObject(oaTextModel);


            CorpMessageCorpconversationAsyncsendResponse rsp = client.Execute(req,
            DDApiService.Instance.GetAccessToken());
            int iResult = 0;
            return JsonConvert.SerializeObject(new ErrorModel
            {
                IsError = rsp.IsError,
                errorCode = int.TryParse(rsp.ErrCode, out iResult) ? iResult : iResult,
                errorMessage = rsp.ErrMsg
            });
        }
    }
}