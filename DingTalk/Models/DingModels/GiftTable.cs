namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GiftTable")]
    public partial class GiftTable
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(30)]
        public string TaskId { get; set; }
        /// <summary>
        /// 礼品编号(预留) 启动，等价于Id
        /// </summary>
        [StringLength(500)]
        public string GiftNo { get; set; }
        /// <summary>
        /// 礼品名称
        /// </summary>
        [StringLength(500)]
        public string GiftName { get; set; }
        /// <summary>
        /// 礼品数量
        /// </summary>
        [StringLength(500)]
        public string GiftCount { get; set; }
    }
}
