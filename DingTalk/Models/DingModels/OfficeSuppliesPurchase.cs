namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OfficeSuppliesPurchase")]
    public partial class OfficeSuppliesPurchase
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(300)]
        public string TaskId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(500)]
        public string CodeNo { get; set; }
        /// <summary>
        /// 名称
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
        [StringLength(500)]
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
        /// 申请人
        /// </summary>
        [StringLength(300)]
        public string ApplyMan { get; set; }
        /// <summary>
        /// 申请部门
        /// </summary>
        [StringLength(300)]
        public string Dept { get; set; }
        
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDelete { get; set; }
    }
}
