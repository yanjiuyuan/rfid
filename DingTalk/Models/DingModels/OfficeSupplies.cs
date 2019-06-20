namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    /// <summary>
    /// 办公用品
    /// </summary>
    public partial class OfficeSupplies
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(500)]
        public string CodeNo { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [StringLength(300)]
        public string Name { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        [StringLength(300)]
        public string Standard { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(500)]
        public string Unit { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [StringLength(500)]
        public string Count { get; set; }
        /// <summary>
        /// 实际价格
        /// </summary>
        [StringLength(500)]
        public string Price { get; set; }
        /// <summary>
        /// 预计价格
        /// </summary>
        public string ExpectPrice { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        [StringLength(500)]
        public string Purpose { get; set; }
        /// <summary>
        /// 需用日期
        /// </summary>
        [StringLength(500)]
        public string UrgentDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Mark { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDelete { get; set; }
    }
}
