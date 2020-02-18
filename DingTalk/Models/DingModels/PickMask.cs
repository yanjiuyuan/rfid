namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PickMask")]
    public partial class PickMask
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [StringLength(200)]
        public string BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [StringLength(200)]
        public string EndTime { get; set; }

        /// <summary>
        /// 领用数量
        /// </summary>
        public int PickCount { get; set; }

        /// <summary>
        /// 领用人数
        /// </summary>
        public int PickPeopleCount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300)]
        public string Remark { get; set; }

    }
}