namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TechnicalSupport")]
    public partial class TechnicalSupport
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(30)]
        public string TaskId { get; set; }

        /// <summary>
        /// 技术支持部门
        /// </summary>
        [StringLength(200)]
        public string DeptName { get; set; }

        /// <summary>
        /// 测试项目名称
        /// </summary>
        [StringLength(200)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目负责人
        /// </summary>
        [StringLength(200)]
        public string ResponsibleMan { get; set; }
        /// <summary>
        /// 项目负责人Id
        /// </summary>
        [StringLength(200)]
        public string ResponsibleManId { get; set; }
        /// <summary>
        /// 其他工程师
        /// </summary>
        [StringLength(300)]
        public string OtherEngineers { get; set; }
        /// <summary>
        /// 其他工程师Id
        /// </summary>
        [StringLength(300)]
        public string OtherEngineersId { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        [StringLength(300)]
        public string Customer { get; set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        [StringLength(300)]
        public string EmergencyLevel { get; set; }

        /// <summary>
        /// 要求完成时间
        /// </summary>
        [StringLength(300)]
        public string TimeRequired { get; set; }

        /// <summary>
        /// 所属项目公司(后台处理)
        /// </summary>
        [StringLength(300)]
        public string CompanyName { get; set; }
        /// <summary>
        /// 项目状态(后台处理)
        /// </summary>
        [StringLength(300)]
        public string ProjectState { get; set; }
        /// <summary>
        /// 项目类型(后台处理)
        /// </summary>
        [StringLength(300)]
        public string ProjectType { get; set; }
        /// <summary>
        /// 客户项目整体概况
        /// </summary>
        [StringLength(1000)]
        public string ProjectOverview { get; set; }
        /// <summary>
        /// 技术支持内容要点
        /// </summary>
        [StringLength(1000)]
        public string MainPoints { get; set; }
        /// <summary>
        /// 项目组成员
        /// </summary>
        [StringLength(300)]
        public string TeamMembers { get; set; }
        /// <summary>
        /// 项目组成员Id
        /// </summary>
        [StringLength(300)]
        public string TeamMembersId { get; set; }
        /// <summary>
        /// 处理方案
        /// </summary>
        [StringLength(300)]
        public string TechnicalProposal { get; set; }
        [StringLength(300)]
        public string ProjectNo { get; set; }

        /// <summary>
        /// 项目周期开始时间
        /// </summary>
        [StringLength(100)]
        public string StartTime { get; set; }
        /// <summary>
        /// 项目周期结束时间
        /// </summary>
        [StringLength(100)]
        public string EndTime { get; set; }

        /// <summary>
        /// 是否创建项目(最后一个节点人提交传true)
        /// </summary>
        [NotMapped]
        public bool IsCreateProject { get; set; }

        /// <summary>
        /// 商务对接人
        /// </summary>
        [StringLength(100)]
        public string BusinessDocker { get; set; }
    }
}
