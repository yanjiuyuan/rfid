namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowProgress")]
    public partial class FlowProgress
    {
        [Key]
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal TaskId { get; set; }

        [StringLength(200)]
        public string CurrentManId { get; set; }

        [StringLength(100)]
        public string CurrentMan { get; set; }

        public int? State { get; set; }

        [StringLength(100)]
        public string ApprovedTime { get; set; }

        [StringLength(100)]
        public string StartTime { get; set; }

        [StringLength(1000)]
        public string Remaek { get; set; }
    }
}
