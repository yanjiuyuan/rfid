namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowQx")]
    public partial class FlowQx
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ID { get; set; }

        [StringLength(100)]
        public string FlowId { get; set; }

        public int? QxType { get; set; }

        [StringLength(300)]
        public string Qx_Range { get; set; }

        [StringLength(300)]
        public string User { get; set; }

        [StringLength(300)]
        public string Dept { get; set; }

        [StringLength(300)]
        public string Role { get; set; }

        public int? IsEnable { get; set; }

        [StringLength(100)]
        public string CreateTime { get; set; }
    }
}
