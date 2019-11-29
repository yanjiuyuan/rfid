namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    [Table("ApplicationUnit")]
    public partial class ApplicationUnit
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
        /// 转化应用单位
        /// </summary>
        [StringLength(500)]
        public string ApplicationUnitName { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [StringLength(500)]
        public string Customer { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [StringLength(500)]
        public string Post { get; set; }

        /// <summary>
        /// Tel
        /// </summary>
        [StringLength(500)]
        public string Tel { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [StringLength(500)]
        public string QQ { get; set; }

    }
}
