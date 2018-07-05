namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Approve")]
    public partial class Approve
    {
        [Key]
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ApproveNo { get; set; }

        public int? ApplyNo { get; set; }

        [StringLength(30)]
        public string TaskId { get; set; }

        [StringLength(30)]
        public string HandleTime { get; set; }

        [StringLength(30)]
        public string ApproveTime { get; set; }

        [StringLength(50)]
        public string ApproveMan { get; set; }

        [StringLength(500)]
        public string ApproveView { get; set; }

        public int? Mark { get; set; }

        [StringLength(50)]
        public string FJPath { get; set; }
    }
}
