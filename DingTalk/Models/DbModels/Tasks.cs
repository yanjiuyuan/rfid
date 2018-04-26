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

        [StringLength(50)]
        public string ApplyMan { get; set; }

        [StringLength(500)]
        public string ApplyManId { get; set; }

        [StringLength(30)]
        public string ApplyTime { get; set; }

        public int? IsEnable { get; set; }

        public int? FlowId { get; set; }

        public int? NodeId { get; set; }

        public string Remark { get; set; }

        public bool? IsSend { get; set; }

        public int? State { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(500)]
        public string FileUrl { get; set; }

        public string Title { get; set; }

        [StringLength(500)]
        public string ProjectId { get; set; }

        public bool? IsPost { get; set; }

        [StringLength(500)]
        public string OldImageUrl { get; set; }

        [StringLength(500)]
        public string OldFileUrl { get; set; }
    }
}
