namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Maintain")]
    public partial class Maintain
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }
        

        /// <summary>
        /// 物品名称
        /// </summary>
        [StringLength(300)]
        public string Name { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [StringLength(300)]
        public string CodeNo { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [StringLength(300)]
        public string Standard { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(300)]
        public string Unit { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [StringLength(300)]
        public string Count { get; set; }

        /// <summary>
        /// 单价(预计)
        /// </summary>
        [StringLength(300)]
        public string Price { get; set; }

        /// <summary>
        /// 维修内容
        /// </summary>
        [StringLength(300)]
        public string MaintainContent { get; set; }

        /// <summary>
        /// 需用时间
        /// </summary>
        [StringLength(300)]
        public string NeedTime { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Mark { get; set; }
    }
}