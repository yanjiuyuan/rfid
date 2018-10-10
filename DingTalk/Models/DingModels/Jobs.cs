namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Jobs
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(100)]
        public string JobName { get; set; }
        /// <summary>
        /// 要求
        /// </summary>
        [StringLength(300)]
        public string Require { get; set; }
        /// <summary>
        /// 工作地点
        /// </summary>
        [StringLength(200)]
        public string WorkPlace { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(100)]
        public string CreateTime { get; set; }
        /// <summary>
        /// 薪资
        /// </summary>
        [StringLength(100)]
        public string Pay { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        [StringLength(100)]
        public string Url { get; set; }
        /// <summary>
        /// 大类
        /// </summary>
        [StringLength(100)]
        public string BigType { get; set; }
        /// <summary>
        /// 小类
        /// </summary>
        [StringLength(100)]
        public string Type { get; set; }
    }
}
