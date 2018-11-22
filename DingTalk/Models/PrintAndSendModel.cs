using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class PrintAndSendModel
    {
        /// <summary>
        /// 推送用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string TaskId { get; set; }
        public string OldPath { get; set; }
        /// <summary>
        /// 是否是公车
        /// </summary>
        public bool IsPublic { get; set; }
    }
}