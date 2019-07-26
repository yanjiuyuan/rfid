namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TasksQuery
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public int? TaskId { get; set; }
        /// <summary>
        /// 提交人
        /// </summary>
        [StringLength(50)]
        public string ApplyMan { get; set; }
        /// <summary>
        /// 提交人Id
        /// </summary>
        [StringLength(500)]
        public string ApplyManId { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [StringLength(30)]
        public string ApplyTime { get; set; }

        /// <summary>
        /// 流程Id
        /// </summary>
        public int? FlowId { get; set; }
        /// <summary>
        /// 节点Id
        /// </summary>
        public int? NodeId { get; set; }
   
    
        /// <summary>
        /// 当前状态
        /// </summary>
        public int? State { get; set; }
      
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否退回
        /// </summary>
        public bool? IsBacked { get; set; }

        /// <summary>
        /// 是否抄送
        /// </summary>
        public bool? IsSend { get; set; }

    }
}
