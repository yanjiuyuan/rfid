namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Flows
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>
        /// 流程Id
        /// </summary>
        public int? FlowId { get; set; }

        /// <summary>
        /// 流程名
        /// </summary>
        [StringLength(200)]
        public string FlowName { get; set; }

        /// <summary>
        /// 流程名(移动端)
        /// </summary>
        [StringLength(200)]
        public string FlowNameMobile { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(300)]
        public string CreateMan { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        [StringLength(300)]
        public string CreateManId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(300)]
        public string ApplyTime { get; set; }

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

        /// <summary>
        /// 流程状态
        /// </summary>
        public int? State { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int? IsEnable { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public int? SORT_ID { get; set; }

        /// <summary>
        /// 是否支持手机审批推送
        /// </summary>
        public bool? IsSupportMobile { get; set; }

        /// <summary>
        /// 手机通知推送路径
        /// </summary>
        [StringLength(200)]
        public string ApproveUrl { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBY { get; set; }

        /// <summary>
        /// -200px 300px 
        /// </summary>
        [StringLength(200)]
        public string Position { get; set; }

        /// <summary>
        /// 电脑端图片路径
        /// </summary>
        [StringLength(200)]
        public string PcUrl { get; set; }

        /// <summary>
        /// 手机端发起页面路径
        /// </summary>
        [StringLength(200)]
        public string PhoneUrl { get; set; }

        /// <summary>
        /// 是否流程(可能为功能模块)
        /// </summary>
        public bool? IsFlow { get; set; }

    }
}
