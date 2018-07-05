namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowSort")]
    public partial class FlowSort
    {
        [Key]
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal SORT_ID { get; set; }

        [StringLength(300)]
        public string SORT_NAME { get; set; }

        [StringLength(300)]
        public string DEPT_ID { get; set; }

        [StringLength(300)]
        public string SORT_PARENT { get; set; }

        [StringLength(100)]
        public string CreateTime { get; set; }

        public int? State { get; set; }

        public int? IsEnable { get; set; }
    }
}
