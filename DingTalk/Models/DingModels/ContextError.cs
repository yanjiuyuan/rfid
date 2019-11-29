namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ContextError")]
    public partial class ContextError
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 前端调用参数
        /// </summary>
        public string RequstResult { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [MaxLength(50)]
        public string Method { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        [MaxLength(200)]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求Ip
        /// </summary>
        [MaxLength(200)]
        public string RequestIp { get; set; }

        /// <summary>
        /// 错误发生时间
        /// </summary>
        [MaxLength(50)]
        public string CreateTime { get; set; }
    }
}