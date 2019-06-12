namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MaterialRelease")]
    public partial class MaterialRelease
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }


        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(200)]
        public string TaskId { get; set; }
    
        /// <summary>
        /// 单位名称
        /// </summary>
        [StringLength(200)]
        public string Company { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [StringLength(200)]
        public string CarNo { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [StringLength(200)]
        public string Tel { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [StringLength(200)]
        public string Count { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(200)]
        public string Unit { get; set; }
        /// <summary>
        /// 事由
        /// </summary>
        [StringLength(500)]
        public string Cause { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [StringLength(200)]
        public string Date { get; set; }
    }
}