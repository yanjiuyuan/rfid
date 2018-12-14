namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PurchaseTable")]
    public partial class PurchaseTable
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }

        [StringLength(500)]
        public string CodeNo { get; set; }

        [StringLength(300)]
        public string Name { get; set; }

        [StringLength(300)]
        public string Standard { get; set; }

        [StringLength(500)]
        public string Unit { get; set; }

        [StringLength(500)]
        public string Count { get; set; }

        [StringLength(500)]
        public string Price { get; set; }

        [StringLength(500)]
        public string Purpose { get; set; }

        [StringLength(500)]
        public string UrgentDate { get; set; }

        [StringLength(500)]
        public string Mark { get; set; }

        /// <summary>
        /// ËÍ»õµØµã
        /// </summary>
        [StringLength(100)]
        public string SendPosition { get; set; }
    }
}
