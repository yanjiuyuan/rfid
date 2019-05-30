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


        [StringLength(200)]
        public string OldId { get; set; }

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
        /// 变更类型  1 新增 2 删除 
        /// </summary>
        [StringLength(200)]
        public string ChangeType { get; set; }
    }
}
