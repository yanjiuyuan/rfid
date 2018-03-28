namespace DingTalk.Models.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeInfo")]
    public partial class NodeInfo
    {
        [Key]
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal FlowId { get; set; }

        [StringLength(100)]
        public string A { get; set; }

        [StringLength(100)]
        public string B { get; set; }

        [StringLength(100)]
        public string C { get; set; }

        [StringLength(100)]
        public string D { get; set; }

        [StringLength(100)]
        public string E { get; set; }

        [StringLength(100)]
        public string F { get; set; }

        [StringLength(100)]
        public string G { get; set; }

        [StringLength(100)]
        public string H { get; set; }

        [StringLength(100)]
        public string I { get; set; }

        [StringLength(100)]
        public string J { get; set; }

        [StringLength(100)]
        public string K { get; set; }

        [StringLength(100)]
        public string L { get; set; }

        [StringLength(100)]
        public string M { get; set; }

        [StringLength(100)]
        public string N { get; set; }

        [StringLength(100)]
        public string O { get; set; }

        [StringLength(100)]
        public string P { get; set; }

        [StringLength(100)]
        public string Q { get; set; }

        [StringLength(100)]
        public string R { get; set; }

        [StringLength(100)]
        public string S { get; set; }

        [StringLength(100)]
        public string T { get; set; }

        [StringLength(100)]
        public string U { get; set; }

        [StringLength(100)]
        public string V { get; set; }

        [StringLength(100)]
        public string W { get; set; }

        [StringLength(100)]
        public string X { get; set; }

        [StringLength(100)]
        public string Y { get; set; }

        [StringLength(100)]
        public string Z { get; set; }
    }
}
