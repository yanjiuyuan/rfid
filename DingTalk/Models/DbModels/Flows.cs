namespace DingTalk.Models.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Flows
    {
        [Key]
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal FlowId { get; set; }

        [StringLength(30)]
        public string CreateMan { get; set; }

        [StringLength(100)]
        public string CreateManId { get; set; }

        [StringLength(30)]
        public string ApplyTime { get; set; }

        [StringLength(30)]
        public string ApplyMan { get; set; }

        [StringLength(500)]
        public string FlowName { get; set; }

        public int? State { get; set; }

        public int? IsEnable { get; set; }

        public int? SORT_ID { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }
    }
}
