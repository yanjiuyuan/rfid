namespace DingTalk.Models.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Tasks
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int? TaskId { get; set; }

        [StringLength(30)]
        public string ApplyMan { get; set; }

        [StringLength(30)]
        public string ApplyTime { get; set; }

        public int? IsEnable { get; set; }

        public int? FlowId { get; set; }

        public int? NodeId { get; set; }

        public string Remark { get; set; }
    }
}
