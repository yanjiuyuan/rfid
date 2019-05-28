namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DrawingChange")]
    public partial class DrawingChange
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }

        [StringLength(500)]
        public string BomId { get; set; }

        [StringLength(300)]
        public string DrawingNo { get; set; }

        [StringLength(300)]
        public string CodeNo { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Count { get; set; }

        [StringLength(500)]
        public string MaterialScience { get; set; }

        [StringLength(500)]
        public string Unit { get; set; }

        [StringLength(500)]
        public string Brand { get; set; }

        [StringLength(500)]
        public string Sorts { get; set; }

        [StringLength(500)]
        public string Mark { get; set; }

        public bool? IsDown { get; set; }

        [StringLength(200)]
        public string SingleWeight { get; set; }

        [StringLength(200)]
        public string AllWeight { get; set; }

        [StringLength(200)]
        public string NeedTime { get; set; }

        /// <summary>
        /// 更改类型(目前只有不变(0)、新增(1)、删除(2))
        /// </summary>
        [StringLength(200)]
        public string ChangeType { get; set; }

        /// <summary>
        /// 图纸审批旧BOM的Id
        /// </summary>
        [StringLength(200)]
        public string OldId { get; set; }

        /// <summary>
        /// 图纸审批旧BOM的TaskId
        /// </summary>
        [StringLength(200)]
        public string OldTaskId { get; set; }
        
    }
}
