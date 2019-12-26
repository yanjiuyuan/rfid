namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProjectInfo")]
    public partial class ProjectInfo
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [StringLength(500)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(100)]
        public string CreateTime { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsEnable { get; set; }

        /// <summary>
        /// 项目状态
        /// </summary>
        [StringLength(100)]
        public string ProjectState { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [StringLength(200)]
        public string DeptName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(200)]
        public string ApplyMan { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        [StringLength(300)]
        public string ApplyManId { get; set; }

        /// <summary>
        /// 项目开始时间
        /// </summary>
        [StringLength(200)]
        public string StartTime { get; set; }

        /// <summary>
        /// 项目结束时间
        /// </summary>
        [StringLength(200)]
        public string EndTime { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [StringLength(200)]
        public string ProjectId { get; set; }

        /// <summary>
        /// 项目路径
        /// </summary>
        public string FilePath { get; set; }

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
        /// 合作单位
        /// </summary>
        [StringLength(200)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 项目大类
        /// </summary>
        [StringLength(200)]
        public string ProjectType { get; set; }

        /// <summary>
        /// 项目小类(新增)
        /// </summary>
        [StringLength(200)]
        public string ProjectSmallType { get; set; }

        /// <summary>
        /// 小组成员
        /// </summary>
        public string TeamMembers { get; set; }

        /// <summary>
        /// 小组成员Id
        /// </summary>
        public string TeamMembersId { get; set; }

        /// <summary>
        /// 合作单位(新增)
        /// </summary>
        public string CooperativeUnit { get; set; }

        /// <summary>
        /// 商务对接人
        /// </summary>
        public string BusinessDocker { get; set; }
        
        /// <summary>
        /// 是否可以编辑
        /// </summary>
        [NotMapped]
        public bool IsEdit { get; set; }
    }
}
