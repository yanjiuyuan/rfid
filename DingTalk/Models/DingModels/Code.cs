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

        [StringLength(30)]
        public string TaskId { get; set; }

        [StringLength(500)]
        public string CodeNumber { get; set; }
        /// <summary>
        /// 大类编码
        /// </summary>
        [StringLength(500)]
        public string BigCode { get; set; }
        /// <summary>
        /// 小类编码
        /// </summary>
        [StringLength(500)]
        public string SmallCode { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Unit { get; set; }

        [StringLength(500)]
        public string Standard { get; set; }

        [StringLength(500)]
        public string SurfaceTreatment { get; set; }

        [StringLength(500)]
        public string PerformanceLevel { get; set; }

        [StringLength(500)]
        public string StandardNumber { get; set; }

        [StringLength(500)]
        public string Features { get; set; }

        [StringLength(500)]
        public string purpose { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        [StringLength(500)]
        public string FNote { get; set; }

        /// <summary>
        /// 大类名称
        /// </summary>
        [StringLength(300)]
        public string BigCodeName { get; set; }
        /// <summary>
        /// 小类名称
        /// </summary>
        [StringLength(300)]
        public string SmallCodeName { get; set; }
        /// <summary>
        /// 校验是否合格
        /// </summary>
        [NotMapped]
        public bool IsQualified { get; set; }
    }
}
