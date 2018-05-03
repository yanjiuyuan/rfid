using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using DingTalkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Controllers
{
    public class TopSDKTest
    {
        public DingTalkConfig DTConfig { get; set; } = new DingTalkConfig();
        public void SendMessage(string ApplyManId)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://eco.taobao.com/router/rest");
            CorpMessageCorpconversationAsyncsendRequest req = new CorpMessageCorpconversationAsyncsendRequest();
            req.Msgtype = "oa";//发送消息是以oa的形式发送的,其他的还有text,image等形式
            req.AgentId = long.Parse(DTConfig.AgentId);//微应用ID
            req.UseridList = ApplyManId;//收信息的userId,这个是by公司来区分，在该公司内这是一个唯一标识符
            //req.DeptIdList = "123,456";//部门ID
            req.ToAllUser = false;//是否发给所有人
            //消息文本
            req.Msgcontent = "{\"message_url\": \"http://dingtalk.com\",\"head\": {\"bgcolor\": \"FFBBBBBB\",\"text\": \"头部标题\"},\"body\": {\"title\": \"测试文本\",\"form\": [{\"key\": \"姓名:\",\"value\": \"张三\"},{\"key\": \"爱好:\",\"value\": \"打球、听音乐\"}],\"rich\": {\"num\": \"15.6\",\"unit\": \"元\"},\"content\": \"11大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本大段文本\",\"image\": \"@lADOADmaWMzazQKA\",\"file_count\": \"3\",\"author\": \"李四 \"}}";
            CorpMessageCorpconversationAsyncsendResponse rsp = client.Execute(req,DTConfig.AccessToken);//发送消息
        }
    }
}