namespace DingTalk.Models.DingModels
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

        [StringLength(200)]
        public string RoleName { get; set; }

        [StringLength(200)]
        public string CreateMan { get; set; }

        [StringLength(200)]
        public string CreateManId { get; set; }

        [StringLength(200)]
        public string UserName { get; set; }

        [StringLength(200)]
        public string UserId { get; set; }

        public bool? IsEnable { get; set; }
    }
}
