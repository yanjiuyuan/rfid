namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OverTime")]
    public partial class OverTime
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(100)]
        public string TaskId { get; set; }
        /// <summary>
        /// 加班日期
        /// </summary>
        [StringLength(100)]
        public string DateTime { get; set; }
        /// <summary>
        /// 加班开始时间
        /// </summary>
        [StringLength(100)]
        public string StartTime { get; set; }
        /// <summary>
        /// 加班结束时间
        /// </summary>
        [StringLength(100)]
        public string EndTimeTime { get; set; }
        /// <summary>
        /// 加班时长
        /// </summary>
        [StringLength(100)]
        public string UseTime { get; set; }
        /// <summary>
        /// 加班事由
        /// </summary>
        public string OverTimeContent { get; set; }
        /// <summary>
        /// 有效时间
        /// </summary>
        public string EffectiveTime { get; set; }
    }
}
