namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PurchaseDown")]
    public partial class PurchaseDown
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }

        [StringLength(300)]
        public string OldTaskId { get; set; }

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

        [StringLength(500)]
        public string PurchaseProcedureInfoId { get; set; }

        [StringLength(500)]
        public string FlowType { get; set; }
    }
}
