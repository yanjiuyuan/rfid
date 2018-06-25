namespace DingTalk.Models.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Roles
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(100)]
        public string FlowId { get; set; }

        [StringLength(200)]
        public string NodeName { get; set; }

        public int? IsEnable { get; set; }
    }
}
