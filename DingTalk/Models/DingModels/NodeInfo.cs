namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeInfo")]
    public partial class NodeInfo
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int? NodeId { get; set; }

        [StringLength(100)]
        public string FlowId { get; set; }

        [StringLength(200)]
        public string NodeName { get; set; }

        [StringLength(500)]
        public string NodePeople { get; set; }

        [StringLength(500)]
        public string PeopleId { get; set; }

        [StringLength(200)]
        public string PreNodeId { get; set; }

        public bool? IsAllAllow { get; set; }

        [StringLength(500)]
        public string Condition { get; set; }

        public bool? IsBack { get; set; }

        public bool? IsNeedChose { get; set; }

        public bool? IsSend { get; set; }

        [StringLength(100)]
        public string BackNodeId { get; set; }

        [StringLength(100)]
        public string ChoseNodeId { get; set; }
    }
}
