namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProjectFunding")]
    public partial class ProjectFunding
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
        /// 科目
        /// </summary>
        [StringLength(500)]
        public string Subject { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [StringLength(500)]
        public string Money { get; set; }

        /// <summary>
        /// 金额+名称
        /// </summary>
        [StringLength(500)]
        public string NameAndMoney { get; set; }
        
    }
}
