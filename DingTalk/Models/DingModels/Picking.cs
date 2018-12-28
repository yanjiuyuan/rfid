namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Picking")]
    public partial class Picking
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
        /// 物料编码
        /// </summary>
        [StringLength(200)]
        public string FNumber { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [StringLength(200)]
        public string FName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string FModel { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        [StringLength(200)]
        public string UnitName { get; set; }
        /// <summary>
        /// 领用数量
        /// </summary>
        public string PickCount { get; set; }
    }
}
