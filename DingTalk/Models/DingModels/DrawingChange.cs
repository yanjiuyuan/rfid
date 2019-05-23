namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DrawingChange")]
    public partial class DrawingChange
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }
        /// <summary>
        /// 替换流程流水号
        /// </summary>
        [StringLength(300)]
        public string OldTaskId { get; set; }
        /// <summary>
        /// PDF图纸路径
        /// </summary>
        [StringLength(1000)]
        public string FilePDFUrl { get; set; }
        /// <summary>
        /// PDF图纸路径(需要替换的)
        /// </summary>
        [StringLength(1000)]
        public string OldFilePDFUrl { get; set; }

        /// <summary>
        /// PDF图纸名称(需要替换的)
        /// </summary>
        [StringLength(500)]
        public string OldPDFName { get; set; }
        /// <summary>
        /// PDF图纸名称
        /// </summary>
        [StringLength(500)]
        public string PDFName { get; set; }
        /// <summary>
        /// 其他文件MediaId(需要替换的)
        /// </summary>
        [StringLength(500)]
        public string OldMediaId { get; set; }
        /// <summary>
        /// 其他文件MediaId
        /// </summary>
        [StringLength(500)]
        public string MediaId { get; set; }

        /// <summary>
        /// 其他文件文件名(需要替换的)
        /// </summary>
        [StringLength(500)]
        public string OldFileName { get; set; }

        /// <summary>
        /// 其他文件文件名
        /// </summary>
        [StringLength(500)]
        public string FileName { get; set; }
    }
}