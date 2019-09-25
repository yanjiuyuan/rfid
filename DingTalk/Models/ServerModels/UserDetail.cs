using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.ServerModels
{
    /// <summary>
    /// 用户详情
    /// </summary>
    public class UserDetail
    {
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 钉钉返回的部门Id集合
        /// </summary>

        public string[] department { get; set; }

        //public List<Dictionary<int,int>> orderInDepts { get; set; }

        /// <summary>
        /// 部门信息
        /// </summary>
        public List<string> dept { get; set; }
    }

    public class DeptModel
    {
        public string name { get; set; }
    }
}