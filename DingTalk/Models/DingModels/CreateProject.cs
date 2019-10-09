namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CreateProject")]
    public partial class CreateProject
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(500)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 此字段作废 用ProjectId
        /// </summary>
        [StringLength(500)]
        public string ProjectNo { get; set; }


        [StringLength(100)]
        public string CreateTime { get; set; }

        public bool? IsEnable { get; set; }

        [StringLength(100)]
        public string ProjectState { get; set; }

        [StringLength(200)]
        public string DeptName { get; set; }

        [StringLength(200)]
        public string ApplyMan { get; set; }

        [StringLength(300)]
        public string ApplyManId { get; set; }

        [StringLength(200)]
        public string StartTime { get; set; }

        [StringLength(200)]
        public string EndTime { get; set; }

        [StringLength(200)]
        public string ProjectId { get; set; }

        public string FilePath { get; set; }

        [StringLength(200)]
        public string ResponsibleMan { get; set; }

        [StringLength(200)]
        public string ResponsibleManId { get; set; }

        [StringLength(200)]
        public string CompanyName { get; set; }

        [StringLength(200)]
        public string ProjectType { get; set; }

        public string TeamMembers { get; set; }

        public string TeamMembersId { get; set; }

        [StringLength(200)]
        public string CreateMan { get; set; }


        [StringLength(200)]
        public string CreateManId { get; set; }

        [StringLength(200)]
        public string TaskId { get; set; }

        /// <summary>
        /// 项目立项文件路径
        /// </summary>
        public string ProjectFileUrl { get; set; }

        /// <summary>
        /// 是否评审
        /// </summary>
        public bool? IsReview { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [StringLength(300)]
        public string Customer { get; set; }

        /// <summary>
        /// 项目小类(新增)
        /// </summary>
        [StringLength(200)]
        public string ProjectSmallType { get; set; }

    }
}
