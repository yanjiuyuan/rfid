namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Borrow")]
    public partial class Borrow
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [StringLength(300)]
        public string Supplier { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        [StringLength(300)]
        public string Name { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(300)]
        public string CodeNo { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [StringLength(300)]
        public string Standard { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(300)]
        public string Unit { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [StringLength(300)]
        public string Count { get; set; }

        /// <summary>
        /// 价值
        /// </summary>
        [StringLength(300)]
        public string Price { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        [StringLength(300)]
        public string Purpose { get; set; }

        /// <summary>
        /// 借入周期开始时间
        /// </summary>
        [StringLength(300)]
        public string StartTime { get; set; }

        /// <summary>
        /// 借入周期结束时间
        /// </summary>
        [StringLength(300)]
        public string EndTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Mark { get; set; }
    }
}