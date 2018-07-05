namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkTime")]
    public partial class WorkTime
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(500)]
        public string PurchaseProcedureInfoId { get; set; }

        public bool? IsFinish { get; set; }

        [StringLength(200)]
        public string Worker { get; set; }

        [StringLength(300)]
        public string WorkerId { get; set; }

        [StringLength(200)]
        public string StartTime { get; set; }

        [StringLength(200)]
        public string EndTime { get; set; }

        [StringLength(200)]
        public string UseTime { get; set; }
    }
}
