namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PicInfo")]
    public partial class PicInfo
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(50)]
        public string Type { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        [StringLength(300)]
        public string Path { get; set; }
        /// <summary>
        /// 内容(预留)
        /// </summary>
        [StringLength(500)]
        public string Content { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(200)]
        public string CreateMan { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        [StringLength(200)]
        public string CreateManId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(200)]
        public string CreateTime { get; set; }

        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        [StringLength(200)]
        public string LastModifyTime { get; set; }

        /// <summary>
        /// 最后一次修改人
        /// </summary>
        [StringLength(200)]
        public string LastModifyMan { get; set; }

        /// <summary>
        /// 最后一次修改人Id
        /// </summary>
        [StringLength(200)]
        public string LastModifyManId { get; set; }
    }
}
