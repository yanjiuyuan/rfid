namespace DingTalk.Models.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcedureInfo")]
    public partial class ProcedureInfo
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(200)]
        public string BomId { get; set; }

        [StringLength(500)]
        public string ProcedureName { get; set; }

        [StringLength(500)]
        public string DefaultWorkTime { get; set; }

        [StringLength(500)]
        public string Worker { get; set; }

        [StringLength(500)]
        public string WorkerId { get; set; }

        public bool? IsFinish { get; set; }

        public int? State { get; set; }
    }
}
