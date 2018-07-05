namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Worker")]
    public partial class Worker
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(200)]
        public string WorkerId { get; set; }

        [StringLength(300)]
        public string WorkerName { get; set; }

        [StringLength(300)]
        public string CreateManId { get; set; }

        [StringLength(300)]
        public string CreateTime { get; set; }

        public int? State { get; set; }

        public int? IsEnable { get; set; }
    }
}
