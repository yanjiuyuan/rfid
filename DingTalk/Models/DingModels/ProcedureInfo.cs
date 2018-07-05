namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcedureInfo")]
    public partial class ProcedureInfo
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(500)]
        public string ProcedureName { get; set; }

        [StringLength(500)]
        public string DefaultWorkTime { get; set; }

        public int? State { get; set; }

        [StringLength(200)]
        public string CreateTime { get; set; }

        [StringLength(200)]
        public string ApplyMan { get; set; }

        [StringLength(200)]
        public string ApplyManId { get; set; }
    }
}
