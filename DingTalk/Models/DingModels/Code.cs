namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Code")]
    public partial class Code
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(30)]
        public string TaskId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(500)]
        public string CodeNumber { get; set; }
        /// <summary>
        /// 物料大类编码
        /// </summary>
        [StringLength(500)]
        public string BigCode { get; set; }
        /// <summary>
        /// 物料小类编码
        /// </summary>
        [StringLength(500)]
        public string SmallCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(500)]
        public string Unit { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        [StringLength(500)]
        public string Standard { get; set; }
        /// <summary>
        /// 表面处理
        /// </summary>
        [StringLength(500)]
        public string SurfaceTreatment { get; set; }
        /// <summary>
        /// 性能等级
        /// </summary>
        [StringLength(500)]
        public string PerformanceLevel { get; set; }
        /// <summary>
        /// 标准号
        /// </summary>
        [StringLength(500)]
        public string StandardNumber { get; set; }
        /// <summary>
        /// 典型特征
        /// </summary>
        [StringLength(500)]
        public string Features { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        [StringLength(500)]
        public string purpose { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
