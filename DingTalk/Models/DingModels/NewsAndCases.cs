namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NewsAndCases
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 小类
        /// </summary>
        [StringLength(100)]
        public string Type { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        [StringLength(500)]
        public string ImageUrl { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(300)]
        public string Title { get; set; }
        /// <summary>
        /// 路径信息
        /// </summary>
        [StringLength(300)]
        public string Contents { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(100)]
        public string CreateTime { get; set; }
        /// <summary>
        /// 大类
        /// </summary>
        [StringLength(100)]
        public string BigType { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        [StringLength(200)]
        public string Abstract { get; set; }
        /// <summary>
        /// 访问次数
        /// </summary>
        public double? visitingtime { get; set; }

    }
}
