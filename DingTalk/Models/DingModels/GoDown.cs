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
        public string fNumber { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        [StringLength(300)]
        public string fName { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [StringLength(300)]
        public string fModel { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [StringLength(300)]
        public string unitName { get; set; }

        /// <summary>
        /// 实收数量(可编辑)
        /// </summary>
        [StringLength(300)]
        public string fQty { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [StringLength(300)]
        public string fPrice { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [StringLength(300)]
        public string fAmount { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [StringLength(300)]
        public string fFullName { get; set; }

    }
}