namespace DingTalk.Models.DingModelsHs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Purchase")]
    public partial class Purchase
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

        [StringLength(100)]
        public string PurchaseMan { get; set; }

        [StringLength(100)]
        public string PurchaseManId { get; set; }

        [StringLength(200)]
        public string ChangeType { get; set; }

        [StringLength(200)]
        public string Designer { get; set; }

        [StringLength(200)]
        public string DesignerId { get; set; }
    }
}
