namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Contract")]
    public partial class Contract
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }

        [StringLength(300)]
        public string ContractNo { get; set; }

        [StringLength(300)]
        public string SignUnit { get; set; }

        [StringLength(300)]
        public string SalesManager { get; set; }

        [StringLength(300)]
        public string ContractType { get; set; }

        [StringLength(500)]
        public string Path { get; set; }

        [StringLength(500)]
        public string FilePath { get; set; }
    }
}
