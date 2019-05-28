namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FileChange")]
    public partial class FileChange
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }

        [StringLength(300)]
        public string OldTaskId { get; set; }
        
        [StringLength(300)]
        public string MediaId { get; set; }

        [StringLength(300)]
        public string MediaIdPDF { get; set; }

        [StringLength(300)]
        public string FilePDFUrl { get; set; }

        [StringLength(300)]
        public string OldFilePDFUrl { get; set; }

        [StringLength(300)]
        public string FileUrl { get; set; }

        [StringLength(300)]
        public string OldFileUrl { get; set; }
        
    }
}
