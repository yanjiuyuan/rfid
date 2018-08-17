namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Vote")]
    public partial class Vote
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        [StringLength(200)]
        public string ApplyMan { get; set; }
        /// <summary>
        /// 发起人Id
        /// </summary>
        [StringLength(200)]
        public string ApplyManId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(200)]
        public string Title { get; set; }
        /// <summary>
        /// 选项(逗号隔开)
        /// </summary>
        public string Option { get; set; }
        /// <summary>
        /// 投票数量(逗号隔开)
        /// </summary>
        [StringLength(200)]
        public string VoteCount { get; set; }
        /// <summary>
        /// 所有提交人Id集合(逗号隔开)
        /// </summary>
        public string SubmitterId { get; set; }
    }
}
