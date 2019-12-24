namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TasksQueryPro
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public int? TaskId { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        [StringLength(50)]
        public string FlowName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(500)]
        public int State { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [StringLength(30)]
        public string ApplyTime { get; set; }

        /// <summary>
        /// 最后处理时间
        /// </summary>
        [StringLength(30)]
        public string CurrentTime { get; set; }

        /// <summary>
        /// 节点Id
        /// </summary>
        public string NodeId { get; set; }
        
        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyMan { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public string FlowState { get; set; }
        
        public string FlowId { get; set; }
    }
}
