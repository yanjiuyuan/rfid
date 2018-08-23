namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Flows
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int? FlowId { get; set; }

        [StringLength(200)]
        public string FlowName { get; set; }

        [StringLength(300)]
        public string CreateMan { get; set; }

        [StringLength(300)]
        public string CreateManId { get; set; }

        [StringLength(300)]
        public string ApplyTime { get; set; }

        [StringLength(300)]
        public string ApplyMan { get; set; }

        public int? State { get; set; }

        public int? IsEnable { get; set; }

        public int? SORT_ID { get; set; }
    }
}
