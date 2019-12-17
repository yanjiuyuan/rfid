namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Tasks
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
        /// 部门名称
        /// </summary>
        [StringLength(200)]
        public string Dept { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [StringLength(30)]
        public string ApplyTime { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public int? IsEnable { get; set; }

        /// <summary>
        /// 审批状态(Index=0:待我审批 1:我已审批 2:我发起的 3:抄送我的)
        /// </summary>
        //public int? Type { get; set; }


        /// <summary>
        /// 流程Id
        /// </summary>
        public int? FlowId { get; set; }
        /// <summary>
        /// 节点Id
        /// </summary>
        public int? NodeId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否抄送
        /// </summary>
        public bool? IsSend { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public int? State { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 项目Id
        /// </summary>
        [StringLength(500)]
        public string ProjectId { get; set; }
        /// <summary>
        /// 是否是发起
        /// </summary>
        public bool? IsPost { get; set; }
        /// <summary>
        /// 原图片路径
        /// </summary>
        public string OldImageUrl { get; set; }
        /// <summary>
        /// 原文件路径
        /// </summary>
        public string OldFileUrl { get; set; }
        /// <summary>
        /// 是否退回
        /// </summary>
        public bool? IsBacked { get; set; }
        /// <summary>
        /// 盯盘文件Id
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// PDF文件路径
        /// </summary>
        public string FilePDFUrl { get; set; }
        /// <summary>
        /// 原PDF文件路径
        /// </summary>
        public string OldFilePDFUrl { get; set; }
        /// <summary>
        /// PDF盯盘Id
        /// </summary>
        public string MediaIdPDF { get; set; }
        /// <summary>
        /// PDF状态
        /// </summary>
        [StringLength(500)]
        public string PdfState { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        [StringLength(200)]
        public string ProjectName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [StringLength(100)]
        public string counts { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [StringLength(200)]
        public string NodeName { get; set; }


        /// <summary>
        /// 项目具体类型(电器加工、机械加工、机械采购、其他)
        /// </summary>
        [StringLength(200)]
        public string ProjectType { get; set; }

        [NotMapped]
        public string FlowState { get; set; }

        /// <summary>
        /// 抄送时是否已读
        /// </summary>
        [NotMapped]
        public bool IsRead { get; set; }

        /// <summary>
        /// 是否支持手机端
        /// </summary>
        [NotMapped]
        public bool? IsSupportMobile { get; set; }

        /// <summary>
        /// 流程名
        /// </summary>
        [NotMapped]
        public string FlowName { get; set; }

        /// <summary>
        /// 最近一次处理时间
        /// </summary>
        [NotMapped]
        public string CurrentTime { get; set; }

        /// <summary>
        /// 盯盘数据
        /// </summary>
        //public string SpaceId { get; set; }
    }
}
