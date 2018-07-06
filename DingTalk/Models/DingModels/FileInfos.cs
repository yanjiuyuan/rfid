namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FileInfos
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(200)]
        public string ApplyMan { get; set; }

        [StringLength(200)]
        public string ApplyManId { get; set; }

        [StringLength(200)]
        public string FilePath { get; set; }

        [StringLength(200)]
        public string LastModifyTime { get; set; }

        [StringLength(200)]
        public string LastModifyState { get; set; }

        [StringLength(200)]
        public string MediaId { get; set; }
    }
}
