namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ErrorLogs")]
    public partial class ErrorLogs
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 调用接口人
        /// </summary>
        [StringLength(100)]
        public string ApplyMan { get; set; }
        /// <summary>
        /// 调用接口人Id
        /// </summary>
        [StringLength(100)]
        public string ApplyManId { get; set; }
        /// <summary>
        /// 调用时间(不用传)
        /// </summary>
        [StringLength(100)]
        public string ApplyTime { get; set; }

        /// <summary>
        /// 调用接口Url
        /// </summary>
        [StringLength(200)]
        public string Url { get; set; }
        /// <summary>
        /// 传递的参数(JsonString)
        /// </summary>
        public string Para { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [StringLength(100)]
        public string GetOrPost { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        [StringLength(300)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [StringLength(300)]
        public string ErrorMsg { get; set; }

    }
}