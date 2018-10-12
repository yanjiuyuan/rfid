namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PurchaseOrder")]
    public partial class PurchaseOrder
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }
        /// <summary>
        /// BOM表单Id
        /// </summary>
        [StringLength(500)]
        public string BomId { get; set; }
        /// <summary>
        /// 图号
        /// </summary>
        [StringLength(300)]
        public string DrawingNo { get; set; }

        [StringLength(300)]
        public string CodeNo { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [StringLength(500)]
        public string Count { get; set; }
        /// <summary>
        /// 材料
        /// </summary>
        [StringLength(500)]
        public string MaterialScience { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(500)]
        public string Unit { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        [StringLength(500)]
        public string Brand { get; set; }
        /// <summary>
        /// 种类
        /// </summary>
        [StringLength(500)]
        public string Sorts { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Mark { get; set; }
        /// <summary>
        /// 是否下发
        /// </summary>
        public bool? IsDown { get; set; }
        /// <summary>
        /// 单重
        /// </summary>
        [StringLength(200)]
        public string SingleWeight { get; set; }
        /// <summary>
        /// 总重
        /// </summary>
        [StringLength(200)]
        public string AllWeight { get; set; }
        /// <summary>
        /// 需用时间
        /// </summary>
        [StringLength(200)]
        public string NeedTime { get; set; }
    }
}
