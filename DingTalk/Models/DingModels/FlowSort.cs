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
        public decimal Id { get; set; }

        /// <summary>
        /// 流程大类名
        /// </summary>
        [StringLength(300)]
        public string SORT_NAME { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [StringLength(300)]
        public string DEPT_ID { get; set; }

        /// <summary>
        /// 父节点Id (预留)
        /// </summary>
        [StringLength(300)]
        public string SORT_PARENT { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(100)]
        public string CreateTime { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public int? State { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int? IsEnable { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBY { get; set; }

        /// <summary>
        /// 子节点Id
        /// </summary>
        [StringLength(200)]
        public string Sort_ID { get; set; }

        /// <summary>
        /// 拥有权限的用户名
        /// </summary>
        [StringLength(500)]
        public string ApplyMan { get; set; }
        /// <summary>
        /// 拥有权限的用户Id
        /// </summary>
        [StringLength(500)]
        public string ApplyManId { get; set; }

        [NotMapped]
        public List<Flows> flows { get; set; }
    }
}
