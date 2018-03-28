namespace DingTalk.Models.DbModels
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

        [StringLength(100)]
        public string CurrentManId { get; set; }

        [StringLength(30)]
        public string CurrentMan { get; set; }

        [StringLength(100)]
        public string NextManId { get; set; }

        [StringLength(30)]
        public string NextMan { get; set; }

        [StringLength(30)]
        public string HandleTime { get; set; }

        [StringLength(500)]
        public string FlowNodes { get; set; }

        public int? State { get; set; }

        public int? IsEnable { get; set; }
    }
}
