using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    /// <summary>
    /// 最新版钉钉接口模型 2018-11-27
    /// </summary>
    public class NewOATestModel
    {
        public string msgtype { get; set; }
        public oa oa { get; set; }
    }

    public class oa
    {
        public string message_url { get; set; }

        public head head { get; set; }

        public body body { get; set; }
    }
}