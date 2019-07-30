namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LongitudinalProject")]
    public partial class LongitudinalProject
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(500)]
        public string TaskId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(500)]
        public string Name { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [StringLength(500)]
        public string No { get; set; }

        /// <summary>
        /// 国波经费
        /// </summary>
        [StringLength(500)]
        public string Money { get; set; }

        /// <summary>
        /// 实际到账
        /// </summary>
        [StringLength(500)]
        public string ActualMoney { get; set; }
        
    }
}
