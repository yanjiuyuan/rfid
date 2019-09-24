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


        [StringLength(300)]
        public string ApplyMan { get; set; }
        
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
        /// 是否支持手机
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
        /// 手机端图片路径
        /// </summary>
        [StringLength(200)]
        public string PhoneUrl { get; set; }

        /// <summary>
        /// 选人控件单选或者多选(单选 0 多选 1)
        /// </summary>
        [StringLength(200)]
        public string IsSelectMore { get; set; }

        /// <summary>
        /// 是否必选 (0 不必选 1 必选)
        /// </summary>
        [StringLength(200)]
        public string IsMandatory { get; set; }
        
    }
}
