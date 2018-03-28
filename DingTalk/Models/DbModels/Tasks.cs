namespace DingTalk.Models.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Tasks
    {
        [Key]
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal TaskId { get; set; }

        [StringLength(30)]
        public string ApplyMan { get; set; }

        [StringLength(30)]
        public string ApplyTime { get; set; }

        public int? IsEnable { get; set; }

        public int? FlowId { get; set; }
    }
}
