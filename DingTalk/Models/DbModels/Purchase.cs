namespace DingTalk.Models.DbModels
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

        [StringLength(30)]
        public string FlowId { get; set; }

        [StringLength(30)]
        public string TaskId { get; set; }

        [StringLength(30)]
        public string DrawingNo { get; set; }

        [StringLength(30)]
        public string CodeNo { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Count { get; set; }

        [StringLength(50)]
        public string MaterialScience { get; set; }

        [StringLength(50)]
        public string Unit { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }

        [StringLength(50)]
        public string Sorts { get; set; }

        [StringLength(500)]
        public string Mark { get; set; }
    }
}
