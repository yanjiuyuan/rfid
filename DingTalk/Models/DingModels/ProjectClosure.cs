namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProjectClosure")]
    public partial class ProjectClosure
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(500)]
        public string TaskId { get; set; }

        /// <summary>
        /// 项目类型
        /// </summary>
        [StringLength(500)]
        public string ProjectType { get; set; }

        /// <summary>
        /// 项目负责人
        /// </summary>
        [StringLength(500)]
        public string ResponsibleMan { get; set; }

        /// <summary>
        /// 项目负责人Id
        /// </summary>
        [StringLength(500)]
        public string ResponsibleManId { get; set; }

        /// <summary>
        /// 小组成员
        /// </summary>
        [StringLength(500)]
        public string TeamMembers { get; set; }

        /// <summary>
        /// 小组成员Id
        /// </summary>
        [StringLength(500)]
        public string TeamMembersId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [StringLength(500)]
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [StringLength(500)]
        public string EndTime { get; set; }

        /// <summary>
        /// 实际周期
        /// </summary>
        [StringLength(200)]
        public string ActualCycleStart { get; set; }

        /// <summary>
        /// 实际周期
        /// </summary>
        [StringLength(200)]
        public string ActualCycleEnd { get; set; }
        


        /// <summary>
        /// 是否横向
        /// </summary>
        public bool? IsTransverse { get; set; }

        /// <summary>
        /// 是否纵向
        /// </summary>
        public bool? IsPortrait { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        [StringLength(500)]
        public string ContractAmount { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        [StringLength(500)]
        public string ContractNo { get; set; }

        /// <summary>
        /// 实际到账
        /// </summary>
        public string ActualMoney { get; set; }
        public string SuggestBook1 { get; set; }
        public string PPT2 { get; set; }
        public string DemandBook3 { get; set; }
        public string Drawing4 { get; set; }
        public string Electrical5 { get; set; }
        public string Bom6 { get; set; }
        public string SourceCode7 { get; set; }
        public string UseBook8 { get; set; }
        public string CooperationAgreement9 { get; set; }
        public string Product10 { get; set; }
        public string Solution11 { get; set; }
        public string AcceptanceData14 { get; set; }
        public string ProcessDocumentation15 { get; set; }
        public string TerminationReport16 { get; set; }
        public string PackingList17 { get; set; }
        public string AcceptanceSlip18 { get; set; }

    }
}
