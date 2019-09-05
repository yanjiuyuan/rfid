namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DetailedList")]
    public partial class DetailedList
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
        /// 类型(传 项目采购清单、借用清单、领料清单、入库清单、借用清单、维修清单、受理知识产权清单 四种参数)
        /// </summary>
        [StringLength(500)]
        public string Type { get; set; }

        /// <summary>
        /// ApplyMan
        /// </summary>
        [StringLength(500)]
        public string ApplyMan { get; set; }

        /// <summary>
        /// ApplyManId
        /// </summary>
        [StringLength(500)]
        public string ApplyManId { get; set; }

        /// <summary>
        /// ApplyTime
        /// </summary>
        [StringLength(500)]
        public string ApplyTime { get; set; }

        /// <summary>
        /// 传过来的TaskId
        /// </summary>
        [StringLength(500)]
        public string OldTaskId { get; set; }

    }
}
