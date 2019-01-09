using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.DingModels
{
    public class PrintModel
    {
        /// <summary>
        /// 推送用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string TaskId { get; set; }
    }
}