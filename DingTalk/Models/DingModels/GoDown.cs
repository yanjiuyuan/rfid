namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GoDown")]
    public partial class GoDown
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(30)]
        public string TaskId { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(300)]
        public string FNumber { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        [StringLength(300)]
        public string FName { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [StringLength(300)]
        public string FModel { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [StringLength(300)]
        public string UnitName { get; set; }

        /// <summary>
        /// 实收数量(可编辑)
        /// </summary>
        [StringLength(300)]
        public string FQty { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [StringLength(300)]
        public string FPrice { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [StringLength(300)]
        public string FAmount { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [StringLength(300)]
        public string FFullName { get; set; }

    }
}