namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MaterialCode")]
    public partial class MaterialCode
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        
        [StringLength(500)]
        public string MaterialCodeNumber { get; set; }

        [StringLength(500)]
        public string MaterialName { get; set; }

        [StringLength(500)]
        public string MateriaType { get; set; }
    }
}
